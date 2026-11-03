using System.Text;
using ClosedXML.Excel;

namespace HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Helpers
{
    public static class ExportHelper
    {
        public const string ExcelContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

        /// <summary>
        /// Xuất file Excel (.xlsx) chuẩn Microsoft Excel đẹp mắt, chuyên nghiệp:
        /// - Dòng tiêu đề: Màu xanh UNETI (#1A56DB), chữ trắng in đậm, chiều cao 28pt
        /// - Dữ liệu dòng: Xen kẽ màu nhẹ (Zebra stripes), chiều cao 22pt, căn giữa theo chiều dọc
        /// - Kẻ viền (border) chuẩn cho toàn bộ bảng
        /// - Tự động co giãn độ rộng cột (Auto-fit width) kèm khoảng đệm thoáng mắt, không bị che chữ
        /// - Tự động căn lề thông minh (STT/Mã/Số/Ngày: giữa; Tiền tệ: phải; Tên/Mô tả: trái)
        /// - Bật sẵn tính năng AutoFilter và Freeze Panes cố định dòng tiêu đề khi cuộn
        /// </summary>
        public static byte[] ToExcelBytes(
            string sheetName,
            IReadOnlyList<string> headers,
            IEnumerable<IEnumerable<object?>> rows,
            Action<IXLWorksheet>? customConfig = null)
        {
            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add(string.IsNullOrWhiteSpace(sheetName) ? "DanhSach" : sheetName);

            // 1. Dòng tiêu đề các cột (Row 1)
            for (int c = 0; c < headers.Count; c++)
            {
                var cell = ws.Cell(1, c + 1);
                cell.Value = headers[c];
            }

            var headerRange = ws.Range(1, 1, 1, headers.Count);
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Font.FontColor = XLColor.White;
            headerRange.Style.Font.FontSize = 11;
            headerRange.Style.Font.FontName = "Segoe UI";
            headerRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#1A56DB"); // UNETI Blue
            headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            headerRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            ws.Row(1).Height = 28;

            // 2. Điền dữ liệu từ dòng 2
            int currentRow = 2;
            foreach (var row in rows)
            {
                var rowList = row as IList<object?> ?? row.ToList();
                for (int c = 0; c < headers.Count && c < rowList.Count; c++)
                {
                    var val = rowList[c];
                    var cell = ws.Cell(currentRow, c + 1);
                    var headerName = headers[c];

                    // Xử lý giá trị ô theo kiểu dữ liệu
                    if (val == null)
                    {
                        cell.Value = string.Empty;
                    }
                    else if (val is int intVal)
                    {
                        cell.Value = intVal;
                        cell.Style.NumberFormat.Format = "#,##0";
                    }
                    else if (val is long longVal)
                    {
                        cell.Value = longVal;
                        cell.Style.NumberFormat.Format = "#,##0";
                    }
                    else if (val is decimal decVal)
                    {
                        cell.Value = decVal;
                        cell.Style.NumberFormat.Format = "#,##0 \"VNĐ\"";
                    }
                    else if (val is double dblVal)
                    {
                        cell.Value = dblVal;
                        cell.Style.NumberFormat.Format = "#,##0.##";
                    }
                    else if (val is DateTime dtVal)
                    {
                        cell.Value = dtVal.ToString("dd/MM/yyyy HH:mm");
                    }
                    else if (val is bool bVal)
                    {
                        cell.Value = bVal ? "Hoạt động" : "Bị khóa";
                    }
                    else
                    {
                        string str = val.ToString() ?? string.Empty;
                        // Giữ nguyên số 0 ở đầu số điện thoại
                        cell.SetValue(str);
                        if (str.StartsWith("0") && str.Length >= 9 && str.All(char.IsDigit))
                        {
                            cell.Style.NumberFormat.Format = "@";
                        }
                    }

                    cell.Style.Font.FontName = "Segoe UI";
                    cell.Style.Font.FontSize = 10.5;
                    cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                    // Tự động căn lề theo loại dữ liệu và tiêu đề cột
                    if (headerName.Contains("STT", StringComparison.OrdinalIgnoreCase) ||
                        headerName.StartsWith("Mã", StringComparison.OrdinalIgnoreCase) ||
                        headerName.Contains("Tín chỉ", StringComparison.OrdinalIgnoreCase) ||
                        headerName.Contains("Số lớp", StringComparison.OrdinalIgnoreCase) ||
                        headerName.Contains("Số khóa", StringComparison.OrdinalIgnoreCase) ||
                        headerName.Contains("Học vị", StringComparison.OrdinalIgnoreCase) ||
                        headerName.Contains("Trạng thái", StringComparison.OrdinalIgnoreCase) ||
                        headerName.Contains("Ngày", StringComparison.OrdinalIgnoreCase) ||
                        headerName.Contains("Điện thoại", StringComparison.OrdinalIgnoreCase) ||
                        headerName.Contains("Vai trò", StringComparison.OrdinalIgnoreCase))
                    {
                        cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    }
                    else if (val is decimal || val is double ||
                             headerName.Contains("Học phí", StringComparison.OrdinalIgnoreCase) ||
                             headerName.Contains("Chi phí", StringComparison.OrdinalIgnoreCase) ||
                             headerName.Contains("Giá", StringComparison.OrdinalIgnoreCase))
                    {
                        cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    }
                    else
                    {
                        cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    }

                    // Nhấn mạnh mã định danh
                    if (headerName.StartsWith("Mã", StringComparison.OrdinalIgnoreCase))
                    {
                        cell.Style.Font.Bold = true;
                    }
                }

                // Dòng chẵn lẻ xen kẽ màu nền nhẹ cho dễ đọc (Zebra striping)
                if (currentRow % 2 == 1)
                {
                    ws.Range(currentRow, 1, currentRow, headers.Count).Style.Fill.BackgroundColor = XLColor.FromHtml("#F8FAFC");
                }

                // Không gán cố định ws.Row(currentRow).Height = 22 ở đây vì sẽ tạo cờ customHeight="1" trong OpenXML.
                // Khi customHeight="1", Excel bị khóa chiều cao cố định và KHÔNG tự động dãn chiều cao theo nội dung,
                // khiến các dòng chữ bị ngắt (WrapText) đè chồng dính vào nhau.
                // Để trống (auto height), Excel sẽ tự tính toán chiều cao chuẩn xác theo từng ô nội dung.
                currentRow++;
            }

            int totalRows = Math.Max(currentRow - 1, 1);
            var tableRange = ws.Range(1, 1, totalRows, headers.Count);

            // 3. Kẻ viền (border) toàn bộ bảng dữ liệu
            tableRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            tableRange.Style.Border.InsideBorderColor = XLColor.FromHtml("#E2E8F0");
            tableRange.Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
            tableRange.Style.Border.OutsideBorderColor = XLColor.FromHtml("#94A3B8");

            // 4. Tự động co giãn cột và thêm khoảng đệm an toàn
            ws.Columns().AdjustToContents();
            for (int c = 1; c <= headers.Count; c++)
            {
                var col = ws.Column(c);
                var headerName = headers[c - 1];

                // Cột mô tả / ghi chú / nội dung dài: giới hạn độ rộng hợp lý (30 - 65) và bật WrapText
                if (headerName.Contains("Mô tả", StringComparison.OrdinalIgnoreCase) ||
                    headerName.Contains("Ghi chú", StringComparison.OrdinalIgnoreCase) ||
                    headerName.Contains("Nội dung", StringComparison.OrdinalIgnoreCase))
                {
                    col.Width = Math.Min(Math.Max(col.Width + 4, 30), 65);
                    col.Style.Alignment.WrapText = true;
                }
                else
                {
                    col.Width = Math.Max(col.Width + 4, 12);
                }
            }

            // 5. Cấu hình in ấn trang A4 ngang tự động vừa khít 1 trang chiều ngang
            ws.PageSetup.PageOrientation = XLPageOrientation.Landscape;
            ws.PageSetup.PagesWide = 1;
            ws.PageSetup.PagesTall = 0;

            // 5. Cố định hàng tiêu đề và bật thanh lọc (AutoFilter)
            tableRange.SetAutoFilter();
            ws.SheetView.FreezeRows(1);

            // Cho phép cấu hình bổ sung tùy biến riêng
            customConfig?.Invoke(ws);

            using var ms = new MemoryStream();
            workbook.SaveAs(ms);
            return ms.ToArray();
        }

