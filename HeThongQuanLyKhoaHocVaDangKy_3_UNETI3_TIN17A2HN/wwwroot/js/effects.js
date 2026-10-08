// Họ và tên: Trần Văn Thành - MSV: 23103100076 - Lớp: TIN17A2HN
// Module 1: Hiệu ứng giao diện (Scroll, Count-up, Navbar)

(function () {
    'use strict';

    // 1. Kiểm tra giảm chuyển động (prefers-reduced-motion)
    const prefersReducedMotion = window.matchMedia('(prefers-reduced-motion: reduce)').matches;

    // 2. Chuyển trang mượt mà (Page Transitions)
    function initPageTransitions() {
        if (prefersReducedMotion) return;

        document.addEventListener('click', function (e) {
            const anchor = e.target.closest('a');
            if (!anchor) return;

            const href = anchor.getAttribute('href');
            const target = anchor.getAttribute('target');

            // Bỏ qua các liên kết không kích hoạt chuyển trang
            if (!href ||
                href.startsWith('#') ||
                href.startsWith('javascript:') ||
                href.startsWith('mailto:') ||
                href.startsWith('tel:') ||
                target === '_blank' ||
                anchor.hasAttribute('download') ||
                anchor.hasAttribute('data-no-transition') ||
                anchor.hasAttribute('data-bs-toggle') ||
                e.ctrlKey || e.metaKey || e.shiftKey) {
                return;
            }

            // Kiểm tra link nội bộ cùng origin
            try {
                const url = new URL(anchor.href, window.location.origin);
                if (url.origin === window.location.origin && url.pathname !== window.location.pathname) {
                    e.preventDefault();
                    document.body.classList.add('fx-page-exiting');
                    setTimeout(function () {
                        window.location.href = anchor.href;
                    }, 220);
                }
            } catch (_) {
                // Ignore URL parsing errors
            }
        });
    }

    // 3. Cuộn hiển thị (Scroll Reveal bằng IntersectionObserver)
    function initScrollReveal() {
        const revealElements = document.querySelectorAll('.reveal, .reveal-left, .reveal-zoom, .stagger-card, .stagger-group > *');
        if (!revealElements.length) return;

        if (prefersReducedMotion || !('IntersectionObserver' in window)) {
            revealElements.forEach(el => el.classList.add('is-visible'));
            return;
        }

        const revealObserver = new IntersectionObserver(function (entries, observer) {
            entries.forEach(function (entry) {
                if (entry.isIntersecting) {
                    entry.target.classList.add('is-visible');
                    observer.unobserve(entry.target);
                }
            });
        }, {
            root: null,
            rootMargin: '0px 0px -10px 0px',
            threshold: 0.05
        });

        revealElements.forEach(el => {
            if (!el.classList.contains('is-visible')) {
                revealObserver.observe(el);
            }
        });

        // Dự phòng an toàn: sau 1.4s tự động hiện các thẻ phòng trường hợp scroll chưa chạm
        setTimeout(function () {
            revealElements.forEach(el => el.classList.add('is-visible'));
        }, 1400);
    }

    // 4. Đếm số động (Count-up Animation với easing cubic)
    function formatNumber(value, format) {
        if (format === 'currency') {
            return new Intl.NumberFormat('vi-VN').format(Math.round(value)) + ' đ';
        } else if (format === 'number') {
            return new Intl.NumberFormat('vi-VN').format(Math.round(value));
        }
        return Math.round(value).toString();
    }

    function animateCountUp(element) {
        const targetValue = parseFloat(element.getAttribute('data-count-to') || '0');
        let format = element.getAttribute('data-format') || 'number';
        const suffix = element.getAttribute('data-suffix') || '';
        const duration = parseInt(element.getAttribute('data-duration') || '1200', 10);

        // Tự động kiểm tra tránh lặp hai chữ 'đ đ' nếu đã có ký hiệu đơn vị tiền tệ bên ngoài
        if (format === 'currency') {
            const nextNode = element.nextSibling;
            const nextElement = element.nextElementSibling;
            const hasAdjacentDong = (nextNode && nextNode.nodeType === Node.TEXT_NODE && nextNode.textContent.trim().startsWith('đ')) ||
                                    (nextElement && nextElement.textContent.trim().toLowerCase() === 'đ');
            if (hasAdjacentDong) {
                format = 'number';
            }
        }

        if (prefersReducedMotion) {
            element.textContent = formatNumber(targetValue, format) + suffix;
            return;
        }

        const startTime = performance.now();
        const startValue = 0;

        function update(currentTime) {
            const elapsed = currentTime - startTime;
            const progress = Math.min(elapsed / duration, 1);

            // EaseOutCubic: 1 - (1 - progress)^3
            const easeProgress = 1 - Math.pow(1 - progress, 3);
            const currentValue = startValue + (targetValue - startValue) * easeProgress;

            element.textContent = formatNumber(currentValue, format) + suffix;

            if (progress < 1) {
                requestAnimationFrame(update);
            } else {
                element.textContent = formatNumber(targetValue, format) + suffix;
            }
        }

        requestAnimationFrame(update);
    }

    function initCountUp() {
        const counterElements = document.querySelectorAll('[data-count-to]');
        if (!counterElements.length) return;

        if (prefersReducedMotion || !('IntersectionObserver' in window)) {
            counterElements.forEach(el => animateCountUp(el));
            return;
        }

        const counterObserver = new IntersectionObserver(function (entries, observer) {
            entries.forEach(function (entry) {
                if (entry.isIntersecting) {
                    animateCountUp(entry.target);
                    observer.unobserve(entry.target);
                }
            });
        }, {
            threshold: 0.2
        });

        counterElements.forEach(el => counterObserver.observe(el));
    }

    // 5. Navbar thông minh & nút Lên đầu trang
    function initSmartNavbarAndBackToTop() {
        const navbar = document.querySelector('.uneti-navbar');
        let backToTopBtn = document.getElementById('btnBackToTop');

        // Tạo nút Back-to-top nếu chưa có trong DOM
        if (!backToTopBtn) {
            backToTopBtn = document.createElement('button');
            backToTopBtn.id = 'btnBackToTop';
            backToTopBtn.className = 'fx-back-to-top';
            backToTopBtn.type = 'button';
            backToTopBtn.setAttribute('aria-label', 'Cuộn lên đầu trang');
            backToTopBtn.title = 'Lên đầu trang';
            backToTopBtn.innerHTML = '<i class="bi bi-chevron-up fs-5"></i>';
            document.body.appendChild(backToTopBtn);
        }

        let ticking = false;
        function onScroll() {
            const scrollY = window.scrollY || window.pageYOffset;

            if (navbar) {
                if (scrollY > 10) {
                    navbar.classList.add('is-scrolled');
                } else {
                    navbar.classList.remove('is-scrolled');
                }
            }

            if (backToTopBtn) {
                if (scrollY > 350) {
                    backToTopBtn.classList.add('is-visible');
                } else {
                    backToTopBtn.classList.remove('is-visible');
                }
            }

            ticking = false;
        }

        window.addEventListener('scroll', function () {
            if (!ticking) {
                window.requestAnimationFrame(onScroll);
                ticking = true;
            }
        }, { passive: true });

        // Ban đầu chạy 1 lần để khớp vị trí cuộn hiện tại
        onScroll();

        backToTopBtn.addEventListener('click', function () {
            window.scrollTo({
                top: 0,
                behavior: prefersReducedMotion ? 'auto' : 'smooth'
            });
        });
    }

    // 6. Hiệu ứng Ripple trên các nút bấm
    function initButtonRipple() {
        if (prefersReducedMotion) return;

        document.addEventListener('click', function (e) {
            const btn = e.target.closest('.uneti-btn-primary, .uneti-btn-outline, .uneti-btn-banner-primary, .uneti-btn-banner-secondary, [data-ripple]');
            if (!btn) return;

            const rect = btn.getBoundingClientRect();
            const size = Math.max(rect.width, rect.height);
            const x = e.clientX - rect.left - size / 2;
            const y = e.clientY - rect.top - size / 2;

            const ripple = document.createElement('span');
            ripple.className = 'fx-ripple';
            ripple.style.width = ripple.style.height = `${size}px`;
            ripple.style.left = `${x}px`;
            ripple.style.top = `${y}px`;

            btn.appendChild(ripple);

            setTimeout(function () {
                if (ripple.parentElement) {
                    ripple.parentElement.removeChild(ripple);
                }
            }, 600);
        });
    }

    // 7. Hiệu ứng loading khi submit form
    function initFormSubmitLoading() {
        document.addEventListener('submit', function (e) {
            const form = e.target;
            if (!(form instanceof HTMLFormElement)) return;

            // Kiểm tra tính hợp lệ của Form phía Client trước khi bật loading
            if (window.jQuery && typeof window.jQuery(form).valid === 'function') {
                if (!window.jQuery(form).valid()) return;
            } else if (typeof form.checkValidity === 'function') {
                if (!form.checkValidity()) return;
            }

            const submitBtn = form.querySelector('button[type="submit"]:not([data-no-loading])');
            if (!submitBtn || submitBtn.classList.contains('is-loading')) return;

            const loadingText = submitBtn.getAttribute('data-loading-text') || 'Đang xử lý...';

            submitBtn.classList.add('is-loading');
            submitBtn.disabled = true;

            const spinnerHTML = `
                <span class="fx-btn-spinner-wrap">
                    <span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span>
                    <span>${loadingText}</span>
                </span>
            `;
            submitBtn.insertAdjacentHTML('beforeend', spinnerHTML);

            // Timeout an toàn phòng trường hợp trang bị dừng hoặc lỗi mạng
            setTimeout(function () {
                if (submitBtn.classList.contains('is-loading')) {
                    submitBtn.classList.remove('is-loading');
                    submitBtn.disabled = false;
                    const wrap = submitBtn.querySelector('.fx-btn-spinner-wrap');
                    if (wrap) wrap.remove();
                }
            }, 8000);
        });
    }

    // 8. Hiệu ứng Micro 3D Tilt cho Card trên thiết bị hỗ trợ hover
    function initCardTilt() {
        if (prefersReducedMotion || !window.matchMedia('(hover: hover)').matches) return;

        document.querySelectorAll('.uneti-card-hover[data-tilt]').forEach(function (card) {
            card.addEventListener('mousemove', function (e) {
                const rect = card.getBoundingClientRect();
                const x = e.clientX - rect.left;
                const y = e.clientY - rect.top;

                const centerX = rect.width / 2;
                const centerY = rect.height / 2;

                const rotateX = ((y - centerY) / centerY) * -3; // Tối đa 3 độ
                const rotateY = ((x - centerX) / centerX) * 3;

                card.style.transform = `perspective(1000px) translateY(-5px) rotateX(${rotateX}deg) rotateY(${rotateY}deg)`;
            });

            card.addEventListener('mouseleave', function () {
                card.style.transform = '';
            });
        });
    }

    // 9. Hiệu ứng parallax chuột
    function initHeroParallax() {
        if (prefersReducedMotion || !window.matchMedia('(hover: hover)').matches) return;

        const heroBanner = document.querySelector('.uneti-hero-banner');
        if (!heroBanner) return;

        const parallaxItems = heroBanner.querySelectorAll('[data-parallax]');
        if (!parallaxItems.length) return;

        heroBanner.addEventListener('mousemove', function (e) {
            const rect = heroBanner.getBoundingClientRect();
            const x = (e.clientX - rect.left) / rect.width - 0.5;
            const y = (e.clientY - rect.top) / rect.height - 0.5;

            parallaxItems.forEach(function (item) {
                const depth = parseFloat(item.getAttribute('data-parallax') || '10');
                const moveX = x * depth;
                const moveY = y * depth;
                item.style.transform = `translate3d(${moveX}px, ${moveY}px, 0)`;
            });
        });

        heroBanner.addEventListener('mouseleave', function () {
            parallaxItems.forEach(function (item) {
                item.style.transform = '';
            });
        });
    }

    function refreshEffects() {
        initScrollReveal();
        initCountUp();
        initCardTilt();
    }

    // Expose API cho các thao tác AJAX động
    window.UnetiEffects = {
        refresh: refreshEffects,
        initScrollReveal: initScrollReveal,
        initCountUp: initCountUp,
        initCardTilt: initCardTilt
    };

    // Khởi tạo toàn bộ hiệu ứng khi DOM đã sẵn sàng
    document.addEventListener('DOMContentLoaded', function () {
        initPageTransitions();
        initScrollReveal();
        initCountUp();
        initSmartNavbarAndBackToTop();
        initButtonRipple();
        initFormSubmitLoading();
        initCardTilt();
        initHeroParallax();
    });

})();
