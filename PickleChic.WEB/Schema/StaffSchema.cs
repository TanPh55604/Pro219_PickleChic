using FluentValidation;
using PickleChic.WEB.Model;

namespace PickleChic.WEB.Schema
{
    public class StaffSchema
    {
        public class CreateUpdate : AbstractValidator<StaffModel>
        {
            public CreateUpdate()
            {
                RuleFor(x => x.FullName)
                    .NotEmpty().WithMessage("Họ tên không được để trống")
                    .Length(2, 255).WithMessage("Họ tên có độ dài từ 2 đến 255 ký tự");

                RuleFor(x => x.Username)
                    .NotEmpty().WithMessage("Tên đăng nhập không được để trống")
                    .Length(2, 100).WithMessage("Tên đăng nhập có độ dài từ 2 đến 100 ký tự");

                RuleFor(x => x.Email)
                    .NotEmpty().WithMessage("Email không được để trống")
                    .EmailAddress().WithMessage("Email không đúng định dạng")
                    .MaximumLength(255).WithMessage("Email có độ dài tối đa 255 ký tự");

                RuleFor(x => x.PhoneNumber)
                    .Matches(@"^(0|\+84)[0-9]{9,10}$")
                    .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber))
                    .WithMessage("Số điện thoại không đúng định dạng");

                RuleFor(x => x.RoleId)
                    .GreaterThan(0).WithMessage("Vai trò không hợp lệ");

                RuleFor(x => x.Status)
                    .Must(x => x == 0 || x == 1)
                    .WithMessage("Trạng thái không hợp lệ");
            }

            public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
            {
                var result = await ValidateAsync(
                    ValidationContext<StaffModel>
                        .CreateWithOptions((StaffModel)model, x => x.IncludeProperties(propertyName)));

                if (result.IsValid)
                {
                    return Array.Empty<string>();
                }

                return result.Errors.Select(e => e.ErrorMessage);
            };
        }
    }
}
