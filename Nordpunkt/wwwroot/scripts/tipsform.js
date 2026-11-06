// Flyttar fokus till första fältet med fel när formuläret visas igen (SCRUM-12).
(function () {
    var formular = document.getElementById('tipsformular');
    if (!formular) { return; }

    var forstaFel = formular.querySelector('[aria-invalid="true"]');
    if (forstaFel) {
        forstaFel.focus();
        return;
    }

    // Fel som inte gäller ett visst fält, t.ex. "Tipset kunde inte sparas".
    var allmantFel = formular.querySelector('.form-error.validation-summary-errors');
    if (allmantFel) {
        allmantFel.focus();
    }
})();