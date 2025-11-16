using Microsoft.AspNetCore.Mvc;
using OrderManagementSystem.DTOs;
using OrderManagementSystem.Services;

namespace OrderManagementSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(IOrderService orderService, ILogger<OrdersController> logger)
        {
            _orderService = orderService;
            _logger = logger;
        }

        /// <summary>
        /// Yeni sipariş oluşturur
        /// </summary>
        /// <param name="createOrderDto">Sipariş bilgileri</param>
        /// <returns>Oluşturulan sipariş detayları</returns>
        [HttpPost]
        [ProducesResponseType(typeof(OrderResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<OrderResponseDto>> CreateOrder([FromBody] CreateOrderDto createOrderDto)
        {
            try
            {
                var order = await _orderService.CreateOrderAsync(createOrderDto);
                return CreatedAtAction(nameof(GetOrderById), new { id = order.Id }, order);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Geçersiz sipariş verisi.");
                return BadRequest(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Sipariş işlemi başarısız.");
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Sipariş oluşturulurken beklenmeyen bir hata oluştu.");
                return StatusCode(500, new { error = "Sipariş oluşturulurken bir hata oluştu." });
            }
        }

        /// <summary>
        /// Tüm siparişleri listeler
        /// </summary>
        /// <returns>Sipariş listesi</returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<OrderResponseDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<OrderResponseDto>>> GetAllOrders()
        {
            try
            {
                var orders = await _orderService.GetAllOrdersAsync();
                return Ok(orders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Siparişler getirilirken hata oluştu.");
                return StatusCode(500, new { error = "Siparişler getirilirken bir hata oluştu." });
            }
        }

        /// <summary>
        /// Belirtilen ID'ye sahip siparişin detaylarını getirir
        /// </summary>
        /// <param name="id">Sipariş ID</param>
        /// <returns>Sipariş detayları</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(OrderResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<OrderResponseDto>> GetOrderById(int id)
        {
            try
            {
                var order = await _orderService.GetOrderByIdAsync(id);
                
                if (order == null)
                {
                    return NotFound(new { error = $"Sipariş bulunamadı. ID: {id}" });
                }

                return Ok(order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Sipariş getirilirken hata oluştu. ID: {id}");
                return StatusCode(500, new { error = "Sipariş getirilirken bir hata oluştu." });
            }
        }

        /// <summary>
        /// Belirtilen ID'ye sahip siparişi siler
        /// </summary>
        /// <param name="id">Sipariş ID</param>
        /// <returns>Silme işlemi sonucu</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            try
            {
                var result = await _orderService.DeleteOrderAsync(id);
                
                if (!result)
                {
                    return NotFound(new { error = $"Sipariş bulunamadı. ID: {id}" });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Sipariş silinirken hata oluştu. ID: {id}");
                return StatusCode(500, new { error = "Sipariş silinirken bir hata oluştu." });
            }
        }
    }
}

