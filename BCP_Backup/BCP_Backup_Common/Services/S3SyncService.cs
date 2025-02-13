using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using CommonLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

public class S3SyncService : IS3SyncService
{
    private string bucketName;
    private IAmazonS3 s3Client;
    private TransferUtility fileTransferUtility;
    private string endpoint;
    private RegionEndpoint bucketRegion;
    private string accessKeyId;
    private string secretAccessKey;
    private string sessionToken;

    public S3SyncService(string bucketName, RegionEndpoint bucketRegion, string accessKeyId, string secretAccessKey, string sessionToken, string endpoint = null)
    {
        this.bucketName = bucketName;
        this.bucketRegion = bucketRegion;
        this.accessKeyId = accessKeyId;
        this.secretAccessKey = secretAccessKey;
        this.sessionToken = sessionToken;
        this.endpoint = endpoint;
        AWSConfigsS3.UseSignatureVersion4 = true;

        //this.s3Client = new AmazonS3Client(accessKeyId, secretAccessKey, new AmazonS3Config { RegionEndpoint = bucketRegion, ServiceURL = endpoint });
        //this.fileTransferUtility = new TransferUtility(s3Client);
    }

    // ローカルフォルダとS3バケットを同期するメソッド
    public async Task SyncFolderAsync(string localPath, string s3Path)
    {

        try
        {

            // ローカルフォルダ内のファイルとディレクトリのリストを取得
            var localFiles = Directory.GetFiles(localPath, "*", SearchOption.AllDirectories);
            var localFileKeys = localFiles.Select(f => s3Path + "/" + f.Replace(localPath, "").TrimStart(Path.DirectorySeparatorChar).Replace(Path.DirectorySeparatorChar, '/')).ToList();

            // S3バケット内のファイルリストを取得
            var s3Files = await ListS3FilesAsync(s3Path);

            // 新規または変更されたファイルをアップロード
            foreach (var localFile in localFiles)
            {
                var key = s3Path + "/" + localFile.Replace(localPath, "").TrimStart(Path.DirectorySeparatorChar).Replace(Path.DirectorySeparatorChar, '/');
                if (!s3Files.Contains(key) || await IsFileModifiedAsync(localFile, key))
                {
                    await UploadFileAsync(localFile, key);
                }
            }

            // ローカルフォルダに存在しないS3のファイルを削除
            foreach (var s3File in s3Files)
            {
                if (!localFileKeys.Contains(s3File))
                {
                    await DeleteFileAsync(s3File);
                }
            }

            // ローカルフォルダ内の空ディレクトリを同期
            await SyncEmptyDirectoriesAsync(localPath, s3Path);
        }
        catch (Exception e)
        {
            // 標準エラーをレジストリのエラー情報に設定する。
            Configuration.WriteToRegistry(Constants.Settings.ErrorInfo, e.ToString());
            throw e;
        }

        if (Configuration.ExistsInRegistry(Constants.Settings.ErrorInfo))
        {
            // レジストリのエラー情報を削除する。（以前のエラー情報を継承しないため）
            Configuration.DeleteFromRegistry(Constants.Settings.ErrorInfo);
        }
    }

    // S3バケット内のファイルリストを取得するメソッド
    private async Task<List<string>> ListS3FilesAsync(string prefix)
    {
        var s3Files = new List<string>();
        var request = new ListObjectsV2Request
        {
            BucketName = this.bucketName,
            Prefix = prefix
        };

        ListObjectsV2Response response;
        do
        {
            response = await s3Client.ListObjectsV2Async(request);
            s3Files.AddRange(response.S3Objects.Select(o => o.Key));
            request.ContinuationToken = response.NextContinuationToken;
        } while (response.IsTruncated);

        return s3Files;
    }

    // ローカルファイルがS3のファイルより新しいかどうかを確認するメソッド
    private async Task<bool> IsFileModifiedAsync(string localFile, string s3Key)
    {
        var localFileInfo = new FileInfo(localFile);
        var request = new GetObjectMetadataRequest
        {
            BucketName = this.bucketName,
            Key = s3Key
        };

        try
        {
            var response = await s3Client.GetObjectMetadataAsync(request);
            var s3FileSize = response.ContentLength;
            var s3LastModified = response.LastModified;

            // ファイルサイズと最終更新日時を比較
            return localFileInfo.Length != s3FileSize || localFileInfo.LastWriteTimeUtc > s3LastModified;
        }
        catch (AmazonS3Exception e) when (e.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return true; // S3にファイルが存在しない場合は新規ファイルとみなす
        }
    }

