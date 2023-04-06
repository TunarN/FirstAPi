using FirstApi.Data.DAL;
using FirstApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FirstApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProductController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var products = _context.Products.ToList();
            return StatusCode(200, products);
        }

        [HttpGet("{id}")]
        public IActionResult GetOne(int id)
        {
            Product product=_context.Products.FirstOrDefault(x => x.Id == id);
            if (product == null) return StatusCode(StatusCodes.Status404NotFound);
            return Ok(product);
        }

        [HttpPost]
        public IActionResult AddProduct(Product product)
        {
            _context.Products.Add(product);
            _context.SaveChanges();
            return StatusCode(StatusCodes.Status201Created,product);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteProduct(int id)
        {
            var product=_context.Products.FirstOrDefault(p=> p.Id == id);
            if(product==null) return NotFound();
            _context.Products.Remove(product);
            _context.SaveChanges();
            return StatusCode(StatusCodes.Status204NoContent);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateProduct(int id,Product product)
        {
            var existProduct = _context.Products.FirstOrDefault(p=>p.Id == id);
            if (existProduct == null) return NotFound();
            existProduct.Name= product.Name;
            existProduct.SalePrice= product.SalePrice;
            existProduct.CostPrice= product.CostPrice;
            existProduct.IsActive= product.IsActive;
            _context.SaveChanges();
            return StatusCode(StatusCodes.Status204NoContent);
        }

        [HttpPatch]
        public IActionResult ChangeStatus(int id,bool IsActive)
        {
            var existProduct = _context.Products.FirstOrDefault(p => p.Id == id);
            if (existProduct == null) return NotFound();
            existProduct.IsActive= IsActive;
            _context.SaveChanges();
            return StatusCode(StatusCodes.Status204NoContent);
        }
    }
}
