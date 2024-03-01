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
    public class UserController : ControllerBase
    {
        private IUsersService _usersService;
        private IMapper _mapper;
        private IAuthService _authService;

        public UserController(IUsersService usersService, IAuthService authService, IMapper mapper)
        {
            _usersService = usersService;
            _mapper = mapper;
            _authService = authService;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _usersService.GetAllUsers();
            var response = new ApiResponse();
            response.Result = users;
            response.IsSucceed = true;
            response.StatusCode = HttpStatusCode.OK;
            return Ok(response);
        }

        [HttpGet("{id}", Name = "GetUserById")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _usersService.GetUserById(id);
            if(user != null)
            {
                var response = new ApiResponse()
                {
                    Result = _mapper.Map<UserResponse>(user),
                    IsSucceed = true,
                    StatusCode = HttpStatusCode.OK
                };
                return Ok(response);
            }
            return NotFound(new ApiResponse(){ IsSucceed = false, StatusCode = HttpStatusCode.NotFound, ErrorMessages = {"No user with such id"}});
        }
    }
}