using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace mpitfinal2026blazor.Models
{
    public class TaskModel
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("group")]
        public int GroupId { get; set; }

        [JsonPropertyName("expiration_date")]
        public DateTime ExpirationDate { get; set; }

        [JsonPropertyName("created_by")]
        public int CreatedBy { get; set; }
    }

    public class SolutionModel
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("task")]
        public int TaskId { get; set; }

        [JsonPropertyName("student")]
        public int StudentId { get; set; }

        [JsonPropertyName("text")]
        public string Text { get; set; } = string.Empty;

        [JsonPropertyName("grade")]
        public int? Grade { get; set; }

        [JsonPropertyName("submitted_at")]
        public DateTime SubmittedAt { get; set; }
    }
}
