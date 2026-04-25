using System;

namespace mpitfinal2026blazor.Models
{
    public class SolutionModel
    {
        public int Id { get; set; }
        public int TaskId { get; set; }
        public User Student { get; set; } = new User();
        public string Text { get; set; } = string.Empty;
        public int? Grade { get; set; }
        public DateTime SubmittedAt { get; set; }
    }
}
