using Webhooks.Api.Models;

namespace Webhooks.Api.Repositories
{
    internal sealed class InMemoryWebhookSubscriptionRepository
    {
        private readonly List<WebhookSubscription> _subscriptions = new();

        public void Add(WebhookSubscription subscription)
        {
            _subscriptions.Add(subscription);
        }

        public IReadOnlyList<WebhookSubscription> GetByEventType(string eventType)
        {
            return _subscriptions.Where(x => x.EventType == eventType).ToList().AsReadOnly();
        }
    }
}
