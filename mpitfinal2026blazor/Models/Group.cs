namespace mpitfinal2026blazor.Models
{
    public class Group
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Teacher { get; set; }
        public List<int> Students { get; set; } = new();
        public string InviteCode { get; set; } = string.Empty;
    }
}
