// Mobilmeny: öppna och stäng med knapp, Enter, mellanslag och Esc.
(function () {
    var knapp = document.getElementById('menyknapp');
    var meny = document.getElementById('huvudmeny');
    if (!knapp || !meny) { return; }
 
    // Utan JavaScript är menyn alltid synlig. Med JavaScript styr knappen.
    document.documentElement.classList.add('js');
 
    function oppna() {
        meny.classList.add('is-open');
        knapp.setAttribute('aria-expanded', 'true');
        knapp.textContent = '✕ Stäng';
    }
 
    function stang(flyttaFokus) {
        meny.classList.remove('is-open');
        knapp.setAttribute('aria-expanded', 'false');
        knapp.textContent = '☰ Meny';
        if (flyttaFokus) { knapp.focus(); }
    }
 
    knapp.addEventListener('click', function () {
        if (knapp.getAttribute('aria-expanded') === 'true') {
            stang(false);
        } else {
            oppna();
        }
    });
 
    document.addEventListener('keydown', function (e) {
        if (e.key === 'Escape' && knapp.getAttribute('aria-expanded') === 'true') {
            stang(true);
        }
    });
})();