using AutoMapper;
using CwkSocial.Api.Contracts.Identity;
using CwkSocial.Api.Contracts.UserProfiles.Requests;
using CwkSocial.Api.Contracts.UserProfiles.Responses;
using CwkSocial.Api.Filters;
using CwkSocial.Application.Identity.Commands;
using CwkSocial.Application.UserProfiles.Commands;
using CwkSocial.Application.UserProfiles.Queries;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CwkSocial.Api.Controllers.V1
{
    [ApiVersion("1.0")]
    [Route(ApiRoutes.baseRoute)]
    [ApiController]
    public class IdentityController : BaseController
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public IdentityController(IMediator mediator, IMapper mapper) 
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        [ValidateModel]
        [HttpPost(ApiRoutes.Identity.Registration)]
        public async Task<IActionResult> Register(UserRegistration registration)
        {
            var command = _mapper.Map<RegisterIdentity>(registration);

            var result = await _mediator.Send(command);

            if (result.IsError)
            {
                return HandleErrorResponse(result.Errors);
            }

            var authenticationResult = new AuthenticationResult()
            {
                Token = result.Payload
            };

            return Ok(authenticationResult);
        }

        [HttpPost(ApiRoutes.Identity.Login)]
        [ValidateModel]
        public async Task<IActionResult> Login(Login login)
        {
            var command = _mapper.Map<LoginCommand>(login);
            var result = await  _mediator.Send(command);

            if (result.IsError)
            {
                return HandleErrorResponse(result.Errors);
            }

            var authenticationResult = new AuthenticationResult
            {
                Token = result.Payload
            };  

            return Ok(authenticationResult); 
        }

        [ValidateModel]
        [HttpPost]
        public async Task<IActionResult> CreateUserProfile([FromBody] UserProfileCreateUpdate profile)
        {
            var command = _mapper.Map<CreateUserCommand_bck>(profile);

            var response = await _mediator.Send(command);

            var userProfile = _mapper.Map<UserProfileResponse>(response.Payload);

            return CreatedAtAction(nameof(GetUserProfileById), new { id = userProfile.UserProfileId }, userProfile);
        }
    }
}
