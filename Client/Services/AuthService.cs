using Microsoft.AspNetCore.Components.WebAssembly.Http;
using Shared.Models;
using System.Net.Http.Json;
using static System.Net.WebRequestMethods;

namespace Client.Services
{
    public class AuthService
    {
        private readonly HttpClient httpClient;

        public AuthService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<bool> LoginAsync(string username, string password)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "api/User/login")
            {
                Content = JsonContent.Create(new LoginDto { Username = username, Password = password })
            };
            request.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);

            HttpResponseMessage? response = await httpClient.SendAsync(request);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> LogoutAsync()
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "api/User/logout");
            request.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);

            HttpResponseMessage? response = await httpClient.SendAsync(request);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> IsLoggedInAsync()
        {
            try
            {
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "api/User/verify");
                request.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);

                HttpResponseMessage? response = await httpClient.SendAsync(request);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Login status check failed: {ex.Message}");
                return false;
            }
        }

        public async Task<UserDto?> GetCurrentUserAsync()
        {
            try
            {
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "api/User/verify");
                request.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);
                HttpResponseMessage? response = await httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                    return null;

                return await response.Content.ReadFromJsonAsync<UserDto>();
            }
            catch
            {
                return null;
            }
        }
    }
}
