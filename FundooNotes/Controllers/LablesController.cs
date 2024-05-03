using Azure.Core;
using BuisinessLayer.service.Iservice;
using CommonLayer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.Models.ResponceDto;
using RepositaryLayer.Entity;
using System.Security.Claims;

namespace FundooNotes.Controllers
{
   // [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class LabelsController : ControllerBase
    {
        private readonly INotesLabelService service;
        public LabelsController(INotesLabelService service)
        {
            this.service = service;
        }

        [HttpPost]
        public  ResponceStructure<string> CreateNewLabel(string labelName,int notesId)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var result =  service.CreateNewLabel(labelName, notesId, userId);
            if (result==1)
                return new ResponceStructure<string>("Label Created successfully.");
            else
                return new ResponceStructure<string>( "Label Created successfully.");
        }

        [HttpPut]
        public  ResponceStructure<string> UpdateLabelName(string labelName,string newLabelName,int noteId)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var result = service.UpdateLabelName(labelName,newLabelName,noteId,userId);
            if (result == 1)
                return new ResponceStructure<string>("Label Name Updated  successfully.");
            else
                return new ResponceStructure<string>("Failed to Update  Label Name");
        }

        [HttpDelete]
        public ResponceStructure<string> DeleteLabel(string labelName,int noteId)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var result =  service.DeleteLabel(labelName,noteId,userId);
            if (result == 1)
                return new ResponceStructure<string>("Label Deleted successfully.");
            else
                return new ResponceStructure<string>("Failed to Delete Label.");
        }

        [HttpGet("GetLabel")]
        public  ResponceStructure<List<LabelResponce>> GetLabel()
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var label =  service.GetLabel(userId);
            return new ResponceStructure<List<LabelResponce>>(label);

        }
    }
}
