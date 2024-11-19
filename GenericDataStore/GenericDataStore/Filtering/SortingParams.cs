namespace GenericDataStore.Filtering
{
    public class SortingParams
    {
        public int Order { get; set; } = -1;
        public string Field { get; set; }

        public string Type { get; set; } = "text";

        public override bool Equals(object? obj)
        {
            if (obj == null)
            {
                return this == null;
            }
            if (obj is SortingParams sortingParams)
            {
                if (sortingParams.Field == this.Field && sortingParams.Order == this.Order && sortingParams.Type == this.Type)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
