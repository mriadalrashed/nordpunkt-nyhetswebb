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

## Köra lokalt

1. Klona repot och öppna `Nordpunkt.slnx` i Visual Studio.
2. Skapa filen `Nordpunkt/appsettings.Development.json` (den ligger inte i Git):

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

3. Starta projektet (F5). Första gången visas Umbracos installationssida: skapa ett eget adminkonto och välj **SQLite**.
4. Backoffice finns på `/umbraco`.

Lägg inte in `ConnectionStrings` för hand före installationen. Installationen skriver dem själv till `appsettings.Local.json`.

Databasen, loggar och `appsettings.Local.json` ligger inte i Git. Var och en har sin egen lokala databas och sitt eget adminkonto. Checka aldrig in lösenord.

## Arbetssätt i Git

- `main` är skyddad. Ingen pushar direkt dit.
- En branch per Jira-ärende: `SCRUM-<nr>-<kort-namn>`, t.ex. `SCRUM-30-artikel`.
- Commit-meddelanden börjar med ärendet: `SCRUM-30 Lägg till dokumenttyp Artikel`.
- Pull request mot `main`, minst **en annan person** godkänner innan merge.

## Definition of Done

Alla acceptanskriterier uppfyllda · fungerar på mobil och dator · fungerar med tangentbord (synligt fokus) · granskad av någon annan och mergad till `main` · ärendet flyttat till Done i Jira.
