namespace GenericDataStore.Models
{
    public class DashboardTable
    {
        public Guid DashboardTableId { get; set; }

        public Guid? AppUserId { get; set; }
        public Guid? ObjectTypeId { get; set; }

        public string? RootFilter { get; set; }
        public int? Size { get; set; }

        public int? Position { get; set; }

    }
}
