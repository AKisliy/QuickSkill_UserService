using System.ComponentModel.DataAnnotations;
using System.Net;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserService.Core.Exceptions;
using UserService.Core.Interfaces;
using UserService.Core.Interfaces.Services;
using UserService.Core.Models;
using UserService.WebApi.Dtos;
using UserService.WebApi.Extensions;

namespace UserService.WebApi.Controllers
{
    [ApiController]
    [Route("api/userservice/user")]
    public class UserController : ControllerBase
    {
        private readonly IUsersService _usersService;
        private readonly IMapper _mapper;
        private readonly IAuthService _authService;

        public UserController(IUsersService usersService, IAuthService authService, IMapper mapper)
        {
            _usersService = usersService;
            _mapper = mapper;
            _authService = authService;
        }

        /// <summary>
        /// Get all users (just for testing purposes)
        /// </summary>
        /// <returns>List of UserResponse</returns>
        /// <response code="200">Success</response>
        /// <response code="400">Bad request</response>
        [HttpGet(Name = "GetAllUsers")]
        [ProducesResponseType(typeof(List<UserResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _usersService.GetAllUsers();
            return Ok(users.Select(u => _mapper.Map<UserResponse>(u)));
        }

        /// <summary>
        /// Get current user(by token)
        /// </summary>
        /// <returns>UserResponse</returns>
        /// <response code="200">Success</response>
        /// <response code="404">User with this id wasn't found</response>
        [HttpGet("current", Name = "GetCurrentUser")]
        [Authorize]
        [ProducesResponseType(typeof(UserResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetUser()
        {
            int id = HttpContext.GetUserId();
            var user = await _usersService.GetUserById(id);
            return Ok(_mapper.Map<UserResponse>(user));
        }

        /// <summary>
        /// Get user by Id (u may use it, when u need to get information about different user, not current)
        /// </summary>
        /// <returns>UserResponse</returns>
        /// <response code="200">Success</response>
        /// <response code="404">User with this id wasn't found</response>
        [HttpGet("{id}", Name = "Get User by Id")]
        [Authorize]
        [ProducesResponseType(typeof(OtherUserResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _usersService.GetUserById(id);
            return Ok(_mapper.Map<OtherUserResponse>(user));
        }

        /// <summary>
        /// Add XP to user with ID
        /// </summary>
        /// <param name="xpCnt">Count of XP to add</param>
        /// <returns>bool</returns>
        /// <response code="200">Success</response>
        /// <response code="404">User with this id wasn't found</response>
        [Authorize]
        [HttpPost("xp/{xpCnt}", Name = "UpdateUserXp")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> UpdateUserXp(int xpCnt)
        {
            int id = HttpContext.GetUserId();
            await _usersService.UpdateUserXp(id, xpCnt);
            return Ok();
        }

        /// <summary>
        /// Delete user with id
        /// </summary>
        /// <response code="200">Success</response>
        /// <response code="404">User with this id wasn't found</response>
        [Authorize]
        [HttpDelete(Name = "DeleteUser")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> DeleteUser()
        {
            int id = HttpContext.GetUserId();
            await _usersService.DeleteUser(id);
            return Ok();
        }

        /// <summary>
        /// Set user today's activity to "Active"
        /// </summary>
        /// <response code="200">Success</response>
        /// <response code="404">User with this id wasn't found</response>
        [Authorize]
        [HttpPut("activity", Name = "SetUserActivity")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> SetUserActivity()
        {
            int id = HttpContext.GetUserId();
            await  _usersService.SetUserActivity(id);
            return Ok();
        }

        /// <summary>
        /// Get user activity for current week
        /// </summary>
        /// <response code="200">Success</response>
        /// <response code="404">User with this id wasn't found</response>
        [Authorize]
        [HttpGet("activity/week", Name = "GetUserWeekActivity")]
        [ProducesResponseType(typeof(List<UserActivity>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetUserWeekActivity()
        {
            int id = HttpContext.GetUserId();
            var res = await _usersService.GetUserActivityForWeek(id);
            return Ok(res);
        }

        /// <summary>
        /// Get user activity for month in year
        /// </summary>
        /// <param name="month">Number of month</param>
        /// <param name="year">Number of year</param>
        /// <response code="200">Success</response>
        /// <response code="404">User with this id wasn't found</response>
        [Authorize]
        [HttpGet("activity/month", Name = "GetUserMonthActivity")]
        [ProducesResponseType(typeof(List<UserActivity>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetUserMonthActivity(int month, int year)
        {
            int id = HttpContext.GetUserId();
            var res = await _usersService.GetUserActivityForMonth(id, month, year);
            return Ok(res);
        }

        /// <summary>
        /// Set new Firstname to the user
        /// </summary>
        /// <param name="newName">New firstname</param>
        /// <response code="200">Success</response>
        /// <response code="404">User with this id wasn't found</response>
        /// <response code="500">Something bad happened</response>
        [Authorize]
        [HttpPatch("firstname")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public async Task<IActionResult> SetUserFirstName([Required]string newName)
        {
            int id = HttpContext.GetUserId();
            await _usersService.SetUserFirstName(id, newName);
            return Ok();
        }

        /// <summary>
        /// Set new lastname to the user
        /// </summary>
        /// <param name="newLastName">New lastname</param>
        /// <response code="200">Success</response>
        /// <response code="404">User with this id wasn't found</response>
        /// <response code="500">Something bad happened :(</response>
        [Authorize]
        [HttpPatch("lastname")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public async Task<IActionResult> SetUserLastName([Required]string newLastName)
        {
            int id = HttpContext.GetUserId();
            await _usersService.SetUserLastName(id, newLastName);
            return Ok();
        }

        /// <summary>
        /// Set new username to the user
        /// </summary>
        /// <param name="newUsername">New username</param>
        /// <response code="200">Success</response>
        /// <response code="404">User with this id wasn't found</response>
        /// <response code="500">Something bad happened :(</response>
        [Authorize]
        [HttpPatch("username")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public async Task<IActionResult> SetUserUsername([Required]string newUsername)
        {
            int id = HttpContext.GetUserId();
            await _usersService.SetUserUsername(id, newUsername);
            return Ok();
        }

        /// <summary>
        /// Set new description to the user
        /// </summary>
        /// <param name="newDescription">New description</param>
        /// <response code="200">Success</response>
        /// <response code="404">User with this id wasn't found</response>
        /// <response code="500">Something bad happened :(</response>
        [Authorize]
        [HttpPatch("description")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public async Task<IActionResult> SetUserDescription([Required]string newDescription)
        {
            int id = HttpContext.GetUserId();
            await _usersService.SetUserDescription(id, newDescription);
            return Ok();
        }

        /// <summary>
        /// Set new photo to the user
        /// </summary>
        /// <param name="file">New photo</param>
        /// <response code="200">Success</response>
        /// <response code="404">User with this id wasn't found</response>
        /// <response code="500">Something bad happened :(</response>
        [Authorize]
        [HttpPatch("photo")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public async Task<IActionResult> SetUserPhoto([Required]IFormFile file)
        {
            int id = HttpContext.GetUserId();
            await _usersService.SetUserPhoto(id, file);
            return Ok();
        }

        /// <summary>
        /// Delete user photo. New sample photo will be placed automatically.
        /// </summary>
        /// <response code="200">Success</response>
        /// <response code="404">User with this id wasn't found</response>
        /// <response code="500">Something bad happened :(</response>
        [Authorize]
        [HttpDelete("photo")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public async Task<IActionResult> DeleteUserPhoto()
        {
            int id = HttpContext.GetUserId();
            await _usersService.DeleteUserPhoto(id);
            return Ok();
        }

        /// <summary>
        /// Set new goal text to the user
        /// </summary>
        /// <param name="newGoal">Text of new goal</param>
        /// <response code="200">Success</response>
        /// <response code="404">User with this id wasn't found</response>
        /// <response code="500">Something bad happened :(</response>
        [Authorize]
        [HttpPatch("goaltext")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public async Task<IActionResult> SetUserGoalText([Required]string newGoal)
        {
            int id = HttpContext.GetUserId();
            await _usersService.SetGoalText(id, newGoal);
            return Ok();
        }

        /// <summary>
        /// Set new goal's days amount to the user
        /// </summary>
        /// <param name="daysCount">Count of days per week (should be in [1,7]) </param>
        /// <response code="200">Success</response>
        /// <response code="404">User with this id wasn't found</response>
        /// <response code="500">Something bad happened :(</response>
        [Authorize]
        [HttpPatch("goaldays")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public async Task<IActionResult> SetUserGoalDays([Required]int daysCount)
        {
            int id = HttpContext.GetUserId();
            await _usersService.SetGoalDays(id, daysCount);
            return Ok();
        }

        /// <summary>
        /// Remove user's goal text and days (pay attention, that if we remove either only text or only days - both fields will be removed, 
        /// because days without text make no sense as well as text without days)
        /// </summary>
        /// <response code="200">Success</response>
        /// <response code="404">User with this id wasn't found</response>
        /// <response code="500">Something bad happened :(</response>
        [Authorize]
        [HttpDelete("goal")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public async Task<IActionResult> DeleteUserGoal()
        {
            int id = HttpContext.GetUserId();
            await _usersService.DeleteGoalTextAndDays(id);
            return Ok();
        }

        /// <summary>
        /// Check user's password
        /// </summary>
        /// <param name="password">Password to check</param>
        /// <response code="200">Success</response>
        /// <response code="404">User with this id wasn't found</response>
        /// <response code="403">Incorrect password</response>
        /// <response code="500">Something bad happened :(</response>
        [Authorize]
        [HttpGet("password/check")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public async Task<IActionResult> CheckUserPassword(string password)
        {
            int id = HttpContext.GetUserId();
            await _authService.CheckPassword(id, password);
            return Ok();
        }

        /// <summary>
        /// Change user's password
        /// </summary>
        /// <param name="request">Contains old and new password</param>
        /// <response code="200">Success</response>
        /// <response code="400">BadRequest. Check your request body</response>
        /// <response code="404">User with this id wasn't found</response>
        /// <response code="403">Incorrect password</response>
        /// <response code="500">Something bad happened :(</response>
        [Authorize]
        [HttpPatch("password/change")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> ChangeUserPassword([FromBody] ChangePasswordRequest request)
        {
            if(request == null)
                throw new BadRequestException("Bad request body");
            int id = HttpContext.GetUserId();
            await _authService.ChangePassword(id, request.OldPassword, request.NewPassword);
            return Ok();
        }
    }
}