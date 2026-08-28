namespace Domain.Entities
{
    public class UserFollow
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid FollowerId { get; set; }

        public User Follower { get; set; } = default!;

        public Guid FollowingId { get; set; }

        public User Following { get; set; } = default!;

        public bool IsDeleted { get; set; }

        public string CreatedBy { get; set; } = default!;

        public DateTime DateCreated { get; set; } = DateTime.UtcNow;

        public DateTime DateUpdated { get; set; }
    }
}