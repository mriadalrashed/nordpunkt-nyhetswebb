using System.ComponentModel.DataAnnotations;

namespace Nordpunkt.Models;

// Det besökaren skickar in via formuläret Tipsa redaktionen (SCRUM-11).
public class TipsFormModel
{
    [Required(ErrorMessage = "Skriv en rubrik för ditt tips.")]
    [StringLength(150, ErrorMessage = "Rubriken får vara högst 150 tecken.")]
    [Display(Name = "Rubrik")]
    public string Rubrik { get; set; } = string.Empty;

    [Required(ErrorMessage = "Beskriv vad ditt tips handlar om.")]
    [StringLength(4000, ErrorMessage = "Beskrivningen får vara högst 4000 tecken.")]
    [Display(Name = "Beskrivning")]
    public string Beskrivning { get; set; } = string.Empty;

    [StringLength(100, ErrorMessage = "Namnet får vara högst 100 tecken.")]
    [Display(Name = "Namn (frivilligt)")]
    public string? Namn { get; set; }

    [StringLength(254, ErrorMessage = "E-postadressen får vara högst 254 tecken.")]
    [EmailAddress(ErrorMessage = "Skriv en e-postadress i formen namn@exempel.se, eller lämna fältet tomt.")]
    [Display(Name = "E-post (frivilligt)")]
    public string? Epost { get; set; }



    // Fälla för automatiska program (honeypot, SCRUM-19). Fältet är dolt för människor och ska vara tomt.
    public string? Webbplats { get; set; }

}