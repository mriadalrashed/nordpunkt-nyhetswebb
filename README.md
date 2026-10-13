# Nordpunkt Nyhetsbyrå

Skolprojekt (EC Utbildning, .NET Web Development) av Projektgrupp 007. En testversion av en nyhetswebb byggd helt i Umbraco, med påhittat innehåll.

| Person | Område |
|---|---|
| Riad Alrashed | A – Tipsa redaktionen |
| Rasmus Pieplow | B – Redaktionellt flöde och artikelsida |
| Markus Balian | C – Startsida och navigation |

## Teknik

- Umbraco 18.2.0
- .NET 10 (SDK 10.0.x)
- SQLite i utveckling
- uSync 18.1.5 för att dela dokumenttyper och inställningar via Git

## Köra lokalt första gången

1. Klona repot och öppna `Nordpunkt.slnx` i Visual Studio.
2. Kopiera `Nordpunkt/dummy.appsettings.json` och döp kopian till `appsettings.json` (i samma mapp).
3. Skapa filen `Nordpunkt/appsettings.Development.json`:

   ```json
   {
     "$schema": "appsettings-schema.json",
     "Umbraco": {
       "CMS": {
         "Runtime": { "Mode": "Development" },
         "ModelsBuilder": { "ModelsMode": "SourceCodeAuto" },
         "Hosting": { "Debug": true },
         "Global": { "UseHttps": false }
       }
     }
   }
   ```

4. Starta projektet med profilen **Umbraco.Web.UI** (F5). Första gången visas Umbracos installationssida: skapa ett eget adminkonto och välj **SQLite**.
5. Logga in på `/umbraco`, gå till **Settings → uSync** och klicka **Import** under **Everything**.

`appsettings.json`, `appsettings.Development.json`, `appsettings.Local.json`, databasen och loggarna ligger inte i Git. Var och en har sin egen lokala databas och sitt eget adminkonto. Checka aldrig in lösenord eller nycklar.

Lägg inte in `ConnectionStrings` för hand före installationen. Installationen skriver dem själv.

## uSync

Dokumenttyper, datatyper och innehåll sparas i databasen, som inte ligger i Git. uSync skriver dem som filer i `Nordpunkt/uSync/`, och den mappen ligger i Git.

- **När du ändrar något i backoffice:** uSync exporterar automatiskt när du sparar. Gör commit på filerna i `uSync/` tillsammans med din kod.
- **Efter varje `git pull`:** starta projektet, gå till **Settings → uSync** och klicka **Import** under **Everything**. Klicka **Report** först om du vill se vad som ändras.

Ändras en gemensam inställning görs det i `dummy.appsettings.json`, och alla uppdaterar sin egen `appsettings.json` för hand.

## Arbetssätt i Git

- `dev` är arbetsgrenen och `main` är den stabila versionen. Båda är skyddade, ingen pushar direkt dit.
- En branch per Jira-ärende, skapad från senaste `dev`: `SCRUM-<nr>-<kort-namn>`, t.ex. `SCRUM-30-artikel`.
- Commit-meddelanden börjar med ärendet: `SCRUM-30 Lägg till dokumenttyp Artikel`.
- Pull request mot `dev`, minst **en annan person** godkänner innan merge.
- I slutet av varje sprint görs en pull request från `dev` till `main`.

## Definition of Done

Alla acceptanskriterier uppfyllda · fungerar på mobil och dator · fungerar med tangentbord (synligt fokus) · granskad av någon annan och mergad till `dev` · ärendet flyttat till Done i Jira.