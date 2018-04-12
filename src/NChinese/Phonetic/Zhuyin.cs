using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace NChinese.Phonetic
{
    // 注音符號的音調。0=輕聲, 1=一聲, 2=二聲, 3=三聲, 4=四聲。
    public enum ZhuyinTone { Tone0, Tone1, Tone2, Tone3, Tone4 };

    /// <summary>
    /// 代表一個中文字的注音符號。
    /// </summary>
    public class Zhuyin
    {
        private ZhuyinTone m_Tone;
        private char[] m_Symbols;   // 固定三個字元長，依序為聲符、介符、韻符（沒有的部分填空白）。


        // 全部的注音符號
        internal const string AllSymbols = "ㄅㄆㄇㄈㄉㄊㄋㄌㄍㄎㄏㄐㄑㄒㄓㄔㄕㄖㄗㄘㄙㄧㄨㄩㄚㄛㄜㄝㄞㄟㄠㄡㄢㄣㄤㄥㄦ";

        // 聲符
        internal const string Consonants = "ㄅㄆㄇㄈㄉㄊㄋㄌㄍㄎㄏㄐㄑㄒㄓㄔㄕㄖㄗㄘㄙ";

        // 介符
        internal const string MiddleVowels = "ㄧㄨㄩ";

        // 韻符
        internal const string EndVowels = "ㄚㄛㄜㄝㄞㄟㄠㄡㄢㄣㄤㄥㄦ";

        // 結合韻
        internal static string[] JoinedVowels = 
        {
            "ㄧㄚ", "ㄧㄛ", "ㄧㄝ", "ㄧㄞ", "ㄧㄠ", "ㄧㄡ", "ㄧㄢ", "ㄧㄣ", "ㄧㄤ", "ㄧㄥ",
            "ㄨㄚ", "ㄨㄛ", "ㄨㄞ", "ㄨㄟ", "ㄨㄢ", "ㄨㄣ", "ㄨㄤ", "ㄨㄥ",
            "ㄩㄝ", "ㄩㄢ", "ㄩㄣ", "ㄩㄥ"
        };

        // 音調符號。一聲符號的 Unicode 數值是 0x02c9，不是全形空白!! （可參考線上全字庫，微軟 IMM API 也是傳回這個字元）。
        public const string ToneCharacters = "˙\x02c9ˊˇˋ";
        public const char Tone0Char = '˙';      // 輕聲
        public const char Tone1Char = '\x02c9'; // 一聲符號

        public Zhuyin()
        {
            m_Tone = ZhuyinTone.Tone1; // 預設為一聲
            m_Symbols = new char[3];
            m_Symbols[0] = ' ';
            m_Symbols[1] = ' ';
            m_Symbols[2] = ' ';
        }

        /// <summary>
        /// 剖析傳入的字串，並建立成注音符號物件。
        /// </summary>
        /// <param name="zhuyinStr">代表某個中文字的注音符號，共四個字符，最後一個是音調符號。可包含全型空白。</param>
        public Zhuyin(string zhuyinStr)
            : this()
        {
            Parse(zhuyinStr);
        }

        public Zhuyin(Zhuyin zy)
            : this()
        {
            if (!zy.Validate())
            {
                throw new Exception("無效的注音符號: " + zy.ToString());
            }

            // 拷貝成員
            this.m_Tone = zy.m_Tone;
            zy.m_Symbols.CopyTo(m_Symbols, 0);
        }

        /// <summary>
        /// 剖析傳入的注音字根，並建立成 Zhuyin 物件傳回。
        /// </summary>
        /// <param name="zhuyinStr">代表某個中文字的注音符號，共四個字符，最後一個是音調符號。可包含全型空白。</param>
        /// <returns></returns>
        private void Parse(string zhuyinStr)
        {
            string s = Zhuyin.FillSpaces(zhuyinStr);

            if (s.Trim().Length < 1)
            {
                throw new Exception("指定的注音符號為空字串!");
            }

            m_Symbols[0] = s[0];
            m_Symbols[1] = s[1];
            m_Symbols[2] = s[2];

            // 取出音調
            m_Tone = ZhuyinTone.Tone1;  // 預設為一聲
            switch (s[3])
            {
                case 'ˊ':
                    m_Tone = ZhuyinTone.Tone2;
                    break;
                case 'ˇ':
                    m_Tone = ZhuyinTone.Tone3;
                    break;
                case 'ˋ':
                    m_Tone = ZhuyinTone.Tone4;
                    break;
                case Tone0Char:
                    m_Tone = ZhuyinTone.Tone0;
                    break;
                default:
                    break;
            }

            if (!Validate())
            {
                throw new Exception("無效的注音符號: " + zhuyinStr);
            }
        }

        /// <summary>
        /// 判斷是否為合法的注音。
        /// </summary>
        /// <returns></returns>
        private bool Validate()
        {
            // 判斷三個音節是否皆為空白
            if (IsEmpty())
                return false;

            // 判斷結合韻
            string s = new string(m_Symbols, 1, 2);
            if (s.Trim().Length == 2)
            {
                bool isValid = false;
                for (int i = 0; i < Zhuyin.JoinedVowels.Length; i++)
                {
                    if (Zhuyin.JoinedVowels[i].Equals(s))
                    {
                        isValid = true;
                        break;
                    }
                }
                if (!isValid)
                    return false;
            }
            return true;
        }

        public override string ToString()
        {
            return this.ToString(true);
        }

        /// <summary>
        /// 傳回注音符號字串。
        /// </summary>
        /// <param name="useSpace">是否使用全型空白填補沒有注音符號字元的部分。</param>
        /// <returns></returns>
        public string ToString(bool useSpace)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < m_Symbols.Length; i++)
            {
                if (Char.IsWhiteSpace(m_Symbols[i]))
                {
                    if (!useSpace)
                        continue;
                    sb.Append("　");    // 全型空白
                }
                else
                {
                    sb.Append(m_Symbols[i]);
                }
            }
            sb.Append(GetToneChar());
            return sb.ToString();
        }

        /// <summary>
        /// 傳回音調符號的字元。
        /// </summary>
        /// <returns></returns>
        private char GetToneChar()
        {
            int i = Convert.ToInt32(m_Tone);
            return Zhuyin.ToneCharacters[i];
        }

        public override bool Equals(object obj)
        {
            Zhuyin zy = (Zhuyin)obj;

            if (this == zy)
                return true;

            if (this.m_Tone != zy.m_Tone)
                return false;
            for (int i = 0; i < m_Symbols.Length; i++)
            {
                if (m_Symbols[i] != zy.m_Symbols[i])
                    return false;
            }
            return true;
        }

        public override int GetHashCode()
        {
            int hash = 0;
            for (int i = 0; i < m_Symbols.Length; i++)
            {
                hash += m_Symbols[i].GetHashCode();
            }
            hash += m_Tone.GetHashCode();

            return hash;
        }

        /// <summary>
        /// 判斷注音符號是否皆為空白。
        /// </summary>
        /// <returns></returns>
        public bool IsEmpty()
        {
            if (m_Symbols[0] == ' ' && m_Symbols[1] == ' ' && m_Symbols[2] == ' ')
                return true;
            return false;
        }

        #region 靜態方法

        /// <summary>
        /// 剖析傳入的注音按鍵字串，並建立 Zhuyin 物件。
        /// </summary>
        /// <param name="zhuyinKeys"></param>
        /// <returns></returns>
        public static Zhuyin ParseKeyString(string zhuyinKeys)
        {
            Zhuyin zy = new Zhuyin();
            string symbols = ZhuyinKeyMappings.KeyToSymbol(zhuyinKeys);
            zy.Parse(symbols);
            return zy;
        }

        /// <summary>
        /// 將傳入的注音符號字串移除空白。
        /// </summary>
        /// <param name="zhuyinStr"></param>
        /// <returns></returns>
        public static string RemoveSpaces(string zhuyinStr)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < zhuyinStr.Length; i++)
            {
                if (!Char.IsWhiteSpace(zhuyinStr[i]))
                {
                    sb.Append(zhuyinStr[i]);
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// 將傳入的注音符號補滿全形空白。
        /// </summary>
        /// <param name="zhuyinStr"></param>
        /// <returns>長度固定為 4 的字串（有可能都是全型空白）。</returns>
        public static string FillSpaces(string zhuyinStr)
        {
            char[] result = new char[] { '　', '　', '　', '　' };    // 四個全型空白

            if (String.IsNullOrEmpty(zhuyinStr))
            {
                return new String(result);
            }

            for (int i = 0; i < zhuyinStr.Length && i < 4; i++)
            {
                char ch = zhuyinStr[i];

                if (IsConsonant(ch))
                {
                    result[0] = ch;
                }
                else if (IsMiddleVowel(ch))
                {
                    result[1] = ch;
                }
                else if (IsEndVowel(ch))
                {
                    result[2] = ch;
                }
                else if (IsTone(ch))
                {
                    result[3] = ch;
                }
            }

            // 如果聲符、介符、韻符任一存在，就必須有音調符號
            if (result[3] == '　')
            {
                if (result[0] != '　' || result[1] != '　' || result[2] != '　')
                {
                    result[3] = Tone1Char;  // 補一聲符號
                }
            }

            return new String(result);
        }

        /// <summary>
        /// 判斷傳入的字元是否為聲符，例如：'ㄅ'。
        /// </summary>
        /// <param name="ch"></param>
        /// <returns></returns>
        public static bool IsConsonant(char ch)
        {
            return (Consonants.IndexOf(ch) >= 0);
        }

        /// <summary>
        /// 判斷傳入的字元是否為介符，例如：'ㄨ'。
        /// </summary>
        /// <param name="ch"></param>
        /// <returns></returns>
        public static bool IsMiddleVowel(char ch)
        {
            return (MiddleVowels.IndexOf(ch) >= 0);
        }

        /// <summary>
        /// 判斷傳入的字元是否為韻符，例如：'ㄣ'。
        /// </summary>
        /// <param name="ch"></param>
        /// <returns></returns>
        public static bool IsEndVowel(char ch)
        {
            return (EndVowels.IndexOf(ch) >= 0);
        }

        /// <summary>
        /// 判斷傳入的字元是否為注音的音調符號。
        /// </summary>
        /// <param name="ch">輸入字元。</param>
        /// <returns>若為音調符號，則傳回 True，否則傳回 False。</returns>
        public static bool IsTone(char ch)
        {
            for (int i = 0; i < Zhuyin.ToneCharacters.Length; i++)
            {
                if (Zhuyin.ToneCharacters[i] == ch)
                    return true;
            }
            return false;
        }


        [Obsolete("請改用 IsBopomofo(char)。")]
        public static bool IsZhuyinSymbol(char ch)
        {
            return (Zhuyin.AllSymbols.IndexOf(ch) >= 0);
        }

        /// <summary>
        /// 判斷傳入的字元是否為注音符號（音調符號不算在內）。
        /// </summary>
        /// <param name="ch">輸入字元。</param>
        /// <returns>若為注音符號，則傳回 True，否則傳回 False。</returns>
        public static bool IsBopomofo(char ch)
        {
            return (Zhuyin.AllSymbols.IndexOf(ch) >= 0);
        }

        /// <summary>
        /// 判斷傳入的字元是否為注音符號（音調符號不算在內）。
        /// </summary>
        /// <param name="ch"></param>
        /// <returns></returns>
        public static bool IsBopomofo(string ch)
        {
            return (Zhuyin.AllSymbols.IndexOf(ch) >= 0);
        }

        /// <summary>
        /// 比較兩個注音符號字串是否相同（全型空白會先去除再比較）。
        /// </summary>
        /// <param name="zhuyin1"></param>
        /// <param name="zhuyin2"></param>
        /// <returns></returns>
        public static bool IsEqual(string zhuyin1, string zhuyin2)
        {
            string s1 = RemoveSpaces(zhuyin1);
            string s2 = RemoveSpaces(zhuyin2);
            return (s1.Equals(s2));
        }

        /// <summary>
        /// 取得指定的注音按鍵字串的音調。
        /// </summary>
        /// <param name="zhuyinStr">注音按鍵字串（標準鍵盤對應）。</param>
        /// <returns></returns>
        public static ZhuyinTone GetTone(string zhuyinKeys)
        {
            int toneIdx = (int)ZhuyinTone.Tone1;  // 找不到就視為一聲。

            for (int i = zhuyinKeys.Length - 1; i >= 1; i--)
            {
                toneIdx = ZhuyinKeyMappings.StdToneKeys.IndexOf(zhuyinKeys[i]);
                if (toneIdx >= 0)
                    break;
            }
            return (ZhuyinTone)toneIdx;
        }

        /// <summary>
        /// 把 WinAPI 反查得到的中文注音符號中的一聲字元（0x02c9）轉換成指定的字串。
        /// </summary>
        /// <param name="zyCode">中文字的注音符號。</param>
        /// <param name="newStr">作為替換的字串。</param>
        /// <returns>轉換後的注音符號字串。</returns>
        public static string ReplaceFirstTone(string zyCode, string newStr)
        {
            if (String.IsNullOrEmpty(zyCode))
                return "";
            return zyCode.Replace("\x02c9", newStr);
        }


        #endregion

        #region 屬性

        public ZhuyinTone Tone
        {
            get { return m_Tone; }
            set { m_Tone = value; }
        }

        public string Value
        {
            get { return ToString(); }
            set
            {
                Parse(value);
            }
        }

        #endregion
    }
}
