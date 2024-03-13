using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserService.Infrastructure.Options
{
    public class GithubOptions
    {
        public string ClientId { get; set; } = null!;
        public string SecretKey { get; set; } = null!;
    }
}