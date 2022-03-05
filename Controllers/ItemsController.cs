using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Play.Common;
using Play.Inventory;
using Play.Inventory.Clients;
using Play.Inventory.Entities;

namespace Play.Infra.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class ItemsController : ControllerBase
    {
        private readonly ILogger<ItemsController> _logger;

        private readonly CatalogClient _catalogClient;

        IRepository<InventoryItem> _inventoryRepository;

        private readonly IRepository<CatalogItem> _catalogRepository;

        public ItemsController(ILogger<ItemsController> logger, IRepository<InventoryItem> inventoryRepository, CatalogClient catalogClient, IRepository<CatalogItem> catalogRepository)
        {
            _logger = logger;
            _inventoryRepository = inventoryRepository;
            _catalogClient = catalogClient;
            _catalogRepository = catalogRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<InventoryItemDto>>> GetAsync(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                return BadRequest();
            }

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

            return Ok(inventoryItems);
        }

        [HttpPost]
        public async Task<ActionResult> CreateAsync(GrantItemDto grantItemRequest)
        {
            if (grantItemRequest.UserId == null || grantItemRequest.UserId == null)
            {
                return BadRequest();
            }

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
            return Ok("Added succesfully");
        }
    }
}
