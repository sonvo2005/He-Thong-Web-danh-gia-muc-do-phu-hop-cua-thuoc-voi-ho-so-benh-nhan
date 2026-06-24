// ===== AUTO-DISMISS ALERTS =====
document.addEventListener('DOMContentLoaded', () => {
    // Auto dismiss success alerts after 4s
    document.querySelectorAll('.alert-success.alert-dismissible').forEach(el => {
        setTimeout(() => {
            const bs = bootstrap.Alert.getOrCreateInstance(el);
            bs?.close();
        }, 4000);
    });

    // Confirm delete forms
    document.querySelectorAll('form[data-confirm]').forEach(form => {
        form.addEventListener('submit', e => {
            if (!confirm(form.dataset.confirm)) e.preventDefault();
        });
    });

    // Active nav link highlight
    const path = window.location.pathname.toLowerCase();
    document.querySelectorAll('.navbar .nav-link').forEach(link => {
        const href = link.getAttribute('href')?.toLowerCase();
        if (href && href !== '/' && path.startsWith(href)) {
            link.classList.add('active');
            link.style.background = 'rgba(255,255,255,0.2)';
        }
    });

    // Animate stat cards on load
    document.querySelectorAll('.stat-card').forEach((card, i) => {
        card.style.opacity = '0';
        card.style.transform = 'translateY(20px)';
        setTimeout(() => {
            card.style.transition = 'opacity 0.4s ease, transform 0.4s ease';
            card.style.opacity = '1';
            card.style.transform = 'translateY(0)';
        }, i * 80);
    });
});

// ===== GLOBAL HELPERS =====
function confirmDelete(msg) {
    return confirm(msg || 'Bạn có chắc muốn xóa mục này?');
}

function formatDate(dateStr) {
    if (!dateStr) return '';
    const d = new Date(dateStr);
    return d.toLocaleDateString('vi-VN');
}
