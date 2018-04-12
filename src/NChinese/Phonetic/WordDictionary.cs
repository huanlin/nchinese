using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Serilog;

namespace NChinese.Phonetic
{
    public class WordDictionary : Dictionary<string, WordData>
    {
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var target = obj as WordDictionary;
            if (target == null)
                return false;
            if (ReferenceEquals(this, target))
                return true;
            if (Count != target.Count)
                return false;
            foreach (var key in Keys)
            {
                if (!target.ContainsKey(key))
                    return false;
                if (!this[key].Equals(target[key]))
                    return false;
            }
            return true;
        }

        public async Task LoadFromFileAsync(string filename)
        {
            if (!File.Exists(filename))
            {
                throw new FileNotFoundException("找不到檔案", filename);
            }

            using (var reader = new StreamReader(filename, Encoding.UTF8))
            {
                string line = await reader.ReadLineAsync();
                while (line != null)
                {


                    line = await reader.ReadLineAsync();
                }
            }
        }

        /// <summary>
        /// Parse word data from a string and add it to the dictionary.
        /// </summary>
        /// <param name="str">以下兩種格式都合法：
        ///     字詞 頻率 ㄅ ㄆ ㄇ ㄈ
        ///     字詞 ㄅ ㄆ ㄇ ㄈ
        /// </param>
        /// <returns></returns>
        public bool AddWord(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return false;
            }

            string[] parts = str.Split(' ');
            if (parts?.Length >= 2)
            {
                var key = parts[0];   // 字詞
                int freq = 0;
                int phoneticIndex = 2;
                try
                {
                    freq = Convert.ToInt32(parts[1]);               
                }
                catch
                {
                    // 沒有「字詞頻率」
                    phoneticIndex = 1;
                }

                // 如果片語已經存在表中，則覆蓋之。
                WordData wordData = null;
                if (ContainsKey(key)) 
                {
                    wordData = this[key];
                    wordData.ZhuyinList.Clear();
                }
                else
                {
                    wordData = new WordData();
                    Add(key, wordData);
                }

                wordData.Frequency = freq;
                for (int i = phoneticIndex; i < parts.Length; i++)
                {
                    wordData.ZhuyinList.Add(parts[i]);
                }
                
                Log.Verbose("加入字詞: {Word} {Freq} {@Data}", key, freq, wordData);
                return true;
            }
            Log.Warning($"格式不正確: {str}");
            return false;
        }

    }
}
