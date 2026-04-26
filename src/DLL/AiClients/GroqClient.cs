using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
namespace DLL.AiClients
{
    public class GroqClient : IAiClient
    {
        private readonly HttpClient _httpClient;
        public GroqClient(HttpClient httpClient) 
        {
            _httpClient = httpClient;
        }
        public async Task<Request> GenerateTextAsync(Request request, string apiKey)
        {
            try
            {
                var requestBody = new
                {
                    model = "openai/gpt-oss-120b",
                    messages = new[]
                    {
                        new { role = "user", content = request.PromptText }
                    },
                    temperature = 0.7
                };
                string jsonPayload = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
                var req = new HttpRequestMessage(HttpMethod.Post, "https://api.groq.com/openai/v1/chat/completions");
                req.Content = content;
                req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
                var response = await _httpClient.SendAsync(req);
                response.EnsureSuccessStatusCode();
                string responseJson = await response.Content.ReadAsStringAsync();
                using JsonDocument doc = JsonDocument.Parse(responseJson);
                string resultText = doc.RootElement
                    .GetProperty("choices")[0]
                    .GetProperty("message")
                    .GetProperty("content")
                    .GetString() ?? "";
                int tokens = 0;
                if (doc.RootElement.TryGetProperty("usage", out JsonElement usageElement))
                {
                    tokens = usageElement.GetProperty("total_tokens").GetInt32();
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