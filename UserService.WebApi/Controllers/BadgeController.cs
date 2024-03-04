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

        [HttpGet("user/{id}", Name = "GetAllUserBadges")]
        public async Task<IActionResult> GetAllBadgesForUser(int id)
        {
            var response = new ApiResponse();
            var userBadges =  await _service.GetAllBadgesForUser(id);
            if(userBadges == null)
            {
                response.IsSucceed = false;
                response.ErrorMessages.Add("No badges found for this user");
                response.StatusCode = HttpStatusCode.NotFound;
                return NotFound(response);
            }

            var result = userBadges.Select(ub => 
            {
                return new UserBadgeResponse()
                {
                    UserId = ub.UserId,
                    Name = ub.Badge.Name,
                    TaskToAchieve = ub.Badge.TaskToAchieve,
                    Required = ub.Badge.Required,
                    Progress = ub.Progress,
                    Achieved = ub.Achieved,
                    Photo = ub.Badge.Photo,
                    GrayPhoto = ub.Badge.GrayPhoto
                };
            });
            response.IsSucceed = true;
            response.StatusCode = HttpStatusCode.OK;
            response.Result = result;
            return Ok(response);
        }

        [HttpGet("{id}", Name = "GetBadgeById")]
        public async Task<IActionResult> GetBadgeById(int id)
        {
            var response = new ApiResponse();

            var badge = await _service.GetBadgeById(id);

            if(badge == null)
            {
                response.ErrorMessages.Add("Badge with this id wasn't found");
                response.IsSucceed = false;
                response.StatusCode = HttpStatusCode.NotFound;
                return NotFound(response);
            }
            response.IsSucceed = true;
            response.StatusCode = HttpStatusCode.OK;
            response.Result = _mapper.Map<BadgeResponse>(badge);
            return Ok(response);
        }

        [HttpPost(Name = "CreateNewBadge")]
        public async Task<IActionResult> CreateNewBadge([FromBody] BadgeRequest badge)
        {
            var response = new ApiResponse();

            var id = await _service.CreateBadge(badge.Name, badge.Photo, badge.GrayPhoto, badge.Required, badge.Task);
            if(id == null)
            {
                response.IsSucceed = false;
                response.ErrorMessages.Add("Something went wrong while creating");
                response.StatusCode = HttpStatusCode.Conflict;
                return Conflict(response);
            }
            response.IsSucceed = true;
            response.Result = id;
            response.StatusCode = HttpStatusCode.Created;
            return Created($"api/badges/badge/{id}", id);
        }

        [HttpPut("user/{id}/{badgeId}", Name = "UpdateBadgeForUser")]
        public async Task<IActionResult> UpdateBadgeForUser([FromBody] UserBadgeRequest request)
        {
            var response = new ApiResponse();

            var res = await _service.UpdateBadgeInfoForUser(request.UserId,request.BadgeId, request.Progress);

            if(!res)
            {
                response.IsSucceed = false;
                response.ErrorMessages.Add("Something went wrong during updating");
                response.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest(response);
            }
            response.IsSucceed = true;
            response.StatusCode = HttpStatusCode.OK;
            return Ok(response);
        }

        [HttpPut("badge/{id}", Name = "UpdateBadgeInformation")]
        public async Task<IActionResult> UpdateBadge(int id, [FromBody] BadgeRequest request)
        {
            var response = new ApiResponse();

            var badge = _mapper.Map<Badge>(request);
            badge.Id = id;
            var result = await _service.UpdateBadge(badge);
            if(!result)
            {
                response.IsSucceed = false;
                response.ErrorMessages.Add($"No badge with id {id} found");
                response.StatusCode = HttpStatusCode.NotFound;
                return NotFound(response);
            }
            response.IsSucceed = true;
            response.StatusCode = HttpStatusCode.OK;
            return Ok(response);
        }

        [HttpDelete("{id}", Name = "DeletBadge")]
        public async Task<IActionResult> DeleteBadge(int id)
        {
            var response = new ApiResponse();

            var result = await _service.DeleteBadge(id);
            if(!result)
            {
                response.IsSucceed = false;
                response.ErrorMessages.Add("Error occured while deleting(maybe id is invalid)");
                response.StatusCode = HttpStatusCode.NotFound;
                return NotFound(response);
            }
            response.IsSucceed = true;
            response.StatusCode = HttpStatusCode.OK;
            return Ok(response);
        }
    }
}