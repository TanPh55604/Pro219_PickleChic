using FluentValidation;
using PickleChic.WEB.Constant;
using PickleChic.WEB.Model;

namespace PickleChic.WEB.Schema
{
    public class CustomerAddressSchema
    {
        public class CreateUpdate : AbstractValidator<CustomerAddressFormModel>
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
                    .NotNull().WithMessage("Vui lòng chọn phường / xã")
                    .GreaterThan(0).WithMessage("Vui lòng chọn phường / xã");
            }

            public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
            {
                var propertiesToValidate = propertyName is nameof(CustomerAddressFormModel.ProvinceId)
                    or nameof(CustomerAddressFormModel.DistrictId)
                    or nameof(CustomerAddressFormModel.WardId)
                    ? new[]
                    {
                        nameof(CustomerAddressFormModel.ProvinceId),
                        nameof(CustomerAddressFormModel.DistrictId),
                        nameof(CustomerAddressFormModel.WardId)
                    }
                    : new[] { propertyName };

                var result = await ValidateAsync(
                    ValidationContext<CustomerAddressFormModel>
                        .CreateWithOptions((CustomerAddressFormModel)model, x => x.IncludeProperties(propertiesToValidate)));

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
