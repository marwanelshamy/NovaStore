// Minimal frontend helpers for the storefront
document.addEventListener('DOMContentLoaded', function () {
    // Example: enhance product add buttons to show a simple toast
    document.querySelectorAll('form[asp-controller="Cart"]').forEach(function (form) {
        form.addEventListener('submit', function (e) {
            // Let the form submit normally (server handles cart). Optionally show a quick UI hint.
            const btn = form.querySelector('button[type="submit"]');
            if (btn) {
                btn.disabled = true;
                setTimeout(() => btn.disabled = false, 1200);
            }
        });
    });
});
