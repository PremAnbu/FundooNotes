using BuisinessLayer.service.Iservice;
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
    public class UserNotesServiceImpl : IUserNotesService
    {
        private readonly IUserNotesRepo userNotesRepo;

        public UserNotesServiceImpl(IUserNotesRepo userNotesRepo)
        {
            this.userNotesRepo = userNotesRepo;
        }

        private UserNotes MapToUserNotesEntity(UserNotesRequest request,int userId)
        {
            return new UserNotes
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
        public Task<IEnumerable<UserNotesResponce>> CreateUserNotes(UserNotesRequest request, int userId)
        {
            return userNotesRepo.CreateUserNotes(MapToUserNotesEntity(request,userId));
        }

        public Task<UserNotesResponce> UpdateNote(int noteId, UserNotesRequest updatedNote, int userId)
        {
            return userNotesRepo.UpdateNote(noteId, (MapToUserNotesEntity(updatedNote, userId)));
        }
        public Task<bool> DeleteNote(int noteId)
        {
            return userNotesRepo.DeleteNote(noteId);
        }

        public Task<IEnumerable<UserNotesResponce>> GetAllNotes(int userId)
        {
            return userNotesRepo.GetAllNotes(userId);
        }
    }
}
