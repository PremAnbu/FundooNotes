using CommonLayer.Models.RequestDto;
using RepositaryLayer.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositaryLayer.Repositary.IRepo
{
    public interface ICollaborationRepo
    {
        public Task<bool> AddCollaborator(int NoteId, CollaborationRequest Request);
        public Task<IEnumerable<Collaboration>> GetCollaborators(int CollaborationId);
        public Task<bool> RemoveCollaborator(int CollaborationId);
    }
}
