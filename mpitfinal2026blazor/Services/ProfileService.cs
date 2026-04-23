using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Net.Http.Json;
using System.Security.AccessControl;

namespace mpitfinal2026blazor.Services
{
    public class ProfileService
    {
        private const string baseUrl = "http://127.0.0.1:8000";
        private readonly HttpClient _httpClient;

        public ProfileService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> LogIn(string username, string password)
        {
            string? refresh = string.Empty;

            var userData = new { username, password };
            try
            {
                var requestMessage = new HttpRequestMessage(HttpMethod.Post, new Uri($"{baseUrl}/auth/jwt/create/"));
                var jsonRequest = JsonSerializer.Serialize(userData, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower });
                requestMessage.Content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
                var response = await _httpClient.SendAsync(requestMessage);

                if (!response.IsSuccessStatusCode) return "nokey";

                var authJson = await response.Content.ReadFromJsonAsync<JsonElement>();
                refresh = authJson.GetProperty("refresh").GetString();
                if (refresh == null) return "nokey";

            }
            catch (Exception ex)
            {
                return $"nokey {ex.Message}";
            }
            if (refresh == null)
            {
                return "nokey refresh is null";
            }
            return refresh;
        }

        public async Task<string?> Register(string email, string username, string password, string first_name, string last_name, string user_type)
        {
            string output = "success";

            var registerData = new { email, username, password, first_name, last_name, user_type };

            try
            {
                var requestMessage = new HttpRequestMessage(HttpMethod.Post, new Uri($"{baseUrl}/auth/users/"));
                var jsonRequest = JsonSerializer.Serialize(registerData, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower });
                requestMessage.Content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
                var response = await _httpClient.SendAsync(requestMessage);

                if (response.StatusCode == System.Net.HttpStatusCode.Created)
                {
                    return output;
                }
                return response.Content.ToString();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public async Task<string> GetAccessToken(string refreshToken)
        {
            var refreshData = new { refresh = refreshToken };

            try
            {
                var authRes = await _httpClient.PostAsJsonAsync($"{baseUrl}/auth/jwt/refresh/", refreshData);
                if (!authRes.IsSuccessStatusCode) return string.Empty;

                var authJson = await authRes.Content.ReadFromJsonAsync<JsonElement>();
                string? access = authJson.GetProperty("access").GetString();

                if (access == null || access == string.Empty) return string.Empty;

                return access;

            }
            catch
            {
                return string.Empty;
            }
        }

        public async Task<JsonElement?> GetYourself(string accessToken)
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, $"{baseUrl}/auth/users/me/");

            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            try
            {
                using var authRes = await _httpClient.SendAsync(request);

                if (!authRes.IsSuccessStatusCode) return null;

                var authJson = await authRes.Content.ReadFromJsonAsync<JsonElement>();

                return authJson;
            }
            catch
            {
                return null;
            }
        }

    }
}
