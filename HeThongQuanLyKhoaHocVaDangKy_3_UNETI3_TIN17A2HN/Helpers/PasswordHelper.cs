// Họ và tên: Trần Văn Thành
// Mã sinh viên: 23103100076
// Module 1: Mã hóa mật khẩu SHA-256

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

        // Tạo token đặt lại mật khẩu (hạn 15 phút)
        public static string GenerateResetToken(string email, string currentPasswordHash, int expireMinutes = 15)
        {
            var expiry = DateTime.Now.AddMinutes(expireMinutes);
            var normalizedEmail = email.ToLower().Trim();
            var raw = $"{normalizedEmail}|{expiry.Ticks}|{currentPasswordHash}|{Salt}";
            using var sha = SHA256.Create();
            var hash = Convert.ToHexString(sha.ComputeHash(Encoding.UTF8.GetBytes(raw)));
            var payload = $"{normalizedEmail}|{expiry.Ticks}|{hash}";
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(payload));
        }

        // Xác thực mã Token đặt lại mật khẩu
        public static bool VerifyResetToken(string email, string currentPasswordHash, string token, out string errorMessage)
        {
            errorMessage = string.Empty;
            try
            {
                if (string.IsNullOrWhiteSpace(token))
                {
                    errorMessage = "Mã xác thực không được để trống.";
                    return false;
                }

                var decoded = Encoding.UTF8.GetString(Convert.FromBase64String(token));
                var parts = decoded.Split('|');
                if (parts.Length != 3)
                {
                    errorMessage = "Mã xác thực không đúng định dạng.";
                    return false;
                }

                var tokenEmail = parts[0];
                if (!long.TryParse(parts[1], out var ticks))
                {
                    errorMessage = "Thời hạn mã xác thực không hợp lệ.";
                    return false;
                }

                var expiry = new DateTime(ticks);
                if (DateTime.Now > expiry)
                {
                    errorMessage = "Liên kết đặt lại mật khẩu đã hết hạn (chỉ có hiệu lực trong 15 phút). Vui lòng gửi lại yêu cầu mới.";
                    return false;
                }

                var normalizedEmail = email.ToLower().Trim();
                if (!string.Equals(tokenEmail, normalizedEmail, StringComparison.OrdinalIgnoreCase))
                {
                    errorMessage = "Email không trùng khớp với mã xác thực.";
                    return false;
                }

                var expectedHash = parts[2];
                var raw = $"{normalizedEmail}|{ticks}|{currentPasswordHash}|{Salt}";
                using var sha = SHA256.Create();
                var actualHash = Convert.ToHexString(sha.ComputeHash(Encoding.UTF8.GetBytes(raw)));

                if (!string.Equals(expectedHash, actualHash, StringComparison.OrdinalIgnoreCase))
                {
                    errorMessage = "Liên kết đặt lại mật khẩu này đã được sử dụng hoặc không còn hiệu lực. Vui lòng gửi yêu cầu mới.";
                    return false;
                }

                return true;
            }
            catch
            {
                errorMessage = "Mã xác thực không hợp lệ.";
                return false;
            }
        }
    }
}
