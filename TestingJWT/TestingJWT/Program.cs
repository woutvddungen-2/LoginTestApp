using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

class Program
{
    static async Task Main()
    {
        var baseUrl = "https://localhost:5001"; // your API base URL
        using var client = new HttpClient();

        // -------------------- 1️ Login --------------------
        var loginData = new
        {
            username = "alice",
            password = "password123"
        };

        var loginJson = new StringContent(
            JsonSerializer.Serialize(loginData),
            Encoding.UTF8,
            "application/json");

        var loginResponse = await client.PostAsync($"{baseUrl}/api/User/login", loginJson);
        if (!loginResponse.IsSuccessStatusCode)
        {
            Console.WriteLine("Login failed: " + loginResponse.StatusCode);
            return;
        }

        var loginContent = await loginResponse.Content.ReadAsStringAsync();
        Console.WriteLine(loginContent);
        var loginResult = JsonSerializer.Deserialize<LoginResponse>(loginContent);

        Console.WriteLine("Token received:");
        Console.WriteLine(loginResult);

        // -------------------- 2️ Verify token locally --------------------
        if (!TryParseJwt(loginResult.token, out var jwtToken))
        {
            Console.WriteLine("Token is malformed or invalid!");
            return;
        }

        Console.WriteLine("Token is valid locally. Claims:");
        foreach (var claim in jwtToken.Claims)
        {
            Console.WriteLine($" - {claim.Type}: {claim.Value}");
        }

        // -------------------- 3️ Call protected endpoint --------------------
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", loginResult.token);

        var productsResponse = await client.GetAsync($"{baseUrl}/api/User/products");
        var productsContent = await productsResponse.Content.ReadAsStringAsync();

        Console.WriteLine("Products response:");
        Console.WriteLine(productsContent);
    }

    // -------------------- Helper: parse JWT --------------------
    static bool TryParseJwt(string token, out JwtSecurityToken? jwtToken)
    {
        jwtToken = null;
        try
        {
            var handler = new JwtSecurityTokenHandler();
            jwtToken = handler.ReadJwtToken(token); // parses token without validation
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception while parsing token: " + ex.Message);
            return false;
        }
    }

    // -------------------- Helper class --------------------
    class LoginResponse
    {
        public string token { get; set; } = string.Empty;
    }
}
