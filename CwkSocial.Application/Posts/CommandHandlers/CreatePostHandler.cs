using Cwk.Domain.Aggregates.PostAggregate;
using Cwk.Domain.Exceptions;
using CwkSocial.Application.Enums;
using CwkSocial.Application.Models;
using CwkSocial.Application.Posts.Commands;
using CwkSocial.Dal;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CwkSocial.Application.Posts.CommandHandlers
{
    internal class CreatePostHandler : IRequestHandler<CreatePost, OperationResult<Post>>
    {
        private readonly DataContext _context;

        public CreatePostHandler(DataContext context)
        {
            _context = context;
        }
        public async Task<OperationResult<Post>> Handle(CreatePost request, CancellationToken cancellationToken)
        {
           var result = new OperationResult<Post>();

            try
            {
                var post = Post.CreatePost(request.UserProfileId, request.TextContent);

                _context.Posts.Add(post);
                await _context.SaveChangesAsync(cancellationToken);

                result.Payload = post;
            }
            catch (PostNotValidException ex)
            {
                ex.ValidationErrors.ForEach(e =>
                {
                    result.AddError(ErrorCode.ValidationError, e);
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
