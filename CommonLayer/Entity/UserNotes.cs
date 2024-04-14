//using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositaryLayer.Entity
{
    public class UserNotes
    {
        public int UserNotesId { get; set; }
        [Required]
        public string Title { get; set; }
        public string Description { get; set; }

        [DefaultValue("2022-05-20T12:12:55.389Z")]
        public string Colour { get; set; }
        public bool IsArchived { get; set; }
        public bool IsPinned { get; set; }
        public bool IsDeleted { get; set; }
        public int UserId { get; set; }
        public string Email {  get; set; }

    }
}
