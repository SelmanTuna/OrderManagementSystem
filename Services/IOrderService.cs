using OrderManagementSystem.DTOs;

namespace OrderManagementSystem.Services
{
    public interface IOrderService
    {
        Task<OrderResponseDto> CreateOrderAsync(CreateOrderDto createOrderDto);
        Task<List<OrderResponseDto>> GetAllOrdersAsync();
        Task<OrderResponseDto?> GetOrderByIdAsync(int orderId);
        Task<bool> DeleteOrderAsync(int orderId);
    }
}

