using BuisinessLayer.service.Iservice;
using CommonLayer.Models.RequestDto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FundooNotes.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserNotesController : ControllerBase
    {
        private readonly IUserNotesService service;
        public UserNotesController(IUserNotesService service)
        {
            this.service = service;
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
            return Ok(await service.DeleteNoteAsync(noteId));
        }

        [HttpGet("{email}")]
        public async Task<IActionResult> GetAllNotes(string email)
        {
            return Ok(await service.GetAllNoteAsync(email));
        }
    }
}
