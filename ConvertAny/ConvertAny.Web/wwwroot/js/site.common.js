const resetBtn = document.getElementById("resetFiles");

(function (o) {
    if (!o) return;

    o.addEventListener("click", function (e) {
        preventDefaults(e);
        pageReload();
    });
})(resetBtn);

function goTop(x, y) {
    x = x === undefined || x === null ? 0 : x;
    y = y === undefined || y === null ? 0 : x;
    window.scrollTo(x, y);
}

function preventDefaults(e) {
    e.preventDefault();
    e.stopPropagation();
}

function pageReload() {
    location.reload();
    goTop();
}

