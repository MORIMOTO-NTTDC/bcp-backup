# BCP Backup

## 説明

BCP Backupは、バックアップを行うためのソフトウェアです。  
バックアップを行うサービスとその設定するためのクライアントで構成されます。

## インストール

管理者ユーザーでコマンドプロンプトを開き、以下のコマンドを実行します。

```cmd
InstallUtil.exe .\BCP_Backup_Service.exe
```

以下のメッセージが表示されたら、インストールが完了です。

```cmd
コミット段階が正常に終了しました。

トランザクション インストールが完了しました。
```

* 自動（遅延）起動の設定

```cmd
sc config BCP_Backup_Service start= delayed-auto
```
注）「自動（遅延実行）」は、Windowsサービスのスタートアップの種類の一つです。この設定により、システム起動時にサービスが自動的に開始されますが、他の自動スタートのサービスがすべて開始された後に遅れて開始されます。これにより、システムの起動時間を短縮し、重要なサービスが先に起動することを優先できます。



## アンインストール

管理者ユーザーでコマンドプロンプトを開き、以下のコマンドを実行します。

```bash
InstallUtil.exe /u .\BCP_Backup_Service.exe
```

以下のメッセージが表示されたら、アンインストールが完了です。

```bash
サービス 'BCP_Backup_Service' は正常にシステムから削除されました。

アンインストールか完了しました。
```

## 使い方

### サービスの起動

管理者ユーザーでコマンドプロンプトを開き、以下のコマンドを実行します。

```bash
net start BCP_Backup_Service
```

i以下のメッセージが表示されたら、サービスが起動されています。

```bash
BCP Backup Service サービスを開始します.
BCP Backup Service サービスは正常に開始されました。
```


### サービスの停止

管理者ユーザーでコマンドプロンプトを開き、以下のコマンドを実行します。

```bash
net stop BCP_Backup_Service
```

以下のメッセージが表示されたら、サービスが停止されています。

```bash
BCP Backup Service サービスを停止中です.
BCP Backup Service サービスは正常に停止されました。
```

### ローカルテスト用の設定

#### AWS CLI のインストール

https://docs.aws.amazon.com/ja_jp/cli/latest/userguide/getting-started-install.html

#### MinIOのインストール、起動

https://min.io/docs/minio/windows/index.html

minio.exe,mc.exeをダウンロードし、任意のフォルダに配置します。
また、環境変数pathに配置したフォルダを追加します。
ex) C:\bin

``` cmd
minio.exe server C:\minio --console-address :9001
```

#### プロファイル、エンドポイントの設定

* プロファイルの設定
```cmd
aws configure set aws_access_key_id  minioadmin --profile minio
aws configure set aws_secret_access_key  minioadmin --profile minio
aws configure set region ap-northeast-1 --profile minio
aws configure set s3.signature_version s3v4 --profile minio
```

※ access_key、secret_keyはMinIOの設定に合わせて変更してください。

* デフォルトプロファイル、エンドポイントの設定
```cmd
set AWS_DEFAULT_PROFILE=minio
set AWS_ENDPOINT_URL_S3=http://127.0.0.1:9000
set AWS_ENDPOINT_URL_STS=http://127.0.0.1:9000
```
※ AWS_ENDPOINT_URL_S3はMinIOの設定に合わせて変更してください。

#### 動作確認
```
>aws s3 mb s3://test-bucket
make_bucket: test2

>aws s3 cp README.md s3://test-bucket
upload: .\README.md to s3://test/README.md

>aws s3 ls s3://test-bucket
2021-07-07 16:00:00       1024 README.md
```
※ > はコマンドプロンプトのプロンプトです。コマンドプロンプトに直接入力してください。  
　下の行は、実行結果です。正常であれば、同じような結果が表示されます。

#### バケットの削除
```cmd
> aws s3 ls
2024-09-11 23:12:38 test-bucket

> aws s3 rb s3://test-bucket --force
delete: s3://test-bucket/s3Path/file1.txt
delete: s3://test-bucket/s3Path/file2.txt
remove_bucket: test-bucket

> aws s3 ls
（何も表示されない or 他のバケットが表示される）
```

