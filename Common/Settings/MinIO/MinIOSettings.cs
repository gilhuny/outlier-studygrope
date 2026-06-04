namespace StudyGroup.Api.Common.Settings.MinIO;


public class MinIOSettings
{
    public required string Endpoint { get; set; }
    public required string AccessKey { get; set; }
    public required string SecretKey { get; set; }
    public required string BucketName { get; set; }
    public bool Secure { get; set; }
}