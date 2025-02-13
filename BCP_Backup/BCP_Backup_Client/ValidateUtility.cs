using System;
using System.Text.RegularExpressions;

namespace BCP_Backup_Client
{
    public class ValidateUtility
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
        /// <summary>
        /// 引数の文字列が半角英数字記号のみで構成されているかを調べる。
        /// </summary>
        /// <param name="text">チェック対象の文字列。</param>
        /// <returns>引数が英数字のみで構成されていればtrue、そうでなければfalseを返す。</returns>
        public static bool IsAlphaNumericSymbol(string text)
        {
            if (IsHalfWidthChar(text))
            {
                if (!IsHalfKatakana(text))
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 半角文字チェック
        /// </summary>
        /// <param name="val">対象文字列</param>
        /// <returns>true:半角文字のみ　false:全角文字を含む</returns>
        public static bool IsHalfWidthChar(string str)
        {
            // nullの場合はfalseを返す
            if (str == null)
            {
                return false;
            }
            // 半角文字チェック
            return new Regex("^[ -~｡-ﾟ]*$").IsMatch(str);
        }
        /// <summary>
        /// 文字列が半角カタカナ（句読点～半濁点）かどうかを判定します
        /// </summary>
        /// <param name="target">対象の文字列</param>
        /// <returns>文字列が半角カタカナ（句読点～半濁点）の場合はtrue、それ以外はfalse</returns>
        public static bool IsHalfKatakana(string target)
        {
            return new Regex(@"[\uFF61-\uFF9F]").IsMatch(target);
        }
        /// <summary>
        /// 全角文字チェック
        /// </summary>
        /// <param name="val">対象文字列</param>
        /// <returns>true:全角文字のみ　false:半角文字を含む</returns>
        public static bool IsFullWidthChar(string str)
        {
            // nullの場合はfalseを返す
            if (str == null)
            {
                return false;
            }
            // 全角文字チェック
            return (!Regex.IsMatch(str, @"[ -~｡-ﾟ]"));
        }
        /// <summary>
        /// 指定されたファイル名に不正がないかチェックする
        /// </summary>
        /// <param name="fileName">チェックするファイル名。</param>
        /// <returns>不正がないと判断された場合は、True。それ以外は、False。</returns>
        public static bool IsValidFileName(string fileName)
        {
            try
            {
                //GetFullPathメソッドに渡して、例外がスローされるか確かめる
                System.IO.Path.GetFullPath(fileName);
            }
            catch (ArgumentException)
            {
                //ファイル名に不正な文字が含まれている場合など
                return false;
            }
            catch (System.Security.SecurityException)
            {
                //アクセスできない場合
                return false;
            }
            catch (NotSupportedException)
            {
                //ボリューム識別子以外に「:」がある場合
                return false;
            }
            catch (System.IO.PathTooLongException)
            {
                //パス名が長すぎる場合
                return false;
            }
            return true;
        }
    }
}
