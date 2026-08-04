using FluentValidation;
using PickleChic.WEB.Constant;
using PickleChic.WEB.Model;

namespace PickleChic.WEB.Schema
{
    public class CustomerSchema
    {
        public class CreateUpdate : AbstractValidator<CustomerModel>
        {
            public CreateUpdate()
            {
                RuleFor(x => x.Username)
                    .NotEmpty().WithMessage("Tên đăng nhập không được để trống")
                    .Length(2, 100).WithMessage("Tên đăng nhập có độ dài từ 2 đến 100 ký tự");

                RuleFor(x => x.FullName)
                    .NotEmpty().WithMessage("Họ tên không được để trống")
                    .Length(2, 255).WithMessage("Họ tên có độ dài từ 2 đến 255 ký tự");

                RuleFor(x => x.Email)
                    .NotEmpty().WithMessage("Email không được để trống")
                    .EmailAddress().WithMessage("Email không đúng định dạng")
                    .MaximumLength(255).WithMessage("Email có độ dài tối đa 255 ký tự");

                RuleFor(x => x.PhoneNumber)
                    .Must(Validation.Phone.IsValidOrEmpty)
                    .WithMessage(Validation.Phone.ErrorMessage);

                RuleFor(x => x.Gender)
                    .NotNull().WithMessage("Giới tính không được để trống");

                RuleFor(x => x.DateOfBirth)
                    .LessThan(DateTime.Today).WithMessage("Ngày sinh không hợp lệ");

                RuleFor(x => x.TotalPoints)
                    .GreaterThanOrEqualTo(0).WithMessage("Điểm tích lũy không được nhỏ hơn 0");

                RuleFor(x => x.RankId)
                    .GreaterThan(0).WithMessage("Hạng khách hàng không hợp lệ");
            }

            public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
            {
                var result = await ValidateAsync(
                    ValidationContext<CustomerModel>
                        .CreateWithOptions((CustomerModel)model, x => x.IncludeProperties(propertyName)));

                if (result.IsValid)
                {
                    return Array.Empty<string>();
                }

                return result.Errors.Select(e => e.ErrorMessage);
            };
        }
    }
}
