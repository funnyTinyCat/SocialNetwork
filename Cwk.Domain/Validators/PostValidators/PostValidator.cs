using Cwk.Domain.Aggregates.PostAggregate;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cwk.Domain.Validators.PostValidators
{
    internal class PostValidator : AbstractValidator<Post>
    {
        public PostValidator()
        {
            RuleFor(p => p.TextContent).NotNull().WithMessage("Post text content should not be null.")
                .NotEmpty().WithMessage("Post text content should not be empty");

        }
    }
}
