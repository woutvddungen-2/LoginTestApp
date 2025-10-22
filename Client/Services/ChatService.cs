using Microsoft.AspNetCore.Components.WebAssembly.Http;
using Shared.Models;
using System.Net.Http;
using System.Net.Http.Json;

namespace Client.Services
{
    public class ChatService
    {
        private readonly HttpClient httpClient;

        public ChatService(HttpClient http)
        {
            httpClient = http;
        }

        // --- Get all groups the user is a member of ---
        public async Task<List<ChatGroupDto>?> GetGroupsAsync()
        {
            HttpRequestMessage? request = new HttpRequestMessage(HttpMethod.Get, "api/Messages/JoinedGroups");
            request.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);
            HttpResponseMessage? response = await httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Failed: {response.StatusCode}");
                return null;
            }
            var chatGroups = new List<ChatGroupDto>();
            chatGroups = await response.Content.ReadFromJsonAsync<List<ChatGroupDto>>();
            return chatGroups;
        }

        // --- Get messages for a specific group ---
        public async Task<List<Message>?> GetMessagesAsync(int groupId)
        {
            HttpRequestMessage? request = new HttpRequestMessage(HttpMethod.Get, $"api/Messages/Group/{groupId}");
            request.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);
            HttpResponseMessage? response = await httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Failed: {response.StatusCode}");
                return null;
            }
            var messages = new List<Message>();
            messages = await response.Content.ReadFromJsonAsync<List<Message>>();
            return messages;
            //return await httpClient.GetFromJsonAsync<List<Message>>($"api/Messages/Group/{groupId}")
            //       ?? new List<Message>();
        }

        // --- Send a message to a group ---
        public async Task<Message> SendMessageAsync(int groupId, string content)
        {
            MessageDto? dto = new MessageDto { GroupId = groupId, Content = content };

            HttpRequestMessage? request = new HttpRequestMessage(HttpMethod.Post, $"api/Messages/Send")
            { 
                Content = JsonContent.Create(dto)
            };
            request.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);
            HttpResponseMessage? response = await httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                // Optionally, read the error message from the response
                string? error = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Failed to send message. Status: {response.StatusCode}, Error: {error}");
            }

            Message? message = await response.Content.ReadFromJsonAsync<Message>();
            if (message == null)
                throw new InvalidOperationException("Response did not contain a valid message.");

            return message;
        }
    }

    // --- Minimal ChatGroup model for client ---

    public class Message
    {
        public int SenderId { get; set; }
        public int GroupId { get; set; }
        public string Content { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }
}
