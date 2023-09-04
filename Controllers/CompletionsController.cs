using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace smidigAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompletionsController : ControllerBase
    {
        private static readonly HttpClient httpClient = new HttpClient();
        private readonly IConfiguration _configuration;

        public CompletionsController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ChatMessageRequest messageRequest)
        {
            try
            {
                var options = new
                {
                    model = "gpt-3.5-turbo",
                    messages = new[]
                    {
                        new { role = "user", content = messageRequest.Message }
                    },
                    max_tokens = 400
                };
                var json = JsonConvert.SerializeObject(options);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                httpClient.DefaultRequestHeaders.Clear();
                var API_KEY = _configuration["OpenAI:ApiKey"];
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {API_KEY}");
                var response = await httpClient.PostAsync("https://api.openai.com/v1/chat/completions", content);
                response.EnsureSuccessStatusCode();
                var data = await response.Content.ReadAsStringAsync();
                return Ok(data);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
                return StatusCode(500);
            }
        }
    }

    public class ChatMessageRequest
    {
        public string? Message { get; set; }
    }
}