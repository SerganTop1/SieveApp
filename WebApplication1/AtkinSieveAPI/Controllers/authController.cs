using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SieveApp.Data;
using SieveApp.Models;
using SieveApp.Services;
using System.Security.Cryptography;
using System.Text;

namespace SieveApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly JwtService _jwtService;

        public AuthController(AppDbContext dbContext, JwtService jwtService)
        {
            _dbContext = dbContext;
            _jwtService = jwtService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] User user)
        {
            if (await _dbContext.Users.AnyAsync(u => u.Username == user.Username))
                return BadRequest("Username already exists.");

            user.PasswordHash = HashPassword(user.PasswordHash);
            user.Token = _jwtService.GenerateToken(user.Username);

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            return Ok(new { Token = user.Token });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] User user)
        {
            var existingUser = await _dbContext.Users
                .FirstOrDefaultAsync(u => u.Username == user.Username);

            if (existingUser == null  !VerifyPassword(user.PasswordHash, existingUser.PasswordHash))
                return Unauthorized("Invalid credentials.");

            existingUser.Token = _jwtService.GenerateToken(existingUser.Username);
            await _dbContext.SaveChangesAsync();

            return Ok(new { Token = existingUser.Token });
        }

        [HttpPatch("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordModel model)
        {
            var user = await _dbContext.Users
                .FirstOrDefaultAsync(u => u.Username == model.Username);

            if (user == null  !VerifyPassword(model.OldPassword, user.PasswordHash))
                return Unauthorized("Invalid credentials.");

            user.PasswordHash = HashPassword(model.NewPassword);
            user.Token = _jwtService.GenerateToken(user.Username);

            await _dbContext.SaveChangesAsync();

            return Ok(new { Token = user.Token });
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        private bool VerifyPassword(string password, string storedHash)
        {
            return HashPassword(password) == storedHash;
        }
    }

    public class ChangePasswordModel
    {
        public string Username { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
}