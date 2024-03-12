using System.Net;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using UserService.Core.Exceptions;
using UserService.Core.Interfaces;
using UserService.Core.Interfaces.Services;
using UserService.WebApi.Dtos;

namespace UserService.WebApi.Controllers
{
    [ApiController]
    [Route("api/user")]
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

        [HttpPost("{id}/xp/{xpCnt}", Name = "UpdateUserXp")]
        public async Task<IActionResult> UpdateUserXp(int id, int xpCnt)
        {
            var response = new ApiResponse();
            var res = await _usersService.UpdateUserXp(id, xpCnt);
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

        [HttpPut("{id}/activity", Name = "SetUserActivity")]
        public async Task<IActionResult> SetUserActivity(int id)
        {
            var apiResponse = new ApiResponse();
            try{
                await  _usersService.SetUserActivity(id);
                apiResponse.IsSucceed = true;
                apiResponse.StatusCode = HttpStatusCode.OK;
                return Ok(apiResponse);
            } 
            catch(NotFoundException ex){
                apiResponse.IsSucceed = false;
                apiResponse.StatusCode = HttpStatusCode.NotFound;
                apiResponse.ErrorMessages.Add(ex.Message);
                return NotFound(apiResponse);
            }
            catch(Exception ex)
            {
                apiResponse.IsSucceed = false;
                apiResponse.StatusCode = HttpStatusCode.BadRequest;
                apiResponse.ErrorMessages.Add(ex.Message);
                return BadRequest(apiResponse);
            }
        }

        [HttpGet("{id}/activity/week", Name = "GetUserWeekActivity")]
        public async Task<IActionResult> GetUserWeekActivity(int id)
        {
            var apiResponse = new ApiResponse();
            try
            {
                var res = await _usersService.GetUserActivityForWeek(id);
                apiResponse.IsSucceed = true;
                apiResponse.Result = res;
                return Ok(apiResponse);
            }
            catch(NotFoundException ex)
            {
                apiResponse.IsSucceed = false;
                apiResponse.ErrorMessages.Add(ex.Message);
                return NotFound(apiResponse);
            }
            catch(Exception ex)
            {
                apiResponse.IsSucceed = false;
                apiResponse.ErrorMessages.Add(ex.Message);
                return StatusCode(500, apiResponse);
            }
        }

        [HttpGet("{id}/activity/month", Name = "GetUserMonthActivity")]
        public async Task<IActionResult> GetUserMonthActivity(int id, int month, int year)
        {
            var apiResponse = new ApiResponse();
            try
            {
                var res = await _usersService.GetUserActivityForMonth(id, month, year);
                apiResponse.IsSucceed = true;
                apiResponse.Result = res;
                return Ok(res);
            }
            catch(NotFoundException ex)
            {
                apiResponse.IsSucceed = false;
                apiResponse.ErrorMessages.Add(ex.Message);
                return NotFound(apiResponse);
            }
            catch(BadRequestException ex)
            {
                apiResponse.IsSucceed = false;
                apiResponse.ErrorMessages.Add(ex.Message);
                return BadRequest(apiResponse);
            }
        }
    }
}