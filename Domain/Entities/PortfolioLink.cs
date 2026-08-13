using Domain.Enums;

namespace Domain.Entities
{
    public class PortfolioLink
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid ProfessionalProfileId { get; set; }

        public ProfessionalProfile ProfessionalProfile { get; set; } = default!;

        public string Title { get; set; } = default!;

        public string Url { get; set; } = default!;

        public PortfolioLinkType LinkType { get; set; }

        public string? Description { get; set; }

        public string? ThumbnailUrl { get; set; }

        public string? ThumbnailPublicId { get; set; }

        public int ViewCount { get; set; }

        public int ClickCount { get; set; }

        public bool IsDeleted { get; set; }

        public string CreatedBy { get; set; } = default!;

        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
    }
}