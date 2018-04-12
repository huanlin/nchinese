using System.Collections.Generic;

namespace NChinese.Phonetic
{
    /// <summary>
    /// 字元與其所屬的注音串列。
    /// </summary>
    public class CharZhuyinInfo
    {
        private string theChar;     // 記住：Unicode 字元必須以 string 儲存，不可用 char !! (搜尋: surrogate pairs)
        private List<string> phonetics; // 注音字根串列
        private ChineseCharFrequence freq;   // 使用頻率代碼：0/1/2 = 常用/次常用/罕用。

        public CharZhuyinInfo() 
        {
            theChar = "";
            phonetics = new List<string>();
            freq = ChineseCharFrequence.Common;
        }

        public string Character 
        { 
            get { return theChar; }
            set { theChar = value; }
        }

        public List<string> Phonetics
        {
            get { return phonetics; }
        }

        /// <summary>
        /// 是否為多音字.
        /// </summary>
        public bool IsPolyphonic
        {
            get
            {
                return phonetics.Count > 1;
            }
        }

        public ChineseCharFrequence Frequence
        {
            get
            {
                return freq;
            }

            set
            {
                freq = value;
            }
        }
    }
}
