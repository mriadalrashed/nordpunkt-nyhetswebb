using Microsoft.AspNetCore.Mvc;
using Nordpunkt.Models;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Logging;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Cms.Web.Website.Controllers;
using Umbraco.Extensions;

namespace Nordpunkt.Controllers;

// Tar emot formuläret Tipsa redaktionen och sparar tipset i mappen Tips (SCRUM-11).
public class TipsController : SurfaceController
{
    private const string Felmeddelande = "Tipset kunde inte sparas just nu. Försök igen om en stund. Det du skrev finns kvar.";

    private readonly IContentService _contentService;
    private readonly ILogger<TipsController> _logger;

    public TipsController(
        IUmbracoContextAccessor umbracoContextAccessor,
        IUmbracoDatabaseFactory databaseFactory,
        ServiceContext services,
        AppCaches appCaches,
        IProfilingLogger profilingLogger,
        IPublishedUrlProvider publishedUrlProvider,
        IContentService contentService,
        ILogger<TipsController> logger)
        : base(umbracoContextAccessor, databaseFactory, services, appCaches, profilingLogger, publishedUrlProvider)
    {
        _contentService = contentService;
        _logger = logger;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Skicka(TipsFormModel model)
    {
        // 0. Spam-fällan (SCRUM-19): en människa ser inte fältet Webbplats, så om det är ifyllt
        //    är det ett automatiskt program. Inget sparas, men avsändaren ser samma bekräftelse.
        if (!string.IsNullOrWhiteSpace(model.Webbplats))
        {
            _logger.LogInformation("Ett tips stoppades av spam-fällan.");
            return TillTackSidan();
        }

        // 1. Fel i formuläret: visa sidan igen med felmeddelanden och det som skrevs.
        if (!ModelState.IsValid)
        {
            return CurrentUmbracoPage();
        }

        // 2. Hitta mappen Tips i roten.
        var tipsmapp = _contentService.GetRootContent()
            .FirstOrDefault(x => x.ContentType.Alias == "tipsmapp");

        if (tipsmapp is null)
        {
            _logger.LogError("Mappen Tips (tipsmapp) finns inte i roten.");
            ModelState.AddModelError(string.Empty, Felmeddelande);
            return CurrentUmbracoPage();
        }

        // 3. Spara tipset. Det publiceras aldrig.
        try
        {
            var tips = _contentService.Create(model.Rubrik, tipsmapp.Id, "tips");
            tips.SetValue("rubrik", model.Rubrik);
            tips.SetValue("beskrivning", model.Beskrivning);
            tips.SetValue("namn", model.Namn);
            tips.SetValue("epost", model.Epost);
            tips.SetValue("mottaget", DateTime.Now);

            var resultat = _contentService.Save(tips);
            if (!resultat.Success)
            {
                throw new InvalidOperationException($"Save misslyckades: {resultat.Result}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ett tips kunde inte sparas.");
            ModelState.AddModelError(string.Empty, Felmeddelande);
            return CurrentUmbracoPage();
        }

        // 4. Post-Redirect-Get: skicka besökaren till bekräftelsesidan.
        return TillTackSidan();
    }

    // Skickar besökaren till sidan "Tack" under Tipsa redaktionen.
    private IActionResult TillTackSidan()
    {
        var tackSida = CurrentPage?.Children()
            .FirstOrDefault(x => x.ContentType.Alias == "tipsTack");

        return tackSida is null
            ? RedirectToCurrentUmbracoPage()
            : RedirectToUmbracoPage(tackSida);
    }
}