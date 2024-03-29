using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using UserService.Core.Interfaces.Services;
using UserService.WebApi.Dtos;
using UserService.Infrastructure;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authentication;

namespace UserService.WebApi.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IOptions<MyCookiesOptions> _cookiesOptions;

        public AuthController(IAuthService authService, IOptions<MyCookiesOptions> cookiesOptions)
        {
            _authService = authService;
            _cookiesOptions = cookiesOptions;
        }

        /// <summary>
        /// Register new user
        /// </summary>
        /// <param name="user">Request body</param>
        /// <response code="200">Successfully registered</response>
        /// <response code="400">Bad request body</response>
        /// <response code="409">Conflict while creating</response>
        [HttpPost("register")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.Conflict)]
        public async Task<IActionResult> Register([FromBody] UserRegisterRequest user)
        {
            await _authService.Register(user.Firstname, user.Lastname, user.Email, user.Password);
            return Ok();
        }

        /// <summary>
        /// Login user
        /// </summary>
        /// <param name="user">Request body</param>
        /// <response code="200">Successfully login</response>
        /// <response code="404">User not found/invalid credentials</response>
        [HttpPost("login")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> Login([FromBody] UserLoginRequest user)
        {
            var token = await _authService.Login(user.Email, user.Password);
            HttpContext.Response.Cookies.Append(_cookiesOptions.Value.TokenFieldName, token);
            return Ok();
        }

        [HttpGet("login/github")]
        public IResult GitHubLogin()
        {
            return Results.Challenge(
                new AuthenticationProperties()
                {
                    RedirectUri = "https://localhost:7182/swagger/index.html"
                },
                new List<string>() { "github" }
            );
        }

        /// <summary>
        /// Verify user(called automatically)
        /// </summary>
        /// <param name="token">Verification token</param>
        /// <response code="200">Successfully verified</response>
        /// <response code="400">Can't verify</response>
        [HttpGet("verify")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> Verify(string token)
        {
            await _authService.Verify(token);
            return Ok();
        }

        /// <summary>
        /// Forgot password
        /// </summary>
        /// <param name="email">Email of user</param>
        /// <response code="400">Empty of incorrect email</response>
        /// <response code="202">Email sent to mailbox</response>
        /// <response code="404">No user with this email</response>
        [HttpPost("forgot-password")]
        [ProducesResponseType((int)HttpStatusCode.Accepted)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> ForgotPassword([EmailAddress] string email)
        {
            await _authService.ForgotPassword(email);
            return Accepted();
        }

        /// <summary>
        /// Reset password
        /// </summary>
        /// <param name="token">Reset token(get it from URL body)</param>
        /// <param name="password">New password</param>
        /// <response code="403">Empty of incorrect token</response>
        /// <response code="200">Success</response>
        /// <response code="404">No user found with this token</response>
        [HttpGet("reset-password")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(403)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> ResetPassword(string token, string password)
        {
            await _authService.ResetPassword(password, token);
            return Ok();
        }
    }
}