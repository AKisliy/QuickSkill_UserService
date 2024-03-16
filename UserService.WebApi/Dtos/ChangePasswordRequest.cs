using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Org.BouncyCastle.Asn1.Mozilla;

namespace UserService.WebApi.Dtos
{
    public class ChangePasswordRequest
    {
        public string OldPassword { get; set; } = null!;

        public string NewPassword { get; set; } = null!;
    }
}