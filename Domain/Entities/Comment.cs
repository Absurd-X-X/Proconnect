namespace Domain.Entities
{
    public class Comment
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid PostId { get; set; }

        public Post Post { get; set; } = default!;

        public Guid UserId { get; set; }

        public User User { get; set; } = default!;

        public string Content { get; set; } = default!;

        public DateTime DateCreated { get; set; } = DateTime.UtcNow;

        public string CreatedBy { get; set; } = default!;

        public DateTime DateUpdated { get; set; }
    }
}
