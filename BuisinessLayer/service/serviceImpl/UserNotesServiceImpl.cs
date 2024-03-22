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

        private UserNotes MapToUserNotesEntity(UserNotesRequest request)
        {
            return new UserNotes
            {
                Description = request.Description,
                Title = request.Title,
                Colour = request.Colour,
                IsArchived = false,
                IsPinned = false,
                IsDeleted = false,
                Email = request.Email,
            };
        }
        public Task<IEnumerable<UserNotesResponce>> CreateUserNotes(UserNotesRequest request)
        {
            Console.WriteLine("service impl");
            return userNotesRepo.CreateUserNotes(MapToUserNotesEntity(request));
        }

        public Task<UserNotesResponce> UpdateNoteAsync(int noteId, UserNotesRequest updatedNote)
        {
            return userNotesRepo.UpdateNoteAsync(noteId, (MapToUserNotesEntity(updatedNote)));
        }
        public Task<bool> DeleteNoteAsync(int noteId)
        {
            return userNotesRepo.DeleteNoteAsync(noteId);
        }

        public Task<IEnumerable<UserNotesResponce>> GetAllNoteAsync(string email)
        {
            return userNotesRepo.GetAllNoteAsync(email);
        }
    }
}
