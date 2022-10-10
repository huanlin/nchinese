using System;
using System.Globalization;

namespace NChinese;

public static class StringExtensions
{
    /// <summary>
    /// 判斷傳入的字元是否為ㄅㄆㄇㄈ（注意不含讀音符號）。
    /// </summary>
    /// <param name="aChar">類行為 String 的中文字元（unicode 字元不能以 char）</param>
    /// <returns></returns>
    public static bool IsZhuyinSymbol(this string aChar)
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
    public static bool IsCJK(this string aChar)
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

    /// <summary>
    /// 從輸入字串中尋找第一組連續的中日韓表意文字，並傳回那組文字的起始和結束索引。
    /// </summary>
    /// <param name="input"></param>
    /// <returns>若有找到連續的 CJK 字元，傳回那組文字的起始和結束索引。若沒找到，則傳回的起始索引和結束索引都是 -1。</returns>
    public static (int StartIndex, int StopIndex) FindConsecutiveUnihan(this string input)
    {
        int start = -1;
        int stop = -1;

        var charEnum = StringInfo.GetTextElementEnumerator(input);
        while (charEnum.MoveNext())
        {
            string text = charEnum.GetTextElement();
            if (text.IsCJK())
            {
                if (start < 0)
                {
                    start = charEnum.ElementIndex;
                }
                stop = charEnum.ElementIndex;
            }
            else
            {
                if (start >= 0)
                {
                    break;
                }
            }
        }
        return (start, stop);
    }
}
