using FluentValidation;
using PickleChic.WEB.Model;

namespace PickleChic.WEB.Schema;

public static class GuestCheckoutAddressSchema
{
    public class CreateUpdate : AbstractValidator<OfflinePosGuestAddress>
    {
        public CreateUpdate()
        {
            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Vui lòng nhập họ tên")
                .MaximumLength(100);

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Vui lòng nhập số điện thoại")
                .Matches(@"^0\d{9}$").WithMessage("Số điện thoại không hợp lệ");

            RuleFor(x => x.DetailInfo)
                .NotEmpty().WithMessage("Vui lòng nhập địa chỉ chi tiết")
                .MaximumLength(250);

            RuleFor(x => x.ProvinceId)
                .NotNull().WithMessage("Vui lòng chọn tỉnh/thành")
                .GreaterThan(0);

            RuleFor(x => x.DistrictId)
                .NotNull().WithMessage("Vui lòng chọn quận/huyện")
                .GreaterThan(0);

            RuleFor(x => x.WardId)
                .GreaterThan(0).WithMessage("Vui lòng chọn phường/xã");
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
