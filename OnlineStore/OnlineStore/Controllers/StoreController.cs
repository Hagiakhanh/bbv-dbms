using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineStore.DTOs.Store;
using OnlineStore.Services.Store;

namespace OnlineStore.Controllers
{
    [ApiController]
    [Authorize]
    public class StoreController : ControllerBase
    {
        private readonly IStoreService _storeService;

        public StoreController(IStoreService storeService)
        {
            _storeService = storeService;
        }

        [HttpGet("store")]
        public async Task<ActionResult<StoreResponse>> GetStore()
        {
            var result = await _storeService.GetStoreAsync();
            if (result == null)
            {
                return NotFound(new { message = "Store context not found." });
            }

            return Ok(result);
        }

        [HttpGet("updates/unread-count")]
        public async Task<ActionResult<UnreadCountResponse>> GetUnreadCount()
        {
            var result = await _storeService.GetUnreadNotificationCountAsync();
            return Ok(result);
        }
    }
}
