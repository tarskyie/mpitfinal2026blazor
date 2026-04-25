using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace mpitfinal2026blazor.Services
{
    public class ZhgutLlmService
    {
        private readonly HttpClient _httpClient;
        private readonly StorageService _storageService;
        private readonly string _backendBaseUrl = "https://w29dq7t4-8000.euw.devtunnels.ms";
        private readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        private readonly JsonSerializerOptions _jsonSerializerOptionsSnakeCase = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower };

        public ZhgutLlmService(HttpClient httpClient, StorageService storageService)
        {
            _httpClient = httpClient;
            _storageService = storageService;
        }

        public async Task<List<PreviewGradeResponse>?> GetPreviewGradesAsync(int taskId, string token)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, $"{_backendBaseUrl}/api/tasks/{taskId}/preview_grades/");
            requestMessage.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.SendAsync(requestMessage);

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<PreviewGradeResponse>>(jsonResponse, _jsonSerializerOptionsSnakeCase);
            }

            return null;
        }

        public async Task<ChatCompletionResponse?> GetChatCompletionAsync(ChatCompletionRequest request, string? apiKey = null, string? baseUrl = null, bool useBackend = false)
        {
            var url = useBackend ? $"{_backendBaseUrl}/api/llm/chat/completions/" : $"{RemoveTrailingSlash(baseUrl)}/v1/chat/completions";
            return await SendLlmRequestAsync<ChatCompletionResponse>(url, request, apiKey, useBackend);
        }

        public async Task<TextCompletionResponse?> GetTextCompletionAsync(TextCompletionRequest request, string? apiKey = null, string? baseUrl = null, bool useBackend = false)
        {
            var url = useBackend ? $"{_backendBaseUrl}/api/llm/completions/" : $"{RemoveTrailingSlash(baseUrl)}/v1/completions";
            return await SendLlmRequestAsync<TextCompletionResponse>(url, request, apiKey, useBackend);
        }

        private async Task<T?> SendLlmRequestAsync<T>(string url, object request, string? apiKey, bool useBackend) where T : class
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, url);
            
            if (useBackend)
            {
                requestMessage.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);
            }
            else if (!string.IsNullOrEmpty(apiKey))
            {
                requestMessage.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);
            }

            var jsonRequest = JsonSerializer.Serialize(request, _jsonSerializerOptions);
            requestMessage.Content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(requestMessage);

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<T>(jsonResponse, _jsonSerializerOptions);
            }

            return null;
        }
        
        private string RemoveTrailingSlash(string? path)
        {
            if (string.IsNullOrEmpty(path)) return string.Empty;
            if (path.EndsWith("/"))
            {
                return path.Remove(path.Length - 1);
            }
            return path;
        }
    }

    public class ChatMessage
    {
        public string Role { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;

        public ChatMessage() { }
        public ChatMessage(string role, string content)
        {
            Role = role;
            Content = content;
        }
    }

    public class ChatCompletionRequest
    {
        public string Model { get; set; } = string.Empty;
        public List<ChatMessage> Messages { get; set; } = new();
        public double Temperature { get; set; } = 0.7;
    }

    public class ChatCompletionResponse
    {
        public string Id { get; set; } = string.Empty;
        public string Object { get; set; } = string.Empty;
        public long Created { get; set; }
        public string Model { get; set; } = string.Empty;
        public List<ChatChoice> Choices { get; set; } = new();
    }

    public class ChatChoice
    {
        public int Index { get; set; }
        public ChatMessage Message { get; set; } = new();
    }


    public class TextCompletionRequest
    {
        public string Model { get; set; } = string.Empty;
        public string Prompt { get; set; } = string.Empty;
        public int MaxTokens { get; set; } = 150;
        public double Temperature { get; set; } = 0.7;
    }

    public class TextCompletionResponse
    {
        public string Id { get; set; } = string.Empty;
        public string Object { get; set; } = string.Empty;
        public long Created { get; set; }
        public string Model { get; set; } = string.Empty;
        public List<TextChoice> Choices { get; set; } = new();
    }

    public class TextChoice
    {
        public string Text { get; set; } = string.Empty;
        public int Index { get; set; }
    }

    public class PreviewGradeRequest
    {

    }

    public class PreviewGradeResponse
    {
        public int SolutionId { get; set; }
        public int StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string PreviewGrade { get; set; } = string.Empty;
    }
}
