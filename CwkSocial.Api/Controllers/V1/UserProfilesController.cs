using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CwkSocial.Api.Controllers.V1
{
    [ApiVersion("1.0")]
    [Route(ApiRoutes.baseRoute)]
    [ApiController]
    public class UserProfilesController : ControllerBase
    {
        public UserProfilesController()
        {
            
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProfiles()
        {
            return (IActionResult) Task.FromResult(Ok());
        }

        [HttpPost]
        public async Task<IActionResult> CreateUserProfile()
        {
            return (IActionResult)Task.FromResult(Ok());
        }


    }
}
