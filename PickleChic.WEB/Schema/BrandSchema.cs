using FluentValidation;
using PickleChic.WEB.Model;

namespace PickleChic.WEB.Schema
{
    public class BrandSchema
    {
        public class CreateUpdate : AbstractValidator<BrandModel>
        {
            public CreateUpdate()
            {
                RuleFor(x => x.Name)
                    .NotEmpty().WithMessage("Tên thương hiệu không được để trống")
                    .Length(2, 255).WithMessage("Tên thương hiệu có độ dài từ 2 đến 255 ký tự");

                RuleFor(x => x.Description)
                    .MaximumLength(1000).WithMessage("Mô tả có độ dài tối đa 1000 ký tự");

                RuleFor(x => x.Status)
                    .Must(x => x == 0 || x == 1)
                    .WithMessage("Trạng thái không hợp lệ");
            }

            public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
            {
                var result = await ValidateAsync(
                    ValidationContext<BrandModel>
                        .CreateWithOptions((BrandModel)model, x => x.IncludeProperties(propertyName)));

                if (result.IsValid)
                {
                    return Array.Empty<string>();
                }

                return result.Errors.Select(e => e.ErrorMessage);
            };
        }
    }
}