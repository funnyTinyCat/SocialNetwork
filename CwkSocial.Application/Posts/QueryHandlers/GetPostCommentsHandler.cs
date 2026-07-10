using Cwk.Domain.Aggregates.PostAggregate;
using CwkSocial.Application.Enums;
using CwkSocial.Application.Models;
using CwkSocial.Application.Posts.Queries;
using CwkSocial.Dal;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CwkSocial.Application.Posts.QueryHandlers
{
    internal class GetPostCommentsHandler : IRequestHandler<GetPostComments, OperationResult<List<PostComment>>>
    {
        private readonly DataContext _context;

        public GetPostCommentsHandler(DataContext context)
        {
            _context = context;
        }

        public async Task<OperationResult<List<PostComment>>> Handle(GetPostComments request, CancellationToken cancellationToken)
        {
            var result = new OperationResult<List<PostComment>>();

            try
            {
                var post = await _context.Posts
                    .Include(p => p.Comments)
                    .FirstOrDefaultAsync(p => p.PostId == request.PostId);

                result.Payload = post.Comments.ToList();
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
