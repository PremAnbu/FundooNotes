using Azure.Core;
using BuisinessLayer.service.Iservice;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RepositaryLayer.Entity;

namespace FundooNotes.Controllers
{
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
        public async Task<int> CreateNewLabel(string labelName)
        {
           return await service.CreateNewLabel(labelName);
        }

        [HttpPut("{labelName}/{newLabelName}")]
        public async Task<int> UpdateLabelName(string labelName,string newLabelName)
        {
            return await service.UpdateLabelName(labelName,newLabelName);
        }

        [HttpDelete("{labelName}")]
        public async Task<int> DeleteLabel(string labelName)
        {
            return await service.DeleteLabel(labelName);
        }
    }
}
