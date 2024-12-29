using System.ComponentModel.DataAnnotations;
using Webhooks.Api.Repositories;

namespace Webhooks.Api.Services
{
    internal sealed class WebhookDispatcher
    {
        private readonly HttpClient _httpClient;
        private readonly InMemoryWebhookSubscriptionRepository _webhookSubscriptionRepository;
        public WebhookDispatcher(HttpClient httpClient, 
            InMemoryWebhookSubscriptionRepository webhookSubscriptionRepository)
        {
            _httpClient = httpClient;
            _webhookSubscriptionRepository = webhookSubscriptionRepository;
        }

        public async Task DispatchWebhookAsync(string eventType, object payload)
        {
            var subscriptions = _webhookSubscriptionRepository.GetByEventType(eventType);
            foreach (var subscription in subscriptions)
            {
                var request = new
                {
                    Id = Guid.NewGuid(),
                    subscription.EventType,
                    SubscriptionId = subscription.Id,
                    TimestampAttribute = DateTime.UtcNow,
                    Data = payload
                };

                await _httpClient.PostAsJsonAsync(subscription.WebhookUrl, request);
            }
        }
    }
}
