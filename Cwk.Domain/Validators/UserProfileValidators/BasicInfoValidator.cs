using Cwk.Domain.Aggregates.UserProfileAggregate;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cwk.Domain.Validators.UserProfileValidators
{
    public class BasicInfoValidator : AbstractValidator<BasicInfo>
    {
        public BasicInfoValidator()
        {
            RuleFor(bi => bi.FirstName).NotNull().WithMessage("First name is required.")
                .MinimumLength(3).WithMessage("First name must be at least 3 characters long.")
                .MaximumLength(50).WithMessage("First name must be maximum 50 characters long.");
            RuleFor(bi => bi.LastName).NotNull().WithMessage("Last name is required.")
                .MinimumLength(3).WithMessage("Last name must be at least 3 characters long.")
                .MaximumLength(50).WithMessage("Last name must be at most 50 characters long.");
            RuleFor(bi => bi.EmailAddress).NotNull().WithMessage("Email address is required field.")
                .EmailAddress().WithMessage("That is not valid email format.");
            RuleFor(bi => bi.DateOfBirth).InclusiveBetween(new DateTime(DateTime.Now.AddYears(-120).Ticks),
                new DateTime(DateTime.Now.AddYears(-18).Ticks))
                .WithMessage("Age must be above 18 years."); 

        }
    }
}
