using CommonLayer.Models.RequestDto;
using CommonLayer.Models.ResponceDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisinessLayer.service.Iservice
{
    public interface INotes
    {
        public List<NotesResponce> CreateUserNotes(NotesRequest request,int userId);
        public NotesResponce UpdateNote(int noteId, NotesRequest updatedNote,int userId);
        public bool DeleteNote(int noteId,int userId);
        public List<NotesResponce> GetAllNotes(int userId);
        public List<NotesResponce> GetNoteById(int noteId, int userId);
        public int UpdateArchive(int noteId,int userId);
        public int UpdateTrash(int noteId, int userId);
        public int UpdateNoteColour(int noteId,int userId,string colour);


    }
}
