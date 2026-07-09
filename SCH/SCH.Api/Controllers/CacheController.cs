namespace SCH.API.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using SCH.Models.Auth.Constants;
    using SCH.Shared.Cache;
    using SCH.Shared.Exceptions;

    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CacheController : ControllerBase
    {
        private readonly ICacheService _cacheService;

        public CacheController(ICacheService cacheService)
        {
            _cacheService = cacheService;
        }

        // POST: api/cache/clear
        [HttpPost("clear")]
        [Authorize(Policy = Policy.ClearCache)]
        public IActionResult Clear()
        {
            _cacheService.Clear();
            return Ok();
        }

        // DELETE: api/cache/{key}
        [HttpDelete("{key}")]
        [Authorize(Policy = Policy.ClearCache)]
        public IActionResult Remove(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw SCHDomainException.BadRequest("Key must not be empty.");
            }

            _cacheService.Remove(key);
            return Ok();
        }
    }
}
