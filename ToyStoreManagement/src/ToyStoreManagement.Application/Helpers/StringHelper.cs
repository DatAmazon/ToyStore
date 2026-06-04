using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace ToyStoreManagement.Application.Helpers
{
    public static class StringHelper
    {
        public static string RemoveDiacritics(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return text;

            text = text.Normalize(NormalizationForm.FormD);
            var chars = text.Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark).ToArray();
            var result = new string(chars).Normalize(NormalizationForm.FormC);
            
            // Thay thế 'đ' và 'Đ' thủ công vì NormalizationForm.FormD không xử lý được
            result = result.Replace('đ', 'd').Replace('Đ', 'D');

            // Loại bỏ các ký tự đặc biệt, giữ lại chữ cái, số, dấu chấm, dấu gạch ngang và gạch dưới
            result = Regex.Replace(result, @"[^a-zA-Z0-9\.\-_]", " ");
            result = Regex.Replace(result, @"\s+", "-"); // Thay khoảng trắng bằng dấu gạch ngang

            return result.Trim('-').ToLower();
        }
    }
}
