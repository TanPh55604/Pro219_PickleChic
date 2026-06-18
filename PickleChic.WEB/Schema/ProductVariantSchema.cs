using FluentValidation;
using PickleChic.WEB.Model;

namespace PickleChic.WEB.Schema
{
    public class ProductVariantSchema
    {
        public class CreateUpdate : AbstractValidator<ProductVariantModel>
        {
            public CreateUpdate()
            {
                RuleFor(x => x.ProductId)
                    .GreaterThan(0).WithMessage("Sản phẩm không hợp lệ");

                RuleFor(x => x.SKU)
                    .NotEmpty().WithMessage("SKU không được để trống")
                    .MaximumLength(100).WithMessage("SKU có độ dài tối đa 100 ký tự");

                RuleFor(x => x.VariantName)
                    .MaximumLength(500).WithMessage("Tên biến thể có độ dài tối đa 500 ký tự");

                RuleFor(x => x.Price)
                    .GreaterThan(0).WithMessage("Giá phải lớn hơn 0");

                RuleFor(x => x.StockQuantity)
                    .GreaterThanOrEqualTo(0).WithMessage("Tồn kho không được âm");

                RuleFor(x => x.Status)
                    .Must(x => x == 0 || x == 1)
                    .WithMessage("Trạng thái không hợp lệ");
            }

            public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
            {
                var result = await ValidateAsync(
                    ValidationContext<ProductVariantModel>
                        .CreateWithOptions((ProductVariantModel)model, x => x.IncludeProperties(propertyName)));

                if (result.IsValid)
                {
                    return Array.Empty<string>();
                }

                return result.Errors.Select(e => e.ErrorMessage);
            };
        }
    }
}
