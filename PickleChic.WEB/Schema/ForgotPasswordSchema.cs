using FluentValidation;
using PickleChic.WEB.Model;

namespace PickleChic.WEB.Schema
{
    public class ForgotPasswordSchema
    {
        public class ForgotPassword : AbstractValidator<ForgotPasswordModel>
        {
            public ForgotPassword()
            {
                RuleFor(x => x.Email)
                    .NotEmpty().WithMessage("Email không được để trống")
                    .EmailAddress().WithMessage("Email không đúng định dạng")
                    .MaximumLength(255).WithMessage("Email có độ dài tối đa 255 ký tự");
            }

            public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
            {
                var result = await ValidateAsync(
                    ValidationContext<ForgotPasswordModel>
                        .CreateWithOptions((ForgotPasswordModel)model, x => x.IncludeProperties(propertyName)));

                if (result.IsValid)
                {
                    return Array.Empty<string>();
                }

                return result.Errors.Select(e => e.ErrorMessage);
            };
        }
    }
}
