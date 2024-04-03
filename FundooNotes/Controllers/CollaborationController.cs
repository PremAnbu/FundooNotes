using Azure.Core;
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
   // [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CollaborationController : ControllerBase
    {
        public readonly ICollaborationService collaborationBL;
        private readonly IDistributedCache _cache;

        public CollaborationController(ICollaborationService collaborationBL, IDistributedCache cache)
        {
            this.collaborationBL = collaborationBL;
            _cache = cache;
        }

        [HttpPost]
        public async Task<IActionResult> AddCollaborator(int NoteId, [FromBody] CollaborationRequest request)
        { 
            var result=await collaborationBL.AddCollaborator(NoteId, request);
            if (result)
                return Ok("Collaboration added successfully.");
            else
                return BadRequest("Failed to add collaboration.");
        }

        [HttpGet("{CollaborationId}")]
        public async Task<IActionResult> GetCollaborator(int CollaborationId)
        {
            //Redis data structure create key and value p
            var cacheKey = $"Labels_{CollaborationId}";
            var cachedLabels = await _cache.GetStringAsync(cacheKey);

            // Return cached data if available
            if (!string.IsNullOrEmpty(cachedLabels))
            {
                Console.WriteLine("cashe memeory");
                return Ok(JsonSerializer.Deserialize<Collaboration>(cachedLabels));
            }
            // Cache data if not already cached
             
            var collaborations = await collaborationBL.GetCollaborators(CollaborationId);
            if (collaborations != null)
            {
                var cacheOptions = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10) // Cache expiration time
                };
                await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(collaborations), cacheOptions);
                return Ok(collaborations);
            }
            return NotFound("No collaborations found for the specified collaborations id.");
        }

        [HttpDelete("RemoveCollaborator/{CollaborationId}")]
        public async Task<IActionResult> RemoveCollaborator(int CollaborationId)
        {
            var result = await collaborationBL.RemoveCollaborator(CollaborationId);
            if (result)
                return Ok("Collaborator removed successfully.");
            else
                return BadRequest("Failed to remove collaborator.");
        }
    }
}
