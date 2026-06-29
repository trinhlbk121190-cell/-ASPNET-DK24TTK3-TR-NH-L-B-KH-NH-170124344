// MedForum Site JS

// Auto-hide alerts after 4s
document.addEventListener('DOMContentLoaded', function () {
    const alerts = document.querySelectorAll('.alert-toast');
    alerts.forEach(a => setTimeout(() => { a.style.opacity = '0'; a.style.transition = 'opacity 0.5s'; setTimeout(() => a.remove(), 500); }, 4000));

    // Sticky navbar scroll effect
    const nav = document.getElementById('mainNav');
    if (nav) {
        window.addEventListener('scroll', () => {
            nav.style.boxShadow = window.scrollY > 10 ? '0 4px 30px rgba(0,0,0,0.3)' : 'none';
        });
    }

    // Close user dropdown when clicking outside
    document.addEventListener('click', function (e) {
        const menu = document.querySelector('.user-menu');
        const dropdown = document.getElementById('userDropdown');
        if (menu && dropdown && !menu.contains(e.target)) {
            dropdown.classList.remove('show');
        }
    });
});

function toggleUserMenu() {
    const dd = document.getElementById('userDropdown');
    if (dd) dd.classList.toggle('show');
}

// Animate stat numbers on scroll
const animateCounters = () => {
    document.querySelectorAll('.stat-number, .stat-value').forEach(el => {
        const target = parseInt(el.textContent);
        if (isNaN(target) || el.dataset.animated) return;
        el.dataset.animated = true;
        let current = 0;
        const step = Math.ceil(target / 40);
        const timer = setInterval(() => {
            current = Math.min(current + step, target);
            el.textContent = current;
            if (current >= target) clearInterval(timer);
        }, 30);
    });
};
const observer = new IntersectionObserver(entries => {
    entries.forEach(e => { if (e.isIntersecting) animateCounters(); });
});
document.querySelectorAll('.hero-stats, .stats-grid').forEach(el => observer.observe(el));
