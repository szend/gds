namespace GenericDataStore.Models
{
    public class Chart
    {
        public Guid CharteId { get; set; }

        public Guid? AppUserId { get; set; }

        public Guid? ObjectTypeId { get; set; }

        public string? RootFilter { get; set; }

        public int? Size { get; set; }

        public int? Position { get; set; }

        public string? Type { get; set; }

        public string? GroupOption { get; set; }

        public string? Xcalculation { get; set; }

        public string? Ycalculation { get; set; }

    }
}
