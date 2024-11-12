namespace GenericDataStore.Models
{
    public class Chart
    {
        public Guid ChartId { get; set; }

        public Guid? AppUserId { get; set; }

        public Guid? ObjectTypeId { get; set; }

        public Guid? GroupId { get; set; }

        public string? RootFilter { get; set; }

        public int? Size { get; set; }

        public int? Position { get; set; }

        public string? Type { get; set; }

        public string? GroupOption { get; set; }

        public string? Xcalculation { get; set; }

        public string? Ycalculation { get; set; }

        public string? Colorcalculation { get; set; }

        public string? Regression { get; set; }
        public string? Step { get; set; }

        public string? Fill { get; set; }

        public bool? Stacked { get; set; }



    }
}
