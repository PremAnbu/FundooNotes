using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLayer.Models
{
    /// <summary>
    /// Represents the model for Notes Label Request
    /// </summary>
    public class NotesLabel
    {
        [Required(ErrorMessage = "NoteId should not be empty")]
        public long NoteId { get; set; }

        [Required(ErrorMessage = "LabelName should not be empty")]
        public string LabelName { get; set; }
    }
}
