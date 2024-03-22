using CommonLayer.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositaryLayer.DTO.RequestDto
{
    public class UserRequest
    {
        [Required]
        public String FirstName { get; set; }
        public String LastName { get; set; }

        [Required]
        [UserRequestValidation]
        public String Email { get; set; }

        [Required]
        public String Password { get; set; }
    }
}
