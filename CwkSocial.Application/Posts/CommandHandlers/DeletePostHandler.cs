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
    internal class DeletePostHandler : IRequestHandler<DeletePost, OperationResult<Post>>
    {
        private readonly DataContext _context;

        public DeletePostHandler(DataContext context)
        {
            _context = context;
        }
        public async Task<OperationResult<Post>> Handle(DeletePost request, CancellationToken cancellationToken)
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
                        Message = $"Not found post with ID {request.PostId}"
                    };

                    result.Errors.Add(error);

                    return result;
                }

                result.Payload = post;

                _context.Remove(post);
                await _context.SaveChangesAsync();

            }
            catch (PostNotValidException ex)
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
                result.IsError = true;

                var error = new Error
                {
                    Code = ErrorCode.UnknownError,
                    Message = $"{ex.Message}"
                };

                result.Errors.Add(error);
            }

            return result;
        }
    }
}
