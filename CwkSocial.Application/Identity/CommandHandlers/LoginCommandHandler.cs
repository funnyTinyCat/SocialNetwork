using AutoMapper;
using CwkSocial.Application.Enums;
using CwkSocial.Application.Identity.Commands;
using CwkSocial.Application.Models;
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
using System.Text;
using System.Threading.Tasks;

namespace CwkSocial.Application.Identity.CommandHandlers
{
    internal class LoginCommandHandler : IRequestHandler<LoginCommand, OperationResult<string>>
    {
        private readonly DataContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _config;

        public LoginCommandHandler(DataContext context, UserManager<IdentityUser> userManager,
            IConfiguration config)
        {
            _context = context;
            _userManager = userManager;
            _config = config;
        }
        public async Task<OperationResult<string>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var result = new OperationResult<string>();

            try
            {
                var identity = await _userManager.FindByEmailAsync(request.Username);

                if (identity == null)
                {
                    result.IsError = true;
                    var error = new Error
                    {
                        Code = ErrorCode.IdentityUserDoesNotExist,
                        Message = $"Unable to find the user with the specified username."
                    };
                    result.Errors.Add(error);

                    return result;
                }

                var validPassword = await _userManager.CheckPasswordAsync(identity, request.Password);

                if (!validPassword)
                {
                    result.IsError = true;
                    var error = new Error
                    {
                        Code = ErrorCode.IncorrectPassword,
                        Message = $"Provided password is not valid. Login failed."
                    };
                    result.Errors.Add(error);

                    return result;
                }

                var userProfile = await _context.UserProfiles.FirstOrDefaultAsync(up => up.IdentityId == identity.Id);

                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_config["JwtSettings:SigningKey"]!);
                var tokenDescriptor = new SecurityTokenDescriptor()
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                        new Claim(JwtRegisteredClaimNames.Sub, identity.Email!),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.Email, identity.Email!),
                        new Claim("IdentityId", identity.Id),
                        new Claim("UserProfileId", userProfile.UserProfileId.ToString())
                    }),
                    Expires = DateTime.Now.AddHours(2),
                    Audience = _config["JwtSettings:Audience"],
                    Issuer = _config["JwtSettings:Issuer"],
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                        SecurityAlgorithms.HmacSha256Signature)
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                result.Payload = tokenHandler.WriteToken(token);

                return result;  
            }
            catch (Exception ex) 
            {
                var error = new Error
                {
                    Code = ErrorCode.UnknownError,
                    Message = $"{ex.Message}"
                };
                result.Errors.Add(error);
                result.IsError = true;
            }

            return result;
        }
    }
}