#### レジストリの設定

* レジストリの設定
```cmd
REG ADD HKLM\SOFTWARE\bcpSoft /v S3BACKET /d ut-bucket
REG ADD HKLM\SOFTWARE\bcpSoft /v accessKeyId /d minioadmin 
REG ADD HKLM\SOFTWARE\bcpSoft /v secretAccessKey /d minioadmin 
REG ADD HKLM\SOFTWARE\bcpSoft /v sessionToken /d test
REG ADD HKLM\SOFTWARE\bcpSoft /v S3_ENDPOINT /d http://127.0.0.1:9000
REG ADD HKLM\SOFTWARE\bcpSoft /v ACCOUNT_ID /d testuser 
REG ADD HKLM\SOFTWARE\bcpSoft /v LOCAL_DIR /d C:\BCP\localdir 
```
* レジストリの確認
```cmd
REG QUERY HKLM\SOFTWARE\bcpSoft
```

ポリシーの設定
参考：https://qiita.com/y_k/items/9c134f7be0263d64a89b
注）policy系のコマンドは非推奨となっていたので差し替えた。
これが何で必要なのか？不明。
```cmd
mc alias set vender http://127.0.0.1:9000 minioadmin minioadmin
mc admin policy create vender vender-policy vender-policy.json
mc admin user add vender vender01 vender123
mc admin policy attach vender vender-policy --user vender01
```

認証情報を取得
```cmd
aws sts assume-role ^
--role-arn "arn:xxx:xxx:xxx:xxxx" ^
--role-session-name "anything" ^
--policy "{\"Version\":\"2012-10-17\",\"Statement\":[{\"Action\":[\"s3:GetObject\",\"s3:PutObject\",\"s3:ListBucket\",\"s3:DeleteObject\"],\"Effect\":\"Allow\",\"Resource\":[\"arn:aws:s3:::vender-01-bucket/*\"],\"Sid\":\"\"}]}" ^
--duration-seconds 43600
```

実行例
```cmd
{
    "Credentials": {
        "AccessKeyId": "X3G3RHT51B8IFQTFN0A9",
        "SecretAccessKey": "LL24MsIBhyiojfCn8NgFii2R+aRPxcWWDzcglKsE",
        "SessionToken": "eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJhY2Nlc3NLZXkiOiJYM0czUkhUNTFCOElGUVRGTjBBOSIsImV4cCI6MTcyODI2OTcwMCwicGFyZW50IjoibWluaW9hZG1pbiIsInNlc3Npb25Qb2xpY3kiOiJleUpXWlhKemFXOXVJam9pTWpBeE1pMHhNQzB4TnlJc0lsTjBZWFJsYldWdWRDSTZXM3NpUldabVpXTjBJam9pUVd4c2IzY2lMQ0pCWTNScGIyNGlPbHNpY3pNNlIyVjBUMkpxWldOMElpd2ljek02VEdsemRFSjFZMnRsZENJc0luTXpPbEIxZEU5aWFtVmpkQ0pkTENKU1pYTnZkWEpqWlNJNld5SmhjbTQ2WVhkek9uTXpPam82ZG1WdVpHVnlMVEF4TFdKMVkydGxkQzhxSWwxOVhYMD0ifQ.FW2ees0NIWqBsepXGoyCQvUS6Yn-hTZxXO9TQcyWpLcqRhfzBFcImAs5BGSjDMPkSndMtPZ9dd9w6ZHb072YoA",
        "Expiration": "2024-10-07T02:55:00+00:00"
    },
    "AssumedRoleUser": {
        "Arn": ""
    }
}
```

