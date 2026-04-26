using BLL.Services;
using Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
namespace CommandProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AiRequestController(RequestService requestService) : ControllerBase
    {
        [HttpGet("Request/{prompt}/{provider}")]
        public async Task<IActionResult> Request(string prompt, string provider) 
        {
            Request request = new Request
            {
                Id = Guid.NewGuid().ToString(),
                PromptText = prompt,
                CreatedAt = DateTime.Now
            };
            if (provider .ToLower() == "claude") request.Provider = AiProvider.Claude;
            else if (provider.ToLower() == "openai") request.Provider = AiProvider.OpenAI;
            else if (provider.ToLower() == "tts") request.Provider = AiProvider.TTS;
            else return BadRequest("Invalid provider. Supported providers are 'Claude', 'OpenAI', and 'TTS'.");
            var result = await requestService.ProcessRequestAsync(request);
            if (!result.IsSuccessful) return BadRequest(result.ErrorMessage);
            return Ok(result);
        } 
    }
}