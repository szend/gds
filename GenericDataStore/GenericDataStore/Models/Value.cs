using GenericDataStore.Models;
using System.Text.Json.Serialization;

namespace GenericDataStore.Models
{
    public class Value
    {
        public Guid ValueId { get; set; }

        public Guid DataObjectId { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public Guid ObjectTypeId { get; set; }

        public string? Name { get; set; }
        public string? ValueString { get; set; }
        public string? Color { get; set; }
    }
}

public class ValueComparable : IEqualityComparer<Value>
{
    public bool Equals(Value x, Value y)
    {
        return x.DataObjectId == y.DataObjectId;
    }

    public int GetHashCode(Value obj)
    {
        return obj.ValueId.GetHashCode();
    }
}
