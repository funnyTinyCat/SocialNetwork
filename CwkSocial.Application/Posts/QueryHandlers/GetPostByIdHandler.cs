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
    internal class GetPostByIdHandler : IRequestHandler<GetPostById, OperationResult<Post>>
    {
        private readonly DataContext _context;

        public GetPostByIdHandler(DataContext context)
        {
            _context = context;
        }

        public async Task<OperationResult<Post>> Handle(GetPostById request, CancellationToken cancellationToken)
        {
            var result = new OperationResult<Post>();

            var post = await _context.Posts.FirstOrDefaultAsync(p => p.PostId == request.PostId);

            if (post == null)
            {
                result.AddError(ErrorCode.NotFound, string.Format(PostsErrorMessages.PostNotFound, request.PostId));
             
                return result;
            }

            result.Payload = post;

            return result;
        }
    }
}
