using Domain.Enums;

namespace Domain.Entities
{
    public class Post
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid UserId { get; set; }

        public User User { get; set; } = default!;

        public string Content { get; set; } = default!;

        public string PostContenetUrl { get; set; } = default!;

        public Visibility Visibility { get; set; } = Visibility.Public;

        public DateTime DateCreated { get; set; } = DateTime.UtcNow;

        public DateTime DateUpdated { get; set; }

        public bool IsDeleted { get; set; }

        public string CreatedBy { get; set; } = default!;

        public ICollection<PostLike> PostLikes { get; set; } = new HashSet<PostLike>();

        public ICollection<Report> Reports { get; set; } = new HashSet<Report>();

        public ICollection<Comment> Comments { get; set; } = new HashSet<Comment>();
    }
}
