using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace GenericDataStore.Models
{
    public class UserMessage
    {
        public Guid? UserMessageId { get; set; }

        [InverseProperty(nameof(AppUser.Sent))]
        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public AppUser? SendUser { get; set; }
        [ForeignKey("SendUser")]
        public Guid? SendUserId { get; set; }

        [InverseProperty(nameof(AppUser.Received))]
        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public AppUser? ReceivUser { get; set; }
        [ForeignKey("ReceivUser")]
        public Guid? ReceivUserId { get; set; }

        public string? Comment { get; set; }

        public DateTime? Date { get; set; }

        public Guid? ObjectTypeId { get; set; }

        public Guid? LastMessageId { get; set; }

        public bool? NoVisibleReceiver { get; set; }
        public bool? NoVisibleSender { get; set; }

        [NotMapped]
        public string? SenderName { get; set; }
        [NotMapped]
        public string? ReceiverName { get; set; }

        [NotMapped]
        public string? SenderMail { get; set; }
        [NotMapped]
        public string? ReceiverMail { get; set; }

    }
}
