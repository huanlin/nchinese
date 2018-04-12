namespace NChinese.Phonetic
{
    /// <summary>
    /// 注音字根查詢工具。
    /// 此類別可查詢單一中文字的所有注音字根（使用自建注音字根表），
    /// 亦提供查詢整串中文字的注音字根（使用 IFELanguage）。
    /// </summary>
    public static class ZhuyinQueryHelper
    {
        private static bool m_Initialized = false;
        private static CharZhuyinTable m_ZhuyinTable = null;

        private static CharZhuyinTable ZhuyinTable
        {
            get
            {
                if (m_Initialized == false)
                {
                    Initialize();
                }
                return m_ZhuyinTable;
            }
        }

        /// <summary>
        /// 執行此類別的初始工作。由於載入完整的注音字根表會用掉較多資源，
        /// 因此提供此函式，讓呼叫端在適當時機呼叫此函式。
        /// </summary>
        public static void Initialize()
        {
            if (!m_Initialized)
            {
                m_ZhuyinTable = new CharZhuyinTable();
                m_ZhuyinTable.Load();
                m_Initialized = true;
            }
        }

        /// <summary>
        /// 釋放先前配置的資源。
        /// </summary>
        public static void Release()
        {
            m_ZhuyinTable.Clear();
            m_ZhuyinTable = null;            
        }

        /// <summary>
        /// 判斷傳入的中文字是否為多音字。
        /// </summary>
        /// <param name="aChar"></param>
        /// <returns></returns>
        public static bool IsPolyphonic(string aChar)
        {
            if (!aChar.IsCJK())
                return false;

            if (m_Initialized == false)
            {
                Initialize();
            }

            CharZhuyinInfo charInfo = ZhuyinTable.Find(aChar);
            if (charInfo != null) 
            {
                return (charInfo.IsPolyphonic);
            }
            return false;
        }


        /// <summary>
        /// 取得指定中文字的注音字根鍵盤碼（標準注音鍵盤）。
        /// 注意：聲符一聲沒不會有對應的按鍵！
        /// </summary>
        /// <param name="aChar">輸入字元。</param>
        /// <returns>注音字根陣列。</returns>
        public static string[] GetZhuyinKeys(string aChar)
        {
            string[] result = new string[0];    // Empty string array.

            if (!aChar.IsCJK() && !aChar.IsZhuyinSymbol())
                return result;

            CharZhuyinInfo charInfo = ZhuyinTable.Find(aChar);

            if (charInfo != null)
            {
                result = charInfo.Phonetics.ToArray();
            }
            return result;
        }

        /// <summary>
        /// 取得指定中文字的注音字根符號（ㄅㄆㄇㄈ）。
        /// 注意：傳回的一聲符號看似全形空白，其實不是！
        /// </summary>
        /// <param name="aChar"></param>
        /// <param name="fillSpaces">每一組字根是否要以全型空白補滿四個字元。</param>
        /// <returns></returns>
        public static string[] GetZhuyinSymbols(string aChar, bool fillSpaces)
        {
            string[] keys = GetZhuyinKeys(aChar);   // 取得該字的所有注音字根按鍵碼.
            string[] symbols = new string[keys.Length];

            // 將每一組字根的按鍵字元轉換成ㄅㄆㄇㄈ符號.
            for (int i = 0; i < keys.Length; i++)
            {
                symbols[i] = ZhuyinKeyMappings.KeyToSymbol(keys[i]);
                if (fillSpaces)
                {
                    symbols[i] = Zhuyin.FillSpaces(symbols[i]);
                }
            }
            return symbols;
        }
    }
}
