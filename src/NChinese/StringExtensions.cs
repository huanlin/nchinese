using System;

namespace NChinese
{
    public static class StringExtensions
    {
        /// <summary>
        /// 判斷傳入的字元是否為ㄅㄆㄇㄈ（注意不含讀音符號）。
        /// </summary>
        /// <param name="aChar">類行為 String 的中文字元（unicode 字元不能以 char）</param>
        /// <returns></returns>
        public static bool IsBopomofo(this string aChar)
        {
            if (String.IsNullOrEmpty(aChar))
                return false;
            int code = Char.ConvertToUtf32(aChar, 0);
            if (code >= 0x3105 && code <= 0x3129) // ㄅㄆㄇㄈ
                return true;
            return false;
        }

        /// <summary>
        /// 判斷傳入的字元是否為中日韓表意文字（CJK Ideographs；aka Unihan）。注意不含注音符號ㄅㄆㄇㄈ。
        /// </summary>
        /// <param name="aChar"></param>
        /// <returns></returns>
        public static bool IsUnihan(this string aChar)
        {
            if (String.IsNullOrEmpty(aChar))
                return false;
            int code = Char.ConvertToUtf32(aChar, 0);
            if (code >= 0x4e00 && code <= 0x9fff) // CJK Unified Ideographs （中日韓統一表意文字）
                return true;
            else if (code >= 0x3400 && code <= 0x4dbf)   // CJK Unified Ideographs Extension A
                return true;
            else if (code >= 0x20000 && code <= 0x2a6df) // CJK Unified Ideographs Extension B
                return true;
            else if (code >= 0x2a700 && code <= 0x2b73f) // CJK Unified Ideographs Extension C
                return true;
            else if (code >= 0x2b740 && code <= 0x2b81f) // CJK Unified Ideographs Extension D
                return true;
            else if (code >= 0x2b820 && code <= 0x2ceaf) // CJK Unified Ideographs Extension E
                return true;
            else if (code >= 0x2ceb0 && code <= 0x2ebef) // CJK Unified Ideographs Extension F
                return true;
            else if (code >= 0xf900 && code <= 0xfaff) // 中日韓兼容表意文字
                return true;
            return false;
        }

    }
}
