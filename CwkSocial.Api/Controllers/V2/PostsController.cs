using Cwk.Domain.Aggregates.PostAggregate;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CwkSocial.Api.Controllers.V2
{
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class PostsController : ControllerBase
    {

        [HttpGet("{id}")]
        public IActionResult GetById(Guid id)
        {
           
            return Ok();
        }

    }
}
