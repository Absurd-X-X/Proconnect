namespace Domain.Entities
{
    public class FileUpload
    {
        public Guid Id = Guid.NewGuid();

        public Guid UserId { get; set; }

        public User User { get; set; } = default!;

        public string FileName { get; set; } = default!;

        public string FileUrl { get; set; } = default!;

        public string FileType { get; set; } = default!;

        public int FileSize { get; set; }

        public DateTime UploadedOn { get; set; } = DateTime.UtcNow;

        public bool IsDeleted { get; set; }

        public string CreatedBy { get; set; } = default!;
    }
}