    // ローカルファイルをS3にアップロードするメソッド
    private async Task UploadFileAsync(string localFile, string s3Key)
    {
        // マルチパートアップロードを使用してファイルをアップロード
        await fileTransferUtility.UploadAsync(localFile, bucketName, s3Key);
#if DEBUG
        Console.WriteLine($"Uploaded: {s3Key}");
#endif
    }

    // S3からファイルを削除するメソッド
    private async Task DeleteFileAsync(string s3Key)
    {
        var deleteRequest = new DeleteObjectRequest
        {
            BucketName = bucketName,
            Key = s3Key
        };

        await s3Client.DeleteObjectAsync(deleteRequest);
#if DEBUG
        Console.WriteLine($"Deleted: {s3Key}");
#endif
    }

    // IS3SyncService インターフェースのメソッドを実装
    public void UpdateS3Client(string accessKeyId, string secretAccessKey, string sessionToken)
    {
        // キーやトークンが更新されていない場合は何もしない
        if (s3Client != null && this.accessKeyId == accessKeyId && this.secretAccessKey == secretAccessKey && this.sessionToken == sessionToken)
        {
            return;
        }

        this.accessKeyId = accessKeyId;
        this.secretAccessKey = secretAccessKey;
        this.sessionToken = sessionToken;
        this.endpoint = Configuration.ReadFromRegistry(Constants.Settings.S3Endpoint);

        if (this.endpoint == null)
        {
            this.s3Client = new AmazonS3Client(accessKeyId, secretAccessKey, sessionToken, new AmazonS3Config { RegionEndpoint = this.bucketRegion });
        }
        else
        {
            this.s3Client = new AmazonS3Client(accessKeyId, secretAccessKey, sessionToken, new AmazonS3Config { RegionEndpoint = this.bucketRegion, ServiceURL = this.endpoint });
        }
        this.fileTransferUtility = new TransferUtility(s3Client);
    }


    // IS3SyncService インターフェースのメソッドを実装
    public void UpdateS3ClientTest(string accessKeyId, string secretAccessKey, string sessionToken)
    {
        // キーやトークンが更新されていない場合は何もしない
        if (s3Client != null && this.accessKeyId == accessKeyId && this.secretAccessKey == secretAccessKey && this.sessionToken == sessionToken)
        {
            return;
        }

        this.accessKeyId = accessKeyId;
        this.secretAccessKey = secretAccessKey;
        this.sessionToken = sessionToken;
        this.endpoint = Configuration.ReadFromRegistry(Constants.Settings.S3Endpoint);

        if (this.endpoint == null)
        {
            this.s3Client = new AmazonS3Client(accessKeyId, secretAccessKey, sessionToken, new AmazonS3Config { RegionEndpoint = this.bucketRegion });
        }
        else
        {
            if (sessionToken == null)
            {
                this.s3Client = new AmazonS3Client(accessKeyId, secretAccessKey, new AmazonS3Config { RegionEndpoint = this.bucketRegion, ServiceURL = this.endpoint });

            }
            else
            {
                this.s3Client = new AmazonS3Client(accessKeyId, secretAccessKey, sessionToken, new AmazonS3Config { RegionEndpoint = this.bucketRegion, ServiceURL = this.endpoint });
            }
        }
        this.fileTransferUtility = new TransferUtility(s3Client);
    }

    public void SetBucketName(string bucketName) {
        this.bucketName = bucketName;
    }

