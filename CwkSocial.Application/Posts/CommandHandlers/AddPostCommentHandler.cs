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
    internal class AddPostCommentHandler : IRequestHandler<AddPostComment, OperationResult<PostComment>>
    {
        private readonly DataContext _context;

        public AddPostCommentHandler(DataContext context)
        {
            _context = context;
        }
        public async Task<OperationResult<PostComment>> Handle(AddPostComment request, CancellationToken cancellationToken)
        {
            var result = new OperationResult<PostComment>();

            try
            {
                var post = await _context.Posts.FirstOrDefaultAsync(p => p.PostId == request.PostId);

                if (post == null)
                {
                    result.IsError = true;

                    var error = new Error
                    {
                        Code = ErrorCode.NotFound,
                        Message = $"Post with ID {request.PostId} has not been found."
                    };

                    result.Errors.Add(error);

                    return result; 
                }

                var comment = PostComment.CreatePostComment(request.PostId, request.Text, request.UserProfileId);   
                
                post.AddPostComment(comment);

                _context.Posts.Update(post);

                await _context.SaveChangesAsync();

                result.Payload = comment;
            }
            catch (PostCommentNotValidException ex)
            {
                result.IsError = true;

                ex.ValidationErrors.ForEach(er =>
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
