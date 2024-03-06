using System.Net;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using UserService.Core.Interfaces;
using UserService.Core.Interfaces.Services;
using UserService.WebApi.Dtos;
using UserService.Infrastructure;
using System.ComponentModel.DataAnnotations;
using UserService.Core.Exceptions;

namespace UserService.WebApi.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IUsersService _usersService;
        private readonly IMapper _mapper;
        private readonly IAuthService _authService;
        private readonly IOptions<MyCookiesOptions> _cookiesOptions;

        public AuthController(IUsersService usersService, IAuthService authService, IOptions<MyCookiesOptions> cookiesOptions, IMapper mapper)
        {
            _usersService = usersService;
            _mapper = mapper;
            _authService = authService;
            _cookiesOptions = cookiesOptions;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterRequest user)
        {
            var response = new ApiResponse();
            if(user == null)
            {
                response.Result = HttpStatusCode.BadRequest;
                response.ErrorMessages.Add("Empty request body");
                response.IsSucceed = false;
                return BadRequest(response);
            }
            try{
                await _authService.Register(user.Firstname, user.Lastname, user.Email, user.Password);
                response.StatusCode = HttpStatusCode.OK;
                response.IsSucceed = true;
                return Ok(response);
            }
            catch(Exception ex)
            {
                response.StatusCode = HttpStatusCode.Conflict;
                response.IsSucceed = false;
                response.ErrorMessages.Add(ex.Message);
                return Conflict(response);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginRequest user)
        {
            var response = new ApiResponse();
            try
            {
                var token = await _authService.Login(user.Email, user.Password);
                HttpContext.Response.Cookies.Append(_cookiesOptions.Value.TokenFieldName, token);
                return Ok();
            }
            catch(Exception ex)
            {
                response.IsSucceed = false;
                response.ErrorMessages.Add(ex.Message);
                // response.ErrorMessages.Add(ex.Source);
                // response.ErrorMessages.Add(ex.StackTrace);
                response.StatusCode = HttpStatusCode.NotFound;
                return NotFound(response);
            }
        }

        [HttpGet("verify")]
        public async Task<IActionResult> Verify(string token)
        {
            var response = new ApiResponse();
            try
            {
                await _authService.Verify(token);
                response.IsSucceed = true;
                response.StatusCode = HttpStatusCode.OK;
                return Ok(response);
            }
            catch(Exception ex)
            {
                response.IsSucceed = false;
                response.StatusCode = HttpStatusCode.BadRequest;
                response.ErrorMessages.Add(ex.Message);
                return BadRequest(response);
            }
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([EmailAddress] string email)
        {
            if(string.IsNullOrEmpty(email))
                return BadRequest();
            var response = new ApiResponse();
            try
            {
                await _authService.ForgotPassword(email);
                response.IsSucceed = true;
                response.StatusCode = HttpStatusCode.Accepted;
                return Accepted(response);
            }
            catch(Exception ex)
            {
                response.IsSucceed = false;
                response.StatusCode = HttpStatusCode.NotFound;
                response.ErrorMessages.Add(ex.Message);
                return NotFound(response);
            }
        }

        [HttpGet("reset-password")]
        public async Task<IActionResult> ResetPassword(string token, string password)
        {
            if(string.IsNullOrEmpty(token))
                return BadRequest();
            var response = new ApiResponse();
            try
            {
                await _authService.ResetPassword(password, token);
                return Accepted();
            }
            catch(TokenException ex)
            {
                return StatusCode(403, ex.Message);
            }
            catch(NotFoundException ex)
            {
                return StatusCode(404, ex.Message);
            }
        }
    }
}