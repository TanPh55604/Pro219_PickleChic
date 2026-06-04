using FluentValidation;
using PickleChic.WEB.Model;

namespace PickleChic.WEB.Schema
{
    public class AdminChangePasswordSchema
    {
        public class ChangePassword : AbstractValidator<AdminChangePasswordModel>
        {
            public ChangePassword()
            {
                RuleFor(x => x.CurrentPassword)
                    .NotEmpty().WithMessage("Mật khẩu hiện tại không được để trống");

                RuleFor(x => x.NewPassword)
                    .NotEmpty().WithMessage("Mật khẩu mới không được để trống")
                    .MinimumLength(6).WithMessage("Mật khẩu phải có ít nhất 6 ký tự")
                    .Matches("[A-Z]").WithMessage("Mật khẩu phải có ít nhất 1 ký tự hoa")
                    .Matches("[a-z]").WithMessage("Mật khẩu phải có ít nhất 1 ký tự thường")
                    .Matches("[0-9]").WithMessage("Mật khẩu phải có ít nhất 1 chữ số")
                    .Matches("[^a-zA-Z0-9]").WithMessage("Mật khẩu phải có ít nhất 1 ký tự đặc biệt")
                    .NotEqual(x => x.CurrentPassword).WithMessage("Mật khẩu mới không được trùng mật khẩu hiện tại");

                RuleFor(x => x.ConfirmPassword)
                    .NotEmpty().WithMessage("Xác nhận mật khẩu không được để trống")
                    .Equal(x => x.NewPassword).WithMessage("Xác nhận mật khẩu không khớp");
            }

            public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
            {
                var result = await ValidateAsync(
                    ValidationContext<AdminChangePasswordModel>
                        .CreateWithOptions((AdminChangePasswordModel)model, x => x.IncludeProperties(propertyName)));

                if (result.IsValid)
                {
                    return Array.Empty<string>();
                }

                return result.Errors.Select(e => e.ErrorMessage);
            };
        }
    }
}