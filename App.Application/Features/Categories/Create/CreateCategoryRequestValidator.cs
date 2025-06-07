using FluentValidation;

namespace App.Application.Features.Categories.Create;

public class UpdateCategoryRequestValidator : AbstractValidator<CreateCategoryRequest>
{
    public UpdateCategoryRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .Length(3, 20).WithMessage("Name must be between 3 and 20 characters long.");
    }
}