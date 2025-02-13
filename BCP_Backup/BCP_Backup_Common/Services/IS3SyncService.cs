using System.Threading.Tasks;

public interface IS3SyncService
{
    Task SyncFolderAsync(string localPath, string s3Path);
    Task RestoreFolderAsync(string s3Path, string localPath);
    void UpdateS3Client(string accessKeyId, string secretAccessKey, string sessionToken);
    void SetBucketName(string bucketaName);
}
