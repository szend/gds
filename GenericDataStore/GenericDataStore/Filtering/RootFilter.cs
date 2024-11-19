namespace GenericDataStore.Filtering
{
    public class RootFilter
    {
        public List<Filter>? Filters { get; set; }

        public List<Filter>? ValueFilters { get; set; }

        public List<Filter>? ParentValueFilters { get; set; }


        public string? Logic { get; set; }

        public List<SortingParams>? SortingParams { get; set; }

        public List<SortingParams>? ValueSortingParams { get; set; }
        public List<SortingParams>? ParentValueSortingParams { get; set; }



        public int Take { get; set; } = 0;

        public int Skip { get; set; } = 0;

        public int ValueTake { get; set; } = 0;

        public int ValueSkip { get; set; } = 0;



        public bool Equals(RootFilter obj)
        {
            if(obj == null)
            {
                return this == null;
            }
            if(obj.ValueTake == this.ValueTake && obj.ValueSkip == this.ValueSkip && obj.Take == this.Take && obj.Skip == this.Skip)
            {
                if(obj.Filters.SequenceEqual(this.Filters) && obj.ValueFilters.SequenceEqual(this.ValueFilters) && obj.ParentValueFilters.SequenceEqual(this.ParentValueFilters))
                {
                    if (obj.SortingParams.SequenceEqual(this.SortingParams) && obj.ValueSortingParams.SequenceEqual(this.ValueSortingParams) && obj.ParentValueSortingParams.SequenceEqual(this.ParentValueSortingParams))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }

    public class Filter
    {
        public string Field { get; set; }

        public string Operator { get; set; }

        public object Value { get; set; }

        public override bool Equals(object? obj)
        {
            if (obj == null)
            {
                return this == null;
            }
            if ((obj as Filter)?.Field == this.Field && (obj as Filter)?.Operator == this.Operator && (obj as Filter)?.Value.ToString() == this.Value.ToString())
            {
                return true;
            }
            return false;
        }

    }
}
