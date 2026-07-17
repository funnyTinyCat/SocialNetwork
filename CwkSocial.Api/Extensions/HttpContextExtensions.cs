using System.Runtime.CompilerServices;
using System.Security.Claims;

namespace CwkSocial.Api.Extensions
{
    public static class HttpContextExtensions
    {
        public static Guid GetUserProfileIdClaimValue(this HttpContext context ) 
        {
   
            return GetGuidClaimValue("UserProfileId", context);
        }

        public static Guid GetIdentityIdClaimValue(this HttpContext context)
        {

            return GetGuidClaimValue("IdentityId", context);
        }

        private static Guid GetGuidClaimValue(string key, HttpContext context)
        {
            var identity = context?.User.Identity as ClaimsIdentity;
            var id = identity?.FindFirst(key)?.Value;

            return Guid.Parse ( id! );
        }
    }

}
