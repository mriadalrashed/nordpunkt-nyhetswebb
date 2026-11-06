using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;

namespace Nordpunkt.Notifications;

// Stoppar sparning av en artikel som har en huvudbild men ingen alttext (SCRUM-35).
public class AlttextKravHandler : INotificationHandler<ContentSavingNotification>
{
    public void Handle(ContentSavingNotification notification)
    {
        foreach (var innehall in notification.SavedEntities)
        {
            if (innehall.ContentType.Alias != "artikel")
            {
                continue;
            }

            var harBild = !string.IsNullOrWhiteSpace(innehall.GetValue<string>("huvudbild"));
            var harAlttext = !string.IsNullOrWhiteSpace(innehall.GetValue<string>("alttext"));

            if (harBild && !harAlttext)
            {
                notification.CancelOperation(new EventMessage(
                    "Alttext saknas",
                    "Artikeln har en huvudbild men ingen alttext. Beskriv vad bilden visar i fältet Alttext, så att även den som inte ser bilden förstår den.",
                    EventMessageType.Error));
            }
        }
    }
}

// Registrerar handlern när Umbraco startar.
public class AlttextKravComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        builder.AddNotificationHandler<ContentSavingNotification, AlttextKravHandler>();
    }
}