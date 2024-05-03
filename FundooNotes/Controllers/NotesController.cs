using BuisinessLayer.service.Iservice;
using CommonLayer.Models;
using CommonLayer.Models.RequestDto;
using CommonLayer.Models.ResponceDto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using RepositaryLayer.Entity;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Json;

namespace FundooNotes.Controllers
{
    // [Authorize]
    [Route("api/Notes")]
    [ApiController]
    public class NotesController : ControllerBase
    {
        private readonly INotes service;
        private readonly IDistributedCache _cache;
        public NotesController(INotes service, IDistributedCache cache)
        {
            this.service = service;
            _cache = cache;
        }

        [HttpPost]
        public ResponceStructure<List<NotesResponce>> CreateUserNotes(NotesRequest request)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            return new ResponceStructure<List<NotesResponce>>(service.CreateUserNotes(request, userId));
        }

        [AllowAnonymous]
        [HttpPut]
        public  ResponceStructure<NotesResponce> UpdateNoteAsync(int noteId, [FromBody] NotesRequest updatedNote)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            return new ResponceStructure<NotesResponce>(service.UpdateNote(noteId, updatedNote, userId));
        }

        [HttpDelete]
        public  ResponceStructure<string> DeleteNoteAsync(int noteId)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var result =  service.DeleteNote(noteId,userId);
            if (result)
                return new ResponceStructure<string>("UserNotes removed successfully.");
            else
                return new ResponceStructure<string>("Failed to remove UserNotes.");
        }

        [HttpGet]
        public ResponceStructure<List<NotesResponce>> GetAllNotes()
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            //Redis data structure create key and value p
            //var cacheKey = $"Labels_{userId}";
            //var cachedLabels =  _cache.GetString(cacheKey);

            //// Return cached data if available
            //if (!string.IsNullOrEmpty(cachedLabels))
            //{
            //    Console.WriteLine("cashe memeory");
            //    return new ResponceStructure<List<NotesResponce>>(200,JsonSerializer.Deserialize<List<NotesResponce>>(cachedLabels));
           // }
            // Cache data if not already cached
            var userNotes =  service.GetAllNotes(userId);
            return new ResponceStructure<List<NotesResponce>>( userNotes);

            //if (userNotes != null)
            //{
            //    var cacheOptions = new DistributedCacheEntryOptions
            //    {
            //        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10) // Cache expiration time
            //    };
            //     _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(userNotes), cacheOptions);
            //    return new ResponceStructure<List<NotesResponce>>(200, userNotes);
            //}
           // return new ResponceStructure < List < NotesResponce >> (404,"No userNotes found for the specified user Email.");
        }

        [HttpGet("GetByNoteId")]
        public ResponceStructure<List<NotesResponce>> GetNoteById(int noteId)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var userNotes =  service.GetNoteById(noteId,userId);
            return new ResponceStructure<List<NotesResponce>>(userNotes);

        }

        [HttpPut("UpdateColourByNoteId")]
        public int UpdateNoteColour(int noteId,string colour)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var val = service.UpdateNoteColour(noteId,userId,colour);
            return val;
        }

        [HttpPut("UpdateArchiveByNoteId")]
        public int UpdateArchive(int noteId)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var val = service.UpdateArchive(noteId, userId);
            return val;
        }

        [HttpPut("UpdateTrashByNoteId")]
        public int UpdateTrash(int noteId)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var val = service.UpdateTrash(noteId, userId);
            return val;
        }
    }
}
