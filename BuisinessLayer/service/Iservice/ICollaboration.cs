﻿using CommonLayer.Models.RequestDto;
using RepositaryLayer.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisinessLayer.service.Iservice
{
    public interface ICollaboration             
    {
       public bool AddCollaborator(int NoteId, CollaborationRequest Request,int userId);
       public Collaboration GetCollaborators(int CollaborationId,int userId);
       public  bool RemoveCollaborator(int CollaborationId, int userId);

    }
}
