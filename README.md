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
- **Efter varje `git pull`:** starta projektet och gå direkt till **Settings → uSync → Everything**. Välj pilen vid **Import** och sedan **Import Force**. Spara inget och klicka inte på **Export** innan dess.
- **Kontrollera sedan:** kör `git --no-pager diff --ignore-cr-at-eol --stat`. Vid start skriver uSync den egna databasen till filerna (`ExportAtStartup`), så en gammal databas kan skriva över det som kom med `git pull`. Listas en `.config`-fil, kör `git restore` på den medan projektet är igång och gör **Import Force** igen.

Ändras en gemensam inställning görs det i `dummy.appsettings.json`, och alla uppdaterar sin egen `appsettings.json` för hand.

**Bra att veta**
- En ändring i `.cshtml` kräver att projektet stoppas och startas igen. För CSS och JavaScript räcker Ctrl+F5 i webbläsaren.
- Filerna i `umbraco/models` skrivs om vid varje start. Kör `git --no-pager diff --ignore-cr-at-eol --stat` innan du kör `git restore` på dem: en fil som listas med siffror har en riktig ändring och ska vara med i din commit.

## Innehållsmodell

| Dokumenttyp | Används till | Fält |
|---|---|---|
| `Startsida` | Roten, `/`. Visar de tio senaste artiklarna | Rubrik, Sidfotstext, Tipssida |
| `Nyheter` | Alla artiklar, tio per sida | Dölj i menyn (från `Navigation`) |
| `Artikel` | En nyhetsartikel under Nyheter | Rubrik, Ingress, Brödtext, Publiceringsdatum, Författare, Kategori, Huvudbild, Alttext, Bildtext |
| `Tipsa redaktionen` | Sidan med tipsformuläret | Rubrik, Ingress, Personuppgifter |
| `Tack för tips` | Bekräftelsen efter ett inskickat tips | Rubrik, Text |
| `Tipsmapp` | Mappen Tips i roten. Publiceras aldrig | – |
| `Tips` | Ett inskickat tips. Ingen mall, publiceras aldrig | Rubrik, Beskrivning, Namn, E-post, Mottaget |

Varje dokumenttyp som visas för besökare har en egen mall. Sidhuvud, sidfot och artikellistan är partial views (`Header`, `Footer`, `ArticleList`) som delas av mallarna.

**Varför så här**
- **Texter i Umbraco, inte i koden:** rubriker, sidfot, länken till tipssidan och texten om personuppgifter är fält, så att redaktören kan ändra dem utan en utvecklare.
- **Menyn byggs från innehållsträdet:** sidorna direkt under Startsida visas i menyn. En ny sida syns där utan kodändring.
- **Composition `Navigation`:** fältet "Dölj i menyn" (`umbracoNaviHide`) skapas en gång och läggs på de dokumenttyper som behöver det, i dag `Nyheter`. Ska fler sidtyper kunna döljas läggs compositionen till på dem, i stället för att samma fält skapas igen.
- **Datatypen `Kategori`:** en lista med fem fasta värden (Lokalt, Samhälle, Kultur, Sport, Opinion), så att redaktören inte kan stava fel eller hitta på en ny kategori.
- **Alttext på artikeln, inte på bilden i Media:** samma bild kan behöva olika beskrivning i olika artiklar. Alttext krävs när artikeln har en bild. Det kontrolleras i kod (`AlttextKravHandler`), eftersom fältet inte kan vara obligatoriskt för artiklar utan bild.
- **Tips som innehållsnoder:** tipsen sparas i Umbracos databas och skyddas av Umbracos inloggning och användargrupper. Ingen egen tabell eller egen inloggning behövs.

## Roller och behörigheter

Webbplatsen har tre roller med olika behörigheter.

| Roll | Användargrupp | Når i innehållet | Får göra | Får inte göra |
|---|---|---|---|---|
| Administratör | `Administrators` (inbyggd) | Allt | Allt, även inställningar och användare | – |
| Redaktör | `Redaktör` | Allt, även mappen Tips | Läsa, skapa, spara, publicera, avpublicera och radera | Inställningar och användare |
| Skribent | `Skribent` | Bara Nyheter | Läsa, skapa och spara | Publicera, avpublicera, radera och läsa tips |

