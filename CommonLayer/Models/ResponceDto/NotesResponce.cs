using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLayer.Models.ResponceDto
{
    public class NotesResponce
    {
        public int UserNotesId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Colour { get; set; }
        public DateTime Reminder {  get; set; }
        public bool IsArchived { get; set; } = false;
        public bool IsPinned { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}
