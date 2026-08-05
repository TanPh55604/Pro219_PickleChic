using FluentValidation;
using PickleChic.WEB.Model;

namespace PickleChic.WEB.Schema;

public static class GuestCheckoutContactSchema
{
    public class ContactOnly : AbstractValidator<OfflinePosGuestAddress>
    {
        public ContactOnly()
        {
            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Vui lòng nhập họ tên")
                .MaximumLength(100);

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Vui lòng nhập số điện thoại")
                .Matches(@"^0\d{9}$").WithMessage("Số điện thoại không hợp lệ");
        }

        public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
        {
            var result = await ValidateAsync(
                ValidationContext<OfflinePosGuestAddress>.CreateWithOptions(
                    (OfflinePosGuestAddress)model,
                    x => x.IncludeProperties(propertyName)));

            return result.IsValid
                ? Array.Empty<string>()
                : result.Errors.Select(e => e.ErrorMessage);
        };
    }
}
