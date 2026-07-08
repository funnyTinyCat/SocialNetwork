using Cwk.Domain.Aggregates.UserProfileAggregate;
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
                result.IsError = true;
                var error = new Error
                {
                    Code = Enums.ErrorCode.NotFound,
                    Message = $"User Profile with ID {request.UserProfileId} is not found."
                };
                result.Errors.Add(error);

                return result;
            }

            result.Payload = userProfile;

            return result;
        }
    }
}
