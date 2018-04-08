# NChinese

NChinese 是一套用來處理中文字詞的函式庫。主要的用途是將一串中文字的轉換成對應的拼音或注音符號（ㄅㄆㄇㄈ……）。

> **作者註**
> 
> 以往，我是透過 [IFELanguage](https://msdn.microsoft.com/en-us/library/windows/desktop/hh851778(v=vs.85).aspx) 來取得一串中文字的注音字根。然而，這種方法到了 Windows 10 似乎已經無法使用。
>
> 根據我的測試，即使 Windows 10 有安裝微軟注音輸入法，也無法使用 MSIME.Taiwan 來反查注音字根。奇怪的是，MSIME.China 依然可用，但 MSIME.China 只能取得拼音。
>
> 於是，在這個函式庫裡面，我是先取得中文的拼音組合，然後再將拼音轉換成對應的注音符號。
>
> 雖然以目前的作法，已經能夠同時支援注音與拼音，但考慮到 IFELanguage 是 COM 介面，而且跟微軟 Windows 的輸入法綁在一起，無法跨平台，因此我打算將來透過內建詞庫的方式來查詢中文字詞的注音和拼音。具體來說，詞庫的部分很可能會直接使用 [libchewing](https://github.com/chewing/libchewing) 的詞庫資料檔。

## 相依套件

 * [NUnit](http://nunit.org/) 

## 授權

此專案是採用 LGPL 3.0 授權。

[libchewing](https://github.com/chewing/libchewing) 則是採用 GNU LGPL 2.1 授權。
