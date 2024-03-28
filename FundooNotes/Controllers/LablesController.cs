using Azure.Core;
using BuisinessLayer.service.Iservice;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RepositaryLayer.Entity;

namespace FundooNotes.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class LabelsController : ControllerBase
    {
        private readonly INotesLabelService service;
        public LabelsController(INotesLabelService service)
        {
            this.service = service;
        }

        [HttpPost("{labelName}")]
        public async Task<IActionResult> CreateNewLabel(string labelName)
        {
           var result = await service.CreateNewLabel(labelName);
            if (result==1)
                return Ok("Label Created successfully.");
            else
                return BadRequest("Failed to add Label.");
        }

        [HttpPut("{labelName}/{newLabelName}")]
        public async Task<IActionResult> UpdateLabelName(string labelName,string newLabelName)
        {
            var result=await service.UpdateLabelName(labelName,newLabelName);
            if (result == 1)
                return Ok("Label Name Updated  successfully.");
            else
                return BadRequest("Failed to Update  Label Name");
        }

        [HttpDelete("{labelName}")]
        public async Task<IActionResult> DeleteLabel(string labelName)
        {
            var result = await service.DeleteLabel(labelName);
            if (result == 1)
                return Ok("Label Deleted successfully.");
            else
                return BadRequest("Failed to Delete Label.");
        }
    }
}
