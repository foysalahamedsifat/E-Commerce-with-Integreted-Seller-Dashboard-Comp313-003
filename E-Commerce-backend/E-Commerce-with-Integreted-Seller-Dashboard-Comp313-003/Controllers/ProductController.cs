using E_Commerce_Backend.Models;
using JWTAuthentication.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;

namespace JWTAuthentication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly string _imageUploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images"); // Folder to save images

        public ProductController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/products
        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            var products = await _context.Products.ToListAsync();
            return Ok(products);
        }

        // GET: api/products/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();
            return Ok(product);
        }

        // POST: api/products
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddProduct([FromForm] Product product, IFormFile image)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (image != null)
            {
                var imageUrl = await SaveImageAsync(image);
                product.ImageUrl = imageUrl;
            }

            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetProduct), new { id = product.ProductId }, product);
        }

        // PUT: api/products/{id}
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromForm] Product updatedProduct, IFormFile image)
        {
            if (id != updatedProduct.ProductId) return BadRequest();

            if (image != null)
            {
                var imageUrl = await SaveImageAsync(image);
                updatedProduct.ImageUrl = imageUrl;
            }

            _context.Entry(updatedProduct).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/products/{id}
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // Helper method to save image and return the URL
        private async Task<string> SaveImageAsync(IFormFile image)
        {
            if (image == null || image.Length == 0) return null;

            var filePath = Path.Combine(_imageUploadFolder, Guid.NewGuid().ToString() + Path.GetExtension(image.FileName));

            // Ensure directory exists
            if (!Directory.Exists(_imageUploadFolder))
            {
                Directory.CreateDirectory(_imageUploadFolder);
            }

            // Save the image to the server
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }

            // Return the image URL (can be adjusted based on your URL structure)
            return $"/images/{Path.GetFileName(filePath)}";
        }
    }
}
