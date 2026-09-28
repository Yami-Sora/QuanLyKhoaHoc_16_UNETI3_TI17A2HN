// Họ và tên: Trần Văn Thành
// Mã sinh viên: 23103100076
// Nội dung thực hiện: Module 1 - Lớp mã hóa mật khẩu an toàn SHA-256

using System.Security.Cryptography;
using System.Text;

namespace HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Helpers
{
    public static class PasswordHelper
    {
        private const string Salt = "UNETI_TIN17A2HN_De16_SecureSalt_2026!";

        public static string HashPassword(string rawPassword)
        {
            if (string.IsNullOrEmpty(rawPassword)) return string.Empty;
            using var sha256 = SHA256.Create();
            var combined = rawPassword + Salt;
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(combined));
            return Convert.ToHexString(bytes);
        }

        public static bool VerifyPassword(string rawPassword, string hashedPassword)
        {
            if (string.IsNullOrEmpty(rawPassword) || string.IsNullOrEmpty(hashedPassword)) return false;
            var hashOfInput = HashPassword(rawPassword);
            return string.Equals(hashOfInput, hashedPassword, StringComparison.OrdinalIgnoreCase);
        }
    }
}
