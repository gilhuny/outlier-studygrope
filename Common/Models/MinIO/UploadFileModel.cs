namespace StudyGroup.Api.Common.Models.MinIO
{
    public record UploadFileModel(Guid FileName, string ContentType, long Size, Stream Data);
}
