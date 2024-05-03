using CommonLayer.Models.ResponceDto;
using RepositaryLayer.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositaryLayer.Repositary.IRepo
{
    public interface INotesRepo
    {
        public List<NotesResponce> CreateUserNotes(Notes request);
        public NotesResponce UpdateNote(int noteId, Notes updatedNote);
        public bool DeleteNote(int noteId,int userId);
        public List<NotesResponce> GetAllNotes(int userId);
        public List<NotesResponce> GetNoteById(int noteId,int userId);
        public int UpdateArchive(int noteId, int userId);
        public int UpdateTrash(int noteId, int userId);
        public int UpdateNoteColour(int noteId, int userId,string colour);

    }
}
