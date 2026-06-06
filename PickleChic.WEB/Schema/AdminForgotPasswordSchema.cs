using FluentValidation;
using PickleChic.WEB.Model;

namespace PickleChic.WEB.Schema
{
    public class AdminForgotPasswordSchema
    {
        public class ForgotPassword : AbstractValidator<AdminForgotPasswordModel>
        {
            public ForgotPassword()
            {
                RuleFor(x => x.EmailOrUsername)
                    .NotEmpty().WithMessage("Email hoặc tên đăng nhập không được để trống")
                    .MaximumLength(255).WithMessage("Email hoặc tên đăng nhập có độ dài tối đa 255 ký tự");
            }

            public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
            {
                var result = await ValidateAsync(
                    ValidationContext<AdminForgotPasswordModel>
                        .CreateWithOptions((AdminForgotPasswordModel)model, x => x.IncludeProperties(propertyName)));

                if (result.IsValid)
                {
                    return Array.Empty<string>();
                }

                return result.Errors.Select(e => e.ErrorMessage);
            };
        }
    }
}
