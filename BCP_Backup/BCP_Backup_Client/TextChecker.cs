using System.Text.RegularExpressions;

namespace BCP_Backup_Client
{
    public class TextChecker
    {
        /// <summary>
        /// 引数の文字列に値（半角スペースのみ除く）が設定されているかを調べる。
        /// </summary>
        /// <param name="text">チェック対象の文字列。</param>
        /// <returns>引数に値（半角スペースのみ除く）があればtrue、そうでなければfalseを返す。</returns>
        public static bool IsRequire(string text)
        {
            if (text == null || text == string.Empty)
            {
                return false;
            }
            return (text.Trim().Length > 0);
        }
        /// <summary>
        /// 引数の文字列が半角数字のみで構成されているかを調べる。
        /// </summary>
        /// <param name="text">チェック対象の文字列。</param>
        /// <returns>引数が数字のみで構成されていればtrue、そうでなければfalseを返す。</returns>
        public static bool IsNumeric(string text)
        {
            // 文字列の先頭から末尾までが、数字のみとマッチするかを調べる。
            return (Regex.IsMatch(text, @"^[0-9]+$"));
        }
        /// <summary>
        /// 引数の文字列が半角英字のみで構成されているかを調べる。
        /// </summary>
        /// <param name="text">チェック対象の文字列。</param>
        /// <returns>引数が英字のみで構成されていればtrue、そうでなければfalseを返す。</returns>
        public static bool IsAlpha(string text)
        {
            // 文字列の先頭から末尾までが、英字のみとマッチするかを調べる。
            return (Regex.IsMatch(text, @"^[a-zA-Z]+$"));
        }
        /// <summary>
        /// 引数の文字列が半角英数字のみで構成されているかを調べる。
        /// </summary>
        /// <param name="text">チェック対象の文字列。</param>
        /// <returns>引数が英数字のみで構成されていればtrue、そうでなければfalseを返す。</returns>
        public static bool IsAlphaNumeric(string text)
        {
            // 文字列の先頭から末尾までが、英数字のみとマッチするかを調べる。
            return (Regex.IsMatch(text, @"^[0-9a-zA-Z]+$"));
        }
    }
}
