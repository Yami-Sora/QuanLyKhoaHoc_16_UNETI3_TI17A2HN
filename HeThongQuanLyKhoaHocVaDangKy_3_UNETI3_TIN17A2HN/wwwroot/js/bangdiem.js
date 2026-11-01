// @ts-nocheck
(function () {
    // Lắng nghe sự kiện gõ điểm để tự động tính toán tổng kết và xếp loại realtime
    const table = document.getElementById('tableDiem');
    if (!table) return;

    function parseDiem(val) {
        if (!val) return 0;
        const clean = String(val).trim().replace(',', '.');
        const num = parseFloat(clean);
        return isNaN(num) ? 0 : num;
    }

    function tinhDiemDong(row) {
        const idx = row.getAttribute('data-index');
        const ccInput = row.querySelector('.input-cc');
        const gkInput = row.querySelector('.input-gk');
        const ckInput = row.querySelector('.input-ck');

        let cc = parseDiem(ccInput?.value);
        let gk = parseDiem(gkInput?.value);
        let ck = parseDiem(ckInput?.value);

        // Giới hạn [0, 10]
        cc = Math.max(0, Math.min(10, cc));
        gk = Math.max(0, Math.min(10, gk));
        ck = Math.max(0, Math.min(10, ck));

        // Công thức: CC*0.1 + GK*0.4 + CK*0.5
        const tongKet = Math.round((cc * 0.1 + gk * 0.4 + ck * 0.5) * 100) / 100;

        // Xếp loại
        let xepLoai = "Yếu";
        let badgeClass = "text-danger";
        if (tongKet >= 9.0) {
            xepLoai = "Xuất sắc";
            badgeClass = "text-success";
        } else if (tongKet >= 8.0) {
            xepLoai = "Giỏi";
            badgeClass = "text-success";
        } else if (tongKet >= 6.5) {
            xepLoai = "Khá";
            badgeClass = "text-primary";
        } else if (tongKet >= 5.0) {
            xepLoai = "Trung bình";
            badgeClass = "text-primary";
        }

        const isDat = tongKet >= 5.0;

        // Cập nhật DOM trên dòng
        const labelTongKet = document.getElementById('tongket_' + idx);
        const labelXepLoai = document.getElementById('xeploai_' + idx);
        const labelKetQua = document.getElementById('ketqua_' + idx);

        if (labelTongKet) {
            labelTongKet.textContent = tongKet.toFixed(2);
            labelTongKet.className = 'fw-bold fs-6 label-tongket ' + badgeClass;
        }

        if (labelXepLoai) {
            labelXepLoai.textContent = xepLoai;
        }

        if (labelKetQua) {
            labelKetQua.textContent = isDat ? 'ĐẠT' : 'KHÔNG ĐẠT';
            labelKetQua.className = 'badge px-2.5 py-1 rounded-pill label-ketqua ' + (isDat ? 'bg-success' : 'bg-danger');
        }
    }

    function capNhatThongKeLop() {
        const rows = document.querySelectorAll('.diem-row');
        if (rows.length === 0) return;

        let tongDiem = 0;
        let countCham = 0;
        let countDat = 0;
        let countKhongDat = 0;

        rows.forEach(r => {
            const idx = r.getAttribute('data-index');
            const labelTongKet = document.getElementById('tongket_' + idx);
            if (labelTongKet) {
                const tk = parseDiem(labelTongKet.textContent);
                tongDiem += tk;
                countCham++;
                if (tk >= 5.0) countDat++;
                else countKhongDat++;
            }
        });

        const dtb = countCham > 0 ? (tongDiem / countCham).toFixed(2) : "--";
        const tyLeDat = countCham > 0 ? Math.round((countDat / countCham) * 1000) / 10 : 0;

        const elDiemTB = document.getElementById('statDiemTB');
        const elTyLeDat = document.getElementById('statTyLeDat');
        const elSoDat = document.getElementById('statSoDat');
        const elSoKhongDat = document.getElementById('statSoKhongDat');
        const elDaCham = document.getElementById('statDaCham');
        const elChuaCham = document.getElementById('statChuaCham');

        if (elDiemTB) elDiemTB.textContent = dtb;
        if (elTyLeDat) elTyLeDat.textContent = tyLeDat + '%';
        if (elSoDat) elSoDat.textContent = String(countDat);
        if (elSoKhongDat) elSoKhongDat.textContent = String(countKhongDat);
        if (elDaCham) elDaCham.textContent = String(countCham);
        if (elChuaCham) elChuaCham.textContent = String(Math.max(0, rows.length - countCham));
    }

    // Gán event listener oninput cho mọi input điểm
    table.addEventListener('input', function (e) {
        const target = e.target;
        if (target && target instanceof HTMLElement && target.classList.contains('diem-input')) {
            const row = target.closest('.diem-row');
            if (row) {
                tinhDiemDong(row);
                capNhatThongKeLop();
            }
        }
    });
})();
