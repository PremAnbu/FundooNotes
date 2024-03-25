using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositaryLayer.GlobalCustomException
{
    public class UserNotesNotPresentException : Exception
    {
        public UserNotesNotPresentException()
        {
        }

        public UserNotesNotPresentException(string message)
            : base(message)
        {
        }

        public UserNotesNotPresentException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
