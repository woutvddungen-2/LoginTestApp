using System.Net.Http.Json;
using Microsoft.AspNetCore.Components.WebAssembly.Http;

namespace Client.Services
{
    public class AuthService
    {
        private readonly HttpClient _http;

        public AuthService(HttpClient http)
        {
            _http = http;
        }
        public async Task<bool> IsLoggedInAsync()
        {
            try
            {
                HttpRequestMessage? request = new HttpRequestMessage(HttpMethod.Get, "api/User/verify");
                request.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);

                HttpResponseMessage? response = await _http.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    LoginStatusResponse? result = await response.Content.ReadFromJsonAsync<LoginStatusResponse>();
                    return result?.loggedIn ?? false;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Login status check failed: {ex.Message}");
                return false;
            }
        }

        public class LoginStatusResponse
        {
            public bool loggedIn { get; set; }
        }
    }
}
