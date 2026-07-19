using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace CwkSocial.Application.Posts
{
    internal class PostsErrorMessages
    {
        public const string PostNotFound = "No post found with ID {0}";

        public const string PostDeleteNotPossible = "Only the owner of the post can delete it";

        public const string PostUpdateNotPossible = "Post update not possible because it's not the post owner that " +
                            "initiates the update ";
    }
}
