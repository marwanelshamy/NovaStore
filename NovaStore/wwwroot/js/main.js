// =====================
// NOVA — Shared JS
// =====================

// Gallery thumbnails (product page)
document.querySelectorAll('.gallery-thumb').forEach((thumb) => {
    thumb.addEventListener('click', () => {
        document.querySelectorAll('.gallery-thumb').forEach(t => t.classList.remove('active'));
        thumb.classList.add('active');
        const mainImg = document.getElementById('mainImgEl');
        if (mainImg) {
            mainImg.src = thumb.querySelector('img').src;
        }
    });
});

// Size buttons (product page)
document.querySelectorAll('.size-btn:not(.unavailable)').forEach(btn => {
    btn.addEventListener('click', () => {
        btn.closest('.size-options')?.querySelectorAll('.size-btn').forEach(b => b.classList.remove('active'));
        btn.classList.add('active');
    });
});

// Color swatches
document.querySelectorAll('.color-swatch').forEach(swatch => {
    swatch.addEventListener('click', () => {
        const parent = swatch.closest('.color-row') || swatch.closest('.color-swatches');
        parent?.querySelectorAll('.color-swatch').forEach(s => s.classList.remove('active'));
        swatch.classList.add('active');
    });
});

// Tab buttons (product page)
document.querySelectorAll('.tab-btn').forEach(btn => {
    btn.addEventListener('click', () => {
        document.querySelectorAll('.tab-btn').forEach(b => b.classList.remove('active'));
        document.querySelectorAll('.tab-content').forEach(c => c.classList.remove('active'));
        btn.classList.add('active');
        const tab = document.getElementById('tab-' + btn.dataset.tab);
        if (tab) tab.classList.add('active');
    });
});

// View toggle (shop page)
document.querySelectorAll('.view-toggle').forEach(btn => {
    btn.addEventListener('click', () => {
        document.querySelectorAll('.view-toggle').forEach(b => b.classList.remove('active'));
        btn.classList.add('active');
    });
});

// Qty control (product page standalone)
window.changeQty = function (delta) {
    const input = document.getElementById('qtyInput');
    if (input) input.value = Math.max(1, parseInt(input.value) + delta);
};