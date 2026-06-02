using System.Text;

namespace ToyStoreManagement.Application.Helpers
{
        public static class CurrencyHelper
    {
        private static readonly string[] Ones = { "không", "một", "hai", "ba", "bốn", "năm", "sáu", "bảy", "tám", "chín" };
        private static readonly string[] Units = { "", "nghìn", "triệu", "tỷ", "nghìn tỷ", "triệu tỷ" };

        public static string ToVietnameseWords(decimal amount)
        {
            if (amount == 0) return "Không đồng";
            if (amount < 0) return "Âm " + ToVietnameseWords(Math.Abs(amount));

            long number = (long)Math.Round(amount);
            string sNumber = number.ToString();
            
            // Chia chuỗi số thành các nhóm 3 chữ số
            List<string> groups = new List<string>();
            for (int i = sNumber.Length; i > 0; i -= 3)
            {
                int start = Math.Max(0, i - 3);
                int length = i - start;
                groups.Add(sNumber.Substring(start, length));
            }

            StringBuilder result = new StringBuilder();
            for (int i = groups.Count - 1; i >= 0; i--)
            {
                string groupWords = ReadGroup(groups[i], i == groups.Count - 1);
                if (!string.IsNullOrEmpty(groupWords))
                {
                    result.Append(groupWords);
                    result.Append(" ");
                    result.Append(Units[i]);
                    result.Append(" ");
                }
            }

            string finalResult = result.ToString().Trim();
            if (string.IsNullOrEmpty(finalResult)) return "Không đồng";
            
            // Viết hoa chữ cái đầu
            finalResult = char.ToUpper(finalResult[0]) + finalResult.Substring(1) + " đồng";
            return finalResult.Replace("  ", " ");
        }

        private static string ReadGroup(string group, bool isFirstGroup)
        {
            int n = int.Parse(group);
            int h = n / 100;
            int t = (n % 100) / 10;
            int u = n % 10;

            if (n == 0 && !isFirstGroup) return "";

            StringBuilder res = new StringBuilder();

            // Đọc hàng trăm
            if (!isFirstGroup || h > 0)
            {
                res.Append(Ones[h] + " trăm ");
            }

            // Đọc hàng chục
            if (t > 0)
            {
                if (t == 1) res.Append("mười ");
                else res.Append(Ones[t] + " mươi ");
            }
            else if (!isFirstGroup && u > 0 || (h > 0 && u > 0))
            {
                res.Append("lẻ ");
            }

            // Đọc hàng đơn vị
            if (u > 0)
            {
                if (u == 1 && t > 1) res.Append("mốt");
                else if (u == 5 && t > 0) res.Append("lăm");
                else res.Append(Ones[u]);
            }

            return res.ToString().Trim();
        }
    }
}
