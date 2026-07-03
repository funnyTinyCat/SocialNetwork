using Microsoft.AspNetCore.Mvc;
using Cwk.Domain.Aggregates.PostAggregate;

namespace CwkSocial.Api.Controllers.V1
{
    [ApiVersion("1.0")]
    [Route(ApiRoutes.baseRoute)]
    [ApiController]
    public class PostsController : ControllerBase
    {
        [HttpGet(ApiRoutes.Posts.idRoute)]
        public IActionResult GetById(Guid id)
        {
           

            return Ok();
        }
    }
}