* accessKeyId, secretAccessKey, sessionTokenをレジストリに設定
```cmd
REG ADD HKLM\SOFTWARE\bcpSoft /f /v accessKeyId /d 1LPBN7B3QDOEZ1L8JD4K 
REG ADD HKLM\SOFTWARE\bcpSoft /f /v secretAccessKey /d rS1UFfC9EfRysX+ClHJKnxntRrjrHDcDuAYlc9Oi
REG ADD HKLM\SOFTWARE\bcpSoft /f /v sessionToken /d eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJhY2Nlc3NLZXkiOiIxTFBCTjdCM1FET0VaMUw4SkQ0SyIsImV4cCI6MTcyOTEyOTc5OCwicGFyZW50IjoibWluaW9hZG1pbiIsInNlc3Npb25Qb2xpY3kiOiJleUpXWlhKemFXOXVJam9pTWpBeE1pMHhNQzB4TnlJc0lsTjBZWFJsYldWdWRDSTZXM3NpUldabVpXTjBJam9pUVd4c2IzY2lMQ0pCWTNScGIyNGlPbHNpY3pNNlVIVjBUMkpxWldOMElpd2ljek02UjJWMFQySnFaV04wSWl3aWN6TTZUR2x6ZEVKMVkydGxkQ0pkTENKU1pYTnZkWEpqWlNJNld5SmhjbTQ2WVhkek9uTXpPam82ZG1WdVpHVnlMVEF4TFdKMVkydGxkQzhxSWwxOVhYMD0ifQ.nd_7f0dYhyFI7ugCF8tZAUH9MKHBdkj2pSepXn176f0QyNKyjNvZ_zpG4dB-Y_PJ143sHXu2YnP41OxPJamtag
```

WriteCredentialToReg.batを実行することで設定することもできます。

* s3の確認
```cmd
aws s3 ls vender-01-bucket --recursive
```

* stubの起動
```cmd  
cd c:\Users\USE\Desktop\BCP_WebAPI_Stub\src
go run main.go
```


* PERMIT_PASSWORDの設定
```cmd
BCP_Backup_gen_password.exe password
REG ADD HKLM\SOFTWARE\bcpSoft /f /v PERMIT_PASSWORD /d 5e884898da28047151d0e56f8dc6292773603d0d6aabbdd62a11ef721d1542d8
REG ADD HKLM\SOFTWARE\bcpSoft /f /v accessKeyId /d CSYRW7GLHOCG1AEF9R50
```

* 1GByteのファイルの作成
```cmd
fsutil file createnew testfile.dat 1073741824
```
1 * 1024 * 1024 * 1024 = 1073741824

* テストデータ作成
```cmd  
aws s3 rm s3://vender-01-bucket --recursive

echo "This is test file 1" > testfile1.txt
echo "This is test file 2" > testfile2.txt
echo "This is test file 3" > testfile3.txt

aws s3 cp testfile1.txt s3://vender-01-bucket/1234567890/
aws s3 cp testfile2.txt s3://vender-01-bucket/1234567890/
aws s3 cp testfile3.txt s3://vender-01-bucket/1234567890/
```

* バックアップソースツール
backup_source.batを実行すると、バックアップソースが作成されます。
本ソリューションの隣のフォルダにoldフォルダが作成され、その中にバックアップソースが作成されます。  
自動的に日付_通番の形のフォルダが作成されます。  
ソリューションのフォルダで実行してください。
```cmd
backup_source.bat                
backup_source_exclude.list
```

実行イメージ
```cmd
本ソリューションのフォルダ
old
└20241028_01
  └BCP_Backup
  └BCP_Backup.zip
```

* パスワード生成ツール
```cmd  
BCP_Backup_gen_password.exe  
```

実行イメージ
```cmd  
>BCP_Backup_gen_password.exe password123
ef92b778bafe771e89245b89ecbc08a44a4e166c06659911881f383d4473e94f
```

* リリース用ZIP作成
make_release_zip.batを実行すると、リリースzip作成されます。  
本ソリューションの隣のフォルダにreleaseフォルダが作成され、その中にreleaseZIPが作成されます。
自動的に日付_通番の形のフォルダが作成されます。  
ソリューションのフォルダで実行してください。
```cmd
make_release_zip.bat 
```
実行イメージ
```cmd
本ソリューションのフォルダ
release
└20241028_01
  └client
  └service
  └relase.zip
```

