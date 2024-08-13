namespace GenericDataStore.Models
{
    public class Option
    {
        public Guid OptionId { get; set; }

        public Guid FieldId { get; set; }

        public string? OptionName { get; set; }

        public int? OptionValue { get; set; }

    }
}
