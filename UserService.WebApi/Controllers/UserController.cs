using System.Net;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using UserService.Core.Exceptions;
using UserService.Core.Interfaces;
using UserService.Core.Interfaces.Services;
using UserService.Core.Models;
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

        /// <summary>
        /// Get all users
        /// </summary>
        /// <returns>List of UserResponse</returns>
        /// <response code="200">Success</response>
        /// <response code="400">Bad request</response>
        [HttpGet(Name = "GetAllUsers")]
        [ProducesResponseType(typeof(List<UserResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        //[Authorize]
        public async Task<IActionResult> GetUsers()
        {
            if(!ModelState.IsValid)
            {
                var response = new ErrorResponse();
                response.ErrorMessages.Add("Something went wrong");
                return BadRequest(response);
            }
            var users = await _usersService.GetAllUsers();
            return Ok(users);
        }

        /// <summary>
        /// Get user By ID
        /// </summary>
        /// <returns>UserResponse</returns>
        /// <response code="200">Success</response>
        /// <response code="404">User with this id wasn't found</response>
        [HttpGet("{id}", Name = "GetUserById")]
        [ProducesResponseType(typeof(UserResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _usersService.GetUserById(id);
            if(user == null)
            {
                var response = new ErrorResponse();
                response.ErrorMessages.Add("User with this id wasn't found");
                return NotFound(response);
            }
            return Ok(_mapper.Map<UserResponse>(user));
        }

        /// <summary>
        /// Add XP to user with ID
        /// </summary>
        /// <param name="id">Id of user</param>
        /// <param name="xpCnt">Count of XP to add</param>
        /// <returns>bool</returns>
        /// <response code="200">Success</response>
        /// <response code="404">User with this id wasn't found</response>
        [HttpPost("{id}/xp/{xpCnt}", Name = "UpdateUserXp")]
        [ProducesResponseType(typeof(UserResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> UpdateUserXp(int id, int xpCnt)
        {
            var response = new ErrorResponse();
            var res = await _usersService.UpdateUserXp(id, xpCnt);
            if(!res)
            {
                response.ErrorMessages.Add("User with id: {id} wasn't found");
                return NotFound(response);
            }
            return Ok(response);
        }

        /// <summary>
        /// Delete user with id
        /// </summary>
        /// <param name="id">Id of user</param>
        /// <response code="200">Success</response>
        /// <response code="404">User with this id wasn't found</response>
        [HttpDelete("{id}", Name = "DeleteUser")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var res = await _usersService.DeleteUser(id);
            if(!res)
            {
                var response = new ErrorResponse();
                response.ErrorMessages.Add("User with id: {id} wasn't found");
                return NotFound(response);
            }
            return Ok();
        }

        /// <summary>
        /// Set user today's activity to "Active"
        /// </summary>
        /// <param name="id">Id of user</param>
        /// <response code="200">Success</response>
        /// <response code="404">User with this id wasn't found</response>
        [HttpPut("{id}/activity", Name = "SetUserActivity")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> SetUserActivity(int id)
        {
            var apiResponse = new ErrorResponse();
            try{
                await  _usersService.SetUserActivity(id);
                return Ok();
            }
            catch(NotFoundException ex){
                apiResponse.ErrorMessages.Add(ex.Message);
                return NotFound(apiResponse);
            }
            catch(Exception ex)
            {
                apiResponse.ErrorMessages.Add(ex.Message);
                return BadRequest(apiResponse);
            }
        }

        /// <summary>
        /// Get user activity for current week
        /// </summary>
        /// <param name="id">Id of user</param>
        /// <response code="200">Success</response>
        /// <response code="404">User with this id wasn't found</response>
        [HttpGet("{id}/activity/week", Name = "GetUserWeekActivity")]
        [ProducesResponseType(typeof(List<UserActivity>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetUserWeekActivity(int id)
        {
            var apiResponse = new ErrorResponse();
            try
            {
                var res = await _usersService.GetUserActivityForWeek(id);
                return Ok(res);
            }
            catch(NotFoundException ex)
            {
                apiResponse.ErrorMessages.Add(ex.Message);
                return NotFound(apiResponse);
            }
            catch(Exception ex)
            {
                apiResponse.ErrorMessages.Add(ex.Message);
                return StatusCode(500, apiResponse);
            }
        }

        /// <summary>
        /// Get user activity for month in year
        /// </summary>
        /// <param name="id">Id of user</param>
        /// <param name="month">Number of month</param>
        /// <param name="year">Number of year</param>
        /// <response code="200">Success</response>
        /// <response code="404">User with this id wasn't found</response>
        [HttpGet("{id}/activity/month", Name = "GetUserMonthActivity")]
        [ProducesResponseType(typeof(List<UserActivity>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetUserMonthActivity(int id, int month, int year)
        {
            var apiResponse = new ErrorResponse();
            try
            {
                var res = await _usersService.GetUserActivityForMonth(id, month, year);
                return Ok(res);
            }
            catch(NotFoundException ex)
            {
                apiResponse.ErrorMessages.Add(ex.Message);
                return NotFound(apiResponse);
            }
            catch(BadRequestException ex)
            {
                apiResponse.ErrorMessages.Add(ex.Message);
                return BadRequest(apiResponse);
            }
        }
    }
}