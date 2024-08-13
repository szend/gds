using GenericDataStore.Models;

namespace GenericDataStore.InputModels
{
    public class CalculatedField
    {
        public string Name { get; set; }
        public Guid TypeId { get; set; }
        public string CalculationString { get; set; }

        public string OriginalType { get; set; }
    }
}
