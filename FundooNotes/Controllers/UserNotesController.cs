using BuisinessLayer.service.Iservice;
using CommonLayer.Models.RequestDto;
using CommonLayer.Models.ResponceDto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using RepositaryLayer.Entity;
using System.Security.Claims;
using System.Text.Json;

namespace FundooNotes.Controllers
{
  //  [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserNotesController : ControllerBase
    {
        private readonly IUserNotesService service;
        private readonly IDistributedCache _cache;

        public UserNotesController(IUserNotesService service, IDistributedCache cache)
        {
            this.service = service;
            _cache = cache;

        }

        [HttpPost("User Notes")]
        public async Task<IActionResult> CreateUserNotes(UserNotesRequest request)
        {
            return Ok(await service.CreateUserNotes(request));
        }

        [HttpPut("{noteId}")]
        public async Task<IActionResult> UpdateNoteAsync(int noteId, [FromBody] UserNotesRequest updatedNote)
        {
            return Ok(await service.UpdateNoteAsync(noteId, updatedNote));
        }

        [HttpDelete("{noteId}")]
        public async Task<IActionResult> DeleteNoteAsync(int noteId)
        {
            var result = await service.DeleteNoteAsync(noteId);
            if (result)
                return Ok("UserNotes removed successfully.");
            else
                return BadRequest("Failed to remove UserNotes.");
        }

        [HttpGet("{email}")]
        public async Task<IActionResult> GetAllNotes(string email)
        {
            //Redis data structure create key and value p
            var cacheKey = $"Labels_{email}";
            var cachedLabels = await _cache.GetStringAsync(cacheKey);

            // Return cached data if available
            if (!string.IsNullOrEmpty(cachedLabels))
            {
                Console.WriteLine("cashe memeory");
                return Ok(JsonSerializer.Deserialize<List<UserNotesResponce>>(cachedLabels));
            }
            // Cache data if not already cached
            var userNotes = await service.GetAllNoteAsync(email);
            if (userNotes != null)
            {
                var cacheOptions = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10) // Cache expiration time
                };
                await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(userNotes), cacheOptions);
                return Ok(userNotes);
            }
            return NotFound("No userNotes found for the specified user Email.");
        }
    }
}
