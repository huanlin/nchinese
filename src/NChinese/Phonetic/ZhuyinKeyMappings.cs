using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NChinese.Phonetic;

/// <summary>
/// 注音符號與鍵盤按鍵的對應。
/// </summary>
internal static class ZhuyinKeyMappings
{
    // 音調按鍵（標準鍵盤對應），一聲以空白代表。
    internal const string StdToneKeys = "7 634";
    internal const string EtenToneKeys = "1 234";

    // 鍵盤對應的按鍵字元（注意最後面五個字元是輕聲、一聲、二聲、三聲、四聲）。
    internal const string StdKeys = "1qaz2wsxedcrfv5tgbyhnujm8ik,9ol.0p;/-" + StdToneKeys;   // 標準鍵盤
    internal const string EtenKeys = "bpmfdtnlvkhg7c,./j;'sexuaorwiqzy890-=" + EtenToneKeys; // 倚天鍵盤

    // 按鍵字元所對應的注音符號（聲符若看不清楚，可貼到記事本查看）
    private const string ZhuyinSymbols =
        "ㄅㄆㄇㄈㄉㄊㄋㄌㄍㄎㄏㄐㄑㄒㄓㄔㄕㄖㄗㄘㄙㄧㄨㄩㄚㄛㄜㄝㄞㄟㄠㄡㄢㄣㄤㄥㄦ˙\x02c9ˊˇˋ";

    private static Dictionary<char, char> m_KeySymbolTable; // 按鍵與符號對應表.

    static ZhuyinKeyMappings()
    {
        // 建立對應表
        m_KeySymbolTable = new Dictionary<char, char>();
        for (int i = 0; i < StdKeys.Length; i++)
        {
            m_KeySymbolTable.Add(StdKeys[i], ZhuyinSymbols[i]);
        }
    }

    /// <summary>
    /// 將指定的注音按鍵字元轉換成注音符號字元。
    /// </summary>
    /// <param name="zhuyinKey"></param>
    /// <returns></returns>
    public static char KeyToSymbol(char zhuyinKey)
    {
        char symbol;

        if (m_KeySymbolTable.TryGetValue(zhuyinKey, out symbol))
            return symbol;
        throw new Exception("無法將取得按鍵字元 [" + zhuyinKey.ToString() + "] 所對應的注音符號!");
    }

    /// <summary>
    /// 將指定的注音按鍵字元轉換成注音符號字元。
    /// </summary>
    /// <param name="keys"></param>
    /// <returns></returns>
    public static string KeyToSymbol(string zhuyinKeys)
    {
        char[] symbols = new char[zhuyinKeys.Length];
        for (int i = 0; i < zhuyinKeys.Length; i++)
        {
            symbols[i] = KeyToSymbol(zhuyinKeys[i]);
        }
        return new String(symbols);
    }
}
