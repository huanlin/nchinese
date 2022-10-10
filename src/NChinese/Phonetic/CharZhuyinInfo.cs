using System.Collections.Generic;

namespace NChinese.Phonetic
{
    /// <summary>
    /// 字元與其所屬的注音串列。
    /// </summary>
    public sealed class CharZhuyinInfo
    {        
        public CharZhuyinInfo() 
        {
            Character = "";
            Phonetics = new List<string>();
            Frequence = ChineseCharFrequence.Common;
        }
        
        public string Character { get; set; } // 注意：Unicode 字元必須以 string 儲存，不可用 char !! (搜尋: surrogate pairs)

        /// <summary>
        /// 注音字根串列。
        /// </summary>
        public List<string> Phonetics { get; private set; }

        /// <summary>
        /// 是否為多音字.
        /// </summary>
        public bool IsPolyphonic
        {
            get
            {
                return Phonetics.Count > 1;
            }
        }

        /// <summary>
        /// 使用頻率代碼：0/1/2 = 常用/次常用/罕用。
        /// </summary>
        public ChineseCharFrequence Frequence { get; set; }
    }
}
