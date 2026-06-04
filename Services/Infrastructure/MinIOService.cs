using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;
using Minio.Exceptions;
using StudyGroup.Api.Common.Settings.MinIO;
using StudyGroup.Api.Services.Infrastructure.Interfaces;
using StatusGeneric;
using StudyGroup.Api.Common.Models.MinIO;

namespace StudyGroup.Api.Services.Infrastructure;

public class MinIOService : StatusGenericHandler, IMinioService
{
    private readonly MinioClient _minioClient;
    private readonly string _bucketName;
 
    public MinIOService(IOptions<MinIOSettings> options)
    {
        var settings = options.Value;
        _bucketName = settings.BucketName;
 
        var client = new MinioClient()
            .WithEndpoint(settings.Endpoint)
            .WithCredentials(settings.AccessKey, settings.SecretKey);
 
        if (settings.Secure)
            client = client.WithSSL();
 
        _minioClient = (MinioClient)client.Build();
    }
 
    public async Task UploadFileAsync(string folderName, UploadFileModel file)
    {
        try
        {
            var found = await _minioClient.BucketExistsAsync(
                new BucketExistsArgs().WithBucket(_bucketName));
 
            if (!found)
                await _minioClient.MakeBucketAsync(
                    new MakeBucketArgs().WithBucket(_bucketName));
 
            var objectName = BuildObjectName(folderName, file.FileName);
 
            if (file.Data.CanSeek)
                file.Data.Position = 0;
 
            await _minioClient.PutObjectAsync(new PutObjectArgs()
                .WithBucket(_bucketName)
                .WithObject(objectName)
                .WithStreamData(file.Data)
                .WithObjectSize(file.Size)
                .WithContentType(file.ContentType));
 
            Message = $"File '{file.FileName}' uploaded successfully";
        }
        catch (MinioException ex)
        {
            AddError($"[MinIO Upload Error]: {ex.Message}");
        }
    }
 
    public async Task<UploadFileModel?> GetFileAsync(string folderName, Guid fileName)
    {
        try
        {
            var objectName = BuildObjectName(folderName, fileName);
            var memoryStream = new MemoryStream();
 
            var stat = await _minioClient.StatObjectAsync(new StatObjectArgs()
                .WithBucket(_bucketName)
                .WithObject(objectName));
 
            var contentType = stat.ContentType;
 
            await _minioClient.GetObjectAsync(new GetObjectArgs()
                .WithBucket(_bucketName)
                .WithObject(objectName)
                .WithCallbackStream(stream => stream.CopyTo(memoryStream)));
 
            memoryStream.Position = 0;
 
            Message = $"File '{fileName}' retrieved successfully";
            return new UploadFileModel(fileName, contentType, memoryStream.Length, memoryStream);
        }
        catch (ObjectNotFoundException)
        {
            AddError($"File not found. FileId: {fileName}");
            return null;
        }
        catch (MinioException ex)
        {
            throw new MinioException($"[MinIO Error]: {ex.Message}");
        }
    }
 
    public async Task RemoveFileAsync(string folderName, Guid fileName)
    {
        try
        {
            var objectName = BuildObjectName(folderName, fileName);
 
            await _minioClient.RemoveObjectAsync(new RemoveObjectArgs()
                .WithBucket(_bucketName)
                .WithObject(objectName));
 
            Message = $"File '{fileName}' removed successfully";
        }
        catch (ObjectNotFoundException) { }
        catch (Exception ex)
        {
            throw new Exception($"[MinIO Remove Error for {fileName}]: {ex.Message}");
        }
    }
 
    // ── helpers ──────────────────────────────────────────────────────────────
 
    private static string BuildObjectName(string folderName, Guid fileName) =>
        string.IsNullOrWhiteSpace(folderName)
            ? fileName.ToString()
            : $"{folderName.TrimEnd('/')}/{fileName}";
}