using System.Net;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using UserService.Core.Interfaces.Services;
using UserService.Core.Models;
using UserService.WebApi.Dtos;

namespace UserService.WebApi.Controllers
{
    [ApiController]
    [Route("api/badges")]
    public class BadgeController : ControllerBase
    {
        private IBadgeService _service;
        private IMapper _mapper;

        public BadgeController(IBadgeService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        /// <summary>
        /// Get all badges for user
        /// </summary>
        /// <param name="id">User's ID</param>
        /// <returns>List of UserBadgeResponse</returns>
        /// <response code="200">Success</response>
        /// <response code="404">User with this id wasn't found</response>
        [HttpGet("user/{id}", Name = "GetAllUserBadges")]
        [ProducesResponseType(typeof(List<UserBadgeResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetAllBadgesForUser(int id)
        {
            var response = new ErrorResponse();
            var userBadges =  await _service.GetAllBadgesForUser(id);
            if(userBadges == null)
            {
                response.ErrorMessages.Add("No badges found for this user");
                return NotFound(response);
            }

            var result = userBadges.Select(ub => 
                _mapper.Map<UserBadgeResponse>(ub)
            );
            return Ok(result);
        }

        /// <summary>
        /// Get badge by its id
        /// </summary>
        /// <param name="id">Badge ID</param>
        /// <returns>BadgeResponse</returns>
        /// <response code="200">Success</response>
        /// <response code="404">Badge with this id wasn't found</response>
        [HttpGet("{id}", Name = "GetBadgeById")]
        [ProducesResponseType(typeof(BadgeResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetBadgeById(int id)
        {
            var response = new ErrorResponse();

            var badge = await _service.GetBadgeById(id);

            if(badge == null)
            {
                response.ErrorMessages.Add("Badge with this id wasn't found");
                return NotFound(response);
            }
            return Ok(_mapper.Map<BadgeResponse>(badge));
        }

        /// <summary>
        /// Create new badge
        /// </summary>
        /// <param name="badge">New badge body</param>
        /// <returns>ID of badge + uri</returns>
        /// <response code="201">Successfully created</response>
        /// <response code="409">Can't create this badge</response>
        [HttpPost(Name = "CreateNewBadge")]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.Conflict)]
        public async Task<IActionResult> CreateNewBadge([FromBody] BadgeRequest badge)
        {
            var response = new ErrorResponse();

            var id = await _service.CreateBadge(badge.Name, badge.Photo, badge.GrayPhoto, badge.Required, badge.Task);
            if(id == null)
            {
                response.ErrorMessages.Add("Something went wrong while creating");
                return Conflict(response);
            }
            return Created($"api/badges/badge/{id}", id);
        }

        /// <summary>
        /// Update user's badges progress
        /// </summary>
        /// <param name="request">User badge body</param>
        /// <response code="200">Success</response>
        /// <response code="400">Can't update</response>
        [HttpPut("user/{id}/{badgeId}", Name = "UpdateBadgeForUser")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> UpdateBadgeForUser([FromBody] UserBadgeRequest request)
        {
            var response = new ErrorResponse();

            var res = await _service.UpdateBadgeInfoForUser(request.UserId,request.BadgeId, request.Progress);

            if(!res)
            {
                response.ErrorMessages.Add("Something went wrong during updating");
                return BadRequest(response);
            }
            return Ok();
        }

        /// <summary>
        /// Update badge information
        /// </summary>
        /// <param name="id">ID of badge to update</param>
        /// <param name="request">Badge body</param>
        /// <response code="200">Success</response>
        /// <response code="404">No badge with this id</response>
        [HttpPut("badge/{id}", Name = "UpdateBadgeInformation")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> UpdateBadge(int id, [FromBody] BadgeRequest request)
        {
            var response = new ErrorResponse();

            var badge = _mapper.Map<Badge>(request);
            badge.Id = id;
            var result = await _service.UpdateBadge(badge);
            if(!result)
            {
                response.ErrorMessages.Add($"No badge with id {id} found");
                return NotFound(response);
            }
            return Ok();
        }

        /// <summary>
        /// Delete badge
        /// </summary>
        /// <param name="id">ID of badge to delete</param>
        /// <response code="200">Success</response>
        /// <response code="404">No badge with this id</response>
        [HttpDelete("{id}", Name = "DeletBadge")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> DeleteBadge(int id)
        {
            var response = new ErrorResponse();

            var result = await _service.DeleteBadge(id);
            if(!result)
            {
                response.ErrorMessages.Add("Error occured while deleting(maybe id is invalid)");
                return NotFound(response);
            }
            return Ok();
        }
    }
}