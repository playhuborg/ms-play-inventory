using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Play.Common;
using Play.Common.MassTransit;
using Play.Common.MongoDb;
using Play.Inventory.Clients;
using Play.Inventory.Entities;

namespace Play.Inventory
{
    public static class Extensions
    {
        public static InventoryItemDto AsDto(this InventoryItem item, string name, string description)
        {
            return new InventoryItemDto(item.CatalogItemId, name, description, item.Quantity, item.AcquiredDate);
        }

        public static IServiceCollection RegisterClients(this IServiceCollection services, IConfiguration configuration)
        {
            var urlSettings = configuration.GetSection(nameof(ClientUrls)).Get<ClientUrls>();
            services.AddHttpClientWithCircuitBerakerConfig<CatalogClient>(new Uri(urlSettings.CatagoryService));

            return services;
        }

        public static IServiceCollection RegisterRepositories(this IServiceCollection services)
        {
            services.AddMongo()
                            .AddRepository<InventoryItem>("inventoryItems")
                            .AddRepository<CatalogItem>("CatalogItems");

            return services;
        }
    }
}