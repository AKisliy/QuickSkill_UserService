using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace UserService.WebApi.Dtos
{
    public class ErrorResponse
    {
        public List<String> ErrorMessages { get; set; } = new List<string>();
    }
}