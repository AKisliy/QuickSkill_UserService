using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserService.WebApi.Dtos
{
    public class BadgeRequest
    {
        public string Name { get; set; } = null!;
        public string Photo { get; set; } = null!;
        public string GrayPhoto { get; set; } = null!;
        public int Required { get; set; }
        public string Task { get; set; } = null!;
    }
}