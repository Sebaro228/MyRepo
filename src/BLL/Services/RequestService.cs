using DLL.Repositories;
using Domain.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace BLL.Services
{
    public class RequestService
    {
        private readonly IConfiguration _config;
        RequestRepository _requestRepository;
        public RequestService(RequestRepository requestRepository, IConfiguration config)
        {
            _requestRepository = requestRepository;
            _config = config;
        }
        public async Task<Request> ProcessRequestAsync(Request request)
        {
            string apiKey;
            if (request.Provider == AiProvider.Claude) apiKey = _config["ApiKeys:Claude"];
            else if (request.Provider == AiProvider.Gemini) apiKey = _config["ApiKeys:Gemini"];
            else if (request.Provider == AiProvider.Groq) apiKey = _config["ApiKeys:Groq"];
            else apiKey = _config["ApiKeys:OpenAI"];
            return await _requestRepository.AddAsync(request, apiKey);
        }
    }
}