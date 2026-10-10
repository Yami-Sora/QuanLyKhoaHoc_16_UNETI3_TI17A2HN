// Họ và tên: Nguyễn Công Thành
// Mã sinh viên: 23103100102
// Nội dung thực hiện: Module 5 - ViewModel Dashboard tổng quan hệ thống

namespace HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.ViewModels
{
    /// <summary>
    /// ViewModel tổng hợp Dashboard quản trị.
    /// Bao gồm 9 stat cards theo yêu cầu đề bài (Mục 9.3) và dữ liệu biểu đồ Chart.js.
    /// </summary>
    public class DashboardViewModel
    {
        // =====================================================================
        // 9 STAT CARDS THEO YÊU CẦU ĐỀ BÀI (Mục 9.3)
        // =====================================================================

        /// <summary>1. Tổng số môn học</summary>
        public int TongSoMonHoc { get; set; }

        /// <summary>2. Tổng số khóa học</summary>
        public int TongSoKhoaHoc { get; set; }

        /// <summary>3. Số khóa học đang khả dụng (DangMo)</summary>
        public int SoKhoaHocKhaDung { get; set; }

        /// <summary>4. Tổng số học viên</summary>
        public int TongSoHocVien { get; set; }

        /// <summary>5. Tổng số giao dịch (đăng ký)</summary>
        public int TongSoGiaoDich { get; set; }

        /// <summary>6. Số giao dịch đang chờ xử lý</summary>
        public int SoGiaoDichChoXuLy { get; set; }

        /// <summary>7. Số giao dịch đang xử lý</summary>
        public int SoGiaoDichDangXuLy { get; set; }

        /// <summary>8. Số giao dịch hoàn thành</summary>
        public int SoGiaoDichHoanThanh { get; set; }

        /// <summary>9. Tổng doanh thu học phí (SoTienDaDong)</summary>
        public decimal TongDoanhThu { get; set; }

        // =====================================================================
        // SỐ LIỆU BỔ SUNG
        // =====================================================================

        /// <summary>Tổng công nợ học phí còn lại</summary>
        public decimal TongCongNo { get; set; }

        /// <summary>Số giao dịch đã hủy</summary>
        public int SoGiaoDichBiHuy { get; set; }

        /// <summary>Số khóa học đang diễn ra (DangHoc)</summary>
        public int SoKhoaHocDangHoc { get; set; }

        /// <summary>Số khóa học đã kết thúc</summary>
        public int SoKhoaHocDaKetThuc { get; set; }

        /// <summary>Tổng số giảng viên</summary>
        public int TongSoGiangVien { get; set; }

        // =====================================================================
        // DỮ LIỆU BIỂU ĐỒ CHART.JS
        // =====================================================================

        /// <summary>Biểu đồ tròn: Tỷ lệ đăng ký theo trạng thái</summary>
        public List<ChartDataItem> BieuDoDangKyTheoTrangThai { get; set; } = new();

        /// <summary>Biểu đồ cột: Số học viên đăng ký theo khóa (top 10)</summary>
        public List<ChartDataItem> BieuDoHocVienTheoKhoa { get; set; } = new();

        /// <summary>Biểu đồ đường: Số đăng ký theo tháng (12 tháng gần nhất)</summary>
        public List<ChartDataItem> BieuDoDangKyTheoThang { get; set; } = new();

        /// <summary>Biểu đồ tròn: Tỷ lệ Đạt / Không đạt</summary>
        public List<ChartDataItem> BieuDoTyLeDatKhongDat { get; set; } = new();
    }

    /// <summary>
    /// Item dữ liệu dùng chung cho mọi biểu đồ Chart.js.
    /// Label = nhãn trục X hoặc tên phần, Value = giá trị số.
    /// </summary>
    public class ChartDataItem
    {
        public string Label { get; set; } = string.Empty;
        public decimal Value { get; set; }
        public string? Color { get; set; }
    }
}
