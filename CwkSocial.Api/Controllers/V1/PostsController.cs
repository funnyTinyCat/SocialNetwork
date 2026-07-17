using Microsoft.AspNetCore.Mvc;
using Cwk.Domain.Aggregates.PostAggregate;
using MediatR;
using AutoMapper;
using CwkSocial.Application.Posts.Queries;
using CwkSocial.Api.Contracts.Posts.Responses;
using CwkSocial.Api.Filters;
using CwkSocial.Api.Contracts.Posts.Requests;
using CwkSocial.Application.Posts.Commands;
using CwkSocial.Api.Contracts.Common;
using CwkSocial.Application.Enums;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using CwkSocial.Api.Extensions;

namespace CwkSocial.Api.Controllers.V1
{
    [Authorize]
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
            var userProfileId = HttpContext.GetUserProfileIdClaimValue();

            var command = new CreatePost() 
            { 
                UserProfileId = userProfileId,
                TextContent = post.TextContent 
            };

            var result = await _mediator.Send(command);
            var mapped = _mapper.Map<PostResponse>(result.Payload);

            return result.IsError 
                ? HandleErrorResponse(result.Errors) 
                : CreatedAtAction(nameof(GetById), new { id = mapped.PostId }, mapped);
        }

        [ValidateModel]
        [ValidateGuid("id")]
        [HttpPatch(ApiRoutes.Posts.idRoute)]
        public async Task<IActionResult> UpdatePost([FromRoute] string id, [FromBody] PostUpdate post)
        {
            var userProfileId = HttpContext.GetUserProfileIdClaimValue();

            var command = new UpdatePost() { 
                PostId = Guid.Parse(id), 
                Text = post.Text,
                UserProfileId = userProfileId
            };

            var result = await _mediator.Send(command);

            var mapped = _mapper.Map<PostResponse>(result.Payload);

            if (result.IsError)
            {
                return HandleErrorResponse(result.Errors);
            }

            return NoContent();
        }

        [ValidateGuid("id")]
        [HttpDelete(ApiRoutes.Posts.idRoute)]
        public async Task<IActionResult> DeletePost(string id)
        {
            var userProfileId = HttpContext.GetUserProfileIdClaimValue();

            var command = new DeletePost() 
            { 
                PostId = Guid.Parse(id),
                UserProfileId = userProfileId
            };

            var result = await _mediator.Send(command);

            if (result.IsError)
            {
                return HandleErrorResponse(result.Errors);
            }

            return NoContent();
        }

        [ValidateGuid("postId")]
        [HttpGet(ApiRoutes.Posts.postComments)]
        public async Task<IActionResult> GetCommentsByPostId(string postId)
        {
            var query = new GetPostComments() { PostId = Guid.Parse(postId) };

            var result = await _mediator.Send(query);

            if (result.IsError) HandleErrorResponse(result.Errors);

            var comments = _mapper.Map<List<PostCommentResponse>>(result.Payload);

            return  Ok(comments); 
        }

        [HttpPost(ApiRoutes.Posts.postComments)]
        [ValidateModel]
        [ValidateGuid("postId")]
        public async Task<IActionResult> AddCommentToPost(string postId, [FromBody] PostCommentCreate comment)
        {
            var isGuidValid = Guid.TryParse(comment.UserProfileId, out var userProfileId);

            if (!isGuidValid)
            {
                var apiError = new ErrorResponse();

                apiError.StatusCode = 400;
                apiError.StatusPhrase = "Bad Request";
                apiError.Timestamp = DateTime.Now;
                apiError.Errors.Add($"Provided User profile Id is not a valid Guid format.");

                return BadRequest(apiError);
            }

            var command = new AddPostComment() { PostId= Guid.Parse(postId), Text = comment.Text, 
                UserProfileId = userProfileId };

            var result = await _mediator.Send(command);

            if (result.IsError) return HandleErrorResponse(result.Errors);

            var mapped = _mapper.Map<PostCommentResponse>(result.Payload);

            return Ok(mapped); 
        }
    }
}
