using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Org.BouncyCastle.Crypto.Engines;
using UserService.Core.Exceptions;
using UserService.Infrastructure;

namespace UserService.WebApi.Extensions
{
    public static class HttpExtension
    {
        public static int GetUserId(this HttpContext context)
        {
            Claim? claim = context.User.FindFirst("userId");
            if(claim == null)
                throw new BadRequestException("Check your token");
            int id;
            if(!int.TryParse(claim.Value, out id))
                throw new BadRequestException("Invalid field in token");
            return id;
        }
    }
}