namespace GenericDataStore.Models
{
    public class TablePage
    {
        public Guid TablePageId { get; set; }
        public string Name { get; set; }
        public Guid ObjectTypeId { get; set; }
        public string? Html { get; set; }
        public string? Css { get; set; }
        public Guid AppUserId { get; set; }

    }
}
