using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Play.Inventory.Services
{
    public interface IInventoryService
    {
        Task CreateInventory(GrantItemDto grantItemRequest);
        Task<IEnumerable<InventoryItemDto>> GetInventoryByuser(Guid userId);
    }
}