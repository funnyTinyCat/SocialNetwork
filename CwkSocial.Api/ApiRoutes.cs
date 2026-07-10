using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace CwkSocial.Api
{
    public static class ApiRoutes
    {
        public const string baseRoute = "api/v{version:apiVersion}/[controller]";

        public static class UserProfiles
        {
            public const string idRoute = "{id}";
        }

        public static class Posts
        {
            public const string idRoute = "{id}";
            public const string postComments = "{postId}/comments";
            public const string commentById = "{postId}/comments/{commentId}";
        }



    }
}
