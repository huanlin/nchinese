從新酷音資料檔建立字典的命令：

dotnet BuildDictionaryFromChewingData.dll tsi.txt ZhuyinDictionary.txt

其中tsi.txt 是新酷音的字典資料檔，來自 https://github.com/chewing/libchewing。

執行上述命令之後，會產生 ZhuyinDictionary.txt。請將此檔案複製到 src\NChinese\Phonetic 目錄下。
 