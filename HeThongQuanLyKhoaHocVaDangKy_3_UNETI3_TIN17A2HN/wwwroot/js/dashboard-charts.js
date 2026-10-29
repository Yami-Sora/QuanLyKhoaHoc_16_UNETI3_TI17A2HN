// @ts-nocheck
(function () {
    if (typeof Chart === 'undefined') return;

    // Cấu hình chung giao diện Chart.js hiện đại
    Chart.defaults.font.family = "'Inter', system-ui, -apple-system, sans-serif";
    Chart.defaults.color = "#64748b";

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

    // 1. Biểu đồ tròn: Trạng thái đăng ký (Doughnut cao cấp bo góc)
    const ctxTrangThai = document.getElementById('chartTrangThai');
    if (ctxTrangThai) {
        const labelsTrangThai = JSON.parse(ctxTrangThai.getAttribute('data-labels') || '[]');
        const valuesTrangThai = JSON.parse(ctxTrangThai.getAttribute('data-values') || '[]');

        new Chart(ctxTrangThai, {
            type: 'doughnut',
            data: {
                labels: labelsTrangThai,
                datasets: [{
                    data: valuesTrangThai,
                    backgroundColor: ['#f59e0b', '#3b82f6', '#10b981', '#f43f5e'],
                    hoverBackgroundColor: ['#d97706', '#2563eb', '#059669', '#e11d48'],
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
                            padding: 18,
                            font: { size: 12, weight: '600' }
                        }
                    },
                    tooltip: tooltipConfig
                }
            }
        });
    }

    // 2. Biểu đồ cột: Top khóa học (Multi-color Gradients bo tròn)
    const ctxTopKhoa = document.getElementById('chartTopKhoaHoc');
    if (ctxTopKhoa) {
        const labelsTopKhoa = JSON.parse(ctxTopKhoa.getAttribute('data-labels') || '[]');
        const valuesTopKhoa = JSON.parse(ctxTopKhoa.getAttribute('data-values') || '[]');

        const ctx2 = ctxTopKhoa.getContext('2d');
        
        // Mảng màu gradient chuyển tiếp cho từng cột
        const colorPalette = [
            ['#6366f1', '#4338ca'], // Indigo
            ['#3b82f6', '#1d4ed8'], // Blue
            ['#0ea5e9', '#0284c7'], // Sky
            ['#14b8a6', '#0f766e'], // Teal
            ['#10b981', '#047857'], // Emerald
            ['#8b5cf6', '#6d28d9']  // Purple
        ];

        const barGradients = colorPalette.map(c => {
            const g = ctx2.createLinearGradient(0, 0, 0, 240);
            g.addColorStop(0, c[0]);
            g.addColorStop(1, c[1]);
            return g;
        });

        new Chart(ctxTopKhoa, {
            type: 'bar',
            data: {
                labels: labelsTopKhoa,
                datasets: [{
                    label: 'Số học viên',
                    data: valuesTopKhoa,
                    backgroundColor: barGradients,
                    borderRadius: { topLeft: 8, topRight: 8, bottomLeft: 0, bottomRight: 0 },
                    borderSkipped: false,
                    barPercentage: 0.62,
                    categoryPercentage: 0.75
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                scales: {
                    y: {
                        beginAtZero: true,
                        ticks: { precision: 0, font: { size: 11 } },
                        grid: {
                            color: '#f1f5f9',
                            borderDash: [5, 5],
                            drawBorder: false
                        }
                    },
                    x: {
                        ticks: {
                            font: { size: 11, weight: '500' },
                            maxRotation: 20,
                            minRotation: 0,
                            callback: function(value, index) {
                                const label = this.getLabelForValue(value);
                                if (!label) return '';
                                return label.length > 15 ? label.substring(0, 14) + '…' : label;
                            }
                        },
                        grid: { display: false }
                    }
                },
                plugins: {
                    legend: { display: false },
                    tooltip: {
                        ...tooltipConfig,
                        callbacks: {
                            title: function(items) {
                                if (!items.length) return '';
                                return items[0].label; // Hiển thị 100% đầy đủ tên khóa học khi rê chuột
                            },
                            label: function(item) {
                                return ' Sĩ số ghi danh: ' + item.raw + ' học viên';
                            }
                        }
                    }
                }
            }
        });
    }

    // 3. Biểu đồ đường: Đăng ký theo tháng (Smooth Spline Gradient Glow)
    const ctxTheoThang = document.getElementById('chartTheoThang');
    if (ctxTheoThang) {
        const labelsTheoThang = JSON.parse(ctxTheoThang.getAttribute('data-labels') || '[]');
        const valuesTheoThang = JSON.parse(ctxTheoThang.getAttribute('data-values') || '[]');

        const ctx3 = ctxTheoThang.getContext('2d');
        const lineGrad = ctx3.createLinearGradient(0, 0, 0, 240);
        lineGrad.addColorStop(0, 'rgba(99, 102, 241, 0.32)');
        lineGrad.addColorStop(1, 'rgba(99, 102, 241, 0.01)');

        new Chart(ctxTheoThang, {
            type: 'line',
            data: {
                labels: labelsTheoThang,
                datasets: [{
                    label: 'Số lượt ghi danh',
                    data: valuesTheoThang,
                    borderColor: '#6366f1',
                    borderWidth: 3.5,
                    backgroundColor: lineGrad,
                    fill: true,
                    tension: 0.42,
                    pointRadius: 5,
                    pointHoverRadius: 8,
                    pointBackgroundColor: '#ffffff',
                    pointBorderColor: '#6366f1',
                    pointBorderWidth: 3,
                    pointHoverBackgroundColor: '#6366f1',
                    pointHoverBorderColor: '#ffffff',
                    pointHoverBorderWidth: 3
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                scales: {
                    y: {
                        beginAtZero: true,
                        ticks: { precision: 0, font: { size: 11 } },
                        grid: {
                            color: '#f1f5f9',
                            borderDash: [5, 5],
                            drawBorder: false
                        }
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

    // 4. Biểu đồ Doughnut: Tỷ lệ Đạt / Không đạt (Modern Doughnut)
    const ctxTyLeDat = document.getElementById('chartTyLeDat');
    if (ctxTyLeDat) {
        const labelsTyLeDat = JSON.parse(ctxTyLeDat.getAttribute('data-labels') || '[]');
        const valuesTyLeDat = JSON.parse(ctxTyLeDat.getAttribute('data-values') || '[]');

        new Chart(ctxTyLeDat, {
            type: 'doughnut',
            data: {
                labels: labelsTyLeDat,
                datasets: [{
                    data: valuesTyLeDat,
                    backgroundColor: ['#10b981', '#f43f5e', '#cbd5e1'],
                    hoverBackgroundColor: ['#059669', '#e11d48', '#94a3b8'],
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
                            padding: 18,
                            font: { size: 12, weight: '600' }
                        }
                    },
                    tooltip: tooltipConfig
                }
            }
        });
    }
})();
