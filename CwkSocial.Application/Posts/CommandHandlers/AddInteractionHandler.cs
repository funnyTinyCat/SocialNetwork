using Cwk.Domain.Aggregates.PostAggregate;
using CwkSocial.Application.Enums;
using CwkSocial.Application.Models;
using CwkSocial.Application.Posts.Commands;
using CwkSocial.Dal;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CwkSocial.Application.Posts.CommandHandlers
{
    internal class AddInteractionHandler : IRequestHandler<AddInteraction, OperationResult<PostInteraction>>
    {
        private readonly DataContext _context;

        public AddInteractionHandler(DataContext context)
        {
            _context = context;
        }
        public async Task<OperationResult<PostInteraction>> Handle(AddInteraction request, CancellationToken cancellationToken)
        {
            var result = new OperationResult<PostInteraction>();

            try
            {
                var post = await _context.Posts.Include(p => p.Interactions)
                    .FirstOrDefaultAsync(p => p.PostId == request.PostId, cancellationToken);

                if (post == null)
                {
                    result.AddError(ErrorCode.NotFound, PostsErrorMessages.PostNotFound);

                    return result;
                }

                var interaction = PostInteraction.CreatePostInteraction(request.PostId, request.UserProfileId, 
                    request.Type);

                post.AddPostInteraction(interaction);

                _context.Posts.Update(post);
                await _context.SaveChangesAsync(cancellationToken);

                result.Payload = interaction;
                
            }
            catch (Exception ex)
            {
                result.AddUnknownError(ex.Message);
            }

            return result;
        }
    }
}