        /// <summary>
        /// Tạo mảng byte CSV chuẩn UTF-8 đính kèm BOM (dự phòng)
        /// </summary>
        public static byte[] ToCsvBytes(IEnumerable<string> headers, IEnumerable<IEnumerable<object?>> rows)
        {
            var sb = new StringBuilder();

            // 1. Dòng tiêu đề các cột (Thuần danh sách)
            sb.AppendLine(string.Join(",", headers.Select(EscapeCsv)));

            // 2. Các dòng dữ liệu
            foreach (var row in rows)
            {
                sb.AppendLine(string.Join(",", row.Select(val => EscapeCsv(val?.ToString()))));
            }

            var preamble = Encoding.UTF8.GetPreamble();
            var data = Encoding.UTF8.GetBytes(sb.ToString());
            var result = new byte[preamble.Length + data.Length];
            Buffer.BlockCopy(preamble, 0, result, 0, preamble.Length);
            Buffer.BlockCopy(data, 0, result, preamble.Length, data.Length);

            return result;
        }

        private static string EscapeCsv(string? value)
        {
            if (string.IsNullOrEmpty(value)) return "\"\"";

            if (value.StartsWith("0") && value.Length >= 9 && value.All(char.IsDigit))
            {
                return $"=\"{value}\"";
            }

            return "\"" + value.Replace("\"", "\"\"") + "\"";
        }
    }
}
