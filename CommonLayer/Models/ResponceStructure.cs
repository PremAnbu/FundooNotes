using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLayer.Models
{
    public class ResponceStructure<T>
    {
        public bool Success { get; set; }
        public String Message { get; set; }
        public T Data { get; set; }
    }
}
