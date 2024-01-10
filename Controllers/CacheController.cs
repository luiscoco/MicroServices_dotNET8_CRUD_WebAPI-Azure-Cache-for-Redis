using AzureCacheforRedis.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;


namespace AzureCacheforRedis.Controllers
{
    [ApiController]
    [Route("[controller]")]

    public class CacheController : ControllerBase
    {
        private readonly RedisCacheService _cacheService;

        public CacheController(RedisCacheService cacheService)
        {
            _cacheService = cacheService;
        }

        [HttpPost]
        public async Task<IActionResult> Set(string key, string value)
        {
            await _cacheService.SetAsync(key, value);
            return Ok();
        }

        [HttpGet("{key}")]
        public async Task<IActionResult> Get(string key)
        {
            var value = await _cacheService.GetAsync(key);
            return Ok(value);
        }

        [HttpDelete("{key}")]
        public async Task<IActionResult> Delete(string key)
        {
            await _cacheService.DeleteAsync(key);
            return Ok();
        }

        // Add other endpoints for CRUD operations

    }
}
