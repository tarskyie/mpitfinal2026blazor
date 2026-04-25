using System;

namespace mpitfinal2026blazor.Models
{
    public class TaskModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Group { get; set; }
        [System.Text.Json.Serialization.JsonPropertyName("expiration_date")]
        public DateTime ExpirationDate { get; set; }
        public int CreatedBy { get; set; }
    }
}
