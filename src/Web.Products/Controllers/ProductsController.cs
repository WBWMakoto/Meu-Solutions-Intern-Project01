using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductsAPI.Data;
using ProductsAPI.Models;
using Microsoft.Extensions.Logging;

namespace ProductsAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(ApplicationDbContext context, ILogger<ProductsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        //a. GET: /products/all
        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<Product>>> GetAllProducts()
        {
            try
            {
                var products = await _context.Product.ToListAsync(); // Ensure it's the correct DbSet
                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all products");
                return StatusCode(500, "Internal server error");
            }
        }

        //b. GET: /products/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            try
            {
                var product = await _context.Product.FindAsync(id); // Ensure it's the correct DbSet

                if (product == null)
                {
                    return NotFound($"Product with ID {id} not found");
                }

                return Ok(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting product {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        //c. POST: /products
        [HttpPost]
        public async Task<ActionResult<Product>> CreateProduct(Product product)
        {
            try
            {
                product.CreatedAt = DateTime.UtcNow;
                product.UpdatedAt = DateTime.UtcNow;

                await _context.Product.AddAsync(product); // Ensure it's the correct DbSet
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error creating product");
                return StatusCode(500, "Error creating product. The product code might be duplicate.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating product");
                return StatusCode(500, "Internal server error");
            }
        }

        //d. PUT: /products/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, Product product)
        {
            if (id != product.Id)
            {
                return BadRequest("ID mismatch");
            }

            try
            {
                var existingProduct = await _context.Product.FindAsync(id); // Ensure it's the correct DbSet
                if (existingProduct == null)
                {
                    return NotFound($"Product with ID {id} not found");
                }

                // Update properties
                existingProduct.Code = product.Code;
                existingProduct.Name = product.Name;
                existingProduct.Category = product.Category;
                existingProduct.Brand = product.Brand;
                existingProduct.Type = product.Type;
                existingProduct.Description = product.Description;
                existingProduct.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "Concurrency error updating product {Id}", id);
                return StatusCode(500, "Concurrency error occurred");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating product {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        //e. DELETE: /products/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                var product = await _context.Product.FindAsync(id); // Ensure it's the correct DbSet
                if (product == null)
                {
                    return NotFound($"Product with ID {id} not found");
                }

                _context.Product.Remove(product); // Ensure it's the correct DbSet
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting product {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

 

    }
}
