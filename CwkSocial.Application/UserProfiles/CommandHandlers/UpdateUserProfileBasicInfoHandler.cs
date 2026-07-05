using Cwk.Domain.Aggregates.UserProfileAggregate;
using CwkSocial.Application.Enums;
using CwkSocial.Application.Models;
using CwkSocial.Application.UserProfiles.Commands;
using CwkSocial.Dal;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CwkSocial.Application.UserProfiles.CommandHandlers
{
   
    internal class UpdateUserProfileBasicInfoHandler : IRequestHandler<UpdateUserProfileBasicInfo, OperationResult<UserProfile>>
    {
        private readonly DataContext _context;

        public UpdateUserProfileBasicInfoHandler(DataContext context)
        {
            _context = context;
        }
        public async Task<OperationResult<UserProfile>> Handle(UpdateUserProfileBasicInfo request, CancellationToken cancellationToken)
        {
            var result = new OperationResult<UserProfile>();

            try
            {
                var userProfile =
                    await _context.UserProfiles.FirstOrDefaultAsync(up => up.UserProfileId == request.UserProfileId);

                if (userProfile == null)
                {
                    result.IsError = true;

                    var error = new Error
                    {
                        Code = ErrorCode.NotFound,
                        Message = $"No User Profile found with ID {request.UserProfileId}"
                    };

                    result.Errors.Add(error);

                    return result;
                }

                var basicInfo = BasicInfo.CreateBasicInfo(request.FirstName, request.LastName, request.EmailAddress,
                    request.Phone, request.DateOfBirth, request.CurrentCity);

                userProfile.UpdateBasicInfo(basicInfo);

                _context.UserProfiles.Update(userProfile);
                await _context.SaveChangesAsync();

                result.Payload = userProfile;

                return result;
            }
            catch (Exception ex)
            {
                result.IsError = true;
                var error = new Error { Code = ErrorCode.ServerError, Message = ex.Message };

                result.Errors.Add(error);
            }

           return result;
        }
    }
}
