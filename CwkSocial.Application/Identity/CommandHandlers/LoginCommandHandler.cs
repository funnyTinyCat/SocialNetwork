using AutoMapper;
using Cwk.Domain.Aggregates.UserProfileAggregate;
using CwkSocial.Application.Enums;
using CwkSocial.Application.Identity.Commands;
using CwkSocial.Application.Models;
using CwkSocial.Application.Services;
using CwkSocial.Dal;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace CwkSocial.Application.Identity.CommandHandlers
{
    internal class LoginCommandHandler : IRequestHandler<LoginCommand, OperationResult<string>>
    {
        private readonly DataContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IdentityService _identityService;

        public LoginCommandHandler(DataContext context, UserManager<IdentityUser> userManager,
            IdentityService identityService)
        {
            _context = context;
            _userManager = userManager;
            _identityService = identityService; 
        }
        public async Task<OperationResult<string>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var result = new OperationResult<string>();

            try
            {
                var identityUser = await ValidateAndGetIdentityAsync(request, result);

                if (result.IsError)
                {
                    return result;
                }

                var userProfile = await _context.UserProfiles.FirstOrDefaultAsync(up => 
                    up.IdentityId == identityUser.Id);
     
                result.Payload = GetJwtString(identityUser, userProfile);

                return result;  
            }
            catch (Exception ex) 
            {
                result.AddUnknownError(ex.Message);
            }

            return result;
        }

        private async Task<IdentityUser> ValidateAndGetIdentityAsync(LoginCommand request, 
            OperationResult<string> result)
        {
            var identity = await _userManager.FindByEmailAsync(request.Username);

            if (identity == null)
            {
                result.AddError(ErrorCode.IdentityUserDoesNotExist, IdentityErrorMessages.NonExistentIdentityUser);                                             
            }

            var validPassword = await _userManager.CheckPasswordAsync(identity, request.Password);

            if (!validPassword)
            {
                result.AddError(ErrorCode.IncorrectPassword, IdentityErrorMessages.IncorrectPassword); 
            }

            return identity;
        }


        private string GetJwtString(IdentityUser identity, UserProfile profile)
        {
            var claimsIdentity = new ClaimsIdentity(new Claim[]
                {
                    new Claim (JwtRegisteredClaimNames.Sub, identity.Email!),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Email, identity.Email!),
                    new Claim("IdentityId", identity.Id),
                    new Claim("UserProfileId", profile.UserProfileId.ToString())

                });

            var token = _identityService.CreateSecurityToken(claimsIdentity);

            return _identityService.WriteToken(token);
        }
    }
}
