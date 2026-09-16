namespace Application.Common.Dtos.Analytics
{
    public class ViewerContextDto
    {
        public Guid? ActorUserId { get; set; }
        public bool HasProfessionalProfile { get; set; }
        public bool HasRecruiterProfile { get; set; }
        public string? Industry { get; set; }
        public string? CompanySize { get; set; }
    }
}