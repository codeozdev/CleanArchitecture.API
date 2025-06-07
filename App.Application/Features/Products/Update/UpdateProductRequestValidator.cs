using FluentValidation;

namespace App.Application.Features.Products.Update;

public class UpdateProductRequestValidator : AbstractValidator<UpdateProductRequest>
{
    // constructor to define validation rules for UpdateProductRequest
    public UpdateProductRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .Length(3, 20).WithMessage("Name must be between 3 and 20 characters long.");
        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than zero.");
        RuleFor(x => x.Stock)
            .InclusiveBetween(1, 100).WithMessage("Stock must be between 1 and 100.");
        RuleFor(x => x.CategoryId)
            .GreaterThan(0).WithMessage("CategoryId must be a valid category.");
    }
}
