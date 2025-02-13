using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

public class S3SyncServiceIntegrationTests
{
    private const string AccessKey = "minioadmin";
    private const string SecretKey = "minioadmin";
    private const string Endpoint = "http://127.0.0.1:9000";
    private const string BucketName = "test-bucket";
    private readonly IAmazonS3 s3Client;
    private readonly S3SyncService s3SyncService;

    public S3SyncServiceIntegrationTests()
    {
        s3Client = new AmazonS3Client(AccessKey, SecretKey, new AmazonS3Config
        {
            ServiceURL = Endpoint,
            ForcePathStyle = true
        });
        s3SyncService = new S3SyncService(BucketName, Amazon.RegionEndpoint.APNortheast1, AccessKey, SecretKey, null, Endpoint);
        s3SyncService.UpdateS3ClientTest(AccessKey, SecretKey, null);
    }

    // テストメソッドの引数の命名規則
    // pre～: 前提条件 （precondition）
    // arg～：引数　（argument）
    // expt～: 期待値　（expected）
    [Theory(DisplayName = "ファイルパス")]
    [InlineData("01-01.新規ファイル‐単一ファイル", new[] { "localPath/file1.txt" }, new string[] { }, new string[] { "s3Path/file1.txt" })]
    [InlineData("01-02.新規ファイル－単一ファイル中間ディレクトリ", new[] { "localPath/dir1/file1.txt" }, new string[] { }, new string[] { "s3Path/dir1/file1.txt", "s3Path/dir1/" })]
    [InlineData("01-03.新規ファイル－複数ファイル", new[] { "localPath/file1.txt", "localPath/file2.txt" }, new string[] { }, new string[] { "s3Path/file1.txt", "s3Path/file2.txt" })]
    [InlineData("01-04.新規ファイル－複数ファイル中間ディレクトリ", new[] { "localPath/dir1/file1.txt", "localPath/dir1/file2.txt" }, new string[] { }, new string[] { "s3Path/dir1/file1.txt", "s3Path/dir1/file2.txt", "s3Path/dir1/" })]
    [InlineData("01-05.新規ファイル－複数ディレクトリ", new[] { "localPath/dir1/file1.txt", "localPath/dir2/file2.txt" }, new string[] { }, new string[] { "s3Path/dir1/file1.txt", "s3Path/dir2/file2.txt", "s3Path/dir1/", "s3Path/dir2/" })]
    [InlineData("01-06.新規ファイル‐単一ファイル日本語ファイル名", new[] { "localPath/ファイル１.txt" }, new string[] { }, new string[] { "s3Path/ファイル１.txt" })]
    [InlineData("01-07.新規ファイル‐単一ファイル日本語ディレクトリ名", new[] { "localPath/ディレクトリ１/ファイル１.txt" }, new string[] { }, new string[] { "s3Path/ディレクトリ１/ファイル１.txt", "s3Path/ディレクトリ１/" })]
    [InlineData("01-08.新規ファイル‐単一ファイルスペース", new[] { "localPath/file 1.txt" }, new string[] { }, new string[] { "s3Path/file 1.txt" })]
    [InlineData("01-09.新規ファイル‐単一ファイル全角スペース", new[] { "localPath/file　1.txt" }, new string[] { }, new string[] { "s3Path/file　1.txt" })]
    [InlineData("02-01.削除－単一ファイル", new string[] { }, new string[] { "s3Path/file1.txt" }, new string[] { })]
    [InlineData("02-02.削除－単一ファイル対象外含む", new string[] { "localPath/file1.txt" }, new string[] { "s3Path/file1.txt", "s3Path/file2.txt" }, new string[] { "s3Path/file1.txt" })]
    [InlineData("02-03.削除－複数ファイル", new string[] { }, new string[] { "s3Path/file1.txt", "s3Path/file2.txt" }, new string[] { })]
    [InlineData("03-01.空ディレクトリ－単一ディレクトリ", new[] { "localPath/dir1/" }, new string[] { }, new string[] { "s3Path/dir1/" })]
    [InlineData("03-02.空ディレクトリ－ファイルありディレクトリ、空ディレクトリ混合", new[] { "localPath/dir1/", "localPath/dir2/file2.txt" }, new string[] { }, new string[] { "s3Path/dir1/", "s3Path/dir2/file2.txt", "s3Path/dir2/", })]
    [InlineData("03-03.空ディレクトリ－空ディレクトリ削除", new string[] { }, new string[] { "s3Path/dir1/" }, new string[] { })]
    [InlineData("03-04.空ディレクトリ－単一ディレクトリ日本語", new[] { "localPath/ディレクトリ１/" }, new string[] { }, new string[] { "s3Path/ディレクトリ１/" })]
    public async Task SyncFolderAsync_FilePath(string name, string[] preLocalFiles, string[] preS3Files, string[] exptS3Files)
    {
        // Arrange
        var localPath = "localPath";
        var s3Path = "s3Path";

        // テストケース名をログに出力
        Console.WriteLine($"Running test case: {name}");

        // Create local files
        if (Directory.Exists(localPath))
        {
            Directory.Delete(localPath, true);
        }
        Directory.CreateDirectory(localPath);
        foreach (var file in preLocalFiles)
        {
            if (file.EndsWith("/"))
            {
                Directory.CreateDirectory(file);
                continue;
            }
            if (!Directory.Exists(Path.GetDirectoryName(file)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(file));
            }
            File.WriteAllText(file, "test content");
        }

        // Delete bucket if it exists
        if (await DoesBucketExistAsync(BucketName))
        {
            await CleanupBucketAsync(BucketName);
        }

        // Create bucket
        await s3Client.PutBucketAsync(new PutBucketRequest { BucketName = BucketName });

        // Upload pre-existing S3 files
        foreach (var file in preS3Files)
        {
            if (file.EndsWith("/"))
            {
                await s3Client.PutObjectAsync(new PutObjectRequest
                {
                    BucketName = BucketName,
                    Key = file,
                    ContentBody = ""
                });
            }
            else
            {
                await s3Client.PutObjectAsync(new PutObjectRequest
                {
                    BucketName = BucketName,
                    Key = file,
                    ContentBody = "test content"
                });
            }
        }

        // Act
        await s3SyncService.SyncFolderAsync(localPath, s3Path);

        // Assert
        var objects = await ListObjectsAsync(BucketName, s3Path);
        Assert.Equal(exptS3Files.OrderBy(o => o), objects.Select(o => o.Key).OrderBy(o => o));

        // Cleanup
        if (Directory.Exists(localPath))
        {
            Directory.Delete(localPath, true);
        }

        await CleanupBucketAsync(BucketName);
    }

