using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Threading.Tasks;

namespace CommonLibrary
{
    public class JsonUtilty
    {
        /// <summary>
        /// オブジェクト型の入力値をJSON文字列に変換します。
        /// </summary>
        /// <param name="dict">object型の入力値</param>
        /// <returns>JSON文字列</returns>
        public static string ObjectToJson(object dict)
        {
            var json = JsonSerializer.Serialize(dict, JsonUtilty.GetOption());
            return json;
        }

        /// <summary>
        /// オプションを設定。（内部メソッド）
        /// </summary>
        /// <returns>JsonSerializerOptions型のオプション</returns>
        private static JsonSerializerOptions GetOption()
        {
            // UnicodeのRange指定で日本語も正しく表示、インデントされるように指定
            var options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                WriteIndented = true,
            };
            return options;
        }
    }
}
