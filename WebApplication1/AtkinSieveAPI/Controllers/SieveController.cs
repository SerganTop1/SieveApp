using Microsoft.AspNetCore.Mvc;
using SieveApp.Services;

namespace SieveApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SieveController : ControllerBase
    {
        private readonly SieveService _sieveService;

        public SieveController(SieveService sieveService)
        {
            _sieveService = sieveService;
        }

        [HttpPost("prime-numbers")]
        public IActionResult GetPrimes([FromBody] int limit)
        {
            if (limit < 1 || limit > 50)
                return BadRequest("Limit must be between 1 and 50.");

            var primes = _sieveService.SieveOfAtkin(limit);
            return Ok(primes);
        }
    }
}