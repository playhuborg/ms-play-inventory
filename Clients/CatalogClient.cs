using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Play.Inventory.Clients
{

    public class CatalogClient
    {
        private HttpClient _httpClient;

        public CatalogClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<CatalogItemDto>> GetCatalogItemsAsync()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<CatalogItemDto>>("/catalog");
        }
    }
}