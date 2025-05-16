using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VinylShopApi.Data;
using VinylShopApi.Models;

namespace VinylShopApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CartController(AppDbContext context)
        {
            _context = context;
        }

        //dodanie produktu do koszyka
        [HttpPost("add")]
        public IActionResult AddToCart([FromBody] CartItem item)
        {
            var username = User.Identity?.Name;
            if (string.IsNullOrEmpty(username))
                return Unauthorized();

            item.Username = username;
            _context.CartItems.Add(item);
            _context.SaveChanges();

            return Ok(item);
        }

        //pobranie koszyka
        [HttpGet]
        public IActionResult GetMyCart()
        {
            var username = User.Identity?.Name;
            if (string.IsNullOrEmpty(username))
                return Unauthorized();

            var items = _context.CartItems
                .Where(ci => ci.Username == username)
                .ToList();

            return Ok(items);
        }

        //usunięcie produktu z koszyka
        [HttpDelete("{id}")]
        public IActionResult RemoveFromCart(int id)
        {
            var username = User.Identity?.Name;
            if (string.IsNullOrEmpty(username))
                return Unauthorized();

            var item = _context.CartItems.FirstOrDefault(ci => ci.Id == id && ci.Username == username);
            if (item == null)
                return NotFound("Produkt nie istnieje w Twoim koszyku");

            _context.CartItems.Remove(item);
            _context.SaveChanges();

            return Ok("Produkt usunięty z koszyka");
        }

        
    }
}
