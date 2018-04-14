using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace NChinese.Phonetic
{
    /// <summary>
    /// 此類別用來查詢中文字的注音字根。
    /// </summary>
    internal class CharZhuyinTable
    {
        private Dictionary<string, CharZhuyinInfo> m_MostFreqChars;   // 常用字
        private Dictionary<string, CharZhuyinInfo> m_SecondFreqChars; // 次常用字
        private Dictionary<string, CharZhuyinInfo> m_LeastFreqChars;  // 罕用子
        private Dictionary<string, CharZhuyinInfo>[] m_CharListsByFreq;   // 指向各個字元串列的陣列.

        private char[] sepChars = { ' ' };

        public CharZhuyinTable()
        {
            m_MostFreqChars = new Dictionary<string, CharZhuyinInfo>();
            m_SecondFreqChars = new Dictionary<string, CharZhuyinInfo>();
            m_LeastFreqChars = new Dictionary<string, CharZhuyinInfo>();
            
            m_CharListsByFreq = new Dictionary<string,CharZhuyinInfo>[3];
            m_CharListsByFreq[0] = m_MostFreqChars;
            m_CharListsByFreq[1] = m_SecondFreqChars;
            m_CharListsByFreq[2] = m_LeastFreqChars;
        }

        #region 載入函式


        /// <summary>
        /// 從組件資源中載入對照表。
        /// </summary>
        public void Load()
        {
            Assembly asmb = Assembly.GetExecutingAssembly();
            string resourceName = GetType().FullName + ".txt"; 
            Stream stream = asmb.GetManifestResourceStream(resourceName);
            if (stream == null)
            {
                throw new Exception("CharZhuyinTable.Load 找不到資源: " + resourceName);
            }
            using (stream)
            {
                using (StreamReader sr = new StreamReader(stream, Encoding.UTF8))
                {
                    Load(sr);
                }
            }
        }

        /// <summary>
        /// 從檔案載入注音資料。注意此檔案必須是 UTF-8 編碼。
        /// </summary>
        /// <param name="fileName"></param>
        public void Load(string fileName)
        {
            using (StreamReader sr = new StreamReader(fileName, Encoding.UTF8))
            {
                Load(sr);
            }
        }

        /// <summary>
        /// 從串流中讀取對照表。
        /// </summary>
        /// <param name="sr"></param>
        public void Load(StreamReader sr)
        {
            string line = sr.ReadLine();
            while (line != null)
            {
                line = line.Trim();
                if (line.Length > 0 && line[0] != ';')
                {
                    ParseLine(line);
                }
                line = sr.ReadLine();
            }
        }

        /// <summary>
        /// 剖析傳入的字串，取出其中的中文字元、使用頻率、以及注音字根（按鍵）。
        /// </summary>
        /// <param name="line"></param>
        private void ParseLine(string line)
        {
            string[] fields = line.Split(sepChars);
            if (fields.Length < 3)
                return;

            CharZhuyinInfo charInfo = new CharZhuyinInfo
            {
                Character = fields[0] // 中文字元
            };

            int freq = Convert.ToInt32(fields[1]); // 使用頻率 (0/1/2)
            if (freq >= m_CharListsByFreq.Length)  // 防錯：修正使用頻率.
            {
                freq = m_CharListsByFreq.Length - 1;
            }
            charInfo.Frequence = (ChineseCharFrequence)freq;

            for (int i = 2; i < fields.Length; i++) // 注音字根串列
            {
                charInfo.Phonetics.Add(fields[i]);
            }                        

            // 根據使用頻率加入對應的串列.
            m_CharListsByFreq[freq].Add(charInfo.Character, charInfo);
        }

        #endregion

        public void Clear()
        {
            foreach (Dictionary<string, CharZhuyinInfo> charList in m_CharListsByFreq)
            {
                charList.Clear();
            }
        }

        /// <summary>
        /// 尋找指定的漢字。
        /// </summary>
        /// <param name="aChar">字元。</param>
        /// <param name="freqLimit">限定使用頻率。例如：若傳入 1，則只查詢使用頻率代碼 0～1 的字元（常用字與次常用字）。合法值域是 0～2。</param>
        /// <returns>傳回該漢字的 CharZhuyinInfo 物件。若找不到則傳回 null。</returns>
        public CharZhuyinInfo Find(string aChar, ChineseCharFrequence freqLimit)
        {
            CharZhuyinInfo charInfo = null;
            int freqMax = (int)freqLimit;

            for (int freqIdx = 0; freqIdx < m_CharListsByFreq.Length; freqIdx++)
            {
                if (freqIdx > freqMax)
                {
                    break;
                }
                Dictionary<string, CharZhuyinInfo> charList = m_CharListsByFreq[freqIdx];
                if (charList.TryGetValue(aChar, out charInfo))
                {
                    break;
                }
            }
            return charInfo;
        }

        /// <summary>
        /// 尋找指定的漢字。
        /// </summary>
        /// <param name="aChar"></param>
        /// <returns>傳回該漢字的 CharZhuyinInfo 物件。若找不到則傳回 null。</returns>
        public CharZhuyinInfo Find(string aChar)
        {
            return Find(aChar, ChineseCharFrequence.Rare);
        }
    }
}
