using BuisinessLayer.service.Iservice;
using CommonLayer.Models.RequestDto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FundooNotes.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CollaborationController : ControllerBase
    {
        public readonly ICollaborationService collaborationBL;
        public CollaborationController(ICollaborationService collaborationBL)
        {
            this.collaborationBL = collaborationBL;
        }

        [HttpPost]
        public async Task<IActionResult> AddCollaborator(int NoteId, [FromBody] CollaborationRequest request)
        { 
            return Ok(await collaborationBL.AddCollaborator(NoteId, request));
        }
    }
}
