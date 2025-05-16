using Microsoft.AspNetCore.Mvc;
using VinylShopApi.Models;
using VinylShopApi.DTOs;
using VinylShopApi.Data;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;

namespace VinylShopApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly string jwtKey = "tajnykluczjwt123_supersekretnykluczJWT@2025!!";

        public AuthController(AppDbContext context)
        {
            _context = context;
        }

        //rejestracja
        [HttpPost("register")]
        public IActionResult Register(RegisterDto request)
        {
            if (_context.Users.Any(u => u.Username == request.Username))
                return BadRequest("Użytkownik już istnieje");

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
            var user = new User
            {
                Username = request.Username,
                PasswordHash = passwordHash,
                Role = "User"
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            return Ok("Użytkownik zarejestrowany");
        }

        //logowanie
        [HttpPost("login")]
        public IActionResult Login(LoginDto request)
        {
            var user = _context.Users.FirstOrDefault(u => u.Username == request.Username);
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                return Unauthorized("Nieprawidłowe dane logowania");

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(jwtKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Role, user.Role) //claim roli
                }),

                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var jwt = tokenHandler.WriteToken(token);

            return Ok(new { token = jwt });
        }

        //admin pobiera liste userow
        [HttpGet("users")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetAllUsers()
        {
            var users = _context.Users
                .Select(u => new
                {
                    u.Id,
                    u.Username,
                    u.Role
                })
                .ToList();

            return Ok(users);
        }


        //admin usuwa usera
        [HttpDelete("users/{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteUser(int id)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == id);
            if (user == null)
                return NotFound("Użytkownik nie istnieje");

            _context.Users.Remove(user);
            _context.SaveChanges();

            return Ok($"Użytkownik o ID {id} został usunięty");
        }

        //admin moze awansowac usera do admina
        [HttpPut("promote/{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult PromoteToAdmin(int id)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == id);
            if (user == null)
                return NotFound("Użytkownik nie istnieje");

            if (user.Role == "Admin")
                return BadRequest("Użytkownik już ma rolę Admin");

            user.Role = "Admin";
            _context.SaveChanges();

            return Ok($"Użytkownik {user.Username} został awansowany do roli Admin");
        }
    }
}
