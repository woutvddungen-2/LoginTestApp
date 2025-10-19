using Client.Models;
using Microsoft.AspNetCore.Components.WebAssembly.Http;
using Shared.Models;
using System.Net.Http.Json;

namespace Client.Services
{
    public class ProductService
    {
        private readonly HttpClient _httpClient;

        public ProductService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<Product>?> GetProductsAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<Product>>("api/Products");
        }

        public async Task<List<ProductDto>?> GetAllProductsAsync()
        {
            HttpRequestMessage? request = new HttpRequestMessage(HttpMethod.Get, "api/Product/GetAll");
            request.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);
            HttpResponseMessage? response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Failed: {response.StatusCode}");
                return null;
            }
            var products = new List<ProductDto>();
            products = await response.Content.ReadFromJsonAsync<List<ProductDto>>();
            return products;
        }
    }
}
