using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using UserService.Core.Exceptions;
using UserService.WebApi.Dtos;

namespace UserService.WebApi.Handlers
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext,Exception exception,CancellationToken cancellationToken)
        {
            var errorResponse = new ErrorResponse
            {
                Message = exception.Message,
                Title = exception.GetType().Name
            };
            switch (exception)
            {
                case BadRequestException:
                    errorResponse.StatusCode = (int)HttpStatusCode.BadRequest;
                    break;
                case NotFoundException:
                    errorResponse.StatusCode = (int)HttpStatusCode.NotFound;
                    break;
                case CredentialsException:
                case TokenException:
                    errorResponse.StatusCode = (int)HttpStatusCode.Forbidden;
                    break;
                case ConflictException:
                    errorResponse.StatusCode = (int)HttpStatusCode.Conflict;
                    break;
                case UnathorizedException:
                    errorResponse.StatusCode = (int)HttpStatusCode.Unauthorized;
                    break;
                default:
                    errorResponse.StatusCode = (int)HttpStatusCode.InternalServerError;
                    errorResponse.Title = "Internal Server Error";
                    break;
            }
            httpContext.Response.StatusCode = errorResponse.StatusCode;
            await httpContext.Response.WriteAsJsonAsync(errorResponse, cancellationToken);
            return true;
        }
    }
}