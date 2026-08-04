using FluentValidation;
using PickleChic.WEB.Constant;
using PickleChic.WEB.Model;

namespace PickleChic.WEB.Schema
{
    public class OfflinePosGuestAddressSchema
    {
        public class CreateUpdate : AbstractValidator<OfflinePosGuestAddress>
        {
            public CreateUpdate()
            {
                RuleFor(x => x.FullName)
                    .NotEmpty().WithMessage("Họ tên không được để trống")
                    .Length(2, 255).WithMessage("Họ tên có độ dài từ 2 đến 255 ký tự");

                RuleFor(x => x.PhoneNumber)
                    .NotEmpty().WithMessage("Số điện thoại không được để trống")
                    .Matches(Validation.Phone.Pattern).WithMessage(Validation.Phone.ErrorMessage);

                RuleFor(x => x.DetailInfo)
                    .NotEmpty().WithMessage("Địa chỉ chi tiết không được để trống");

                RuleFor(x => x.ProvinceId)
                    .NotNull().WithMessage("Vui lòng chọn tỉnh / thành phố")
                    .GreaterThan(0).WithMessage("Vui lòng chọn tỉnh / thành phố");

                RuleFor(x => x.DistrictId)
                    .NotNull().WithMessage("Vui lòng chọn quận / huyện")
                    .GreaterThan(0).WithMessage("Vui lòng chọn quận / huyện");

                RuleFor(x => x.WardId)
                    .GreaterThan(0).WithMessage("Vui lòng chọn phường / xã");
            }

            public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
            {
                var propertiesToValidate = propertyName is nameof(OfflinePosGuestAddress.ProvinceId)
                    or nameof(OfflinePosGuestAddress.DistrictId)
                    or nameof(OfflinePosGuestAddress.WardId)
                    ? new[]
                    {
                        nameof(OfflinePosGuestAddress.ProvinceId),
                        nameof(OfflinePosGuestAddress.DistrictId),
                        nameof(OfflinePosGuestAddress.WardId)
                    }
                    : new[] { propertyName };

                var result = await ValidateAsync(
                    ValidationContext<OfflinePosGuestAddress>
                        .CreateWithOptions((OfflinePosGuestAddress)model, x => x.IncludeProperties(propertiesToValidate)));

                if (result.IsValid)
                {
                    return Array.Empty<string>();
                }

                return result.Errors
                    .Where(e => e.PropertyName == propertyName)
                    .Select(e => e.ErrorMessage);
            };
        }
    }
}
