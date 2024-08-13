using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace GenericDataStore.Models
{
    public class DatabaseConnectionProperty
    {
        public DatabaseConnectionProperty() 
        {
            this.ObjectType = new HashSet<ObjectType>();

        }
        public Guid DatabaseConnectionPropertyId { get; set; }

        public string ConnectionString { get; set; }

        public bool? Default { get; set; }

        public bool? Public { get; set; }

        public string DatabaseType { get; set; }

        public string DatabaseName { get; set; }

        public string DefaultIdType { get; set; }

        public AppUser? AppUser { get; set; }
        public Guid? AppUserId { get; set; }


        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public virtual ICollection<ObjectType> ObjectType { get; set; }

    }
}