    private async Task<bool> DoesBucketExistAsync(string bucketName)
    {
        try
        {
            var response = await s3Client.GetBucketLocationAsync(new GetBucketLocationRequest { BucketName = bucketName });
            return true;
        }
        catch (AmazonS3Exception ex)
        {
            if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                return false;
            throw;
        }
    }

    private async Task<List<S3Object>> ListObjectsAsync(string bucketName, string prefix)
    {
        var objects = new List<S3Object>();
        var request = new ListObjectsV2Request
        {
            BucketName = bucketName,
            Prefix = prefix
        };

        ListObjectsV2Response response;
        do
        {
            response = await s3Client.ListObjectsV2Async(request);
            objects.AddRange(response.S3Objects);
            request.ContinuationToken = response.NextContinuationToken;
        } while (response.IsTruncated);

        return objects;
    }

    private async Task CleanupBucketAsync(string bucketName)
    {
        var objects = await ListObjectsAsync(bucketName, string.Empty);
        foreach (var obj in objects)
        {
            await s3Client.DeleteObjectAsync(new DeleteObjectRequest
            {
                BucketName = bucketName,
                Key = obj.Key
            });
        }

        // Delete empty file directories
        var directories = await ListDirectoriesAsync(bucketName, string.Empty);
        foreach (var directory in directories)
        {
            await s3Client.DeleteObjectAsync(new DeleteObjectRequest
            {
                BucketName = bucketName,
                Key = directory.Key
            });
        }

        await s3Client.DeleteBucketAsync(new DeleteBucketRequest { BucketName = bucketName });
    }

