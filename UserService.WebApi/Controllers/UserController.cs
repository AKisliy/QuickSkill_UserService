using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
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

        [HttpGet(Name = "GetAllUsers")]
        //[Authorize]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _usersService.GetAllUsers();
            var response = new ApiResponse
            {
                Result = users,
                IsSucceed = true,
                StatusCode = HttpStatusCode.OK
            };
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

        [HttpPost("{id}/xp", Name = "UpdateUserXp")]
        public async Task<IActionResult> UpdateUserXp(int id, int xp)
        {
            var response = new ApiResponse();
            var res = await _usersService.UpdateUserXp(id, xp);
            if(!res)
            {
                response.IsSucceed = false;
                response.ErrorMessages.Add("User with id: {id} wasn't found");
                response.StatusCode = HttpStatusCode.NotFound;
                return NotFound(response);
            }
            response.IsSucceed = true;
            response.StatusCode = HttpStatusCode.OK;
            return Ok(response);
        }

        [HttpDelete("{id}", Name = "DeleteUser")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var response = new ApiResponse();
            var res = await _usersService.DeleteUser(id);

            if(!res)
            {
                response.IsSucceed = false;
                response.ErrorMessages.Add("User with id: {id} wasn't found");
                response.StatusCode = HttpStatusCode.NotFound;
                return NotFound(response);
            }

            response.IsSucceed = true;
            response.StatusCode = HttpStatusCode.OK;
            return Ok(response);
        }
    }
}