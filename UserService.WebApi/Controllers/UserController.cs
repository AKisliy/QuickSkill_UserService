using System.ComponentModel.DataAnnotations;
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

        /// <summary>
        /// Set new Firstname to the user
        /// </summary>
        /// <param name="id">Id of user</param>
        /// <param name="newName">New firstname</param>
        /// <response code="200">Success</response>
        /// <response code="404">User with this id wasn't found</response>
        /// <response code="500">Something bad happened</response>
        [HttpPatch("{id}/firstname")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public async Task<IActionResult> SetUserFirstName(int id, [Required]string newName)
        {
            try
            {
                await _usersService.SetUserFirstName(id, newName);
                return Ok();
            }
            catch(NotFoundException ex)
            {
                var response = new ErrorResponse();
                response.ErrorMessages.Add(ex.Message);
                return NotFound(response);
            }
            catch(Exception ex)
            {
                var response = new ErrorResponse();
                response.ErrorMessages.Add(ex.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, response);
            }
        }

        /// <summary>
        /// Set new lastname to the user
        /// </summary>
        /// <param name="id">Id of user</param>
        /// <param name="newLastName">New lastname</param>
        /// <response code="200">Success</response>
        /// <response code="404">User with this id wasn't found</response>
        /// <response code="500">Something bad happened :(</response>
        [HttpPatch("{id}/lastname")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public async Task<IActionResult> SetUserLastName(int id, [Required]string newLastName)
        {
            try
            {
                await _usersService.SetUserLastName(id, newLastName);
                return Ok();
            }
            catch(NotFoundException ex)
            {
                var response = new ErrorResponse();
                response.ErrorMessages.Add(ex.Message);
                return NotFound(response);
            }
            catch(Exception ex)
            {
                var response = new ErrorResponse();
                response.ErrorMessages.Add(ex.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, response);
            }
        }

        /// <summary>
        /// Set new username to the user
        /// </summary>
        /// <param name="id">Id of user</param>
        /// <param name="newUsername">New username</param>
        /// <response code="200">Success</response>
        /// <response code="404">User with this id wasn't found</response>
        /// <response code="500">Something bad happened :(</response>
        [HttpPatch("{id}/username")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public async Task<IActionResult> SetUserUsername(int id, [Required]string newUsername)
        {
            try
            {
                await _usersService.SetUserUsername(id, newUsername);
                return Ok();
            }
            catch(NotFoundException ex)
            {
                var response = new ErrorResponse();
                response.ErrorMessages.Add(ex.Message);
                return NotFound(response);
            }
            catch(Exception ex)
            {
                var response = new ErrorResponse();
                response.ErrorMessages.Add(ex.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, response);
            }
        }

        /// <summary>
        /// Set new description to the user
        /// </summary>
        /// <param name="id">Id of user</param>
        /// <param name="newDescription">New description</param>
        /// <response code="200">Success</response>
        /// <response code="404">User with this id wasn't found</response>
        /// <response code="500">Something bad happened :(</response>
        [HttpPatch("{id}/description")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public async Task<IActionResult> SetUserDescription(int id, [Required]string newDescription)
        {
            try
            {
                await _usersService.SetUserDescription(id, newDescription);
                return Ok();
            }
            catch(NotFoundException ex)
            {
                var response = new ErrorResponse();
                response.ErrorMessages.Add(ex.Message);
                return NotFound(response);
            }
            catch(Exception ex)
            {
                var response = new ErrorResponse();
                response.ErrorMessages.Add(ex.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, response);
            }
        }

        /// <summary>
        /// Set new photo to the user
        /// </summary>
        /// <param name="id">Id of user</param>
        /// <param name="photoUrl">New photo url</param>
        /// <response code="200">Success</response>
        /// <response code="404">User with this id wasn't found</response>
        /// <response code="500">Something bad happened :(</response>
        [HttpPatch("{id}/photo")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public async Task<IActionResult> SetUserPhoto(int id, [Required]string photoUrl)
        {
            try
            {
                await _usersService.SetUserPhoto(id, photoUrl);
                return Ok();
            }
            catch(NotFoundException ex)
            {
                var response = new ErrorResponse();
                response.ErrorMessages.Add(ex.Message);
                return NotFound(response);
            }
            catch(Exception ex)
            {
                var response = new ErrorResponse();
                response.ErrorMessages.Add(ex.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, response);
            }
        }

        /// <summary>
        /// Check user's password
        /// </summary>
        /// <param name="id">Id of user</param>
        /// <param name="password">Password to check</param>
        /// <response code="200">Success</response>
        /// <response code="404">User with this id wasn't found</response>
        /// <response code="403">Incorrect password</response>
        /// <response code="500">Something bad happened :(</response>
        [HttpGet("{id}/password/check")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public async Task<IActionResult> CheckUserPassword(int id, string password)
        {
            try
            {
                await _authService.CheckPassword(id, password);
                return Ok();
            }
            catch(NotFoundException ex)
            {
                var response = new ErrorResponse();
                response.ErrorMessages.Add(ex.Message);
                return NotFound(response);
            }
            catch(CredentialsException ex)
            {
                var response = new ErrorResponse();
                response.ErrorMessages.Add(ex.Message);
                return StatusCode(403, response);
            }
            catch(Exception ex)
            {
                var response = new ErrorResponse();
                response.ErrorMessages.Add(ex.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, response);
            }
        }

        /// <summary>
        /// Change user's password
        /// </summary>
        /// <param name="id">Id of user</param>
        /// <param name="request">Contains old and new password</param>
        /// <response code="200">Success</response>
        /// <response code="400">BadRequest. Check your request body</response>
        /// <response code="404">User with this id wasn't found</response>
        /// <response code="403">Incorrect password</response>
        /// <response code="500">Something bad happened :(</response>
        [HttpPatch("{id}/password/change")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> ChangeUserPassword(int id, [FromBody] ChangePasswordRequest request)
        {
            if(request == null)
            {
                var errorResponse = new ErrorResponse();
                errorResponse.ErrorMessages.Add("Bad request body");
                return BadRequest(errorResponse);
            }
            try
            {
                await _authService.ChangePassword(id, request.OldPassword, request.NewPassword);
                return Ok();
            }
            catch(NotFoundException ex)
            {
                var errorReponse = new ErrorResponse();
                errorReponse.ErrorMessages.Add(ex.Message);
                return NotFound(errorReponse);
            }
            catch(CredentialsException ex)
            {
                var errorReponse = new ErrorResponse();
                errorReponse.ErrorMessages.Add(ex.Message);
                return StatusCode((int)HttpStatusCode.Forbidden, errorReponse);
            }
            catch(Exception)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }
    }
}