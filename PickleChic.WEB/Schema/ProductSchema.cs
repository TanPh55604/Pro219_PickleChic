using FluentValidation;
using PickleChic.WEB.Model;

namespace PickleChic.WEB.Schema
{
    public class ProductSchema
    {
        public class CreateUpdate : AbstractValidator<ProductModel>
        {
            public CreateUpdate()
            {
                RuleFor(x => x.ProductName)
                    .NotEmpty().WithMessage("Tên sản phẩm không được để trống")
                    .Length(2, 255).WithMessage("Tên sản phẩm có độ dài từ 2 đến 255 ký tự");

                RuleFor(x => x.Description)
                    .MaximumLength(2000).WithMessage("Mô tả có độ dài tối đa 2000 ký tự");

                RuleFor(x => x.CategoryId)
                    .GreaterThan(0).WithMessage("Vui lòng chọn thể loại");

                RuleFor(x => x.BrandId)
                    .GreaterThan(0).WithMessage("Vui lòng chọn thương hiệu");

                RuleFor(x => x.Status)
                    .Must(x => x == 0 || x == 1)
                    .WithMessage("Trạng thái không hợp lệ");
            }

            public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
            {
                var result = await ValidateAsync(
                    ValidationContext<ProductModel>
                        .CreateWithOptions((ProductModel)model, x => x.IncludeProperties(propertyName)));

                if (result.IsValid)
                {
                    return Array.Empty<string>();
                }

                return result.Errors.Select(e => e.ErrorMessage);
            };
        }
    }
}
