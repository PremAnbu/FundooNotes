using BuisinessLayer.service.Iservice;
using CommonLayer.Models.RequestDto;
using RepositaryLayer.Repositary.IRepo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisinessLayer.service.serviceImpl
{
    public class CollaborationServiceImpl : ICollaborationService
    {
        private readonly ICollaborationRepo collaborationRL;

        public CollaborationServiceImpl(ICollaborationRepo collaborationRL)
        {
            this.collaborationRL = collaborationRL;
        }
        public Task<bool> AddCollaborator(int NoteId, CollaborationRequest Request)
        {
            return collaborationRL.AddCollaborator(NoteId, Request);
        }
    }
}
