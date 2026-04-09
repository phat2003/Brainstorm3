// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

document.addEventListener('DOMContentLoaded', () => {
    const feed = document.querySelector('.home-modern-page');
    if (!feed) return;

    const items = Array.from(feed.querySelectorAll('.idea-card-modern'));
    if (items.length <= 6) return;

    const pageSize = 6;
    let visibleCount = 0;

    const loader = document.createElement('div');
    loader.id = 'ideaInfiniteLoader';
    loader.textContent = 'Đang tải thêm bài viết...';

    const sentinel = document.createElement('div');
    sentinel.id = 'ideaInfiniteSentinel';

    feed.appendChild(loader);
    feed.appendChild(sentinel);

    const revealNext = () => {
        const nextCount = Math.min(visibleCount + pageSize, items.length);
        for (let i = visibleCount; i < nextCount; i++) {
            items[i].classList.remove('d-none');
        }
        visibleCount = nextCount;

        if (visibleCount >= items.length) {
            loader.textContent = 'Đã hiển thị tất cả bài viết';
            observer.disconnect();
            sentinel.remove();
        }
    };

    items.forEach(item => item.classList.add('d-none'));
    revealNext();

    const observer = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                revealNext();
            }
        });
    }, { rootMargin: '180px 0px 180px 0px' });

    observer.observe(sentinel);
});
