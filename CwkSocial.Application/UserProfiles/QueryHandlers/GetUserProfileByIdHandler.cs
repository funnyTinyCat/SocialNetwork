using Cwk.Domain.Aggregates.UserProfileAggregate;
using CwkSocial.Application.Enums;
using CwkSocial.Application.Models;
using CwkSocial.Application.UserProfiles.Queries;
using CwkSocial.Dal;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CwkSocial.Application.UserProfiles.QueryHandlers
{
    internal class GetUserProfileByIdHandler : IRequestHandler<GetUserProfileById, OperationResult<UserProfile>>
    {
        private readonly DataContext _context;

        public GetUserProfileByIdHandler(DataContext context)
        {
            _context = context;
        }

        public async Task<OperationResult<UserProfile>> Handle(GetUserProfileById request, CancellationToken cancellationToken)
        {
            var result = new OperationResult<UserProfile>();

            var userProfile = await _context.UserProfiles.FirstOrDefaultAsync(up => 
                up.UserProfileId == request.UserProfileId);

            if (userProfile == null)
            {
                result.AddError(ErrorCode.NotFound, 
                    string.Format(UserProfilesErrorMessages.UserProfileNotFound, request.UserProfileId));                 

                return result;
            }

            result.Payload = userProfile;

            return result;
        }
    }
}
