// @ts-nocheck
// Khai báo biến chart instance toàn cục để hàm lọc AJAX gọi được
let chartInstanceDangKyThang = null;

// Hàm chuyển đổi bộ lọc chu kỳ Section 8 (Phương án B)
function doiChuKyDangKy(filterType, year, clickedBtn) {
    // Cập nhật trạng thái nút bấm
    document.querySelectorAll('.btn-filter-sec8').forEach(el => {
        el.classList.remove('active', 'btn-primary');
        if (el.classList.contains('btn-primary')) {
            el.classList.remove('btn-primary');
            el.classList.add('btn-outline-secondary');
        }
    });

    const dropdownToggle = document.getElementById('dropdownNamDangKy');
    if (clickedBtn) {
        clickedBtn.classList.add('active');
        if (clickedBtn.tagName.toLowerCase() === 'button') {
            clickedBtn.classList.remove('btn-outline-secondary');
            clickedBtn.classList.add('btn-primary');
        }

        if (dropdownToggle) {
            if (filterType === 'year' && year) {
                dropdownToggle.innerHTML = '<i class="bi bi-calendar3 me-1"></i>Năm ' + year;
                dropdownToggle.classList.remove('btn-outline-secondary');
                dropdownToggle.classList.add('btn-primary', 'active');
            } else {
                dropdownToggle.innerHTML = '<i class="bi bi-calendar3 me-1"></i>Theo năm';
                dropdownToggle.classList.remove('btn-primary', 'active');
                dropdownToggle.classList.add('btn-outline-secondary');
            }
        }
    }

    let url = '/ThongKe/GetDangKyTheoThang?filterType=' + encodeURIComponent(filterType);
    if (year) url += '&year=' + encodeURIComponent(year);

    fetch(url)
        .then(res => res.json())
        .then(data => {
            if (!data || !chartInstanceDangKyThang) return;

            const labels = data.map(x => x.label);
            const values = data.map(x => x.value);

            // 1. Cập nhật biểu đồ Chart.js mượt mà
            chartInstanceDangKyThang.data.labels = labels;
            chartInstanceDangKyThang.data.datasets[0].data = values;
            chartInstanceDangKyThang.update();

            // 2. Cập nhật bảng ma trận số liệu bên dưới
            const headerRow = document.getElementById('tableDangKyThangHeader');
            const bodyRow = document.getElementById('tableDangKyThangBody');
            if (headerRow && bodyRow) {
                headerRow.innerHTML = labels.map(l => `<th class="py-2 text-muted fw-bold">${l}</th>`).join('');
                bodyRow.innerHTML = values.map(v => `<td class="py-2 fw-bold text-primary fs-6">${v}</td>`).join('');
            }
        })
        .catch(err => console.error('Lỗi tải dữ liệu đăng ký theo tháng:', err));
}

