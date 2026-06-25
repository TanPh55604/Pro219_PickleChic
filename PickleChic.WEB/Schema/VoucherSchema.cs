using FluentValidation;
using PickleChic.WEB.Constant;
using PickleChic.WEB.Model;

namespace PickleChic.WEB.Schema
{
    public class VoucherSchema
    {
        public class CreateUpdate : AbstractValidator<VoucherModel>
        {
            public CreateUpdate(bool isEdit = false)
            {
                RuleFor(x => x.VoucherCode)
                    .NotEmpty().WithMessage("Mã voucher không được để trống")
                    .Length(2, 50).WithMessage("Mã voucher có độ dài từ 2 đến 50 ký tự");

                RuleFor(x => x.DiscountType)
                    .Must(x => x == VoucherDiscountType.Percent || x == VoucherDiscountType.Fixed)
                    .WithMessage("Loại giảm giá không hợp lệ");

                RuleFor(x => x.DiscountValue)
                    .GreaterThan(0).WithMessage("Giá trị giảm phải lớn hơn 0");

                RuleFor(x => x.DiscountValue)
                    .LessThanOrEqualTo(100)
                    .When(x => x.DiscountType == VoucherDiscountType.Percent)
                    .WithMessage("Giá trị giảm theo phần trăm không được vượt quá 100");

                RuleFor(x => x.MinOrderValue)
                    .GreaterThanOrEqualTo(0).WithMessage("Giá trị đơn tối thiểu không được âm");

                RuleFor(x => x.MaxDiscountAmount)
                    .NotNull()
                    .GreaterThan(0)
                    .When(x => x.DiscountType == VoucherDiscountType.Percent)
                    .WithMessage("Giảm tối đa phải lớn hơn 0 khi giảm theo phần trăm");

                RuleFor(x => x.StartDate)
                    .NotNull()
                    .WithMessage("Vui lòng chọn ngày và giờ bắt đầu");

                RuleFor(x => x.StartDate)
                    .Must((model, startDate) => IsValidStartDate(model, startDate!.Value, isEdit))
                    .When(x => x.StartDate.HasValue)
                    .WithMessage("Ngày bắt đầu phải cách hiện tại ít nhất 10 phút");

                RuleFor(x => x.EndDate)
                    .NotNull()
                    .WithMessage("Vui lòng chọn ngày và giờ kết thúc");

                RuleFor(x => x.EndDate)
                    .GreaterThanOrEqualTo(x => x.StartDate)
                    .When(x => x.StartDate.HasValue && x.EndDate.HasValue)
                    .WithMessage("Ngày kết thúc phải sau hoặc bằng ngày bắt đầu");

                RuleFor(x => x.UsageLimit)
                    .GreaterThan(0).WithMessage("Số lượng phát hành phải lớn hơn 0");

                RuleFor(x => x.CustomerUsageLimit)
                    .GreaterThan(0).WithMessage("Số lần dùng tối đa mỗi khách phải lớn hơn 0");

                RuleFor(x => x.CustomerUsageLimit)
                    .LessThanOrEqualTo(x => x.UsageLimit)
                    .WithMessage("Số lần dùng tối đa mỗi khách không được vượt quá số lượng phát hành");
            }

            public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
            {
                var propertiesToValidate = propertyName is nameof(VoucherModel.StartDate) or nameof(VoucherModel.EndDate)
                    ? new[] { nameof(VoucherModel.StartDate), nameof(VoucherModel.EndDate) }
                    : new[] { propertyName };

                var result = await ValidateAsync(
                    ValidationContext<VoucherModel>
                        .CreateWithOptions((VoucherModel)model, x => x.IncludeProperties(propertiesToValidate)));

                if (result.IsValid)
                {
                    return Array.Empty<string>();
                }

                return result.Errors
                    .Where(e => e.PropertyName == propertyName)
                    .Select(e => e.ErrorMessage);
            };

            private static bool IsValidStartDate(VoucherModel model, DateTime startDate, bool isEdit)
            {
                if (isEdit
                    && model.OriginalStartDate.HasValue
                    && model.OriginalStartDate.Value <= DateTime.Now)
                {
                    return true;
                }

                return startDate >= DateTime.Now.AddMinutes(10);
            }
        }
    }
}
