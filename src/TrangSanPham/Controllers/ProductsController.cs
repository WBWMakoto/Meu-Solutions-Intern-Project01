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

        // GET: /products/all
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

        // GET: /products/5
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

        // POST: /products
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

        // PUT: /products/5
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

        // DELETE: /products/5
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

        // GET: /products/view
        [HttpGet("view")]
        public async Task<IActionResult> GetProductsView()
        {
            try
            {
                var products = await _context.Product.ToListAsync(); // Ensure it's the correct DbSet
                var html = GenerateHtml(products);
                return Content(html, "text/html");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating products view");
                return StatusCode(500, "Internal server error");
            }
        }

        private string GenerateHtml(List<Product> products)
        {
            var html = @"
            <!DOCTYPE html>
            <html>
            <head>
                <title>Products List</title>
                <style>
                    body { font-family: Arial, sans-serif; margin: 20px; }
                    h1 { color: #333; }
                    table { 
                        border-collapse: collapse; 
                        width: 100%;
                        margin-top: 20px;
                    }
                    th, td { 
                        border: 1px solid #ddd; 
                        padding: 12px 8px; 
                        text-align: left; 
                    }
                    th { 
                        background-color: #f4f4f4; 
                        color: #333;
                    }
                    tr:nth-child(even) { 
                        background-color: #f9f9f9; 
                    }
                    tr:hover {
                        background-color: #f5f5f5;
                    }
                </style>
            </head>
            <body>
                <h1>Products List</h1>
                <table>
                    <tr>
                        <th>ID</th>
                        <th>Code</th>
                        <th>Name</th>
                        <th>Category</th>
                        <th>Brand</th>
                        <th>Type</th>
                        <th>Description</th>
                        <th>Created</th>
                        <th>Updated</th>
                    </tr>";

            foreach (var product in products)
            {
                html += $@"
                    <tr>
                        <td>{product.Id}</td>
                        <td>{product.Code}</td>
                        <td>{product.Name}</td>
                        <td>{product.Category}</td>
                        <td>{product.Brand}</td>
                        <td>{product.Type}</td>
                        <td>{product.Description}</td>
                        <td>{product.CreatedAt:yyyy-MM-dd HH:mm:ss}</td>
                        <td>{product.UpdatedAt:yyyy-MM-dd HH:mm:ss}</td>
                    </tr>";
            }

            html += @"
                </table>
            </body>
            </html>";

            return html;
        }
    }
}
