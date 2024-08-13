namespace GenericDataStore.Filtering
{
    public class RootFilter
    {
        public List<Filter>? Filters { get; set; }

        public List<Filter>? ValueFilters { get; set; }

        public string? Logic { get; set; }

        public List<SortingParams>? SortingParams { get; set; }

        public List<SortingParams>? ValueSortingParams { get; set; }


        public int Take { get; set; } = 0;

        public int Skip { get; set; } = 0;

        public int ValueTake { get; set; } = 0;

        public int ValueSkip { get; set; } = 0;
    }

    public class Filter
    {
        public string Field { get; set; }

        public string Operator { get; set; }

        public object Value { get; set; }
    }
}
