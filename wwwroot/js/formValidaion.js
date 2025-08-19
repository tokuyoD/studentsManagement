document.addEventListener("DOMContentLoaded", function() {
    var forms = document.querySelectorAll('.needs-validation');

    Array.prototype.slice.call(forms).forEach(function(form) {
        form.addEventListener('submit', function(event) {
            if (!form.checkValidity()) {
                event.preventDefault();
                event.stopPropagation();

                // 找第一個沒填欄位
                var firstInvalid = form.querySelector(':invalid');
                if (firstInvalid) {
                    var fieldName = firstInvalid.getAttribute('name') || firstInvalid.getAttribute('id');
                    alert(`請填寫欄位: ${fieldName}`);
                    firstInvalid.focus();
                }
            }
            form.classList.add('was-validated');
        }, false);
    });
});
