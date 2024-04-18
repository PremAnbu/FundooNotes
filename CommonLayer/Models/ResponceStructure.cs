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
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }

        public ResponceStructure(int v)
        {
        }
        public ResponceStructure(int statusCode, bool success, string message, T? data)
        {
            StatusCode = statusCode;
            Success = success;
            Message = message;
            Data = data;
        }
        public ResponceStructure(int statusCode, string message)
        {
            StatusCode = statusCode;
            Message = message;
        }
        public ResponceStructure(bool success, string message)
        {
            Success = success;
            Message = message;
        }
        
        public ResponceStructure(int statusCode, string message, T? data)
        {
            StatusCode = statusCode;
            Message = message;
            Data = data;
        }
        public ResponceStructure(int statusCode, T? data)
        {
            StatusCode = statusCode;
            Data = data;
        }
        public ResponceStructure(int statusCode, bool success , T? data)
        {
            StatusCode = statusCode;
            Success = success;
            Data = data;
        }
    }

}
