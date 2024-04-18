using CommonLayer.Models;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.Models.ResponceDto;
using RepositaryLayer.Entity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisinessLayer.service.Iservice
{
    public interface INotesLabelService
    {
        public int CreateNewLabel(string labelName, int noteId,int userId);
        public int UpdateLabelName(string labelName, string newLabelname, int noteId, int userId);
        public int DeleteLabel(string labelName, int noteId, int userId);
        public List<LabelResponce> GetLabel(int userId);



    }
}
