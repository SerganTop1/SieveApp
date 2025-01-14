using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SieveApp.Data;
using SieveApp.Models;
using System.Security.Claims;

namespace SieveApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class HistoryController : ControllerBase
    {
        private readonly AppDbContext _dbContext;

        public HistoryController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("get-history")]
        public async Task<IActionResult> GetHistory()
        {
            var username = User.Identity.Name;
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == username);

            if (user == null)
                return Unauthorized("User not found.");

            var history = await _dbContext.Histories
                .Where(h => h.UserId == user.Id)
                .ToListAsync();

            return Ok(history);
        }

        [HttpDelete("delete-history")]
        public async Task<IActionResult> DeleteHistory()
        {
            var username = User.Identity.Name;
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == username);

            if (user == null)
                return Unauthorized("User not found.");

            var history = _dbContext.Histories.Where(h => h.UserId == user.Id);

            _dbContext.Histories.RemoveRange(history);
            await _dbContext.SaveChangesAsync();

            return Ok("History deleted successfully.");
        }
    }
}