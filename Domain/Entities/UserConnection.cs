using Domain.Enums;

namespace Domain.Entities
{
    public class UserConnection
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid SenderId { get; set; }

        public User Sender { get; set; } = default!;

        public Guid RecieverId {  get; set; }

        public User Reciever { get; set; } = default!;

        public bool IsDeleted { get; set; }

        public string CreatedBy { get; set; } = default!;

        public ConnectionStatus ConnectionStatus { get; set; } = ConnectionStatus.Pending;

        public DateTime DateCreated { get; set; } = DateTime.UtcNow;

        public DateTime DateUpdated { get; set; }
    }
}
