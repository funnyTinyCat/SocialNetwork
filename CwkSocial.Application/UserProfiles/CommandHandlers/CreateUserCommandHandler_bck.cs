using AutoMapper;
using Cwk.Domain.Aggregates.UserProfileAggregate;
using Cwk.Domain.Exceptions;
using CwkSocial.Application.Enums;
using CwkSocial.Application.Models;
using CwkSocial.Application.UserProfiles.Commands;
using CwkSocial.Dal;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CwkSocial.Application.UserProfiles.CommandHandlers
{
    public class CreateUserCommandHandler_bck : IRequestHandler<CreateUserCommand_bck, OperationResult<UserProfile>>
    {
        private readonly DataContext _context;

        public CreateUserCommandHandler_bck(DataContext context)
        {
            _context = context;
        }
        public async Task<OperationResult<UserProfile>> Handle(CreateUserCommand_bck request, CancellationToken cancellationToken)
        {
            var result = new OperationResult<UserProfile>();

            try
            {

                var basicInfo = BasicInfo.CreateBasicInfo(request.FirstName, request.LastName, request.EmailAddress,
                        request.Phone, request.DateOfBirth, request.CurrentCity);

                var userProfile = UserProfile.CreateUserProfile(Guid.NewGuid().ToString(), basicInfo);

                _context.UserProfiles.Add(userProfile);
                await _context.SaveChangesAsync();

                result.Payload = userProfile;

                return result;
            }
            catch (UserProfileNotValidException ex)
            {
                ex.ValidationErrors.ForEach(e =>
                {
                    result.AddError(ErrorCode.ValidationError, e);       
                });
            }
            catch (Exception ex)
            {
                result.AddError(ErrorCode.UnknownError, ex.Message);
            }

            return result;
        }
    }
}
