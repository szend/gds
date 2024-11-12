using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net.Mail;
using System.Text.Json.Serialization;

namespace GenericDataStore.Models
{
    public class ObjectType
    {
        public ObjectType() 
        {
            this.DataObject = new HashSet<DataObject>();
            this.Field = new HashSet<Field>();
            this.ChildObjectTypes = new HashSet<ObjectType>();
            this.ParentObjectTypes = new HashSet<ObjectType>();
            this.Value = new HashSet<Value>();
        }
        public Guid ObjectTypeId { get; set; }

        //[ForeignKey("ParentObjectType")]
        //public Guid? ParentObjectTypeId { get; set; }




        public Guid? AppUserId { get; set; }

        public string? TableName { get; set; }

        public string? Name { get; set; }

        public string? Description { get; set; }
        public string? Category { get; set; }


        public AppUser? AppUser { get; set; }


        public virtual ICollection<Field> Field { get; set; }
        [NotMapped]
        public virtual ICollection<DataObject> DataObject { get; set; }

        public string? Color { get; set; }
        public bool Private { get; set; }
        public bool NoFilterMenu { get; set; }
        public bool DenyAdd { get; set; }
        public bool DenyExport { get; set; }
        public bool DenyChart { get; set; }

        public bool Promoted { get; set; }
        public bool AllUserFullAccess { get; set; }

        public DateTime? CreationDate { get; set; }

        public Guid? DatabaseConnectionPropertyId { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public DatabaseConnectionProperty? DatabaseConnectionProperty { get; set; }


        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        [NotMapped]

        public virtual ICollection<ObjectType> ChildObjectTypes { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        [NotMapped]

        public virtual ICollection<ObjectType> ParentObjectTypes { get; set; }


        [NotMapped]

        public int Count { get; set; }

        [NotMapped]
        public Guid? DashboardTableId { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        [NotMapped]
        public virtual ICollection<Value> Value { get; set; }
    }
}
