using System.Collections.Generic;

namespace NChinese.Phonetic
{
    public sealed class ZhuyinReverseConversionProvider : IReverseConversionProvider
    {
        private ZhuyinDictionary _dict;

        public ZhuyinReverseConversionProvider(bool skipLoadingDictionary = false)
        {
            _dict = ZhuyinDictionary.GetInstanceAsync(skipLoadingDictionary).GetAwaiter().GetResult();
        }

        public bool IsAvailable => true;

        /// <summary>
        /// 是否自動把注音字根以空白填滿成固定長度為 4 的字串。例如 "ㄅㄢ" 會變成 "ㄅ　ㄢ　"。　
        /// </summary>
        public bool AutoFillSpaces { get; set; } = false;

        /// <summary>
        /// 載入自定義辭庫檔案
        /// </summary>
        /// <param name="filename"></param>
        public async void LoadCustomizedDictionaryAsync(string filename)
        {
            await _dict.LoadFromFileAsync(filename);
        }

        /// <summary>
        /// 把輸入字串轉換成注音字根陣列。
        /// </summary>
        /// <param name="input">一個或一串中文字。</param>
        /// <returns>字串陣列。每個元素是一個中文字的注音字根，長度固定為 4，例如："ㄅ　ㄢ　"。</returns>
        public string[] Convert(string input)
        {
            const int MaxPhraseLength = 10;

            var result = new List<string>();
            GetZhuyinFromDictionary(input, result, MaxPhraseLength);

            if (result.Count == 0) // 防錯：萬一找不到任何注音字根，還是要返回一個符合字串長度的陣列。
            {
                for (int i = 0; i < input.Length; i++)
                {
                    result.Add(string.Empty);
                }
            }

            if (AutoFillSpaces)
            {
                for (int i = 0; i < result.Count; i++)
                {
                    result[i] = Zhuyin.FillSpaces(result[i]);
                }
            }

            return result.ToArray();
        }

        /// <summary>
        /// 取得一串中文字的注音字根。以遞迴的方式，先嘗試取得最長片語的注音字根，然後逐漸縮短片語長度。
        /// </summary>
        /// <param name="aChineseText"></param>
        /// <param name="result"></param>
        /// <param name="maxLength">欲查詢的片語最大長度。參考字典的內容，最長的片語應該不會超過 10 個中文字。</param>
        private void GetZhuyinFromDictionary(string aChineseText, List<string> result, int maxLength=10)
        {
            if (string.IsNullOrEmpty(aChineseText))
                return;

            //Log.Debug($"呼叫 GetZhuyinFromDictionary('{aChineseText}', maxLength={maxLength})");

            if (maxLength > aChineseText.Length)
            {
                maxLength = aChineseText.Length;
            }
            string s = aChineseText.Substring(0, maxLength);
            
            while (_dict.ContainsKey(s) == false)
            {
                maxLength--;
                if (maxLength <= 0)
                    return;
                s = s.Substring(0, maxLength);
            }

            // 找到一個字或片語
            var wordData = _dict[s];
            result.AddRange(wordData.ZhuyinList);
            //Log.Debug($"GetZhuyinFromDictionary: 找到片語 '{s}' 的注音字根: '{wordData.ToString()}'");

            // 遞迴呼叫自己來處理剩下的字元。
            aChineseText = aChineseText.Remove(0, s.Length);
            GetZhuyinFromDictionary(aChineseText, result);
        }
    }
}
