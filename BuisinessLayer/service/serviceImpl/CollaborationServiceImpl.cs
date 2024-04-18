using BuisinessLayer.service.Iservice;
using CommonLayer.Models.RequestDto;
using RepositaryLayer.Entity;
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
        public bool AddCollaborator(int NoteId, CollaborationRequest Request,int userId)
        {
            return collaborationRL.AddCollaborator(NoteId, Request,userId);
        }
        public Collaboration GetCollaborators(int CollaborationId, int userId)
        {
            return collaborationRL.GetCollaborators(CollaborationId,userId);
        }

        public bool RemoveCollaborator(int CollaborationId, int userId)
        {
            return collaborationRL.RemoveCollaborator(CollaborationId,userId);
        }
    }
}