**Varför så här**
- Skribenten skriver och redaktören granskar och publicerar. Då går ingen text ut utan att en ansvarig har godkänt den.
- Tips kan innehålla namn och e-post. Därför når bara Redaktör och Administratör mappen Tips.
- Redaktör och Skribent har inte tillgång till inställningar och användare, eftersom de inte behöver det i sitt arbete.
- Vi skapade egna grupper i stället för att ändra de inbyggda `Editors` och `Writers`, som har tillgång till hela innehållet och delen Library. De inbyggda grupperna används inte.

**Kontrollerat med testkonton:**
- Skribent: ser bara delarna Content och Media. I innehållsträdet syns Nyheter men inte Tips. Startsida visas nedtonad och går inte att redigera. Kan spara ett utkast, men knappen Save and publish är inaktiv.
- Redaktör: ser delarna Content och Media samt mappen Tips. Kunde publicera, avpublicera, flytta en artikel till papperskorgen och tömma papperskorgen.

**På en ny dator:** användargrupper och användare följer inte med via uSync och Git. De skapas under Users i varje databas. Lösenord skrivs aldrig in i det här repot.

## Personuppgifter i tipsformuläret

**Vad som sparas:** Rubrik och Beskrivning (obligatoriska), Namn och E-post (frivilliga) och när tipset kom in (Mottaget). Formuläret sparar inget annat, till exempel ingen IP-adress.

**Varför:** redaktionen ska kunna läsa tipset och, om besökaren har lämnat kontaktuppgifter, ställa följdfrågor. Namn och e-post är frivilliga eftersom ett tips går att använda utan dem.

**Vem kan läsa:** bara Redaktör och Administratör, efter inloggning i Umbraco. Tipsen sparas som opublicerade noder i mappen Tips och visas aldrig på webbplatsen. Skribent når inte mappen.

**Utanför Git:** repot är publikt. uSync exporterar därför aldrig tips (`ExcludeContentTypes: "tips"` i `appsettings.uSync.json`). Bara påhittade uppgifter används vid test, till exempel e-post på `example.com`.

**Godkännande:** besökaren måste kryssa i att hon eller han har läst texten om hur uppgifterna hanteras. Annars går tipset inte att skicka. Godkännandet sparas inte separat, eftersom ett tips inte kan finnas utan det.

**Inget löfte om anonymitet:** formuläret säger att redaktionen inte kan kontakta den som lämnar Namn och E-post tomma. Det lovar inte att tipset är anonymt.

**Spamskydd:** ett dolt fält (honeypot) och antiforgery-token. Ett stoppat försök sparas inte. Loggen får bara en rad om att ett tips stoppades, utan innehållet.

**Texten vid formuläret** är fältet Personuppgifter på sidan Tipsa redaktionen. Den ska alltid stämma med det här avsnittet.

## Radering och gallring av tips

Tips kan innehålla namn och e-post. De sparas inte längre än nödvändigt.

**När:** ett tips raderas senast 90 dagar efter att det kom in (fältet Mottaget). En gång i veckan går redaktören igenom listan och raderar alla tips som hinner bli 90 dagar före nästa genomgång.

**Hur (Redaktör eller Administratör):**
1. Gå till Content → Tips och öppna tipset.
2. Välj `...` → Trash och bekräfta. Tipset flyttas till Recycle Bin, och namn och e-post finns kvar där.
3. Gå till Recycle Bin, välj `...` vid tipset → Delete och bekräfta. Först nu är tipset borta helt.

**Regler:**
- Steg 3 får aldrig hoppas över.
- Mappen Tips publiceras aldrig. Så länge den är opublicerad går ett tips inte att publicera.
- Tips exporteras inte av uSync och hamnar därför inte i Git.

**Kontrollerat:** ett påhittat tips skickades från formuläret, lästes av Redaktör och raderades helt, medan ett annat tips fanns kvar. Försök att publicera ett tips stoppades med orsaken `parent is not published`.

## Arbetssätt i Git

- `dev` är arbetsgrenen och `main` är den stabila versionen. Båda är skyddade, ingen pushar direkt dit.
- En branch per Jira-ärende, skapad från senaste `dev`: `SCRUM-<nr>-<kort-namn>`, t.ex. `SCRUM-30-artikel`.
- Commit-meddelanden börjar med ärendet: `SCRUM-30 Lägg till dokumenttyp Artikel`.
- Pull request mot `dev`, minst **en annan person** godkänner innan merge.
- I slutet av varje sprint görs en pull request från `dev` till `main`.

## Definition of Done

Alla acceptanskriterier uppfyllda · fungerar på mobil och dator · fungerar med tangentbord (synligt fokus) · granskad av någon annan och mergad till `dev` · ärendet flyttat till Done i Jira.