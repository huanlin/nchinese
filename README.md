# NChinese

[![Build status](https://ci.appveyor.com/api/projects/status/mtuddcaa62v7kmta/branch/master?svg=true)](https://ci.appveyor.com/project/huanlin/nchinese/branch/master) [![NuGet Badge](https://buildstats.info/nuget/nchinese)](https://www.nuget.org/packages/NChinese/) [![Built with](https://img.shields.io/badge/built%20with-nuke--build-blue.svg)](https://nuke.build/)

NChinese 是一套用來處理中文字詞的函式庫。目前具備的功能，主要是反查一串中文字的注音或拼音。

## 安裝

使用 Nuget 套件管理員來安裝，或執行下列命令：

```txt
Install-Package NChinese -Version 0.3.3
Install-Package NChinese.Imm -Version 0.3.3
```

## 簡介

NChinese 包含兩個套件：

* [NChinese](https://www.nuget.org/packages/NChinese/) - 包含內建中文注音詞庫與相關 API，例如反查一串中文字的注音字根。
* [NChinese.Imm](https://www.nuget.org/packages/NChinese.Imm/) - 此套件會用到 Windows 底層的 Imm.dll 與 IFELanguage COM API，所以只適用於 Windows 作業環境。

兩個套件都有提供反查注音字根的函式，但由於 NChinese.Imm 只能運行於 Windows 環境，故建議盡量使用 NChinese。

### 範例：反查注音字根

```cs
using NChinese;

// 取得一串中文字的注音字根
var zhuyinProvicer = new ZhuyinReverseConversionProvider();
string[] zhuyinArray = zhuyinProvicer.Convert("便宜又方便得不得了");

foreach (var s in zhuyinArray)
    Console.Write($"{s} ");  
```

執行結果:

```txt
ㄆㄧㄢˊ "ㄧˊ ㄧㄡˋ ㄈㄤ ㄅㄧㄢˋ ㄉㄜ˙ ㄅㄨˋ ㄉㄜˊ ㄌㄧㄠˇ
```

中文詞庫與注音字根的資料，是以 [libchewing](https://github.com/chewing/libchewing) 的檔案（tsi.src）為藍本，再經過工具加工之後所產生的。所以透過上述方法所取得的注音字根，在讀音方面都是比較符合台灣的發音習慣。

### 範例：反查拚音字根

如果要取得拼音字根，目前可以用的是 `ImmPinyinReverseConversionProvider`。此類別與上例的 `ZhuyinReverseConversionProvider` 都實作了  `IReverseConversionProvider`，故用法雷同。如下：

```cs
using NChinese.Imm;

// 取得一串中文字的拼音字根
var pinyinProvicer = new ImmPinyinReverseConversionProvider();
string[] pinyininArray = zhuyinProvicer.Convert("便宜又方便得不得了");

foreach (var s in zhuyinArray)
    Console.Write($"{s} "); 
```

執行結果：

```txt
pián yi yòu fāng biàn de bù dé liǎo 
```

如果仔細比較，就可以發現此範例所取得的拼音，和上一個範例所取得的注音，在讀音方面有小差異：「便宜」的「宜」，在拼音裡面是讀作輕聲，而注音則是二聲。

另外要注意的是，用來取得拼音字根的 `ImmPinyinReverseConversionProvider` 是隸屬於 [NChinese.Imm](https://www.nuget.org/packages/NChinese.Imm/) 套件。此類別是利用 IFELanguage COM API 來取得注音字根，所以只能運行於 Windows 作業環境。

NChinese.Imm 套件裡面還有一個 `ImmZhuyinReverseConversionProvider`，用途跟 `NChinese.ZhuyinReverseConversionProvider` 一樣是反查注音字根，但是它在內部實作上，其實是先用 `ImmPinyinReverseConversionProvider` 取得拼音字根，然後再使用 `PinyinToZhuyin` 類別來把拼音轉換成注音符號。

> 為什麼 `ImmZhuyinReverseConversionProvider` 不直接使用 IFELanguage 的 API 來取得注音字根呢？這是因為根據我的測試，即使 Windows 10 有安裝微軟注音輸入法，IFELanguage COM API 都無法使用 "MSIME.Taiwan" 來反查注音字根，只剩下 "MSIME.China" 和 "MSIME.Japan"（反查日本平假名、片假名）可用。這也是為什麼 NChinese 套件要使用內建詞庫的原因：要利用微軟 Windows 底層的 API 來反查注音字根，已經不太可能了（就算可透過安裝其他軟體或匯入 registry 的方式來恢復 "MSIME.Taiwan"，這種作法還是不靠譜）。

## 開發工具

 * Visual Studio 2017

**相依套件**

 * [NUnit](http://nunit.org/) 
 * [Serilog](https://serilog.net/)

**建置工具**
 
 * [Nuke Build](https://nuke.build/)
 * [GitVersion](https://github.com/GitTools/GitVersion)
 * [Appveyor](https://www.appveyor.com/) 

## 未來可能加入什麼功能

* 中文簡繁轉換
* 注音符號轉成拼音
* 捨棄 NChinese.Imm，讓 NChinese 完全支援 .NET Standard 2.0。

## 授權協議

Copyright(c) 2018 Michael Tsai.

此開源專案是採用 [MIT 授權](https://github.com/huanlin/nchinese/blob/master/LICENSE)。

另外，注音字典 [ZhuyinDictionary.txt](https://github.com/huanlin/nchinese/blob/master/src/NChinese/Phonetic/ZhuyinDictionary.txt) 是以新酷音 [libchewing](https://github.com/chewing/libchewing) 的檔案（tsi.src）為藍本，再經過工具加工之後所產生的。新酷音的授權協議是 [GNU LGPL v2.1](https://github.com/chewing/libchewing/blob/master/COPYING)。

