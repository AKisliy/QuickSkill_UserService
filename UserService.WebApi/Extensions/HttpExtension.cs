using System.Security.Claims;
using UserService.Core.Exceptions;

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