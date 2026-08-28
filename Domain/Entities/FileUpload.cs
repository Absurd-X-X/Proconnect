namespace Domain.Entities
{
    public class FileUpload
    {
        public Guid Id = Guid.NewGuid();

        public Guid UserId { get; set; }

        public User User { get; set; } = default!;

        public Guid? PostId { get; set; }

        public Post? Post { get; set; }

        public Guid? MessageId { get; set; }

        public Message? Message { get; set; }

        public string FileName { get; set; } = default!;

        public string FileUrl { get; set; } = default!;

        public string FileType { get; set; } = default!;

        public int FileSize { get; set; }

        public int DisplayOrder { get; set; }

        public DateTime UploadedOn { get; set; } = DateTime.UtcNow;

        public bool IsDeleted { get; set; }

        public string CreatedBy { get; set; } = default!;
    }
}