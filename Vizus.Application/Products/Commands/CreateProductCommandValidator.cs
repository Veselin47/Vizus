using FluentValidation;

namespace Vizus.Application.Products.Commands;

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Името на продукта е задължително.")
            .MaximumLength(200);

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Цената трябва да е по-голяма от 0.");

        RuleFor(x => x.StockQuantity)
            .GreaterThanOrEqualTo(0).WithMessage("Наличността не може да е отрицателна.");

        RuleFor(x => x.CategoryId)
            .GreaterThan(0).WithMessage("Трябва да избереш валидна категория.");
    }
}