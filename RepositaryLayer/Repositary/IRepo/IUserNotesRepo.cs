using CommonLayer.Models.ResponceDto;
using RepositaryLayer.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositaryLayer.Repositary.IRepo
{
    public interface IUserNotesRepo
    {
        public Task<IEnumerable<UserNotesResponce>> CreateUserNotes(UserNotes request);
        public Task<UserNotesResponce> UpdateNoteAsync(int noteId, UserNotes updatedNote);
        public Task<bool> DeleteNoteAsync(int noteId);
        public Task<IEnumerable<UserNotesResponce>> GetAllNoteAsync(string email);
    }
}
