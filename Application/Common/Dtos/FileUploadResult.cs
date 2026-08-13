namespace Application.Common.Dtos
{
    public record FileUploadResult(
        string Url,
        string PublicId,
        string ResourceType);
}