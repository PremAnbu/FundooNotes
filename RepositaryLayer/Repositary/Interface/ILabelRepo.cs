using Microsoft.AspNetCore.Mvc;
using ModelLayer.Models.ResponceDto;
using RepositaryLayer.Entity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositaryLayer.Repositary.IRepo
{
    public interface ILabelRepo
    {
        public int CreateNewLabel(string labelName, int noteId, int userId);
        public int UpdateLabelName(string labelName, string newLabelname, int noteId, int userId);
        public int DeleteLabel(string labelName, int noteId, int userId);
        public List<LabelResponce> GetLabel(int userId);

    }
}
