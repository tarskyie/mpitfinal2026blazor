using System;
using System.Text.Json.Serialization;

namespace mpitfinal2026blazor.Models
{
    public class SolutionModel
    {
        public int Id { get; set; }
        [JsonPropertyName("task")]
        public int TaskId { get; set; }
        public int Student { get; set; }
        public string Text { get; set; } = string.Empty;
        public int? Grade { get; set; }
        [JsonPropertyName("submitted_at")]
        public DateTime SubmittedAt { get; set; }
    }
}
