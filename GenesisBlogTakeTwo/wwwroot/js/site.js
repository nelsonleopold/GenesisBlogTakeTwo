var quill = new Quill('#quillEditor', {
    theme: 'snow'
});

document.getElementById("quillForm").addEventListener("submit", function () {
    document.getElementById("Content").value = quill.container.firstChild.innerHTML;
});