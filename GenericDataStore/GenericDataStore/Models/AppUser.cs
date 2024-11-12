using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations.Schema;

namespace GenericDataStore.Models
{
    [Index(nameof(UserName), IsUnique = true)]
    public class AppUser : IdentityUser<Guid>
    {
        public AppUser()
        {
            this.DataObject = new HashSet<DataObject>();
            this.ObjectType = new HashSet<ObjectType>();
            this.Received = new HashSet<UserMessage>();
            this.Sent = new HashSet<UserMessage>();
            this.DatabaseConnectionProperty = new HashSet<DatabaseConnectionProperty>();

        }

        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        [NotMapped]
        public virtual ICollection<DataObject> DataObject { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public virtual ICollection<ObjectType> ObjectType { get; set; }

        public int AllowedListCount { get; set; }
        public int AllowedDataCount { get; set; }

        public int AllowedExternalDataCount { get; set; }

        public int MaxDataCountInMonth { get; set; }

        public int MaxExternalDataCountInMonth { get; set; }

        public bool HasSub { get; set; }

        public DateTime? SubStart { get; set; }

        public DateTime? NextPay { get; set; }

        public string? SubType { get; set; }

        public bool? PublicDashboard { get; set; }

        //public long AllowedFileSize { get; set; }
        //public string? SubscriptionType { get; set; }
        //public DateTime? SubscriptionEnd { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public virtual ICollection<UserMessage> Received { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public virtual ICollection<UserMessage> Sent { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public virtual ICollection<DatabaseConnectionProperty> DatabaseConnectionProperty { get; set; }




    }
}
