using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using UserService.Core.Interfaces;
using UserService.Core.Interfaces.Services;
using UserService.WebApi.Dtos;
using UserService.Infrastructure;

namespace UserService.WebApi.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private IUsersService _usersService;
        private IMapper _mapper;
        private IAuthService _authService;
        private IOptions<CookiesOptions> _cookiesOptions;

        public AuthController(IUsersService usersService, IAuthService authService, IOptions<CookiesOptions> cookiesOptions, IMapper mapper)
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
            bool res = await _authService.Register(user.Firstname, user.Lastname, user.Email, user.Password);
            if(!res)
            {
                response.Result = HttpStatusCode.Conflict;
                response.ErrorMessages.Add("User with this email already exists");
                response.IsSucceed = false;
                return Conflict(response);
            }
            return Ok();
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
                response.ErrorMessages.Add(ex.Source);
                response.ErrorMessages.Add(ex.StackTrace);
                response.StatusCode = HttpStatusCode.NotFound;
                return NotFound(response);
            }
        }
    }
}