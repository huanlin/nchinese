using System;
using System.Collections.Generic;

namespace NChinese.Phonetic
{
    public sealed class ZhuyinReverseConverter
    {
        private IReverseConversionProvider _converter;

        public ZhuyinReverseConverter(IReverseConversionProvider revConvProvider)
        {
            _converter = revConvProvider ?? throw new ArgumentNullException(nameof(revConvProvider));
        }

        /// <summary>
        /// 取得整串中文字的注音碼。
        /// </summary>
        /// <param name="aChineseText">中文字串。</param>
        /// <returns>字串陣列。每個元素是一個中文字的注音字根，長度固定為 4，例如："ㄅ　ㄢ　"。</returns>
        public string[] GetZhuyin(string aChineseText)
        {
            string[] zhuyinArray = _converter.Convert(aChineseText);

            // 調整注音碼，使其長度補滿四個字元.
            for (int i = 0; i < zhuyinArray.Length; i++)
            {
                zhuyinArray[i] = Zhuyin.FillSpaces(zhuyinArray[i]);
            }

            return zhuyinArray;
        }

        /// <summary>
        /// 反查整串中文字的注音碼，並且利用預先指定的擴充詞庫來修正注音。
        /// </summary>
        /// <param name="aChineseText">中文字串。</param>
        /// <returns>包含注音字根的字串陣列。每個元素代表輸入字串中對應位置的字元的注音字根，而且長度固定為 4。</returns>
        public string[] GetZhuyinWithPhraseTable(string aChineseText)
        {
            string[] zhuyinArray = GetZhuyin(aChineseText);

            // 利用擴充詞庫字根表修正 API 傳回的字根。

            var phraseTbl = ZhuyinPhraseTable.GetInstance();
            SortedList<int, ZhuyinPhrase> matchedPhrases = phraseTbl.FindPhrases(aChineseText);
            int srcIndex;
            ZhuyinPhrase phrase;

            // 由於可能會有多次置換字串的動作，因此必須由字串的尾部往前進行置換。
            for (int i = matchedPhrases.Count - 1; i >= 0; i--)
            {
                srcIndex = matchedPhrases.Keys[i];   // 取得片語在輸入字串中的來源索引。
                phrase = matchedPhrases.Values[i];   // 取得代表片語的物件。

                //DebugOut("\r\nimmPhrase.Text=" + immPhrase.Text);

                foreach (Zhuyin zy in phrase.ZhuyinList)
                {
                    zhuyinArray[srcIndex] = zy.ToString(true);    // 儲存注音字根時，會以全型空白補足 4 碼。
                    srcIndex++;
                }
            }
            return zhuyinArray;
        }

    }
}
