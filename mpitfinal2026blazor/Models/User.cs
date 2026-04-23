using System.Text.Json.Serialization;

namespace mpitfinal2026blazor.Models
{
    public class User
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;

        [JsonPropertyName("username")]
        public string Username { get; set; } = string.Empty;

        [JsonPropertyName("first_name")]
        public string FirstName { get; set; } = string.Empty;

        [JsonPropertyName("last_name")]
        public string LastName { get; set; } = string.Empty;

        [JsonPropertyName("user_type")]
        public string UserType { get; set; } = string.Empty;

        public string DefaultAi { get; set; } = "DefaultAI";
    }
}