    private async Task<List<S3Object>> ListDirectoriesAsync(string bucketName, string prefix)
    {
        var directories = new List<S3Object>();
        var request = new ListObjectsV2Request
        {
            BucketName = bucketName,
            Prefix = prefix,
            Delimiter = "/"
        };

        ListObjectsV2Response response;
        do
        {
            response = await s3Client.ListObjectsV2Async(request);
            directories.AddRange(response.CommonPrefixes.Select(commonPrefix => new S3Object { Key = commonPrefix }));
            request.ContinuationToken = response.NextContinuationToken;
        } while (response.IsTruncated);

        return directories;
    }

    public class FileData
    {
        public FileData(string path, string content)
        {
            Path = path;
            Content = content;
        }
        public string Path { get; set; }
        public string Content { get; set; }
    }

    public static IEnumerable<object[]> GetSource_SyncFolderAsync_CheckFileContent()
    {
        yield return new object[] { "01-01.新規ファイル－単一ファイル", new FileData[] { new FileData("localPath/file1.txt", "テスト１") }, new FileData[] { }, new FileData[] { new FileData("s3Path/file1.txt", "テスト１") } };
        yield return new object[] { "01-01.新規ファイル－複数ファイル", new FileData[] { new FileData("localPath/file1.txt", "テスト１"), new FileData("localPath/file2.txt", "テスト２") }, new FileData[] { }, new FileData[] { new FileData("s3Path/file1.txt", "テスト１"), new FileData("s3Path/file2.txt", "テスト２") } };
        yield return new object[] { "02-01.更新ファイル－単一ファイル", new FileData[] { new FileData("localPath/file1.txt", "テスト１ローカル") }, new FileData[] { new FileData("s3Path/file1.txt", "テスト１Ｓ３") }, new FileData[] { new FileData("s3Path/file1.txt", "テスト１ローカル") } };
        yield return new object[] { "02-01.更新ファイル－複数ファイル", new FileData[] { new FileData("localPath/file1.txt", "テスト１ローカル"), new FileData("localPath/file2.txt", "テスト２ローカル") }, new FileData[] { new FileData("s3Path/file1.txt", "テスト１Ｓ３"), new FileData("s3Path/file2.txt", "テスト２Ｓ３") }, new FileData[] { new FileData("s3Path/file1.txt", "テスト１ローカル"), new FileData("s3Path/file2.txt", "テスト２ローカル") } };
    }

    [Theory(DisplayName = "SyncFolderAsyncファイル内容"),
    MemberData(nameof(GetSource_SyncFolderAsync_CheckFileContent))]
    public async Task SyncFolderAsync_CheckFileContent(String name, FileData[] preLocalFiles, FileData[] preS3Files, FileData[] exptS3Files)
    {
        // Arrange
        var localPath = "localPath";
        var s3Path = "s3Path";

        Console.WriteLine($"Running test case: {name}");

        // Create local files
        Directory.CreateDirectory(localPath);
        foreach (var file in preLocalFiles)
        {
            File.WriteAllText(file.Path, file.Content);
        }

        // Delete bucket if it exists
        if (await DoesBucketExistAsync(BucketName))
        {
            await CleanupBucketAsync(BucketName);
        }

        // Create bucket
        await s3Client.PutBucketAsync(new PutBucketRequest { BucketName = BucketName });

        // Upload pre-existing S3 files
        foreach (var file in preS3Files)
        {
            if (file.Path.EndsWith("/"))
            {
                await s3Client.PutObjectAsync(new PutObjectRequest
                {
                    BucketName = BucketName,
                    Key = file.Path,
                    ContentBody = ""
                });
            }
            else
            {
                await s3Client.PutObjectAsync(new PutObjectRequest
                {
                    BucketName = BucketName,
                    Key = file.Path,
                    ContentBody = file.Content
                });
            }
        }
        // Act
        await s3SyncService.SyncFolderAsync(localPath, s3Path);

        // Assert
        var objects = await ListObjectsAsync(BucketName, s3Path);
        foreach (var file in exptS3Files)
        {
            var s3Object = objects.FirstOrDefault(o => o.Key == $"{s3Path}/{Path.GetFileName(file.Path)}");
            Assert.NotNull(s3Object);
            var content = await GetObjectContentAsync(BucketName, s3Object.Key);
            Assert.Equal(file.Content, content);
        }

        // Cleanup
        Directory.Delete(localPath, true);
        await CleanupBucketAsync(BucketName);
    }

