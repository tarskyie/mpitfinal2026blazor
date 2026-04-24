using mpitfinal2026blazor.Models;
using System.Globalization;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.AccessControl;
using System.Text;
using System.Text.Json;

namespace mpitfinal2026blazor.Services
{
    public class ProfileService
    {
        private const string baseUrl = "https://w29dq7t4-8000.euw.devtunnels.ms";
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

        public async Task<string?> CreateGroup(string accessToken, string name)
        {
            var payload = new { name };
            using var request = new HttpRequestMessage(HttpMethod.Post, $"{baseUrl}/api/groups/");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            try
            {
                var jsonRequest = JsonSerializer.Serialize(payload, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower });
                request.Content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
                var response = await _httpClient.SendAsync(request);
                return await response.Content.ReadAsStringAsync();
            }
            catch { return null; }
        }

        public async Task<string?> GetGroups(string accessToken)
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, $"{baseUrl}/api/groups/");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
            try
            {
                var response = await _httpClient.SendAsync(request);
                return await response.Content.ReadAsStringAsync();
            }
            catch { return null; }
        }

        public async Task<string?> CreateTask(string accessToken, string title, int groupId, DateTime expirationDate)
        {
            var payload = new { title, group = groupId, expiration_date = expirationDate.ToString("yyyy-MM-ddTHH:mm:ss") };
            using var request = new HttpRequestMessage(HttpMethod.Post, $"{baseUrl}/api/tasks/");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
            try
            {
                var jsonRequest = JsonSerializer.Serialize(payload, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower });
                request.Content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
                var response = await _httpClient.SendAsync(request);
                return await response.Content.ReadAsStringAsync();
            }
            catch { return null; }
        }

        public async Task<string?> GetTasks(string accessToken, int groupId)
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, $"{baseUrl}/api/tasks/?group={groupId}");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
            try
            {
                var response = await _httpClient.SendAsync(request);
                return await response.Content.ReadAsStringAsync();
            }
            catch { return null; }
        }

        public async Task<string?> GetSolutionsForTask(string accessToken, int taskId)
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, $"{baseUrl}/api/solutions/?task={taskId}");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
            try
            {
                var response = await _httpClient.SendAsync(request);
                return await response.Content.ReadAsStringAsync();
            }
            catch { return null; }
        }

        public async Task<(bool Success, string Message)> JoinGroup(string accessToken, int groupId, string inviteCode)
        {
            var payload = new { invite_code = inviteCode };
            using var request = new HttpRequestMessage(HttpMethod.Post, $"{baseUrl}/api/groups/{groupId}/join/");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
            try
            {
                var jsonRequest = JsonSerializer.Serialize(payload, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower });
                request.Content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
                var response = await _httpClient.SendAsync(request);
                var body = await response.Content.ReadAsStringAsync();
                return (response.IsSuccessStatusCode, body);
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        public async Task<List<GroupTaskItem>> GetAllStudentGroupTasks(string accessToken)
        {
            var result = new List<GroupTaskItem>();

            var groupsJson = await GetGroups(accessToken);
            if (string.IsNullOrEmpty(groupsJson)) return result;

            List<JsonElement> groups;
            try { groups = JsonSerializer.Deserialize<List<JsonElement>>(groupsJson) ?? new(); }
            catch { return result; }

            foreach (var g in groups)
            {
                int groupId = g.GetProperty("id").GetInt32();
                string groupName = g.TryGetProperty("name", out var n) ? n.GetString() ?? "" : "";

                var tasksJson = await GetTasks(accessToken, groupId);
                if (string.IsNullOrEmpty(tasksJson)) continue;

                List<JsonElement> tasks;
                try { tasks = JsonSerializer.Deserialize<List<JsonElement>>(tasksJson) ?? new(); }
                catch { continue; }

                foreach (var t in tasks)
                {
                    if (t.GetProperty("group").GetInt32() != groupId)
                    {
                        continue;
                    }

                    int taskId = t.GetProperty("id").GetInt32();
                    string title = t.TryGetProperty("title", out var ti) ? ti.GetString() ?? "" : "";
                    DateTime? expDate = null;
                    if (t.TryGetProperty("expiration_date", out var ed) && ed.ValueKind != JsonValueKind.Null
                        && DateTime.TryParse(ed.GetString(), out var parsedDate))
                        expDate = parsedDate;

                    result.Add(new GroupTaskItem
                    {
                        TaskId = taskId,
                        Title = title,
                        GroupId = groupId,
                        GroupName = groupName,
                        ExpirationDate = expDate
                    });
                }
            }

            return result;
        }

        public async Task<(bool Success, string Message)> SubmitSolution(string accessToken, int taskId, string text)
        {
            var payload = new { task = taskId, text };
            using var request = new HttpRequestMessage(HttpMethod.Post, $"{baseUrl}/api/solutions/");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
            try
            {
                var jsonRequest = JsonSerializer.Serialize(payload, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower });
                request.Content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
                var response = await _httpClient.SendAsync(request);
                var body = await response.Content.ReadAsStringAsync();
                return (response.IsSuccessStatusCode, body);
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        public async Task<bool> GradeSolution(string accessToken, int solutionId, int grade)
        {
            var request = new HttpRequestMessage(HttpMethod.Patch, $"{baseUrl}/api/solutions/{solutionId}/");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            request.Content = JsonContent.Create(new { grade });

            var response = await _httpClient.SendAsync(request);
            return response.IsSuccessStatusCode;
        }

        public async Task<List<SolutionModel>> GetMySolutions(string accessToken) {
            List<SolutionModel> solutions = new List<SolutionModel>();

            return solutions;
        }
    }

    public class GroupTaskItem
    {
        public int TaskId { get; set; }
        public string Title { get; set; } = "";
        public int GroupId { get; set; }
        public string GroupName { get; set; } = "";
        public DateTime? ExpirationDate { get; set; }
    }
}