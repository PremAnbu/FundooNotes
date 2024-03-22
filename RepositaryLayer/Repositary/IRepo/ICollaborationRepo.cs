using CommonLayer.Models.RequestDto;
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

    }
}
