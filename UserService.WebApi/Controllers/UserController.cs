using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using UserService.Core.Interfaces;
using UserService.WebApi.Dtos;

namespace UserService.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private IUsersService _service;
        private IMapper _mapper;
        private ApiResponse _response;

        public UserController(IUsersService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
            _response = new ApiResponse();
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _service.GetAllUsers();
            var response = new ApiResponse();
            response.Result = users;
            response.IsSucceed = true;
            response.StatusCode = HttpStatusCode.OK;
            return Ok(response);
        }

        [HttpGet("{id}", Name = "GetUserById")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _service.GetUserById(id);
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