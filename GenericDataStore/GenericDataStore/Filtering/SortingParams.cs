namespace GenericDataStore.Filtering
{
    public class SortingParams
    {
        public int Order { get; set; } = -1;
        public string Field { get; set; }

        public string Type { get; set; } = "text";
    }
}
