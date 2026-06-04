using StatusGeneric;
using StudyGroup.Api.Common.Models.MinIO;

namespace StudyGroup.Api.Services.Infrastructure.Interfaces;

public interface IMinioService : IStatusGeneric
{
    Task UploadFileAsync(string folderName, UploadFileModel file);
    Task<UploadFileModel?> GetFileAsync(string folderName, Guid fileName);
    Task RemoveFileAsync(string folderName, Guid fileName);
}