using Webhooks.Api.Models;
using Webhooks.Api.Repositories;
using Webhooks.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<InMemoryOrderRepository>();
builder.Services.AddSingleton<InMemoryWebhookSubscriptionRepository>();

//builder.Services.AddHttpClient();
builder.Services.AddHttpClient<WebhookDispatcher>();


var app = builder.Build();

if(app.Environment.IsDevelopment())
{
    app.UseSwagger(c => c.RouteTemplate = "webhooksapi/{documentName}.json");
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/webhooksapi/v1.json", "Webhooks.Api v1"));
}

app.UseHttpsRedirection();

app.MapPost("webhooks/subscription", (
    CreateWebhookRequest request,
    InMemoryWebhookSubscriptionRepository subscriptionRepository) =>
{
    var subscription = new WebhookSubscription(
            Guid.NewGuid(),
            request.EventType,
            request.WebhookUrl,
            DateTime.UtcNow
        );
    subscriptionRepository.Add(subscription);
    return Results.Ok(subscription);
});

app.MapPost("/orders", async(
    CreateOrderRequest request,
    InMemoryOrderRepository orderRepository,
    WebhookDispatcher webhookDispatcher ) =>
{
    var order = new Order(Guid.NewGuid(), request.CustomerName, request.Amount, DateTime.UtcNow);
    orderRepository.Add(order);
    await webhookDispatcher.DispatchWebhookAsync("order.created", order);
    return Results.Ok(order);
})
.WithTags("Orders");

app.MapGet("/orders", (
    InMemoryOrderRepository orderRepository) =>
{
    return Results.Ok(orderRepository.GetAll());  // Return all orders
})
.WithTags("Orders");

app.Run();
