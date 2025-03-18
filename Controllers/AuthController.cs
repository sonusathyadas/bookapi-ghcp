using BookAPI.Data;
using BookAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BookAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly BookContext _context;
        private readonly IConfiguration _configuration;
    
        public AuthController(BookContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }
        
        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel loginModel)
        {
            // Validate user credentials (this should check against a database in a real application)
            if (loginModel.Username == "testuser" && loginModel.Password == "password")
            {
                var token = GenerateJwtToken(loginModel.Username);
                return Ok(new { token });
            }

            return Unauthorized("Invalid credentials");
        }

        private string GenerateJwtToken(string username)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username)
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    
        [HttpPost("register")]
        public IActionResult Register([FromBody] User user)
        {
            
            // Check if the username or email already exists
            if (_context.Users.Any(u => u.Username == user.Username || u.Email == user.Email))
            {
                return BadRequest("Username or email already exists.");
            }

            // Add the new user to the database
            _context.Users.Add(user);
            _context.SaveChanges();
            

            return Ok("User registered successfully.");
        }
    }
}
