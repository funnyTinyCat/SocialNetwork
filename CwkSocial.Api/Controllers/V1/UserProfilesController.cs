using CwkSocial.Api.Contracts.UserProfiles.Requests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using AutoMapper;
using CwkSocial.Application.UserProfiles.Commands;
using CwkSocial.Api.Contracts.UserProfiles.Responses;
using System.Diagnostics;
using CwkSocial.Application.UserProfiles.Queries;

namespace CwkSocial.Api.Controllers.V1
{
    [ApiVersion("1.0")]
    [Route(ApiRoutes.baseRoute)]
    [ApiController]
    public class UserProfilesController : ControllerBase
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
            var query = new GetAllUserProfiles();

            var response = await _mediator.Send(query);

            var profiles = _mapper.Map<List<UserProfileResponse>>(response);

            return Ok(profiles);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUserProfile([FromBody] UserProfileCreateUpdate profile)
        {
            var command = _mapper.Map<CreateUserCommand>(profile);

            var response = await _mediator.Send(command);

            var userProfile = _mapper.Map<UserProfileResponse>(response);

            return CreatedAtAction(nameof(GetUserProfileById), new {id = userProfile.UserProfileId }, userProfile);
        }

        [HttpGet(ApiRoutes.UserProfiles.idRoute)]
        public async Task<IActionResult> GetUserProfileById(string id)
        {
            var query = new GetUserProfileById { UserProfileId = Guid.Parse(id) };

            var response = await _mediator.Send(query);

            if (response == null)
                return NotFound();

            var userProfile = _mapper.Map<UserProfileResponse>(response);

            return Ok(userProfile);

        }

        [HttpDelete(ApiRoutes.UserProfiles.idRoute)]
        public async Task<IActionResult> DeleteUserProfile(string id)
        {
            var command = new DeleteUserProfile();
            command.UserProfileId = Guid.Parse(id);

            await _mediator.Send(command);

            return NoContent();
        }

        [HttpPatch(ApiRoutes.UserProfiles.idRoute)]
        public async Task<IActionResult> UpdateUserProfile(string id, UserProfileCreateUpdate profile)
        {
            var command = _mapper.Map<UpdateUserProfileBasicInfo>(profile);
            command.UserProfileId = Guid.Parse(id);

            await _mediator.Send(command);

            return NoContent();
        }

    }
}
