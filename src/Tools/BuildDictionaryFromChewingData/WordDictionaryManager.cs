using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Serilog;

namespace BuildDictionaryFromChewingData
{
    /// <summary>
    /// 此類別可將 libchewing 的 tsc.src 檔案載入至字典結構（載入過程會丟棄重複的字詞），以及把字典結構序列化成 protobuf 格式的二進位檔案。
    /// 
    /// 每個字詞可能有多種注音字根，例如：
    /// 
    /// 一動不如一靜 2 ㄧ ㄉㄨㄥˋ ㄅㄨˋ ㄖㄨˊ ㄧ ㄐㄧㄥˋ
    /// 一動不如一靜 2 ㄧ ㄉㄨㄥˋ ㄅㄨˋ ㄖㄨˊ ㄧˊ ㄐㄧㄥˋ
    /// 一動不如一靜 2 ㄧˊ ㄉㄨㄥˋ ㄅㄨˋ ㄖㄨˊ ㄧ ㄐㄧㄥˋ
    /// 一動不如一靜 2 ㄧˊ ㄉㄨㄥˋ ㄅㄨˋ ㄖㄨˊ ㄧˊ ㄐㄧㄥˋ
    /// 
    /// 其中的 2 是出現頻率，數字越大代表越常出現，優先權越高。
    /// </summary>    
    public class WordDictionaryManager
    {
        public WordDictionary WordDict { get; private set; } = new WordDictionary();

        public ILogger Logger { get; private set; }

        public WordDictionaryManager(ILogger logger) : base()
        {
            Logger = logger;
        }

        /// <summary>
        /// Parse word data from a string and add it to the dictionary.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public bool AddWord(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return false;
            }

            string[] parts = str.Split(' ');
            if (parts?.Length >= 3)
            {
                var key = parts[0]; // 字詞
                var freq = Convert.ToInt32(parts[1]);

                // 如果字詞已經存在表中，則比較頻率。若出現頻率大於或等於既有的字詞，則覆蓋之。
                WordData word = null;
                if (WordDict.ContainsKey(key)) // 字詞已經存在表中
                {
                    word = WordDict[key];
                    if (freq < word.Frequency)
                    {
                        return false;
                    }
                    word.ZhuyinList.Clear();
                }
                else
                {
                    word = new WordData();
                    WordDict.Add(key, word);
                }

                word.Frequency = freq;
                for (int i = 2; i < parts.Length; i++)
                {
                    word.ZhuyinList.Add(parts[i]);
                }
                Logger.Verbose("加入字詞: {Key} {@Word}", key, word);
                return true;
            }
            Logger.Warning($"無效的字詞: {str}");
            return false;
        }

        public void LoadFromEnumerable(IEnumerable<string> lines)
        {
            foreach (var line in lines)
            {
                AddWord(line);
            }
        }

        /// <summary>
        /// 將 libchewing 的 tsi.src 原始文字檔載入至字典結構。載入過程會丟棄重複的字詞。
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public async Task LoadFromTextFileAsync(string filename)
        {
            if (!File.Exists(filename))
                throw new FileNotFoundException("指定的檔案不存在", filename);

            var startTime = DateTime.Now;
            Logger.Information($"Begin loading words from the file: {filename}");

            using (var reader = new StreamReader(filename, Encoding.UTF8))
            {
                string line = await reader.ReadLineAsync();
                while (line != null)
                {
                    AddWord(line);
                    line = await reader.ReadLineAsync();
                }
            }
            Logger.Information($"End loading words from the file: {filename}. Time spent: {DateTime.Now - startTime}");
            Logger.Information($"Totally loaded words: {WordDict.Count}");
        }

        public async Task SaveToTextFileAsync(string filename)
        {
            using (var writer = new StreamWriter(filename, false, Encoding.UTF8))
            {
                foreach (var item in WordDict)
                {
                    await writer.WriteLineAsync($"{item.Key} {item.Value.Frequency} {item.Value.ToString()}");
                }
            }
        }

    }
}
