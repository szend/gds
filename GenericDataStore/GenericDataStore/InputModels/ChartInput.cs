using GenericDataStore.Filtering;

namespace GenericDataStore.InputModels
{
    public class ChartInput
    {
        public string? Xcalculation { get; set; }
        public string? Ycalculation { get; set; }
        public string? Colorcalculation { get; set; }

        public Guid? Id { get; set; }
        public Guid? GroupId { get; set; }

        public string? Type { get; set; }
        public string? GroupOption { get; set; }

        public RootFilter? Filter { get; set; }

        public string? Regression { get; set; }
        public string? Step { get; set; }

        public string? Fill { get; set; }

        public bool? Stacked { get; set; }

        public bool LiveMode { get; set; }

        public List<Guid>? Charts { get; set; }

    }
}
