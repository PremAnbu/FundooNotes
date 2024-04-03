using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CommonLayer.Models
{
    public class ResponceStructure<T>
    {
        [JsonIgnore]
        public int StatusCode { get; set; }
        public bool Success { get; set; } = true;
        public String Message { get; set; } = string.Empty;
        public T? Data { get; set; }
    }
}
