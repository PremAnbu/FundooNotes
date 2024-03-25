using CommonLayer.Models;
using Microsoft.AspNetCore.Mvc;
using RepositaryLayer.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisinessLayer.service.Iservice
{
    public interface INotesLabelService
    {
        public Task<int> CreateNewLabel(string labelName);
        public Task<int> UpdateLabelName(string labelName, string newLabelname);
        public Task<int> DeleteLabel(string labelName);


    }
}