    private async Task<string> GetObjectContentAsync(string bucketName, string key)
    {
        using (var response = await s3Client.GetObjectAsync(bucketName, key))
        using (var reader = new StreamReader(response.ResponseStream))
        {
            return await reader.ReadToEndAsync();
        }
    }

    [Theory(DisplayName = "ファイルパス")]
    [InlineData("01-01.新規ファイル‐単一ファイル", new string[] { "s3Path/file1.txt" }, new string[] { }, new[] { @"localPath\file1.txt" })]
    [InlineData("01-02.新規ファイル－単一ファイル中間ディレクトリ", new string[] { "s3Path/dir1/file1.txt", "s3Path/dir1/" }, new string[] { }, new[] { @"localPath\dir1", @"localPath\dir1\file1.txt" })]
    [InlineData("01-03.新規ファイル－複数ファイル", new string[] { "s3Path/file1.txt", "s3Path/file2.txt" }, new string[] { }, new[] { @"localPath\file1.txt", @"localPath\file2.txt" })]
    [InlineData("01-04.新規ファイル－複数ファイル中間ディレクトリ", new string[] { "s3Path/dir1/file1.txt", "s3Path/dir1/file2.txt", "s3Path/dir1/" }, new string[] { }, new[] { @"localPath\dir1", @"localPath\dir1\file1.txt", @"localPath\dir1\file2.txt" })]
    [InlineData("01-05.新規ファイル－複数ディレクトリ", new string[] { "s3Path/dir1/file1.txt", "s3Path/dir2/file2.txt", "s3Path/dir1/", "s3Path/dir2/" }, new string[] { }, new[] { @"localPath\dir1", @"localPath\dir1\file1.txt", @"localPath\dir2", @"localPath\dir2\file2.txt" })]
    [InlineData("01-06.新規ファイル‐単一ファイル日本語ファイル名", new string[] { "s3Path/ファイル１.txt" }, new string[] { }, new[] { @"localPath\ファイル１.txt" })]
    [InlineData("01-07.新規ファイル‐単一ファイル日本語ディレクトリ名", new string[] { "s3Path/ディレクトリ１/ファイル１.txt", "s3Path/ディレクトリ１/" }, new string[] { }, new[] { @"localPath\ディレクトリ１", @"localPath\ディレクトリ１\ファイル１.txt" })]
    [InlineData("01-08.新規ファイル‐単一ファイルスペース", new string[] { "s3Path/file 1.txt" }, new string[] { }, new[] { @"localPath\file 1.txt" })]
    [InlineData("01-09.新規ファイル‐単一ファイル全角スペース", new string[] { "s3Path/file　1.txt" }, new string[] { }, new[] { @"localPath\file　1.txt" })]
    [InlineData("02-01.空ディレクトリ－単一ディレクトリ", new string[] { "s3Path/dir1/" }, new string[] { }, new[] { @"localPath\dir1" })]
    [InlineData("02-02.空ディレクトリ－ファイルありディレクトリ、空ディレクトリ混合", new string[] { "s3Path/dir1/", "s3Path/dir2/file2.txt", "s3Path/dir2/", }, new string[] { }, new[] { @"localPath\dir1", @"localPath\dir2", @"localPath\dir2\file2.txt" })]
    [InlineData("02-03.空ディレクトリ－単一ディレクトリ日本語", new string[] { "s3Path/ディレクトリ１/" }, new string[] { }, new[] { @"localPath\ディレクトリ１" })]
    [InlineData("03-01.残ファイルあり－複数ディレクトリ", new string[] { "s3Path/dir1/file1.txt", "s3Path/dir2/file2.txt", "s3Path/dir1/", "s3Path/dir2/" }, new string[] { "localPath/dir1/file3.txt" }, new[] { @"localPath\dir1", @"localPath\dir1\file1.txt", @"localPath\dir2", @"localPath\dir2\file2.txt" })]

