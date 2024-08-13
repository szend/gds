using GenericDataStore.Filtering;

namespace GenericDataStore.InputModels
{
    public class ChartInput
    {
        public string? Xcalculation { get; set; }
        public string? Ycalculation { get; set; }

        public Guid Id { get; set; }

        public string? Type { get; set; }
        public string? GroupOption { get; set; }

        public RootFilter? Filter { get; set; }

    }
}
