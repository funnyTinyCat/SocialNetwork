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
                    result.IsError = true;

                    var error = new Error
                    {
                        Code = ErrorCode.NotFound,
                        Message = $"No post found with ID {request.PostId}"
                    };

                    result.Errors.Add(error);

                    return result;
                }

                if (post.UserProfileId != request.UserProfileId)
                {
                    result.IsError = true;

                    var error = new Error
                    {
                        Code = ErrorCode.PostUpdateNotPossible,
                        Message = $"Post update not possible because it's not the post owner that " +
                            " initiates the update "
                    };

                    result.Errors.Add(error);

                    return result;
                }

                post.UpdatePostText(request.Text);
                await _context.SaveChangesAsync();

                result.Payload = post;
            }
            catch (PostNotValidException e)
            {
                result.IsError = true;

                e.ValidationErrors.ForEach(er =>
                {
                    var error = new Error
                    {
                        Code = ErrorCode.NotFound,
                        Message = $"{e.Message}"
                    };

                    result.Errors.Add(error);
                });
            }
            catch (Exception ex)
            {
                var error = new Error
                {
                    Code = ErrorCode.UnknownError,
                    Message = ex.Message
                };
                result.IsError = true;
                result.Errors.Add(error);
            }

            return result;
        }
    }
}
