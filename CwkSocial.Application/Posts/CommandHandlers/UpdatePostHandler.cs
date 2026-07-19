using Cwk.Domain.Aggregates.PostAggregate;
using Cwk.Domain.Exceptions;
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
    internal class UpdatePostHandler : IRequestHandler<UpdatePost, OperationResult<Post>>
    {
        private readonly DataContext _context;

        public UpdatePostHandler(DataContext context)
        {
            _context = context;
        }
        public async Task<OperationResult<Post>> Handle(UpdatePost request, CancellationToken cancellationToken)
        {
            var result = new OperationResult<Post>();

            try
            {
                var post = await _context.Posts.FirstOrDefaultAsync(p => p.PostId == request.PostId);

                if (post == null)
                {
                    result.AddError(ErrorCode.NotFound, string.Format(PostsErrorMessages.PostNotFound, request.PostId));

                    return result;
                }

                if (post.UserProfileId != request.UserProfileId)
                {
                    result.AddError(ErrorCode.PostUpdateNotPossible, PostsErrorMessages.PostUpdateNotPossible);

                    return result;
                }

                post.UpdatePostText(request.Text);
                await _context.SaveChangesAsync(cancellationToken);

                result.Payload = post;
            }
            catch (PostNotValidException e)
            {
                e.ValidationErrors.ForEach(er =>
                {
                    result.AddError(ErrorCode.ValidationError, er);                
                });
            }
            catch (Exception ex)
            {
                result.AddUnknownError(ex.Message);
            }

            return result;
        }
    }
}
