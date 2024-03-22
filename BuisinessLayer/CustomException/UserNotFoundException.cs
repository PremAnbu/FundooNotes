using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisinessLayer.CustomException
{
    public class UserNotFoundException : Exception
    {
       public UserNotFoundException() { }
        public UserNotFoundException(string message) : base(message) { }
    }
}
