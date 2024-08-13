namespace GenericDataStore.InputModels
{
    public class CalculatedColor
    {
        public Guid TypeId { get; set; }

        public Guid FieldId { get; set; }
        public string CalculationColor { get; set; }
    }
}
