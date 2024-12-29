﻿using Webhooks.Api.Models;

namespace Webhooks.Api.Repositories
{
    internal sealed class InMemoryOrderRepository
    {
        private readonly List<Order> _orders = new();

        public void Add(Order order)
        {
            _orders.Add(order);
        }

        public IReadOnlyList<Order> GetAll()
        {
            return _orders.AsReadOnly(); 
        }

    }
}
