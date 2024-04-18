using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ResponceDto
{
    public  class LabelResponce
    {
        public int LabelId { get; set; }
        public string LabelName { get; set; }
        public int UserId { get; set; }
        public string UserNotesId { get; set; }
    }
}
