// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function previewFile(event) {
    var output = document.getElementById("preview");
    var file = event.target.files[0];

    if (file) {
        var reader = new FileReader();
        reader.onload = function () {
            output.src = reader.result;
            output.style.display = "block";
        };
        reader.readAsDataURL(file);
    } else {
        output.src = "/images/default.png";
    }
}
