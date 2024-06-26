﻿using BuisinessLayer.service.Iservice;
using CommonLayer.Models.RequestDto;
using CommonLayer.Models.ResponceDto;
using RepositaryLayer.Entity;
using RepositaryLayer.Repositary.IRepo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisinessLayer.service.serviceImpl
{
    public class NotesImplementationBL : INotes
    {
        private readonly INotesRepo userNotesRepo;

        public NotesImplementationBL(INotesRepo userNotesRepo)
        {
            this.userNotesRepo = userNotesRepo;
        }

        private Notes MapToUserNotesEntity(NotesRequest request,int userId)
        {
            return new Notes
            {
                Description = request.Description,
                Title = request.Title,
                Colour = request.Colour,
                IsArchived = false,
                IsPinned = false,
                IsDeleted = false,
                UserId = userId
               // Email = request.Email,
            };
        }
        public List<NotesResponce> CreateUserNotes(NotesRequest request, int userId)
        {
            return userNotesRepo.CreateUserNotes(MapToUserNotesEntity(request,userId));
        }

        public NotesResponce UpdateNote(int noteId, NotesRequest updatedNote, int userId)
        {
            return userNotesRepo.UpdateNote(noteId, (MapToUserNotesEntity(updatedNote, userId)));
        }
        public bool DeleteNote(int noteId, int userId)
        {
            return userNotesRepo.DeleteNote(noteId,userId);
        }

        public List<NotesResponce> GetAllNotes(int userId)
        {
            return userNotesRepo.GetAllNotes(userId);
        }
        public List<NotesResponce> GetNoteById(int noteId, int userId)
        {
            return userNotesRepo.GetNoteById(noteId,userId);
        }

        public int UpdateArchive(int noteId, int userId)
        {
            return userNotesRepo.UpdateArchive(noteId,userId);
        }
        public int UpdateTrash(int noteId, int userId)
        {
            return userNotesRepo.UpdateTrash(noteId, userId);
        }
        public int UpdateNoteColour(int noteId,int userId, string colour)
        {
            return userNotesRepo.UpdateNoteColour(noteId,userId,colour);

        }
    }
}
