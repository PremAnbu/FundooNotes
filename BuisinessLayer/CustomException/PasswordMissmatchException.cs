using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisinessLayer.CustomException
{
    public class PasswordMissmatchException : Exception
    {
        public PasswordMissmatchException() { 
        
        }
        public PasswordMissmatchException(string message) : base(message)
        {

        }
    }
}
