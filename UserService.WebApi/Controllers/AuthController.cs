using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using UserService.Core.Interfaces;
using UserService.Core.Interfaces.Services;
using UserService.WebApi.Dtos;

namespace UserService.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private IUsersService _usersService;
        private IMapper _mapper;
        private IAuthService _authService;

        public AuthController(IUsersService usersService, IAuthService authService, IMapper mapper)
        {
            _usersService = usersService;
            _mapper = mapper;
            _authService = authService;
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
    }
}