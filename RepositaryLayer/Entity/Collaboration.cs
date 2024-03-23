using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositaryLayer.Entity
{
    public class Collaboration
    {
        public int CollaborationId { get; set; }

        public int UserId { get; set; }

        public int UserNotesId { get; set; }

        public string CollaboratorEmail { get; set; }
    }
}
