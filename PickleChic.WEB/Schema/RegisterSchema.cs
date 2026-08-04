using FluentValidation;
using PickleChic.WEB.Model;

namespace PickleChic.WEB.Schema
{
    public class RegisterSchema
    {
        public class Register : AbstractValidator<RegisterModel>
        {
            public Register()
            {
                RuleFor(x => x.FullName)
                    .NotEmpty().WithMessage("Họ tên không được để trống")
                    .Length(2, 255).WithMessage("Họ tên có độ dài từ 2 đến 255 ký tự");

                RuleFor(x => x.Email)
                    .NotEmpty().WithMessage("Email không được để trống")
                    .EmailAddress().WithMessage("Email không đúng định dạng")
                    .MaximumLength(255).WithMessage("Email có độ dài tối đa 255 ký tự");

                RuleFor(x => x.Password)
                    .NotEmpty().WithMessage("Mật khẩu không được để trống")
                    .MinimumLength(6).WithMessage("Mật khẩu phải có ít nhất 6 ký tự")
                    .MaximumLength(100).WithMessage("Mật khẩu có độ dài tối đa 100 ký tự");

                RuleFor(x => x.ConfirmPassword)
                    .NotEmpty().WithMessage("Xác nhận mật khẩu không được để trống")
                    .Equal(x => x.Password).WithMessage("Xác nhận mật khẩu không khớp");

                RuleFor(x => x.PhoneNumber)
                    .NotEmpty().WithMessage("Số điện thoại không được để trống")
                    .Matches(@"^(0|\+84)[0-9]{9,10}$").WithMessage("Số điện thoại không đúng định dạng");

                RuleFor(x => x.DateOfBirth)
                    .NotEmpty().WithMessage("Ngày sinh không được để trống")
                    .LessThan(DateTime.Today).WithMessage("Ngày sinh không hợp lệ");

                RuleFor(x => x.Gender)
                    .NotEmpty().WithMessage("Giới tính không được để trống");
            }

            public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
            {
                var result = await ValidateAsync(
                    ValidationContext<RegisterModel>
                        .CreateWithOptions((RegisterModel)model, x => x.IncludeProperties(propertyName)));

                if (result.IsValid)
                {
                    return Array.Empty<string>();
                }

                return result.Errors.Select(e => e.ErrorMessage);
            };
        }
    }
}
