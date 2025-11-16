using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderManagementSystem.Data;
using OrderManagementSystem.Models;

namespace OrderManagementSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(ApplicationDbContext context, ILogger<ProductsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Tüm ürünleri listeler
        /// </summary>
        /// <returns>Ürün listesi</returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<Product>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<Product>>> GetAllProducts()
        {
            try
            {
                var products = await _context.Products
                    .OrderBy(p => p.Name)
                    .ToListAsync();
                
                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ürünler getirilirken hata oluştu.");
                return StatusCode(500, new { error = "Ürünler getirilirken bir hata oluştu." });
            }
        }

        /// <summary>
        /// Belirtilen ID'ye sahip ürünün detaylarını getirir
        /// </summary>
        /// <param name="id">Ürün ID</param>
        /// <returns>Ürün detayları</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Product), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Product>> GetProductById(int id)
        {
            try
            {
                var product = await _context.Products.FindAsync(id);
                
                if (product == null)
                {
                    return NotFound(new { error = $"Ürün bulunamadı. ID: {id}" });
                }

                return Ok(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ürün getirilirken hata oluştu. ID: {id}");
                return StatusCode(500, new { error = "Ürün getirilirken bir hata oluştu." });
            }
        }
    }
}

