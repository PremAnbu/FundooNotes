using Microsoft.AspNetCore.Mvc;
using RepositaryLayer.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositaryLayer.Repositary.IRepo
{
    public interface INotesLabelRepo
    {
        public Task<int> CreateNewLabel(string labelName);
        public Task<int> UpdateLabelName(string labelName, string newLabelname);
        public Task<int> DeleteLabel(string labelName);
    }
}
