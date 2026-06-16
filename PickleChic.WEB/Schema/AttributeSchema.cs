using FluentValidation;
using PickleChic.WEB.Model;

namespace PickleChic.WEB.Schema
{
    public class AttributeSchema
    {
        public class CreateUpdate : AbstractValidator<AttributeModel>
        {
            public CreateUpdate()
            {
                RuleFor(x => x.AttributeName)
                    .NotEmpty().WithMessage("Tên thuộc tính không được để trống")
                    .Length(2, 255).WithMessage("Tên thuộc tính có độ dài từ 2 đến 255 ký tự");
            }

            public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
            {
                var result = await ValidateAsync(
                    ValidationContext<AttributeModel>
                        .CreateWithOptions((AttributeModel)model, x => x.IncludeProperties(propertyName)));

                if (result.IsValid)
                {
                    return Array.Empty<string>();
                }

                return result.Errors.Select(e => e.ErrorMessage);
            };
        }

        public class ValueItem : AbstractValidator<AttributeValueModel>
        {
            public ValueItem()
            {
                RuleFor(x => x.Value)
                    .NotEmpty().WithMessage("Giá trị không được để trống")
                    .MaximumLength(255).WithMessage("Giá trị có độ dài tối đa 255 ký tự");

                RuleFor(x => x.Note)
                    .MaximumLength(500).WithMessage("Ghi chú có độ dài tối đa 500 ký tự");
            }
        }
    }
}
