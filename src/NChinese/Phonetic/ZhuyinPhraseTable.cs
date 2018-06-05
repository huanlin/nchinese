using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace NChinese.Phonetic
{
    /// <summary>
    /// 找到的片語資訊。
    /// </summary>
    public sealed class ZhuyinPhrase
    {
        public string Text;             // 片語文字。
        public List<Zhuyin> ZhuyinList; // 片語的注音字根串列。

        public ZhuyinPhrase(string text, List<Zhuyin> zyList)
        {
            this.Text = text;
            this.ZhuyinList = zyList;
        }
    }

    /// <summary>
    /// 注音詞庫檔。用來修正新注音智慧型註音詞庫不正確的字根，例如：
    /// 「什麼」：新注音傳回「ㄕㄜˊ ㄇㄛ˙」，但應該是「ㄕㄜˊ ㄇㄜ˙」。
    /// 「給付」：新注音傳回「ㄍㄟˇ ㄈㄨˋ」，但應該是「ㄐㄧˇ ㄈㄨˋ」。
    /// </summary>
    public sealed class ZhuyinPhraseTable
    {
        private static ZhuyinPhraseTable _phraseTable = null;

        private Dictionary<string, List<Zhuyin>> _table;  // 中文片語的注音字根對照表，每一筆代表一個中文片語的注音字根。

        private ZhuyinPhraseTable()
        {
            _table = new Dictionary<string, List<Zhuyin>>();

            Load();
        }

        /// <summary>
        /// Singleton 建構方法。
        /// </summary>
        /// <returns></returns>
        public static ZhuyinPhraseTable GetInstance()
        {
            if (_phraseTable == null)
            {
                _phraseTable = new ZhuyinPhraseTable();
            }
            return _phraseTable;
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
            while ((line = sr.ReadLine()) != null)
            {
                AddPhrase(line);
            }
        }

        /// <summary>
        /// 加入一個片語。
        /// </summary>
        /// <param name="phraseStr">片語字串，格式為 [中文] [注音字根]，例如：「不禁 ㄅㄨˋ ㄐㄧㄣ」。</param>
        /// <returns></returns>
        public bool AddPhrase(string phraseStr)
        {
            string[] fields;
            char[] sep = new char[] { ' ' };
            string phrase;

            fields = phraseStr.Split(sep, 5, StringSplitOptions.RemoveEmptyEntries);
            if (fields.Length < 2)  // 若只定義中文字，未指定字根，則不處理。
                return false;

            // 取出中文辭彙。
            phrase = fields[0];

            // 移除既有的項目，亦即後來載入的資料會蓋過之前載入的。
            if (_table.ContainsKey(phrase))
            {
                _table.Remove(phrase);
            }
            List<Zhuyin> zyList = new List<Zhuyin>();
            for (int i = 1; i < fields.Length; i++)
            {
                Zhuyin zy = new Zhuyin(fields[i]);
                zyList.Add(zy);
            }
            _table.Add(phrase, zyList);

            return true;
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
				System.Diagnostics.Debug.WriteLine("ImmPhraseTable loaded: " + filename);
            }
        }

        #endregion


        public List<Zhuyin> this[string phrase]
        {
            get
            {
                List<Zhuyin> value = _table[phrase];
                return value;
            }
        }

        /// <summary>
        /// 清除所有辭彙，只保留系統內建的。
        /// </summary>
        public void Reset()
        {
            _table.Clear();
            Load();
        }

        /// <summary>
        /// 傳回指定辭彙對應的注音符號。例如：傳入「什麼」，傳回「ㄕ　ㄜˊ ㄇ　ㄜ˙」。
        /// </summary>
        /// <param name="phrase">中文詞彙。</param>
        /// <returns>注音符號，每個中文字固定傳回四個字元長度的注音符號，其中可能包含全型空白，例如：「ㄕ　ㄜˊ」。</returns>
        public string GetZhyuinSymbols(string phrase)
        {
            List<Zhuyin> zyList = _table[phrase];
            if (zyList == null)
            {
                return "";
            }
            StringBuilder sb = new StringBuilder();
            foreach (Zhuyin zy in zyList) 
            {
                sb.Append(zy.ToString(true));
            }
            return sb.ToString();
        }

        /// <summary>
        /// 找出在指定的字串中出現的所有片語。
        /// </summary>
        /// <param name="text">字串。</param>
        /// <returns>找到的片語集合，它是一個排序過的串列，排序的 key 是片語位於來源字串的的索引，Value 則是 ZhuyinPhrase 型別的物件。</returns>
        public SortedList<int, ZhuyinPhrase> FindPhrases(string text)
        {
            SortedList<int, ZhuyinPhrase> matchedPhrases = new SortedList<int, ZhuyinPhrase>();

            int idx;
            int start;
            int end;
            int count;
            ZhuyinPhrase immPhrase;
            List<Zhuyin> zhuyinList;

            foreach (string phrase in _table.Keys)
            {
                start = 0;
                end = text.Length-1;
                while (start <= end)
                {
                    count = end - start + 1;
                    idx = text.IndexOf(phrase, start, count);
                    if (idx < 0)
                        break;
                    if (matchedPhrases.ContainsKey(idx))
                    {
                        // 若先前已經有符合的片語，而且該片語長度比目前這個片語還要長，則保留既有的，並捨棄目前的片語。
                        var previousMatch = matchedPhrases[idx];
                        if (previousMatch.Text.Length > phrase.Length)
                        {
                            break;
                        }
                        matchedPhrases.Remove(idx);
                    }
                    // 有找到符合的片語。記錄位置、取得注音字根，並繼續往後面找。
                    zhuyinList = _table[phrase];
                    immPhrase = new ZhuyinPhrase(phrase, zhuyinList);
                    matchedPhrases.Add(idx, immPhrase);
                    start = idx + phrase.Length;
                }
            }
            return matchedPhrases;
        }
    }
}
