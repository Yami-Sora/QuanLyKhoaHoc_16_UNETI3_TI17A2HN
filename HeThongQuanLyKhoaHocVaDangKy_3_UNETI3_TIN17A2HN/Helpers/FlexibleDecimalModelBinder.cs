// Họ và tên: Nguyễn Công Thành
// Mã sinh viên: 23103100102
// Module 5: Model Binder xử lý số thực linh hoạt (hỗ trợ cả dấu chấm . và dấu phẩy ,)

using System.Globalization;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Helpers
{
    public class FlexibleDecimalModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            if (valueProviderResult == ValueProviderResult.None)
            {
                return Task.CompletedTask;
            }

            bindingContext.ModelState.SetModelValue(bindingContext.ModelName, valueProviderResult);
            var rawValue = valueProviderResult.FirstValue;

            if (string.IsNullOrWhiteSpace(rawValue))
            {
                return Task.CompletedTask;
            }

            // Chuẩn hóa dấu phẩy thành dấu chấm để hỗ trợ cả 8.5 lẫn 8,5
            var normalized = rawValue.Trim().Replace(',', '.');

            if (decimal.TryParse(normalized, NumberStyles.Any, CultureInfo.InvariantCulture, out var parsedValue))
            {
                bindingContext.Result = ModelBindingResult.Success(parsedValue);
            }
            else
            {
                bindingContext.ModelState.TryAddModelError(bindingContext.ModelName, "Điểm số không hợp lệ.");
            }

            return Task.CompletedTask;
        }
    }

    public class FlexibleDecimalModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder? GetBinder(ModelBinderProviderContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            var modelType = context.Metadata.UnderlyingOrModelType;
            if (modelType == typeof(decimal))
            {
                return new FlexibleDecimalModelBinder();
            }

            return null;
        }
    }
}
