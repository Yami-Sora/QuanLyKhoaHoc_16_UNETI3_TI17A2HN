// Họ và tên: Trần Văn Thành - MSV: 23103100076 - Lớp: TIN17A2HN
// Module 1: UI components (Toast, Confirm Modal)

const UnetiUI = (function () {
    // 1. Toast Notification System
    function ensureToastContainer() {
        let container = document.getElementById('unetiToastContainer');
        if (!container) {
            container = document.createElement('div');
            container.id = 'unetiToastContainer';
            container.className = 'uneti-toast-container position-fixed top-0 end-0 p-3';
            container.setAttribute('aria-live', 'polite');
            container.setAttribute('role', 'region');
            container.setAttribute('aria-label', 'Thông báo hệ thống');
            document.body.appendChild(container);
        }
        return container;
    }

    function toast(options) {
        const {
            type = 'info', // 'success' | 'info' | 'warning' | 'danger'
            title = '',
            message = '',
            duration = 4000
        } = typeof options === 'string' ? { message: options } : (options || {});

        const container = ensureToastContainer();
        const toastEl = document.createElement('div');
        toastEl.className = `uneti-toast uneti-toast-${type} shadow-lg`;
        toastEl.setAttribute('role', 'status');
        toastEl.setAttribute('aria-atomic', 'true');

        let iconClass = 'bi-info-circle-fill';
        let defaultTitle = 'Thông báo';
        if (type === 'success') {
            iconClass = 'bi-check-circle-fill';
            defaultTitle = 'Thành công';
        } else if (type === 'warning') {
            iconClass = 'bi-exclamation-triangle-fill';
            defaultTitle = 'Cảnh báo';
        } else if (type === 'danger') {
            iconClass = 'bi-x-circle-fill';
            defaultTitle = 'Có lỗi xảy ra';
        }

        const displayTitle = title || defaultTitle;

        toastEl.innerHTML = `
            <div class="uneti-toast-indicator"></div>
            <div class="uneti-toast-content d-flex align-items-start gap-2 p-3">
                <span class="uneti-toast-icon fs-5">
                    <i class="bi ${iconClass}"></i>
                </span>
                <div class="flex-grow-1 min-w-0">
                    <div class="uneti-toast-title fw-bold small text-dark">${displayTitle}</div>
                    <div class="uneti-toast-message small text-muted mt-1 text-break">${message}</div>
                </div>
                <button type="button" class="btn-close btn-close-sm shadow-none ms-2" aria-label="Đóng"></button>
            </div>
            <div class="uneti-toast-progress"><div class="uneti-toast-progress-bar"></div></div>
        `;

        const closeBtn = toastEl.querySelector('.btn-close');
        function removeToast() {
            if (toastEl.classList.contains('hiding')) return;
            toastEl.classList.add('hiding');
            setTimeout(() => {
                if (toastEl.parentElement) {
                    toastEl.parentElement.removeChild(toastEl);
                }
            }, 250);
        }

        closeBtn.addEventListener('click', removeToast);

        let timeoutId = null;
        if (duration > 0) {
            const progressBar = toastEl.querySelector('.uneti-toast-progress-bar');
            if (progressBar) {
                progressBar.style.animationDuration = `${duration}ms`;
            }
            timeoutId = setTimeout(removeToast, duration);
            toastEl.addEventListener('mouseenter', () => {
                if (timeoutId) clearTimeout(timeoutId);
                if (progressBar) progressBar.style.animationPlayState = 'paused';
            });
            toastEl.addEventListener('mouseleave', () => {
                timeoutId = setTimeout(removeToast, 1500);
                if (progressBar) progressBar.style.animationPlayState = 'running';
            });
        }

        container.appendChild(toastEl);
        requestAnimationFrame(() => {
            toastEl.classList.add('show');
        });
        return toastEl;
    }

    // 2. Modern Confirm Modal
    function confirm(options) {
        const {
            title = 'Xác nhận thao tác',
            message = 'Bạn có chắc chắn muốn thực hiện hành động này?',
            confirmText = 'Xác nhận',
            cancelText = 'Hủy bỏ',
            type = 'danger', // 'danger' | 'warning' | 'primary'
            onConfirm = null,
            onCancel = null
        } = typeof options === 'string' ? { message: options } : (options || {});

        let modalEl = document.getElementById('unetiConfirmModal');
        if (!modalEl) {
            modalEl = document.createElement('div');
            modalEl.id = 'unetiConfirmModal';
            modalEl.className = 'modal fade';
            modalEl.tabIndex = -1;
            modalEl.setAttribute('aria-hidden', 'true');
            modalEl.innerHTML = `
                <div class="modal-dialog modal-dialog-centered" style="max-width: 440px;">
                    <div class="modal-content border-0 shadow-lg" style="border-radius: var(--uneti-radius-lg, 18px); overflow: hidden;">
                        <div class="modal-body p-4 text-center">
                            <div id="unetiConfirmIcon" class="d-inline-flex justify-content-center align-items-center rounded-circle mb-3" style="width: 56px; height: 56px;">
                                <i class="bi fs-3"></i>
                            </div>
                            <h5 id="unetiConfirmTitle" class="fw-bold text-dark mb-2"></h5>
                            <p id="unetiConfirmMessage" class="text-muted small mb-0" style="line-height: 1.6;"></p>
                        </div>
                        <div class="modal-footer border-0 bg-light px-4 py-3 justify-content-center gap-2">
                            <button type="button" class="btn uneti-btn-outline px-4" data-bs-dismiss="modal" id="unetiConfirmBtnCancel"></button>
                            <button type="button" class="btn px-4 fw-semibold" id="unetiConfirmBtnOk"></button>
                        </div>
                    </div>
                </div>
            `;
            document.body.appendChild(modalEl);
        }

        const iconContainer = modalEl.querySelector('#unetiConfirmIcon');
        const iconEl = iconContainer.querySelector('i');
        const titleEl = modalEl.querySelector('#unetiConfirmTitle');
        const msgEl = modalEl.querySelector('#unetiConfirmMessage');
        const btnOk = modalEl.querySelector('#unetiConfirmBtnOk');
        const btnCancel = modalEl.querySelector('#unetiConfirmBtnCancel');

        titleEl.textContent = title;
        msgEl.innerHTML = message;
        btnCancel.textContent = cancelText;
        btnOk.textContent = confirmText;

        if (type === 'danger') {
            iconContainer.style.backgroundColor = '#fef2f2';
            iconContainer.style.color = '#ef4444';
            iconEl.className = 'bi bi-exclamation-triangle-fill fs-3';
            btnOk.className = 'btn btn-danger px-4 fw-semibold';
        } else if (type === 'warning') {
            iconContainer.style.backgroundColor = '#fffbeb';
            iconContainer.style.color = '#f59e0b';
            iconEl.className = 'bi bi-question-circle-fill fs-3';
            btnOk.className = 'btn btn-warning text-dark px-4 fw-semibold';
        } else {
            iconContainer.style.backgroundColor = '#eff6ff';
            iconContainer.style.color = '#1d4ed8';
            iconEl.className = 'bi bi-info-circle-fill fs-3';
            btnOk.className = 'btn uneti-btn-primary px-4 fw-semibold';
        }

        const bsModal = bootstrap.Modal.getOrCreateInstance(modalEl);

        let confirmed = false;
        const handleOk = function () {
            confirmed = true;
            bsModal.hide();
            if (typeof onConfirm === 'function') onConfirm();
        };

        const handleHidden = function () {
            btnOk.removeEventListener('click', handleOk);
            modalEl.removeEventListener('hidden.bs.modal', handleHidden);
            if (!confirmed && typeof onCancel === 'function') {
                onCancel();
            }
        };

        btnOk.addEventListener('click', handleOk);
        modalEl.addEventListener('hidden.bs.modal', handleHidden);

        bsModal.show();
    }

    // 3. Thông báo module đang phát triển
    const moduleMap = {
        2: {
            name: 'Module 2 – Quản Lý Khóa Học & Lớp Học Phần',
            title: 'Kế hoạch Mở Lớp, Thời Khóa Biểu & Quản Lý Giảng Viên',
            desc: 'Bao gồm: Thiết lập danh mục khóa học, xếp lịch giảng dạy, phân công giảng viên và quản lý kế hoạch đào tạo theo Mục 6 & Mục 10 Đề tài 16.',
            badgeBg: '#fffbeb',
            badgeColor: '#b45309',
            badgeBorder: '#fde68a'
        },
        3: {
            name: 'Module 3 – Quản Lý Học Viên & Hồ Sơ Học Tập',
            title: 'Hồ Sơ Học Viên & Theo Dõi Tiến Trình Học Tập',
            desc: 'Bao gồm: Quản lý danh sách học viên, hồ sơ lý lịch cá nhân, thông tin liên lạc và theo dõi lịch sử tham gia khóa học theo Mục 7 Đề tài 16.',
            badgeBg: '#ecfdf5',
            badgeColor: '#047857',
            badgeBorder: '#a7f3d0'
        },
        4: {
            name: 'Module 4 – Tiếp Nhận Đăng Ký & Quản Lý Học Phí',
            title: 'Tiếp Nhận Đơn, Duyệt Đăng Ký & Đối Soát Công Nợ',
            desc: 'Bao gồm: Đăng ký học phần trực tuyến, xét duyệt hồ sơ, xử lý luồng trạng thái và quản lý công nợ học phí theo Mục 8 Đề tài 16.',
            badgeBg: '#eff6ff',
            badgeColor: '#1d4ed8',
            badgeBorder: '#bfdbfe'
        },
        5: {
            name: 'Module 5 – Kết Quả Học Tập & Báo Cáo Thống Kê',
            title: 'Quản Lý Điểm Số, Đánh Giá & Bảng Điều Khiển Thống Kê',
            desc: 'Bao gồm: Cập nhật điểm chuyên cần, giữa kỳ, cuối kỳ, tính điểm tổng kết và biểu đồ thống kê học vụ theo Mục 9 Đề tài 16.',
            badgeBg: '#fdf2f8',
            badgeColor: '#be185d',
            badgeBorder: '#fbcfe8'
        }
    };

    function showModulePending(soModule, customTitle, customDesc) {
        let info = moduleMap[soModule] || {
            name: `Phân hệ Module ${soModule}`,
            title: customTitle || 'Tính Năng Đang Trong Quá Trình Triển Khai',
            desc: customDesc || 'Tính năng này đang được nhóm phát triển hoàn thiện theo phân công đề tài.',
            badgeBg: '#eff6ff',
            badgeColor: '#1d4ed8',
            badgeBorder: '#bfdbfe'
        };

        if (customTitle) info.title = customTitle;
        if (customDesc) info.desc = customDesc;

        let modalEl = document.getElementById('unetiModuleModal');
        if (!modalEl) {
            modalEl = document.createElement('div');
            modalEl.id = 'unetiModuleModal';
            modalEl.className = 'modal fade';
            modalEl.tabIndex = -1;
            modalEl.setAttribute('aria-hidden', 'true');
            modalEl.innerHTML = `
                <div class="modal-dialog modal-dialog-centered">
                    <div class="modal-content border-0 shadow-lg" style="border-radius: var(--uneti-radius-lg, 18px); overflow: hidden;">
                        <div class="modal-header border-0 pb-0 pt-4 px-4 d-flex justify-content-between align-items-center">
                            <div class="d-flex align-items-center gap-2">
                                <span class="d-inline-flex justify-content-center align-items-center rounded-circle" 
                                      style="width: 44px; height: 44px; background-color: #fffbeb; color: #d97706; border: 1.5px solid #fde68a;">
                                    <i class="bi bi-cone-striped fs-5"></i>
                                </span>
                                <div>
                                    <h5 class="modal-title fw-bold text-dark mb-0">Tính Năng Đang Triển Khai</h5>
                                    <small class="text-muted uneti-fs-12">HỆ THỐNG ĐÀO TẠO THEO PHÂN HỆ</small>
                                </div>
                            </div>
                            <button type="button" class="btn-close shadow-none" data-bs-dismiss="modal" aria-label="Đóng"></button>
                        </div>
                        <div class="modal-body p-4">
                            <div class="mb-3">
                                <span id="unetiModalBadge" class="badge px-3 py-1 rounded-pill fw-semibold mb-2" style="font-size: 12px;"></span>
                                <h6 id="unetiModalTitle" class="fw-bold text-dark mb-2"></h6>
                                <p id="unetiModalDesc" class="text-muted small mb-0" style="line-height: 1.6;"></p>
                            </div>
                            <div class="p-3 rounded-3 small" style="background-color: #eff6ff; border: 1px solid #bfdbfe;">
                                <div class="d-flex align-items-start gap-2">
                                    <i class="bi bi-info-circle-fill text-primary mt-1"></i>
                                    <div class="text-dark">
                                        <strong>Phạm vi hiện tại:</strong> Hệ thống đang phục vụ kiểm thử <strong>Module 1 (Quản lý Môn học & Tài khoản / Phân quyền)</strong>.
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="modal-footer border-0 pt-0 pb-4 px-4 gap-2">
                            <button type="button" class="btn uneti-btn-outline btn-sm px-3" data-bs-dismiss="modal">Đã hiểu</button>
                            <a href="/MonHoc" class="btn uneti-btn-primary btn-sm px-3 text-decoration-none">
                                <i class="bi bi-book-half me-1"></i>Xem môn học Module 1
                            </a>
                        </div>
                    </div>
                </div>
            `;
            document.body.appendChild(modalEl);
        }

        const badge = modalEl.querySelector('#unetiModalBadge');
        badge.textContent = info.name;
        badge.style.backgroundColor = info.badgeBg;
        badge.style.color = info.badgeColor;
        badge.style.border = `1px solid ${info.badgeBorder}`;

        modalEl.querySelector('#unetiModalTitle').textContent = info.title;
        modalEl.querySelector('#unetiModalDesc').innerHTML = info.desc;

        const modal = bootstrap.Modal.getOrCreateInstance(modalEl);
        modal.show();
    }

    return {
        toast,
        confirm,
        showModulePending
    };
})();

// Gắn toàn cục
window.UnetiUI = UnetiUI;

// Hàm thongBaoModule dùng chung toàn hệ thống thay thế cho alert()
function thongBaoModule(soModule) {
    UnetiUI.showModulePending(soModule);
}

// Gắn Confirm Modal cho form có data-uneti-confirm
document.addEventListener('DOMContentLoaded', function () {
    document.addEventListener('submit', function (e) {
        const form = e.target;
        if (!(form instanceof HTMLFormElement)) return;
        const confirmMsg = form.getAttribute('data-uneti-confirm');
        if (confirmMsg && !form.dataset.unetiConfirmed) {
            e.preventDefault();
            const title = form.getAttribute('data-uneti-title') || 'Xác nhận thao tác';
            const type = form.getAttribute('data-uneti-type') || 'warning';
            const okText = form.getAttribute('data-uneti-ok') || 'Xác nhận';
            const cancelText = form.getAttribute('data-uneti-cancel') || 'Hủy';

            UnetiUI.confirm({
                title: title,
                message: confirmMsg,
                type: type,
                confirmText: okText,
                cancelText: cancelText,
                onConfirm: function () {
                    form.dataset.unetiConfirmed = 'true';
                    form.submit();
                }
            });
        }
    });
});

