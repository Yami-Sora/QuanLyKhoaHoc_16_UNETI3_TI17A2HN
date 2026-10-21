// Họ và tên: Trần Văn Thành
// Mã sinh viên: 23103100076
// Module 1: Helper avatar giảng viên theo giới tính (màu avatar, chữ cái đầu, ảnh đại diện)

using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Models;

namespace HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Helpers
{
    /// <summary>
    /// Avatar giảng viên phân biệt Nam / Nữ:
    /// - Ưu tiên giới tính đã lưu trong CSDL (ô chọn Nam/Nữ ở form Thêm/Sửa giảng viên).
    /// - Giảng viên cũ chưa có dữ liệu giới tính thì suy đoán từ họ tên
    ///   (tên đệm "Thị" → Nữ, "Văn" → Nam, hoặc tra tên thường gặp).
    /// </summary>
    public static class GiangVienAvatarHelper
    {
        // Tiền tố học hàm / học vị thường dính trước họ tên: "TS. Nguyễn...", "ThS. Trần..."
        private static readonly string[] TienToHocVi = { "GS", "PGS", "TS", "THS", "CN", "KS" };

        // Tên (từ cuối) thường gặp
        private static readonly string[] TenNu =
        {
            "Lan", "Hằng", "Trang", "Linh", "Thảo", "Hương", "Hoa", "Mai", "Phương", "Yến",
            "Trâm", "Huyền", "Liên", "Bích", "Ngân", "Tuyết", "Nhung", "Quỳnh", "Duyên", "Vy",
            "Chi", "Dung", "Hạnh", "Nga", "Oanh", "Sen", "Thu", "Diễm", "Lệ", "Ngọc", "Hà"
        };

        private static readonly string[] TenNam =
        {
            "Long", "Đức", "Thắng", "Bảo", "Hùng", "Tuấn", "Minh", "Sơn", "Hải", "Cường",
            "Dũng", "Hoàng", "Nam", "Kiệt", "Huy", "Quân", "Khoa", "Phúc", "Vinh", "Tùng",
            "Đạt", "Sang", "Toàn", "Trung", "Kiên", "Hiếu", "Nghĩa", "Tài", "Lộc", "Phát"
        };

        // Tên đệm gợi ý giới tính khi tên chính chưa phân biệt được (vd: Tuấn Anh → Nam, Diệu Linh → Nữ)
        private static readonly string[] DemNam = { "Tuấn", "Hoàng", "Quốc", "Đức", "Minh", "Trọng", "Quang", "Hữu" };
        private static readonly string[] DemNu = { "Diệu", "Ngọc", "Thu", "Phương", "Thanh" };

        // Bảng màu avatar: Nam tông xanh dương, Nữ tông hồng — cùng 1 người luôn cùng 1 màu
        private static readonly string[] MauNam =
        {
            "linear-gradient(135deg, #3b82f6, #1d4ed8)",
            "linear-gradient(135deg, #0ea5e9, #0369a1)",
            "linear-gradient(135deg, #14b8a6, #0f766e)",
            "linear-gradient(135deg, #6366f1, #4338ca)"
        };

        private static readonly string[] MauNu =
        {
            "linear-gradient(135deg, #f43f5e, #be123c)",
            "linear-gradient(135deg, #ec4899, #be185d)",
            "linear-gradient(135deg, #e879f9, #a21caf)",
            "linear-gradient(135deg, #fb7185, #e11d48)"
        };

        private static readonly string[] MauChuaRo =
        {
            "linear-gradient(135deg, #64748b, #334155)",
            "linear-gradient(135deg, #7c3aed, #6d28d9)"
        };

        /// <summary>Tách họ tên thành các từ, bỏ tiền tố học vị (TS., ThS., PGS., GS...).</summary>
        private static string[] TachTen(string? hoTen)
        {
            var words = (hoTen ?? string.Empty)
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .ToList();

            while (words.Count > 1)
            {
                var first = words[0].TrimEnd('.').ToUpperInvariant();
                // Xử lý cả tiền tố gộp kiểu "PGS.TS."
                var parts = first.Split('.', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length > 0 && parts.All(p => TienToHocVi.Contains(p)))
                {
                    words.RemoveAt(0);
                }
                else
                {
                    break;
                }
            }

            return words.ToArray();
        }

        /// <summary>Suy đoán giới tính từ họ tên. Trả về "Nam", "Nữ" hoặc null nếu chưa rõ.</summary>
        public static string? SuyDoanTuTen(string? hoTen)
        {
            var words = TachTen(hoTen);
            if (words.Length == 0) return null;

            // Quy tắc mạnh nhất: tên đệm Thị / Văn
            if (words.Any(w => w == "Thị")) return "Nữ";
            if (words.Any(w => w == "Văn")) return "Nam";

            // Tên chính (từ cuối cùng)
            var ten = words[^1];
            if (TenNu.Contains(ten)) return "Nữ";
            if (TenNam.Contains(ten)) return "Nam";

            // Tên đệm gợi ý
            if (words.Any(w => DemNu.Contains(w))) return "Nữ";
            if (words.Any(w => DemNam.Contains(w))) return "Nam";

            return null;
        }

        /// <summary>Giới tính hiển thị của giảng viên: ưu tiên CSDL, thiếu thì suy từ tên. Chưa rõ trả về "".</summary>
        public static string LayGioiTinh(GiangVien? giangVien)
        {
            if (giangVien == null) return string.Empty;
            if (giangVien.GioiTinh == "Nam" || giangVien.GioiTinh == "Nữ")
            {
                return giangVien.GioiTinh;
            }
            return SuyDoanTuTen(giangVien.HoTen) ?? string.Empty;
        }

        /// <summary>Hai chữ cái đầu làm avatar: lấy từ tên đệm + tên chính (vd: "Mai Lan" → "ML").</summary>
        public static string ChuCaiDau(string? hoTen)
        {
            var words = TachTen(hoTen);
            if (words.Length >= 2) return $"{words[^2][0]}{words[^1][0]}".ToUpper();
            return words.Length == 1 ? words[0][0].ToString().ToUpper() : "?";
        }

        /// <summary>Màu nền avatar theo giới tính (Nam: tông xanh, Nữ: tông hồng).</summary>
        public static string MauAvatar(string? gioiTinh, string? hoTen)
        {
            var palette = gioiTinh == "Nữ" ? MauNu
                        : gioiTinh == "Nam" ? MauNam
                        : MauChuaRo;
            var hash = 0;
            foreach (var c in hoTen ?? string.Empty) hash += c;
            return palette[Math.Abs(hash) % palette.Length];
        }

        /// <summary>Ảnh đại diện chibi theo giới tính (trang chi tiết môn học).</summary>
        public static string AnhDaiDien(string? gioiTinh)
        {
            return gioiTinh == "Nữ"
                ? "~/images/avatars/lecturer-avatar-nu.webp"
                : "~/images/avatars/lecturer-avatar.webp";
        }
    }
}
