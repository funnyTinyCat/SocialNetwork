using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cwk.Domain.Aggregates.UserProfileAggregate;
using Cwk.Domain.Exceptions;
using Cwk.Domain.Validators.PostValidators;

namespace Cwk.Domain.Aggregates.PostAggregate
{
    public  class Post
    {
        private readonly List<PostComment> _comments = new List<PostComment>();
        private readonly List<PostInteraction> _interactions = new List<PostInteraction>();
        private Post()
        {
            
        }
        public Guid PostId { get; private set; }
        public Guid UserProfileId  { get; private set; }

        public UserProfile UserProfile { get; private set; }
        public string TextContent { get; private set; } = string.Empty;
        public DateTime CreatedDate { get; private set; }
        public DateTime LastModified { get; private set; }
        public IEnumerable<PostComment> Comments { get { return _comments; } }
        public IEnumerable<PostInteraction> Interactions { get { return _interactions; } }

        /// <summary>
        /// Creates a new post instance
        /// </summary>
        /// <param name="userProfileId">User profile ID</param>
        /// <param name="textContent">Post content</param>
        /// <returns><see cref="Post"/></returns>
        /// <exception cref="PostNotValidException"></exception>
        // factory method
        public static Post CreatePost(Guid userProfileId, string textContent)
        {
            var validator = new PostValidator();

            var objToValidate = new Post
            {
                UserProfileId = userProfileId,
                TextContent = textContent,
                CreatedDate = DateTime.UtcNow,
                LastModified = DateTime.UtcNow
            };

            var validationResult = validator.Validate(objToValidate);

            if (validationResult.IsValid)
            {
                return objToValidate;
            }

            var exception = new PostNotValidException("Post is not valid.");

            validationResult.Errors.ForEach( vr => exception.ValidationErrors.Add(vr.ErrorMessage));

            throw exception;
        }

        /// <summary>
        /// Updates the post text
        /// </summary>
        /// <param name="textContent">The updated post text.</param>
        /// <exception cref="PostNotValidException" ></exception>

        // public methods:

        public void UpdatePostText(string textContent)
        {
            if (string.IsNullOrWhiteSpace(textContent))
            {
                var exception = new PostNotValidException("Cannot update post text. Post text is not valid.");

                exception.ValidationErrors.Add("The provided text is either null or contains only white space.");

                throw exception;
            }

            TextContent = textContent;
            LastModified = DateTime.UtcNow;
        }

        public void AddPostComment(PostComment comment)
        {
            _comments.Add(comment);
        }

        public void RemovePostComment(PostComment comment)
        {
            _comments.Remove(comment);
        }

        public void AddPostInteraction(PostInteraction interaction)
        {
            _interactions.Add(interaction);
        }

        public void RemovePostInteraction(PostInteraction interaction)
        {
            _interactions.Remove(interaction);
        }
    }
}
