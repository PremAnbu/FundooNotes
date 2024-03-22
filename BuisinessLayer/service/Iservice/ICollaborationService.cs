using CommonLayer.Models.RequestDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisinessLayer.service.Iservice
{
    public interface ICollaborationService
    {
       public Task<bool> AddCollaborator(int NoteId, CollaborationRequest Request);

    }
}
