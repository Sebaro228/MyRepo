using Domain.Models;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
namespace DLL.AiClients
{
    public class TtsClient : IAiClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _audioFolder;
        public TtsClient(HttpClient httpClient, IWebHostEnvironment env)
        {
            _httpClient = httpClient;
            _audioFolder = Path.Combine(env.WebRootPath, "audio");
            if (!Directory.Exists(_audioFolder))
                Directory.CreateDirectory(_audioFolder);
        }
        public async Task<Request> GenerateTextAsync(Request request, string apiKey)
        {
            try
            {
                var requestBody = new
                {
                    model = "tts-1",
                    input = request.PromptText,
                    voice = "alloy",
                    response_format = "mp3"
                };
                string jsonPayload = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
                var req = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/audio/speech");
                req.Content = content;
                req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
                var response = await _httpClient.SendAsync(req);
                response.EnsureSuccessStatusCode();
                byte[] audioBytes = await response.Content.ReadAsByteArrayAsync();
                string fileName = $"{Guid.NewGuid()}.mp3";
                string filePath = Path.Combine(_audioFolder, fileName);
                await File.WriteAllBytesAsync(filePath, audioBytes);
                request.ResponseText = $"/audio/{fileName}";
                request.TokenUsed = request.PromptText.Length; 
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