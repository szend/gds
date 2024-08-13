using GenericDataStore.Models;
using System.Text.Json;

namespace GenericDataStore.InputModels
{
    public class ImportCreateModel
    {
        public List<JsonElement> File { get; set; }
        public ObjectType ObjectType { get; set; }
    }
}
