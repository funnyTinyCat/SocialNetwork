using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace CwkSocial.Api
{
    public class ApiRoutes
    {
        public const string baseRoute = "api/v{version:apiVersion}/[controller]";

        public class UserProfiles
        {
            public const string idRoute = "{id}";
        }

        public class Posts
        {
            public const string idRoute = "{id}";
        }


    }
}
