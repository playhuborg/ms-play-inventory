using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Play.Common;
using Play.Inventory.Entities;

namespace Play.Inventory.Services
{
    public class InventoryService : IInventoryService
    {
        IRepository<InventoryItem> _inventoryRepository;

        private readonly IRepository<CatalogItem> _catalogRepository;

        public InventoryService(IRepository<CatalogItem> catalogRepository, IRepository<InventoryItem> inventoryRepository)
        {
            _catalogRepository = catalogRepository;
            _inventoryRepository = inventoryRepository;
        }

        public async Task<IEnumerable<InventoryItemDto>> GetInventoryByuser(Guid userId)
        {
            var catalogItems = await _catalogRepository.GetAllAsync();
            var testing = await _inventoryRepository.GetAllAsync(item => item.UserId == userId);
            var inventoryItems = (await _inventoryRepository.GetAllAsync(item => item.UserId == userId))
                                    .Select(item =>
                                    {
                                        var catalogItem = catalogItems.Where(catalog => item.CatalogItemId == catalog.Id).FirstOrDefault();
                                        if (catalogItem != null)
                                            return item.AsDto(catalogItem.Name, catalogItem.Description);
                                        return item.AsDto(null, null);
                                    });
            return inventoryItems;
        }

        public async Task CreateInventory(GrantItemDto grantItemRequest)
        {
            var inventoryItem = await _inventoryRepository.GetAsync(item => item.CatalogItemId == grantItemRequest.CatalogItemId && item.UserId == grantItemRequest.UserId);
            if (inventoryItem == null)
            {
                inventoryItem = new InventoryItem
                {
                    CatalogItemId = grantItemRequest.CatalogItemId,
                    Quantity = grantItemRequest.Quantity,
                    UserId = grantItemRequest.UserId,
                    AcquiredDate = DateTimeOffset.Now,
                    Id = Guid.NewGuid()
                };

                await _inventoryRepository.CreateAsync(inventoryItem);
            }
            else
            {
                inventoryItem.Quantity += inventoryItem.Quantity;
                await _inventoryRepository.UpdateAsync(inventoryItem);
            }
        }
    }
}