    // ローカルフォルダにS3バケットの内容をリストアするメソッド
    public async Task RestoreFolderAsync(string s3Path, string localPath)
    {
        try
        {
            // ローカルパス配下ののファイル、サブフォルダをすべて削除する。
            // Delete all files and subdirectories under the specified local path
            foreach (var file in Directory.GetFiles(localPath))
            {
                File.Delete(file);
            }
            foreach (var directory in Directory.GetDirectories(localPath))
            {
                Directory.Delete(directory, true);
            }

            // S3バケット内のファイルリストを取得
            var s3Files = await ListS3FilesAsync(s3Path);

            // ファイルをダウンロードしてローカルに保存
            foreach (var s3File in s3Files)
            {
                var localFilePath = Path.Combine(localPath, s3File.Replace(s3Path + "/", "").Replace('/', Path.DirectorySeparatorChar));
                Directory.CreateDirectory(Path.GetDirectoryName(localFilePath));

                // フォルダ作成かファイルダウンロードかを判断
                if (s3File.EndsWith("/"))
                {
                    Directory.CreateDirectory(localFilePath);
                }
                else
                {
                    await DownloadFileAsync(s3File, localFilePath);
                }
            }   
        }
        catch (Exception e)
        {
            // 標準エラーをレジストリのエラー情報に設定する。
            Configuration.WriteToRegistry(Constants.Settings.ErrorInfo, e.ToString());
            throw e;
        }

        if (Configuration.ExistsInRegistry(Constants.Settings.ErrorInfo))
        {
            // レジストリのエラー情報を削除する。（以前のエラー情報を継承しないため）
            Configuration.DeleteFromRegistry(Constants.Settings.ErrorInfo);
        }

    }

    // S3からファイルをダウンロードするメソッド
    private async Task DownloadFileAsync(string s3Key, string localFile)
    {
        var request = new GetObjectRequest
        {
            BucketName = this.bucketName,
            Key = s3Key
        };

        var response = await s3Client.GetObjectAsync(request);
        using (var responseStream = response.ResponseStream)
        using (var fileStream = new FileStream(localFile, FileMode.Create, FileAccess.Write))
        {
            await responseStream.CopyToAsync(fileStream);
        }

        // ファイルの更新日時をS3のLastModifiedに設定
        File.SetLastWriteTimeUtc(localFile, response.LastModified);
    }

    // ローカルフォルダ内の空ディレクトリを同期するメソッド
    private async Task SyncEmptyDirectoriesAsync(string localPath, string s3Path)
    {
        var localDirectories = Directory.GetDirectories(localPath, "*", SearchOption.AllDirectories);
        var localDirectoryKeys = localDirectories.Select(d => s3Path + "/" + d.Replace(localPath, "").TrimStart(Path.DirectorySeparatorChar).Replace(Path.DirectorySeparatorChar, '/')).ToList();

        var s3Directories = await ListS3DirectoriesAsync(s3Path);
        // ローカルフォルダに存在しないS3のディレクトリを作成
        foreach (var localDirectory in localDirectories)
        {
            var s3Directory = s3Path + "/" + localDirectory.Replace(localPath, "").TrimStart(Path.DirectorySeparatorChar).Replace(Path.DirectorySeparatorChar, '/');
            if (!s3Directories.Contains(s3Directory))
            {
                await CreateDirectoryAsync(s3Directory);
            }
        }
    }

    // S3バケット内のディレクトリリストを取得するメソッド
    private async Task<List<string>> ListS3DirectoriesAsync(string prefix)
    {
        var s3Directories = new List<string>();
        var request = new ListObjectsV2Request
        {
            BucketName = this.bucketName,
            Prefix = prefix,
            Delimiter = "/"
        };

        ListObjectsV2Response response;
        do
        {
            response = await s3Client.ListObjectsV2Async(request);
            s3Directories.AddRange(response.CommonPrefixes.Select(p => p.TrimEnd('/')));
            request.ContinuationToken = response.NextContinuationToken;
        } while (response.IsTruncated);

        return s3Directories;
    }

    // S3にディレクトリを作成するメソッド
    private async Task CreateDirectoryAsync(string s3Directory)
    {
        var putRequest = new PutObjectRequest
        {
            BucketName = this.bucketName,
            Key = s3Directory + "/",
            ContentBody = string.Empty
        };

        await s3Client.PutObjectAsync(putRequest);
#if DEBUG
        Console.WriteLine($"Created directory: {s3Directory}");
#endif
    }
}
