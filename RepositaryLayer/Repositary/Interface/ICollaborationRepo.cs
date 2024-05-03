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
        public bool AddCollaborator(int NoteId, CollaborationRequest Request,int userId);
        public Collaboration GetCollaborators(int CollaborationId, int userId);
        public bool RemoveCollaborator(int CollaborationId,int userId);
    }
}
