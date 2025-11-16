using Microsoft.EntityFrameworkCore;
using OrderManagementSystem.Data;
using OrderManagementSystem.DTOs;
using OrderManagementSystem.Models;

namespace OrderManagementSystem.Services
{
    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<OrderService> _logger;

        public OrderService(ApplicationDbContext context, ILogger<OrderService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<OrderResponseDto> CreateOrderAsync(CreateOrderDto createOrderDto)
        {
            // Validation
            if (string.IsNullOrWhiteSpace(createOrderDto.CustomerName))
                throw new ArgumentException("Müşteri adı boş olamaz.");

            if (string.IsNullOrWhiteSpace(createOrderDto.CustomerEmail))
                throw new ArgumentException("Müşteri e-posta adresi boş olamaz.");

            if (string.IsNullOrWhiteSpace(createOrderDto.ShippingAddress))
                throw new ArgumentException("Teslimat adresi boş olamaz.");

            if (createOrderDto.OrderItems == null || !createOrderDto.OrderItems.Any())
                throw new ArgumentException("Sipariş en az bir ürün içermelidir.");

            // Start transaction
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Check stock availability and get product details
                var productIds = createOrderDto.OrderItems.Select(oi => oi.ProductId).ToList();
                var products = await _context.Products
                    .Where(p => productIds.Contains(p.Id))
                    .ToListAsync();

                if (products.Count != productIds.Distinct().Count())
                {
                    throw new InvalidOperationException("Bir veya daha fazla ürün bulunamadı.");
                }

                // Check stock for each product
                var orderItems = new List<OrderItem>();
                decimal totalAmount = 0;

                foreach (var orderItemDto in createOrderDto.OrderItems)
                {
                    var product = products.FirstOrDefault(p => p.Id == orderItemDto.ProductId);
                    if (product == null)
                    {
                        throw new InvalidOperationException($"Ürün ID {orderItemDto.ProductId} bulunamadı.");
                    }

                    if (product.StockQuantity < orderItemDto.Quantity)
                    {
                        throw new InvalidOperationException(
                            $"Yetersiz stok! Ürün: {product.Name}, " +
                            $"Mevcut stok: {product.StockQuantity}, " +
                            $"İstenen miktar: {orderItemDto.Quantity}");
                    }

                    if (orderItemDto.Quantity <= 0)
                    {
                        throw new ArgumentException($"Ürün miktarı 0'dan büyük olmalıdır. Ürün ID: {orderItemDto.ProductId}");
                    }

                    var itemTotalPrice = product.Price * orderItemDto.Quantity;
                    totalAmount += itemTotalPrice;

                    orderItems.Add(new OrderItem
                    {
                        ProductId = product.Id,
                        Quantity = orderItemDto.Quantity,
                        UnitPrice = product.Price,
                        TotalPrice = itemTotalPrice
                    });

                    // Update stock
                    product.StockQuantity -= orderItemDto.Quantity;
                    product.UpdatedAt = DateTime.UtcNow;
                }

                // Create order
                var order = new Order
                {
                    CustomerName = createOrderDto.CustomerName,
                    CustomerEmail = createOrderDto.CustomerEmail,
                    ShippingAddress = createOrderDto.ShippingAddress,
                    TotalAmount = totalAmount,
                    Status = OrderStatus.Pending,
                    OrderDate = DateTime.UtcNow,
                    OrderItems = orderItems
                };

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation($"Sipariş oluşturuldu. Sipariş ID: {order.Id}, Toplam tutar: {totalAmount:C}");

                // Return response
                return await GetOrderByIdAsync(order.Id) 
                    ?? throw new InvalidOperationException("Sipariş oluşturuldu ancak getirilemedi.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Sipariş oluşturulurken hata oluştu.");
                throw;
            }
        }

        public async Task<List<OrderResponseDto>> GetAllOrdersAsync()
        {
            var orders = await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return orders.Select(MapToOrderResponseDto).ToList();
        }

        public async Task<OrderResponseDto?> GetOrderByIdAsync(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            return order == null ? null : MapToOrderResponseDto(order);
        }

        public async Task<bool> DeleteOrderAsync(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
            {
                return false;
            }

            // Restore stock quantities
            foreach (var orderItem in order.OrderItems)
            {
                var product = orderItem.Product;
                product.StockQuantity += orderItem.Quantity;
                product.UpdatedAt = DateTime.UtcNow;
            }

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Sipariş silindi. Sipariş ID: {orderId}");

            return true;
        }

        private OrderResponseDto MapToOrderResponseDto(Order order)
        {
            return new OrderResponseDto
            {
                Id = order.Id,
                CustomerName = order.CustomerName,
                CustomerEmail = order.CustomerEmail,
                ShippingAddress = order.ShippingAddress,
                TotalAmount = order.TotalAmount,
                Status = order.Status.ToString(),
                OrderDate = order.OrderDate,
                ShippedDate = order.ShippedDate,
                DeliveredDate = order.DeliveredDate,
                OrderItems = order.OrderItems.Select(oi => new OrderItemResponseDto
                {
                    Id = oi.Id,
                    ProductId = oi.ProductId,
                    ProductName = oi.Product.Name,
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice,
                    TotalPrice = oi.TotalPrice
                }).ToList()
            };
        }
    }
}

