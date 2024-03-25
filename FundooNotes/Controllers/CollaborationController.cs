using Azure.Core;
using BuisinessLayer.service.Iservice;
using CommonLayer.Models.RequestDto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RepositaryLayer.Entity;
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

        [HttpGet("{CollaborationId}")]
        public async Task<IActionResult> GetCollaborator(int CollaborationId)
        {
            return Ok(await collaborationBL.GetCollaborators(CollaborationId));
        }

        [HttpDelete("RemoveCollaborator/{CollaborationId}")]
        public async Task<IActionResult> RemoveCollaborator(int CollaborationId)
        {
             return Ok(await collaborationBL.RemoveCollaborator(CollaborationId));
        }
    }
}
