using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Serilog;

namespace NChinese.Phonetic
{
    /// <summary>
    /// 此類別會利用內建辭庫字典來反查注音字根。
    /// </summary>
    public class ZhuyinDictionary : Dictionary<string, PhraseWithZhuyin>
    {
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var target = obj as ZhuyinDictionary;
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

        public async Task LoadFromDefaultFileAsync()
        {
            const string DictFileName = "ZhuyinDictionary.txt";

            var assemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var filename = Path.Combine(assemblyDirectory, "Data\\" + DictFileName);

            await LoadFromFileAsync(filename);
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
                    AddWord(line);

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
                PhraseWithZhuyin phrase = null;
                if (ContainsKey(key)) 
                {
                    phrase = this[key];
                    phrase.ZhuyinList.Clear();
                }
                else
                {
                    phrase = new PhraseWithZhuyin();
                    Add(key, phrase);
                }

                phrase.Text = key;
                phrase.Frequency = freq;
                for (int i = phoneticIndex; i < parts.Length; i++)
                {
                    phrase.ZhuyinList.Add(parts[i]);
                }
                
                Log.Verbose("加入字詞: {Phrase} {Freq} {@Zhuyin}", key, freq, phrase.ZhuyinList);
                return true;
            }
            Log.Warning($"格式不正確: {str}");
            return false;
        }

    }
}
