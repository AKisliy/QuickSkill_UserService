using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using UserService.Core.Interfaces.Services;
using UserService.WebApi.Dtos;
using UserService.Infrastructure;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using UserService.WebApi.Extensions;
using UserService.Infrastructure.Options;

namespace UserService.WebApi.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly MyCookiesOptions _cookiesOptions;
        private readonly FrontendOptions _frontendOptions;

        public AuthController(IAuthService authService, IOptions<MyCookiesOptions> cookiesOptions, IOptions<FrontendOptions> frontendOptions)
        {
            _authService = authService;
            _cookiesOptions = cookiesOptions.Value;
            _frontendOptions = frontendOptions.Value;
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
            var tokens = await _authService.Login(user.Email, user.Password);
            HttpContext.Response.Cookies.Append(_cookiesOptions.TokenFieldName, tokens.JwtToken);
            return Ok(new LoginResponse{ RefreshToken = tokens.RefreshToken });
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
            return Redirect($"{_frontendOptions.BaseUrl}/swagger");
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
        /// Reset password redirection(you don't call it). It's called when user follow the link from his inbox. Then he goes to your /reset-password page with token field in URL.
        /// </summary>
        /// <param name="token">Reset token</param>
        /// <response code="302">Redirection</response>
        [HttpGet("reset-password/redirect")]
        [ProducesResponseType((int)HttpStatusCode.Redirect)]
        public IActionResult RedirectToResetPassword(string token)
        {
            var frontendUrl = $"{_frontendOptions.BaseUrl}/reset-password?token={token}";
            return Redirect(frontendUrl);
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

        /// <summary>
        /// Refresh JWT token
        /// </summary>
        /// <param name="request">Body has two fields: accessToken - expired JWT token, refreshToken - refresh token</param>
        /// <response code="401">Something went wrong while validating JWT or refresh token</response>
        /// <response code="200">Success</response>
        /// <response code="500">Server problems :(</response>
        [HttpPost("refresh")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Refresh([FromBody] RefreshRequest request)
        {
            var newToken = await _authService.GetNewToken(request.AccessToken, request.RefreshToken);
            HttpContext.Response.Cookies.Append(_cookiesOptions.TokenFieldName, newToken);
            return Ok();
        }

        /// <summary>
        /// Revoke refresh token(by JWT). Use it when you need to delete refresh token from DB.
        /// </summary>
        /// <response code="200">Success</response>
        /// <response code="400">Bad JWT</response>
        /// <response code="500">Server problems :(</response>
        [Authorize]
        [HttpDelete("refresh/revoke")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Revoke()
        {
            var userId = HttpContext.GetUserId();
            await _authService.RevokeRefreshToken(userId);
            return Ok();
        }

        /// <summary>
        /// It's for NGINX
        /// </summary>
        /// <response code="200">Success</response>
        /// <response code="401">Unathorized</response>
        [HttpGet("verify-request")]
        [Authorize]
        public IActionResult VerifyToken()
        {
            var userClaims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();

            foreach (var claim in userClaims)
            {
                Response.Headers.Append($"X-Claim-{claim.Type}", claim.Value);
            }
            return Ok();
        }
    }
}