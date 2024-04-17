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
        public Task<IEnumerable<UserNotesResponce>> CreateUserNotes(UserNotesRequest request,int userId);
        public Task<UserNotesResponce> UpdateNote(int noteId, UserNotesRequest updatedNote,int userId);
        public Task<bool> DeleteNote(int noteId);
        public Task<IEnumerable<UserNotesResponce>> GetAllNotes(int userId);
    }
}
