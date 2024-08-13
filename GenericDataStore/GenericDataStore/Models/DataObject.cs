using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace GenericDataStore.Models
{
    public class DataObject
    {
        public DataObject()
        {
            this.Value = new HashSet<Value>();
            this.UserMessage = new HashSet<UserMessage>();
            this.ChildDataObjects = new HashSet<DataObject>();
            this.ParentDataObjects = new HashSet<DataObject>();
        }
        public Guid ObjectTypeId { get; set; }
        public virtual ICollection<Value> Value { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public virtual ICollection<UserMessage> UserMessage { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]

        public virtual ICollection<DataObject>? ChildDataObjects { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public virtual ICollection<DataObject>? ParentDataObjects { get; set; }
    }
}
