using GenericDataStore.Models;

namespace GenericDataStore.InputModels
{
    public class DashboardModel
    {
        public int Position { get; set; }
        public int Size { get; set; }
        public string? Type { get; set; }
        public string? Filter { get; set; }
        public ChartModelType? Chart  { get; set; }
        public ChartInput? ChartInput { get; set; }
        public ObjectType? ObjectType { get; set; }
    }
}
