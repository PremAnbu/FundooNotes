using Azure.Core;
using BuisinessLayer.service.Iservice;
using CommonLayer.Models;
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
        public ResponceStructure<string> AddCollaborator(int NoteId, [FromBody] CollaborationRequest request)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var result = collaborationBL.AddCollaborator(NoteId, request,userId);
            if (result)
                return new ResponceStructure<string>(200,"Collaboration added successfully.");
            else
                return new ResponceStructure<string>(500,"Failed to add collaboration.");
        }

        [HttpGet]
        public ResponceStructure<Collaboration> GetCollaborator(int CollaborationId)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            //Redis data structure create key and value p
            var cacheKey = $"Labels_{CollaborationId}";
            var cachedLabels =  _cache.GetString(cacheKey);

            // Return cached data if available
            if (!string.IsNullOrEmpty(cachedLabels))
            {
                Console.WriteLine("cashe memeory");
                return new ResponceStructure<Collaboration>(200,JsonSerializer.Deserialize<Collaboration>(cachedLabels));
            }
            // Cache data if not already cached
             
            var collaborations =  collaborationBL.GetCollaborators(CollaborationId,userId);
            if (collaborations != null)
            {
                var cacheOptions = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10) // Cache expiration time
                };
                 _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(collaborations), cacheOptions);
                return new ResponceStructure<Collaboration>(200,collaborations);
            }
            return new ResponceStructure<Collaboration>(500,"No collaborations found for the specified collaborations id.");
        }

        [HttpDelete]
        public  ResponceStructure<string> RemoveCollaborator(int CollaborationId)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var result =  collaborationBL.RemoveCollaborator(CollaborationId,userId);
            if (result)
                return new ResponceStructure<string>(200,"Collaborator removed successfully.");
            else
                return new ResponceStructure<string>(500,"Failed to remove collaborator.");
        }
    }
}
