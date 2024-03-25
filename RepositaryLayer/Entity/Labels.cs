using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositaryLayer.Entity
{
    public class Labels
    {
        public int NoteId { get; set; }
        public string LabelName { get; set; }
    }
}
