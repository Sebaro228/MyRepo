using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
namespace Domain.Models
{
    public enum AiProvider { OpenAI, Claude, TTS }
    public class Request
    {
        public string Id { get; set; }
        public AiProvider Provider { get; set; }
        public string PromptText { get; set; }
        public string? ResponceText { get; set; }
        public int TokenUsed { get; set; }
        public bool IsSuccessful { get; set; }
        public string? ErrorMessage { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}