using CommonLayer.Models.RequestDto;
using CommonLayer.Models.ResponceDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisinessLayer.service.Iservice
{
    public interface IUserNotesService
    {
        public Task<IEnumerable<UserNotesResponce>> CreateUserNotes(UserNotesRequest request);
        public Task<UserNotesResponce> UpdateNoteAsync(int noteId, UserNotesRequest updatedNote);
        public Task<bool> DeleteNoteAsync(int noteId);
        public Task<IEnumerable<UserNotesResponce>> GetAllNoteAsync(string email);
    }
}
