using BuisinessLayer.service.Iservice;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.Models.ResponceDto;
using RepositaryLayer.Entity;
using RepositaryLayer.GlobalCustomException;
using RepositaryLayer.Repositary.IRepo;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisinessLayer.service.serviceImpl
{
    public class NotesLabelServiceImpl : INotesLabelService
    {
        private readonly ILabelRepo notesLabelRepo;

        public NotesLabelServiceImpl(ILabelRepo notesLabelRepo)
        {
            this.notesLabelRepo = notesLabelRepo;
        }

        public int CreateNewLabel(string labelName, int noteId, int userId)
        {
            try
            {
                return notesLabelRepo.CreateNewLabel(labelName,noteId,userId);
            }
            catch (Exception ex)
            {
                throw new LabelNotPresentException("Label Not Present");
            }
        }

        public int UpdateLabelName(string labelName, string newLabelname, int noteId, int userId)
        {
            try
            {
                return notesLabelRepo.UpdateLabelName(labelName,newLabelname,noteId,userId);
            }
            catch (Exception ex)
            {
                throw new LabelNotPresentException("Label Not Present");
            }
        }

        public int DeleteLabel(string labelName, int noteId, int userId)
        {
            try
            {
                return notesLabelRepo.DeleteLabel(labelName,noteId,userId);
            }
            catch (Exception ex)
            {
                throw new LabelNotPresentException("Label Not Present");
            }
        }
        public List<LabelResponce> GetLabel(int userId)
        {
            try
            {
                return notesLabelRepo.GetLabel(userId);
            }
            catch (Exception ex)
            {
                throw new LabelNotPresentException("Label Not Present");
            }
        }


    }
}
