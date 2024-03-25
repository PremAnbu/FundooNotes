using BuisinessLayer.service.Iservice;
using Microsoft.AspNetCore.Mvc;
using RepositaryLayer.Entity;
using RepositaryLayer.Repositary.IRepo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisinessLayer.service.serviceImpl
{
    public class NotesLabelServiceImpl : INotesLabelService
    {
        private readonly INotesLabelRepo notesLabelRepo;

        public NotesLabelServiceImpl(INotesLabelRepo notesLabelRepo)
        {
            this.notesLabelRepo = notesLabelRepo;
        }

        public Task<int> CreateNewLabel(string labelName)
        {
            try
            {
                return notesLabelRepo.CreateNewLabel(labelName);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Task<int> UpdateLabelName(string labelName, string newLabelname)
        {
            try
            {
                return notesLabelRepo.UpdateLabelName(labelName,newLabelname);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Task<int> DeleteLabel(string labelName)
        {
            try
            {
                return notesLabelRepo.DeleteLabel(labelName);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
