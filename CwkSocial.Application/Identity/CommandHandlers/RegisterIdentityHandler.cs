using Cwk.Domain.Aggregates.UserProfileAggregate;
using Cwk.Domain.Exceptions;
using CwkSocial.Application.Enums;
using CwkSocial.Application.Identity.Commands;
using CwkSocial.Application.Models;
using CwkSocial.Application.Options;
using CwkSocial.Dal;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;

namespace CwkSocial.Application.Identity.CommandHandlers
{  
    internal class RegisterIdentityHandler : IRequestHandler<RegisterIdentity, OperationResult<string>>
    {
        private readonly DataContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly JwtSettings _jwtSettings;
        private readonly IConfiguration _config;

        public RegisterIdentityHandler(DataContext context, UserManager<IdentityUser> userManager,
            IOptions<JwtSettings> jwtSettings, IConfiguration config)
        {
            _context = context;
            _userManager = userManager;
            _jwtSettings = jwtSettings.Value;
            _config = config;
        }
        public async Task<OperationResult<string>> Handle(RegisterIdentity request, CancellationToken cancellationToken)
        {
            var result = new OperationResult<string>();

            try
            {
                var existingIdentity = await _userManager.FindByEmailAsync(request.Username);

                if (existingIdentity != null)
                {
                    result.IsError = true;
                    var error = new Error
                    {
                        Code = ErrorCode.IdentityUserAlreadyExists,
                        Message = $"There is already user with username {request.Username}"
                    };

                    result.Errors.Add(error);

                    return result;  
                }

                var identity = new IdentityUser
                {
                    UserName = request.Username,
                    Email = request.Username,
                };

                using var transaction = _context.Database.BeginTransaction();

                var createdIdentity = await _userManager.CreateAsync(identity, request.Password);

                if (!createdIdentity.Succeeded)
                {
                    await transaction.RollbackAsync();

                    result.IsError = true;

                    foreach (var identityError in createdIdentity.Errors)
                    {
                        var error = new Error
                        {
                            Code = ErrorCode.IdentityCreationFailed,
                            Message = $"{identityError.Description}"
                        };
                        result.Errors.Add(error);
                    }

                    return result;
                }

                var profileInfo = BasicInfo.CreateBasicInfo(request.FirstName, request.LastName, request.Username,
                    request.Phone, request.DateOfBirth, request.CurrentCity);

                var profile = UserProfile.CreateUserProfile(identity.Id, profileInfo);



                try
                {

                    _context.UserProfiles.Add(profile);


                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();

                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                }

                var tokenHandler = new JwtSecurityTokenHandler();

                var key = Encoding.ASCII.GetBytes(_config["JwtSettings:SigningKey"]!);

                var tokenDescriptor = new SecurityTokenDescriptor()
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                        new Claim(JwtRegisteredClaimNames.Sub, identity.Email),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.Email, identity.Email),
                        new Claim("IdentityId", identity.Id),
                        new Claim("UserProfileId", profile.UserProfileId.ToString()),
                    }),
                    Expires = DateTime.UtcNow.AddHours(2),
                    Audience = _config["JwtSettings:Audience"],
                    Issuer = _config["JwtSettings:Issuer"],
                    SigningCredentials = new SigningCredentials(
                        new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);

                result.Payload = tokenHandler.WriteToken(token);

                return result;
            }
            catch (UserProfileNotValidException ex)
            {
                result.IsError = true;
                ex.ValidationErrors.ForEach(e =>
                {
                    var error = new Error
                    {
                        Code = ErrorCode.ValidationError,
                        Message = $"{ex.Message}"
                    };

                    result.Errors.Add(error);
                });
            }
            catch (Exception ex)
            {
                var error = new Error
                {
                    Code = ErrorCode.UnknownError,
                    Message = $"{ex.Message}"
                };

                result.IsError = true;
                result.Errors.Add(error);
            }

            return result;
        }
    }

}
