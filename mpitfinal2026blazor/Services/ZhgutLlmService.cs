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
        private readonly string _backendBaseUrl = "http://localhost:8000";
        private readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

        public ZhgutLlmService(HttpClient httpClient, StorageService storageService)
        {
            _httpClient = httpClient;
            _storageService = storageService;
        }

        public async Task<List<PreviewGradeResponse>?> GetPreviewGradesAsync(int taskId)
        {
            var token = await _storageService.GetItemAsync<string>("authToken");
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, $"{_backendBaseUrl}/api/tasks/{taskId}/preview_grades/");
            requestMessage.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.SendAsync(requestMessage);

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<PreviewGradeResponse>>(jsonResponse, _jsonSerializerOptions);
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
                var token = await _storageService.GetItemAsync<string>("authToken");
                requestMessage.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
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
        [JsonPropertyName("role")]
        public string Role { get; set; } = string.Empty;

        [JsonPropertyName("content")]
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
        [JsonPropertyName("model")]
        public string Model { get; set; } = string.Empty;

        [JsonPropertyName("messages")]
        public List<ChatMessage> Messages { get; set; } = new();

        [JsonPropertyName("temperature")]
        public double Temperature { get; set; } = 0.7;
    }

    public class ChatCompletionResponse
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("object")]
        public string Object { get; set; } = string.Empty;

        [JsonPropertyName("created")]
        public long Created { get; set; }

        [JsonPropertyName("model")]
        public string Model { get; set; } = string.Empty;

        [JsonPropertyName("choices")]
        public List<ChatChoice> Choices { get; set; } = new();
    }

    public class ChatChoice
    {
        [JsonPropertyName("index")]
        public int Index { get; set; }

        [JsonPropertyName("message")]
        public ChatMessage Message { get; set; } = new();
    }


    public class TextCompletionRequest
    {
        [JsonPropertyName("model")]
        public string Model { get; set; } = string.Empty;

        [JsonPropertyName("prompt")]
        public string Prompt { get; set; } = string.Empty;

        [JsonPropertyName("max_tokens")]
        public int MaxTokens { get; set; } = 150;

        [JsonPropertyName("temperature")]
        public double Temperature { get; set; } = 0.7;
    }

    public class TextCompletionResponse
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("object")]
        public string Object { get; set; } = string.Empty;

        [JsonPropertyName("created")]
        public long Created { get; set; }

        [JsonPropertyName("model")]
        public string Model { get; set; } = string.Empty;

        [JsonPropertyName("choices")]
        public List<TextChoice> Choices { get; set; } = new();
    }

    public class TextChoice
    {
        [JsonPropertyName("text")]
        public string Text { get; set; } = string.Empty;

        [JsonPropertyName("index")]
        public int Index { get; set; }
    }

    public class PreviewGradeRequest
    {

    }

    public class PreviewGradeResponse
    {
        [JsonPropertyName("solution_id")]
        public int SolutionId { get; set; }

        [JsonPropertyName("student_id")]
        public int StudentId { get; set; }

        [JsonPropertyName("student_name")]
        public string StudentName { get; set; } = string.Empty;

        [JsonPropertyName("preview_grade")]
        public string PreviewGrade { get; set; } = string.Empty;
    }
}
