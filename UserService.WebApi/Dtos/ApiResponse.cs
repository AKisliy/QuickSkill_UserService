using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace UserService.WebApi.Dtos
{
    public class ApiResponse
    {
        
        public HttpStatusCode StatusCode { get; set; }
        public bool IsSucceed { get; set; }
        public List<String> ErrorMessages { get; set; } = new List<string>();
        public object? Result { get; set; }
    }
}