(function () {
    // Khởi tạo Bootstrap Tooltips
    if (typeof bootstrap !== 'undefined' && bootstrap.Tooltip) {
        var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
        tooltipTriggerList.forEach(function (tooltipTriggerEl) {
            new bootstrap.Tooltip(tooltipTriggerEl);
        });
    }

    if (typeof Chart === 'undefined') return;

    Chart.defaults.font.family = "'Inter', system-ui, -apple-system, sans-serif";
    Chart.defaults.color = "#64748b";
    // Siêu nét cho hiển thị màn hình và khi in ấn / xuất PDF (Retina 3x sắc nét, khử triệt để mờ vỡ hạt)
    Chart.defaults.devicePixelRatio = Math.max(window.devicePixelRatio || 1, 3);

    const tooltipConfig = {
        backgroundColor: 'rgba(15, 23, 42, 0.92)',
        titleColor: '#ffffff',
        titleFont: { size: 13, weight: 'bold' },
        bodyColor: '#e2e8f0',
        bodyFont: { size: 12 },
        padding: 12,
        cornerRadius: 10,
        boxPadding: 6,
        usePointStyle: true,
        borderColor: 'rgba(255, 255, 255, 0.1)',
        borderWidth: 1
    };

    // Biểu đồ 1: Khóa theo môn (Doughnut bo góc mượt mà)
    const ctxKhoaTheoMon = document.getElementById('chartKhoaTheoMon');
    if (ctxKhoaTheoMon) {
        const labelsKhoaMon = JSON.parse(ctxKhoaTheoMon.getAttribute('data-labels') || '[]');
        const valuesKhoaMon = JSON.parse(ctxKhoaTheoMon.getAttribute('data-values') || '[]');

        new Chart(ctxKhoaTheoMon, {
            type: 'doughnut',
            data: {
                labels: labelsKhoaMon,
                datasets: [{
                    data: valuesKhoaMon,
                    backgroundColor: ['#6366f1', '#3b82f6', '#0ea5e9', '#10b981', '#f59e0b', '#ec4899'],
                    borderWidth: 0,
                    spacing: 3,
                    borderRadius: 6
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                cutout: '70%',
                plugins: {
                    legend: {
                        position: 'bottom',
                        labels: {
                            usePointStyle: true,
                            pointStyle: 'circle',
                            padding: 12,
                            font: { size: 11, weight: '600' }
                        }
                    },
                    tooltip: tooltipConfig
                }
            }
        });
    }

    // Biểu đồ 7: Đạt / Không đạt (Doughnut mềm mại)
    const ctxDatKhongDat = document.getElementById('chartDatKhongDat');
    if (ctxDatKhongDat) {
        const soDat = Number(ctxDatKhongDat.getAttribute('data-dat')) || 0;
        const soKhongDat = Number(ctxDatKhongDat.getAttribute('data-khongdat')) || 0;

        new Chart(ctxDatKhongDat, {
            type: 'doughnut',
            data: {
                labels: ['Đạt yêu cầu', 'Không đạt'],
                datasets: [{
                    data: [soDat, soKhongDat],
                    backgroundColor: ['#10b981', '#f43f5e'],
                    hoverBackgroundColor: ['#059669', '#e11d48'],
                    borderWidth: 0,
                    spacing: 4,
                    borderRadius: 6
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                cutout: '72%',
                plugins: {
                    legend: {
                        position: 'bottom',
                        labels: {
                            usePointStyle: true,
                            pointStyle: 'circle',
                            padding: 14,
                            font: { size: 12, weight: '600' }
                        }
                    },
                    tooltip: tooltipConfig
                }
            }
        });
    }

    // Biểu đồ 8: Đăng ký theo tháng (Bar Gradient tinh tế)
    const ctxDangKyThang = document.getElementById('chartDangKyThang');
    if (ctxDangKyThang) {
        const labelsDangKy = JSON.parse(ctxDangKyThang.getAttribute('data-labels') || '[]');
        const valuesDangKy = JSON.parse(ctxDangKyThang.getAttribute('data-values') || '[]');

        const ctx8 = ctxDangKyThang.getContext('2d');
        const barGrad = ctx8.createLinearGradient(0, 0, 0, 210);
        barGrad.addColorStop(0, '#6366f1');
        barGrad.addColorStop(1, '#4338ca');

        chartInstanceDangKyThang = new Chart(ctxDangKyThang, {
            type: 'bar',
            data: {
                labels: labelsDangKy,
                datasets: [{
                    label: 'Lượt đăng ký',
                    data: valuesDangKy,
                    backgroundColor: barGrad,
                    borderRadius: { topLeft: 6, topRight: 6, bottomLeft: 0, bottomRight: 0 },
                    borderSkipped: false,
                    barPercentage: 0.55
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                scales: {
                    y: {
                        beginAtZero: true,
                        ticks: { precision: 0, font: { size: 11 } },
                        grid: { color: '#f1f5f9', borderDash: [4, 4], drawBorder: false }
                    },
                    x: {
                        ticks: { font: { size: 11, weight: '500' } },
                        grid: { display: false }
                    }
                },
                plugins: {
                    legend: { display: false },
                    tooltip: tooltipConfig
                }
            }
        });
    }

    // Tự động render lại các biểu đồ chuẩn kích thước siêu nét khi mở hộp thoại in
    window.addEventListener('beforeprint', function () {
        for (var id in Chart.instances) {
            Chart.instances[id].resize();
        }
    });
    window.addEventListener('afterprint', function () {
        for (var id in Chart.instances) {
            Chart.instances[id].resize();
        }
    });
})();

document.addEventListener('DOMContentLoaded', function () {
    document.querySelectorAll('.btn-filter-sec8').forEach(function (btn) {
        btn.addEventListener('click', function (e) {
            const filterType = this.getAttribute('data-filter') || '12months';
            const yearAttr = this.getAttribute('data-year');
            const year = yearAttr ? parseInt(yearAttr, 10) : null;
            doiChuKyDangKy(filterType, year, this);
        });
    });
});
