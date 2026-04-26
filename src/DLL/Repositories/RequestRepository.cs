using DLL.AiClients;
using DLL.Context;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLL.Repositories
{
    public class RequestRepository : IRequestRepository
    {
        RequestDbContext _dbContext;
        ClaudeClient _claudeClient;
        OpenAiClient _openAiClient;
        GeminiClient _geminiClient;
        TtsClient _ttsClient; 
        GroqClient _groqClient;
        public RequestRepository(RequestDbContext dbContext, OpenAiClient openAiClient, TtsClient ttsClient, ClaudeClient claudeClient, GeminiClient geminiClient, GroqClient groqClient)
        {
            _dbContext = dbContext;
            _openAiClient = openAiClient;
            _ttsClient = ttsClient;
            _claudeClient = claudeClient;
            _geminiClient = geminiClient;
            _groqClient = groqClient;
        }
        public async Task<Request> AddAsync(Request request, string apiKey)
        {
            if (request.Provider == AiProvider.OpenAI) request = await _openAiClient.GenerateTextAsync(request, apiKey);
            if (request.Provider == AiProvider.Claude) request = await _claudeClient.GenerateTextAsync(request, apiKey);
            if (request.Provider == AiProvider.TTS) request = await _ttsClient.GenerateTextAsync(request, apiKey);
            if (request.Provider == AiProvider.Gemini) request = await _geminiClient.GenerateTextAsync(request, apiKey);
            if (request.Provider == AiProvider.Groq) request = await _groqClient.GenerateTextAsync(request, apiKey); 
            await _dbContext.RequestHistory.AddAsync(request);
            await _dbContext.SaveChangesAsync();
            return request;
        }
    }
}
