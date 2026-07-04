using Cwk.Domain.Aggregates.UserProfileAggregate;
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
   
    internal class UpdateUserProfileBasicInfoHandler : IRequestHandler<UpdateUserProfileBasicInfo>
    {
        private readonly DataContext _context;

        public UpdateUserProfileBasicInfoHandler(DataContext context)
        {
            _context = context;
        }
        public async Task Handle(UpdateUserProfileBasicInfo request, CancellationToken cancellationToken)
        {
            var userProfile =
                await _context.UserProfiles.FirstOrDefaultAsync(up => up.UserProfileId == request.UserProfileId);



            var basicInfo = BasicInfo.CreateBasicInfo(request.FirstName, request.LastName, request.EmailAddress,
                request.Phone, request.DateOfBirth, request.CurrentCity);

            userProfile.UpdateBasicInfo(basicInfo);

            _context.UserProfiles.Update(userProfile);
            await _context.SaveChangesAsync();

           
        }
    }
}
