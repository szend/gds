using GenericDataStore.Filtering;
using GenericDataStore.Models;

namespace GenericDataStore.InputModels
{
    public class AllEditModel
    {
        public DataObject Object { get; set; }
        public RootFilter Filter { get; set; }
    }
}
