// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
window.addEventListener("load", () => {
    const btn = document.getElementById("btnAlert");
    if (btn) {
        btn.addEventListener("click", () => {
            alert("Button Clicked!");
        });
    }
});