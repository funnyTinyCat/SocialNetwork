using CwkSocial.Api.Contracts.UserProfiles.Requests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using AutoMapper;
using CwkSocial.Application.UserProfiles.Commands;
using CwkSocial.Api.Contracts.UserProfiles.Responses;
using System.Diagnostics;
using CwkSocial.Application.UserProfiles.Queries;
using CwkSocial.Application.Enums;
using CwkSocial.Api.Contracts.Common;
using CwkSocial.Api.Filters;

namespace CwkSocial.Api.Controllers.V1
{
    [ApiVersion("1.0")]
    [Route(ApiRoutes.baseRoute)]
    [ApiController]
    public class UserProfilesController : BaseController
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        public UserProfilesController(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProfiles()
        {
            //throw new NotImplementedException("Method not implemented.");

            var query = new GetAllUserProfiles();

            var response = await _mediator.Send(query);

            var profiles = _mapper.Map<List<UserProfileResponse>>(response.Payload);

            return Ok(profiles);
        }

        [ValidateModel]
        [HttpPost]
        public async Task<IActionResult> CreateUserProfile([FromBody] UserProfileCreateUpdate profile)
        {
            var command = _mapper.Map<CreateUserCommand>(profile);

            var response = await _mediator.Send(command);

            var userProfile = _mapper.Map<UserProfileResponse>(response.Payload);

            return CreatedAtAction(nameof(GetUserProfileById), new {id = userProfile.UserProfileId }, userProfile);
        }

        [ValidateGuid("id")]
        [HttpGet(ApiRoutes.UserProfiles.idRoute)]
        public async Task<IActionResult> GetUserProfileById(string id)
        {
            var query = new GetUserProfileById { UserProfileId = Guid.Parse(id) };

            var response = await _mediator.Send(query);

            if (response.IsError)
            {
                return HandleErrorResponse(response.Errors);
            }

            var result = _mapper.Map<UserProfileResponse>(response.Payload);

            return Ok(result);
        }

        [ValidateGuid("id")]
        [HttpDelete(ApiRoutes.UserProfiles.idRoute)]
        public async Task<IActionResult> DeleteUserProfile(string id)
        {
            var command = new DeleteUserProfile();
            command.UserProfileId = Guid.Parse(id);

            var response = await _mediator.Send(command);

            return response.IsError ? HandleErrorResponse(response.Errors) :     NoContent();
        }

        [ValidateModel]
        [ValidateGuid("id")]
        [HttpPatch(ApiRoutes.UserProfiles.idRoute)]
        public async Task<IActionResult> UpdateUserProfile(string id, UserProfileCreateUpdate profile)
        {
            var command = _mapper.Map<UpdateUserProfileBasicInfo>(profile);
             command.UserProfileId = Guid.Parse(id);

            var response = await _mediator.Send(command);

            if (response.IsError)
            {
                return HandleErrorResponse(response.Errors);
            }

            return NoContent();
        }
    }
}
