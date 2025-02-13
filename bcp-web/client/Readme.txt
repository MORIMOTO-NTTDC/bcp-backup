オンライン画面側の資材はビルドが必要です。
ビルドは以下のコマンドで行い、destフォルダに出力されます。


A.画面プロトを作る場合（引数に"DEBUG"を付ける）

build.cmd DEBUG


B.本番用を作る場合（引数なし）

build.cmd


※本番用のbuildを実行すると、プロジェクトの src\main\webapp\resources に実行に必要なすべてのリソースがコピーされます。