using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
namespace DLL.AiClients
{
    public class ClaudeClient : IAiClient
    {
        private readonly HttpClient _httpClient;
        public ClaudeClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<Request> GenerateTextAsync(Request request, string apiKey)
        {
            try
            {
                var requestBody = new
                {
                    model = "claude-3-haiku-20240307",
                    max_tokens = 1024,
                    messages = new[]
                    {
                        new { role = "user", content = request.PromptText }
                    }
                };
                string jsonPayload = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
                var req = new HttpRequestMessage(HttpMethod.Post, "https://api.anthropic.com/v1/messages");
                req.Content = content;
                req.Headers.Add("x-api-key", apiKey);
                req.Headers.Add("anthropic-version", "2023-06-01");
                var response = await _httpClient.SendAsync(req);
                response.EnsureSuccessStatusCode();
                string responseJson = await response.Content.ReadAsStringAsync();
                using JsonDocument doc = JsonDocument.Parse(responseJson);
                string resultText = doc.RootElement
                    .GetProperty("content")[0]
                    .GetProperty("text")
                    .GetString() ?? "";
                int tokens = 0;
                if (doc.RootElement.TryGetProperty("usage", out JsonElement usageElement))
                {
                    int inputTokens = usageElement.TryGetProperty("input_tokens", out var inTokens) ? inTokens.GetInt32() : 0;
                    int outputTokens = usageElement.TryGetProperty("output_tokens", out var outTokens) ? outTokens.GetInt32() : 0;
                    tokens = inputTokens + outputTokens;
                }
                request.ResponseText = resultText;
                request.TokenUsed = tokens;
                request.IsSuccessful = true;
            }
            catch (Exception ex)
            {
                request.IsSuccessful = false;
                request.ErrorMessage = ex.Message;
            }
            return request;
        }
    }
}