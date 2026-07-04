using CwkSocial.Application.UserProfiles.Commands;
using CwkSocial.Dal;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace CwkSocial.Application.UserProfiles.CommandHandlers
{
    internal class DeleteUserProfileHandler : IRequestHandler<DeleteUserProfile>
    {
        private readonly DataContext _context;
        public DeleteUserProfileHandler(DataContext context)
        {
            _context = context;
        }
        public async Task Handle(DeleteUserProfile request, CancellationToken cancellationToken)
        {
            var userProfile = 
                await _context.UserProfiles.FirstOrDefaultAsync(up => up.UserProfileId == request.UserProfileId);

            //if (userProfile == null)
            //    return new Unit();

            _context.UserProfiles.Remove(userProfile);
            await _context.SaveChangesAsync();

           
        }
    }
}