    public async Task RestoreFolderAsync_Test(string name, string[] preS3Files, string[] preLocalFiles, string[] exptLocalFiles)
    {
        // Arrange
        var localPath = "localPath";
        var s3Path = "s3Path";

        // テストケース名をログに出力
        Console.WriteLine($"Running test case: {name}");

        // Create local files
        if (Directory.Exists(localPath))
        {
            Directory.Delete(localPath, true);
        }
        Directory.CreateDirectory(localPath);
        foreach (var file in preLocalFiles)
        {
            if (file.EndsWith("/"))
            {
                Directory.CreateDirectory(file);
                continue;
            }
            if (!Directory.Exists(Path.GetDirectoryName(file)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(file));
            }
            File.WriteAllText(file, "test content");
        }

        // Delete bucket if it exists
        if (await DoesBucketExistAsync(BucketName))
        {
            await CleanupBucketAsync(BucketName);
        }

        // Create bucket
        await s3Client.PutBucketAsync(new PutBucketRequest { BucketName = BucketName });

        // Upload pre-existing S3 files
        foreach (var file in preS3Files)
        {
            if (file.EndsWith("/"))
            {
                await s3Client.PutObjectAsync(new PutObjectRequest
                {
                    BucketName = BucketName,
                    Key = file,
                    ContentBody = ""
                });
            }
            else
            {
                await s3Client.PutObjectAsync(new PutObjectRequest
                {
                    BucketName = BucketName,
                    Key = file,
                    ContentBody = "test content"
                });
            }
        }

        // Act
        await s3SyncService.RestoreFolderAsync(s3Path, localPath);

        // Assert
        var actualLocalFiles = Directory.GetFileSystemEntries(localPath, "*.*", SearchOption.AllDirectories);
        Assert.Equal(exptLocalFiles.OrderBy(o => o), actualLocalFiles.OrderBy(o => o));

        // Cleanup
        Directory.Delete(localPath, true);
        await CleanupBucketAsync(BucketName);
    }

    public static IEnumerable<object[]> GetSource_RestoreFolderAsync_CheckFileContent()
    {
        yield return new object[] { "01-01.新規ファイル－単一ファイル", new FileData[] { new FileData("s3Path/file1.txt", "テスト１") }, new FileData[] { new FileData("localPath/file1.txt", "テスト１") }};
        yield return new object[] { "01-02.新規ファイル－複数ファイル", new FileData[] { new FileData("s3Path/file1.txt", "テスト１"), new FileData("s3Path/file2.txt", "テスト２") }, new FileData[] { new FileData("localPath/file1.txt", "テスト１"), new FileData("localPath/file2.txt", "テスト２") }};
    }

    [Theory(DisplayName = "RestoreFolderAsyncファイル内容"),
    MemberData(nameof(GetSource_RestoreFolderAsync_CheckFileContent))]
    public async Task RestoreFolderAsync_CheckFileContent(String name, FileData[] preS3Files, FileData[] exptLocalFiles)
    {
        // Arrange
        var localPath = "localPath";
        var s3Path = "s3Path";

        // テストケース名をログに出力
        Console.WriteLine($"Running test case: {name}");

        // Create local directory
        Directory.CreateDirectory(localPath);

        // Delete bucket if it exists
        if (await DoesBucketExistAsync(BucketName))
        {
            await CleanupBucketAsync(BucketName);
        }

        // Create bucket
        await s3Client.PutBucketAsync(new PutBucketRequest { BucketName = BucketName });

        // Upload pre-existing S3 files
        foreach (var file in preS3Files)
        {
            if (file.Path.EndsWith("/"))
            {
                await s3Client.PutObjectAsync(new PutObjectRequest
                {
                    BucketName = BucketName,
                    Key = file.Path,
                    ContentBody = ""
                });
            }
            else
            {
                await s3Client.PutObjectAsync(new PutObjectRequest
                {
                    BucketName = BucketName,
                    Key = file.Path,
                    ContentBody = file.Content
                });
            }
        }

        // Act
        await s3SyncService.RestoreFolderAsync(s3Path, localPath);

        // Assert
        foreach (var file in exptLocalFiles)
        {
            if (file.Path.EndsWith("/"))
            {
                Assert.True(Directory.Exists(file.Path));
                continue;
            }
            Assert.True(File.Exists(file.Path));
            Assert.Equal(file.Content, File.ReadAllText(file.Path));
        }

        // Cleanup
        Directory.Delete(localPath, true);
        await CleanupBucketAsync(BucketName);
    }
}
