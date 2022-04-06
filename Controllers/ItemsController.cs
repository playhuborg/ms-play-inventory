using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Play.Inventory;
using Play.Inventory.Services;

namespace Play.Infra.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ItemsController : ControllerBase
    {
        private const string AdminRole = "Admin";
        private readonly ILogger<ItemsController> _logger;

        private readonly IInventoryService inventoryService;

        public ItemsController(ILogger<ItemsController> logger, IInventoryService inventoryService)
        {
            _logger = logger;
            this.inventoryService = inventoryService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<InventoryItemDto>>> GetAsync(Guid userId)
        {
            var currentUserIdString = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            if (currentUserIdString == null)
            {
                return Forbid("Cannot find user id");
            }
            var currentUserId = Guid.Parse(currentUserIdString);

            if (currentUserId != userId && !User.IsInRole(AdminRole))
            {
                return Forbid();
            }

            var inventoryItems = await inventoryService.GetInventoryByuser(userId);

            return Ok(inventoryItems);
        }

        [HttpPost]
        [Authorize(Roles = AdminRole)]
        public async Task<ActionResult> CreateAsync(GrantItemDto grantItemRequest)
        {
            if (grantItemRequest.UserId == Guid.Empty)
            {
                return BadRequest();
            }

            await inventoryService.CreateInventory(grantItemRequest);
            return Ok("Added succesfully");
        }
    }
}
