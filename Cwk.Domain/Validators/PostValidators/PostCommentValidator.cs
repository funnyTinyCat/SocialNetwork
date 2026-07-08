using Cwk.Domain.Aggregates.PostAggregate;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cwk.Domain.Validators.PostValidators
{
    internal class PostCommentValidator : AbstractValidator<PostComment>
    {
        public PostCommentValidator()
        {
            RuleFor(pc => pc.Text).NotNull().WithMessage("Could not be null value.")
                .NotEmpty().WithMessage("Comment text should not be empty.")
                .MaximumLength(1000)
                .MinimumLength(1);


        }
    }
}
