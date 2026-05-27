using FluentValidation;
using PickleChic.WEB.Model;

namespace PickleChic.WEB.Schema
{
    public class CategorySchema
    {
        public class CreateUpdate : AbstractValidator<CategoryModel>
        {
            public CreateUpdate()
            {
                RuleFor(x => x.Name)
                    .NotEmpty().WithMessage("Tên thể loại không được để trống")
                    .Length(2, 255).WithMessage("Tên thể loại có độ dài từ 2 đến 255 ký tự");

                RuleFor(x => x.LinkImage)
                    .MaximumLength(500).WithMessage("Đường dẫn ảnh có độ dài tối đa 500 ký tự");

                RuleFor(x => x.Description)
                    .MaximumLength(1000).WithMessage("Mô tả có độ dài tối đa 1000 ký tự");
            }

            public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
            {
                var result = await ValidateAsync(
                    ValidationContext<CategoryModel>
                        .CreateWithOptions((CategoryModel)model, x => x.IncludeProperties(propertyName)));

                if (result.IsValid)
                {
                    return Array.Empty<string>();
                }

                return result.Errors.Select(e => e.ErrorMessage);
            };
        }
    }
}