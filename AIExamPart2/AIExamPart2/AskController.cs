using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AIExamPart2
{
    [ApiController]
    [Route("api/[controller]")]
    public class AskController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private const string OpenWebUiUrl = "http://localhost:3000/api/chat/completions";
        private const string BearerToken = "";

        public AskController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        [HttpPost("basic")]
        public async Task<IActionResult> Ask([FromBody] AskBasicRequest request)
        {
            var payload = new
            {
                model = request.Model,
                messages = new[]
                {
                    new { role = "user", content = request.Question }
                }
            };

            var json = JsonConvert.SerializeObject(payload);
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, OpenWebUiUrl)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
            httpRequest.Headers.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", BearerToken);

            var response = await _httpClient.SendAsync(httpRequest);
            var content = await response.Content.ReadAsStringAsync();

            return Content(content, "application/json");
        }

        [HttpPost("rag")]
        public async Task<IActionResult> AskWithRag([FromBody] AskWithRagRequest request)
        {
            var payload = new
            {
                model = request.Model,
                rag = new { name = request.RagId },
                messages = new[]
                {
                    new { role = "user", content = request.Question }
                }
            };

            var json = JsonConvert.SerializeObject(payload);
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, OpenWebUiUrl)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
            httpRequest.Headers.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", BearerToken);

            var response = await _httpClient.SendAsync(httpRequest);
            var content = await response.Content.ReadAsStringAsync();

            return Content(content, "application/json");
        }
    }

    public class AskBasicRequest
    {
        public string Question { get; set; }
        public string Model { get; set; }
    }

    public class AskWithRagRequest
    {
        public string Question { get; set; }
        public string Model { get; set; } // e.g., "deepseek-r1:1.5b"
        public string RagId { get; set; } // RAG ID or slug from OpenWebUI
    }
}