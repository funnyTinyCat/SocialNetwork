using Microsoft.AspNetCore.Mvc;
using Cwk.Domain.Aggregates.PostAggregate;
using MediatR;
using AutoMapper;
using CwkSocial.Application.Posts.Queries;
using CwkSocial.Api.Contracts.Posts.Responses;
using CwkSocial.Api.Filters;
using CwkSocial.Api.Contracts.Posts.Requests;
using CwkSocial.Application.Posts.Commands;

namespace CwkSocial.Api.Controllers.V1
{
    [ApiVersion("1.0")]
    [Route(ApiRoutes.baseRoute)]
    [ApiController]
    public class PostsController : BaseController
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public PostsController(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper; 
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPosts()
        {
            var query = new GetAllPosts();

            var result = await _mediator.Send(query);

            var posts = _mapper.Map<List<PostResponse>>(result.Payload);

            return result.IsError ? HandleErrorResponse(result.Errors) : Ok(posts);
        }

        [ValidateGuid("id")]
        [HttpGet(ApiRoutes.Posts.idRoute)]
        public async Task<IActionResult> GetById(string id)
        {
            var postId = Guid.Parse(id);

            var query = new GetPostById() { PostId = postId };

            var result = await _mediator.Send(query);

            var post = _mapper.Map<PostResponse>(result.Payload);
  
            if (result.IsError)
            {
                HandleErrorResponse(result.Errors);  
            }

            return Ok(post);
        }

        [ValidateModel]
        [HttpPost]
        public async Task<IActionResult> CreatePost([FromBody] PostCreate post)
        {
            var command = new CreatePost() { UserProfileId = post.UserProfileId, TextContent = post.TextContent };

            var result = await _mediator.Send(command);
            var mapped = _mapper.Map<PostResponse>(result.Payload);

            return result.IsError 
                ? HandleErrorResponse(result.Errors) 
                : CreatedAtAction(nameof(GetById), new { id = mapped.PostId }, mapped);
        }
    }
}
