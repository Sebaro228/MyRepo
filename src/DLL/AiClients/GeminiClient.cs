using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
namespace DLL.AiClients
{
    public class GeminiClient : IAiClient
    {
        private readonly HttpClient _httpClient;
        public GeminiClient(HttpClient httpClient) 
        {
            _httpClient = httpClient;
        }
        public async Task<Request> GenerateTextAsync(Request request, string apiKey)
        {
            try
            {
                var requestBody = new
                {
                    contents = new[]
                    {
                        new
                        {
                            parts = new[]
                            {
                                new { text = request.PromptText }
                            }
                        }
                    },
                    generationConfig = new
                    {
                        temperature = 0.7
                    }
                };
                string jsonPayload = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
                var req = new HttpRequestMessage(HttpMethod.Post, "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent");
                req.Content = content;
                req.Headers.Add("x-goog-api-key", apiKey);
                var response = await _httpClient.SendAsync(req);
                response.EnsureSuccessStatusCode();
                string responseJson = await response.Content.ReadAsStringAsync();
                using JsonDocument doc = JsonDocument.Parse(responseJson);
                string resultText = doc.RootElement
                    .GetProperty("candidates")[0]
                    .GetProperty("content")
                    .GetProperty("parts")[0]
                    .GetProperty("text")
                    .GetString() ?? "";
                int tokens = 0;
                if (doc.RootElement.TryGetProperty("usageMetadata", out JsonElement usageElement))
                {
                    if (usageElement.TryGetProperty("totalTokenCount", out JsonElement totalTokensElement))
                    {
                        tokens = totalTokensElement.GetInt32();
                    }
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