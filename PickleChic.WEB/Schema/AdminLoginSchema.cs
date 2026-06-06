using FluentValidation;
using PickleChic.WEB.Model;

namespace PickleChic.WEB.Schema
{
    public class AdminLoginSchema
    {
        public class Login : AbstractValidator<LoginModel>
        {
            public Login()
            {
                RuleFor(x => x.Email)
                    .NotEmpty().WithMessage("Email hoặc tên đăng nhập không được để trống")
                    .MaximumLength(255).WithMessage("Email hoặc tên đăng nhập có độ dài tối đa 255 ký tự");


                RuleFor(x => x.Password)
                    .NotEmpty().WithMessage("Mật khẩu không được để trống")
                    .MinimumLength(6).WithMessage("Mật khẩu phải có ít nhất 6 ký tự")
                    .MaximumLength(100).WithMessage("Mật khẩu có độ dài tối đa 100 ký tự");
            }

            public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
            {
                var result = await ValidateAsync(
                    ValidationContext<LoginModel>
                        .CreateWithOptions((LoginModel)model, x => x.IncludeProperties(propertyName)));

                if (result.IsValid)
                {
                    return Array.Empty<string>();
                }

                return result.Errors.Select(e => e.ErrorMessage);
            };
        }
    }
}