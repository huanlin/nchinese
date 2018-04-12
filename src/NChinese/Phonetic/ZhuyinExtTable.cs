using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.IO;

namespace NChinese.Phonetic
{
    /// <summary>
    /// 擴充的注音組字字根對照檔。
    /// 用來弭補 ImmGetConversionList 的不足，例如：「一」和「們」都只傳回一組注音字根。
    /// </summary>
    [Obsolete("此類別已經不需要，若有需要增加的字，請加入 CharZhuyinTable.txt。")]
    public class ZhuyinExtTable
    {
        private static ZhuyinExtTable m_ImmExtTbl = null;

        private Dictionary<char, List<Zhuyin>> m_Table;  // 中文字與注音字根對照表，每一筆代表一個中文字的所有注音字根。

        private ZhuyinExtTable()
        {
            m_Table = new Dictionary<char, List<Zhuyin>>();

            Load();
        }

        /// <summary>
        /// Singleton 建構方法。
        /// </summary>
        /// <returns></returns>
        public static ZhuyinExtTable GetInstance()
        {
            if (m_ImmExtTbl == null)
            {
                m_ImmExtTbl = new ZhuyinExtTable();
            }
            return m_ImmExtTbl;
        }

        #region 載入函式

        /// <summary>
        /// 從組件資源中載入對照表。
        /// </summary>
        public void Load()
        {
            Assembly asmb = Assembly.GetExecutingAssembly();
            string resourceName = this.GetType().FullName + ".txt"; // Note: 這種寫法可以避免寫死的 namsepace，而且用於 obfuscator 時也能正常運作。
            Stream stream = asmb.GetManifestResourceStream(resourceName);
            if (stream == null)
            {
                throw new Exception("CharZhuyinTable.Load 找不到資源: " + resourceName);
            }
            using (stream)
            {
                using (StreamReader sr = new StreamReader(stream, Encoding.Default))
                {
                    Load(sr);
                }
            }
        }

        /// <summary>
        /// 從串流中讀取對照表。
        /// </summary>
        /// <param name="sr"></param>
        public void Load(StreamReader sr)
        {
            String line;
            string[] fields;
            char[] sep = new char[] {' '};
            char ch;


            while ((line = sr.ReadLine()) != null)
            {
                fields = line.Split(sep, 5, StringSplitOptions.RemoveEmptyEntries);
                if (fields.Length < 2)  // 若只定義中文字，未指定字根，則不處理。
                    continue;

                // 取出中文字，且不管中文字有幾個，都固定取第一個字。
                ch = fields[0][0];

                // 移除既有的項目，亦即後來載入的資料會蓋過之前載入的。
                if (m_Table.ContainsKey(ch))
                {
                    m_Table.Remove(ch);
                }
                List<Zhuyin> zyList = new List<Zhuyin>();
                for (int i = 1; i < fields.Length; i++)
                {
                    Zhuyin zy = new Zhuyin(fields[i]);
                    zyList.Add(zy);
                }
                m_Table.Add(ch, zyList);  
            }
        }

        /// <summary>
        /// 從外部檔案載入。
        /// </summary>
        /// <param name="filename"></param>
        public void Load(string filename)
        {
			Encoding enc = Encoding.Default;

			// 根據檔案名稱來判斷編碼.
			if (filename.IndexOf(".big5", StringComparison.CurrentCultureIgnoreCase) >= 0) 
			{
				enc = Encoding.GetEncoding(950);
			}
			else if (filename.IndexOf(".utf", StringComparison.CurrentCultureIgnoreCase) >= 0)
			{
				enc = Encoding.UTF8;
			}

			using (StreamReader sr = new StreamReader(filename, enc))
            {
                Load(sr);
            }
        }

        #endregion


        public List<Zhuyin> this[char chineseChar]
        {
            get
            {
                List<Zhuyin> value = m_Table[chineseChar];
                return value;
            }
        }
    }
}
