// Họ và tên: Trần Văn Thành - MSV: 23103100076 - Lớp: TIN17A2HN
// Module 1: Script hỗ trợ UI & Validation

// Hàm thông báo phân hệ các Module khác đang phát triển (Đề tài 16)
function thongBaoModule(soModule) {
    if (window.UnetiUI && typeof window.UnetiUI.showModulePending === 'function') {
        window.UnetiUI.showModulePending(soModule);
    }
}

// =========================================================================
// Tự động xóa thông báo lỗi khi người dùng nhập liệu
// =========================================================================
document.addEventListener('DOMContentLoaded', function () {
    initDynamicFormValidation();
});

function initDynamicFormValidation() {
    // 1. Tự động lắng nghe tất cả input, select, textarea trong các form
    const inputs = document.querySelectorAll('form input, form select, form textarea');

    inputs.forEach(function (input) {
        // Lắng nghe khi người dùng gõ phím, paste, hoặc thay đổi giá trị
        ['input', 'change', 'keyup', 'paste'].forEach(function (eventName) {
            input.addEventListener(eventName, function () {
                clearFieldError(input);
            });
        });

        // Xóa cảnh báo khi focus nếu ô đã có giá trị
        input.addEventListener('focus', function () {
            if (input.value && input.value.trim().length > 0) {
                clearFieldError(input);
            }
        });
    });

    // 2. Xử lý Realtime kiểm tra khớp Mật khẩu & Xác nhận mật khẩu
    const passInputs = document.querySelectorAll('input[type="password"]:not([id*="Confirm"]):not([name*="XacNhan"]):not([name*="Cu"])');
    const confirmInputs = document.querySelectorAll('input[type="password"][id*="Confirm"], input[type="password"][name*="XacNhan"]');

    if (passInputs.length > 0 && confirmInputs.length > 0) {
        const passEl = passInputs[0];
        const confirmEl = confirmInputs[0];

        const validatePasswordMatch = function () {
            const pVal = passEl.value;
            const cVal = confirmEl.value;
            const errEl = findErrorElement(confirmEl);

            if (cVal.length > 0) {
                if (pVal === cVal) {
                    clearFieldError(confirmEl);
                    confirmEl.style.borderColor = '#10b981';
                    confirmEl.style.backgroundColor = '#ecfdf5';
                } else if (cVal.length >= pVal.length) {
                    confirmEl.classList.add('input-validation-error');
                    confirmEl.style.borderColor = '#ef4444';
                    confirmEl.style.backgroundColor = '#fef2f2';
                    if (errEl) {
                        errEl.textContent = 'Mật khẩu xác nhận chưa trùng khớp.';
                        errEl.style.display = 'block';
                        errEl.classList.remove('field-validation-valid');
                        errEl.classList.add('field-validation-error');
                    }
                }
            } else {
                confirmEl.style.borderColor = '';
                confirmEl.style.backgroundColor = '';
            }
        };

        confirmEl.addEventListener('input', validatePasswordMatch);
        passEl.addEventListener('input', function () {
            if (confirmEl.value.length > 0) {
                validatePasswordMatch();
            }
        });
    }

    // 3. Tự động ẩn validation-summary chung khi người dùng sửa lỗi
    const forms = document.querySelectorAll('form');
    forms.forEach(function (form) {
        form.addEventListener('input', function () {
            const summary = form.querySelector('[asp-validation-summary], .validation-summary-errors, .alert-danger');
            if (summary && summary.classList.contains('validation-summary-errors')) {
                summary.style.opacity = '0.7';
            }
        });
    });
}

// Hàm gỡ bỏ cảnh báo đỏ và ẩn dòng lỗi của một ô input
function clearFieldError(input) {
    if (!input) return;

    // 1. Gỡ bỏ các class và style báo lỗi trên input
    input.classList.remove('input-validation-error');
    input.classList.remove('is-invalid');
    input.style.borderColor = '';
    input.style.backgroundColor = '';

    // 2. Tìm phần tử thông báo lỗi (span asp-validation-for)
    const errorEl = findErrorElement(input);
    if (errorEl) {
        errorEl.textContent = '';
        errorEl.style.display = 'none';
        errorEl.classList.remove('field-validation-error');
        errorEl.classList.add('field-validation-valid');
    }

    // 3. Nếu là checkbox điều khoản
    if (input.type === 'checkbox' && input.checked) {
        input.classList.remove('input-validation-error');
        const chkError = findErrorElement(input);
        if (chkError) {
            chkError.textContent = '';
            chkError.style.display = 'none';
        }
    }
}

// Hàm tìm phần tử hiển thị thông báo lỗi của input
function findErrorElement(input) {
    if (!input) return null;

    const name = input.getAttribute('name');
    const id = input.id;

    // Ưu tiên 1: Theo chuẩn data-valmsg-for của ASP.NET Core
    if (name) {
        const el = document.querySelector(`span[data-valmsg-for="${name}"]`);
        if (el) return el;
    }
    if (id) {
        const el = document.querySelector(`span[data-valmsg-for="${id}"]`);
        if (el) return el;
    }

    // Tìm trong container bao quanh
    const container = input.closest('.col-md-6, .col-12, .mb-2, .mb-3, .form-check, .form-group') || input.parentElement;
    if (container) {
        const localError = container.querySelector('.field-validation-error, span[data-valmsg-for], span.text-danger:not(.text-danger-required)');
        if (localError) return localError;
    }

    // Ưu tiên 3: Tìm phần tử anh em kế tiếp
    let next = input.nextElementSibling;
    while (next) {
        if (next.matches && (next.matches('.text-danger') || next.matches('.field-validation-error') || next.matches('.invalid-feedback'))) {
            return next;
        }
        next = next.nextElementSibling;
    }

    return null;
}
