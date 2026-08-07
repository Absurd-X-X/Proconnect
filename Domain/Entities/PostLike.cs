namespace Domain.Entities
{
    public class PostLike
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid PostId { get; set; }

        public Post Post { get; set; } = default!;

        public Guid UserId { get; set; }

        public User User { get; set; } = default!;

        public DateTime DateCreated { get; set; } = DateTime.UtcNow;

        public bool IsDeleted { get; set; }

        public string CreatedBy { get; set; } = default!;
    }
}
