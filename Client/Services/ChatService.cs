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
        public async Task<List<ChatGroupDto>> GetGroupsAsync()
        {
            HttpRequestMessage? request = new HttpRequestMessage(HttpMethod.Get, "api/Messages/JoinedGroups");
            request.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);
            HttpResponseMessage? response = await httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException($"Failed to get groups: {response.StatusCode}");

            List<ChatGroupDto>? chatGroups = await response.Content.ReadFromJsonAsync<List<ChatGroupDto>>();
            if (chatGroups == null)
                throw new InvalidOperationException("Response did not contain valid chat groups.");
            return chatGroups;
        }

        // --- Get messages for a specific group ---
        public async Task<List<MessageDto>> GetMessagesAsync(int groupId)
        {
            HttpRequestMessage? request = new HttpRequestMessage(HttpMethod.Get, $"api/Messages/Group/{groupId}");
            request.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);
            HttpResponseMessage? response = await httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException($"Failed to get messages: {response.StatusCode}");

            List<MessageDto>? messages = await response.Content.ReadFromJsonAsync<List<MessageDto>>();
            if (messages == null)
                throw new InvalidOperationException("Response did not contain valid messages.");

            return messages;
        }

        // --- Send a message to a group ---
        public async Task<MessageDto> SendMessageAsync(int groupId, string content)
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
                string? error = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Failed to send message. Status: {response.StatusCode}, Error: {error}");
            }

            MessageDto? message = await response.Content.ReadFromJsonAsync<MessageDto>();
            if (message == null)
                throw new InvalidOperationException("Response did not contain a valid message.");

            return message;
        }
    }
}
