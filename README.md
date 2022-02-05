# NChinese

[![Build status](https://ci.appveyor.com/api/projects/status/mtuddcaa62v7kmta/branch/master?svg=true)](https://ci.appveyor.com/project/huanlin/nchinese/branch/master)  [![NuGet Badge](https://buildstats.info/nuget/nchinese)](https://www.nuget.org/packages/NChinese/)   [![Built with](https://img.shields.io/badge/built%20with-nuke--build-blue.svg)](https://nuke.build/)

NChinese 是一套用來處理中文字詞的函式庫。目前具備的功能，主要是反查一串中文字的注音或拼音。

## 安裝

使用 Nuget 套件管理員來安裝，或執行下列命令：

```txt
Install-Package NChinese -ProjectName YourProject
```

## 簡介

NChinese 內建中文注音詞庫，並提供了反查注音字根與其他輔助 API。

### 範例：反查注音字根

```cs
using NChinese;

// 取得一串中文字的注音字根
var zhuyinProvider = new ZhuyinReverseConversionProvider();
string[] zhuyinArray = zhuyinProvider.Convert("便宜又方便得不得了");

foreach (var s in zhuyinArray)
    Console.Write($"{s} ");  
```

執行結果:

```txt
ㄆㄧㄢˊ ㄧˊ ㄧㄡˋ ㄈㄤ ㄅㄧㄢˋ ㄉㄜ˙ ㄅㄨˋ ㄉㄜˊ ㄌㄧㄠˇ
```

中文詞庫與注音字根的資料，是以 [libchewing](https://github.com/chewing/libchewing) 的檔案（tsi.src）為藍本，再經過工具加工之後所產生的。所以透過上述方法所取得的注音字根，在讀音方面比較符合台灣的發音習慣。

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

## 授權協議

Copyright(c) 2018 Michael Tsai.

此開源專案是採用 [MIT 授權](https://github.com/huanlin/nchinese/blob/master/LICENSE)。

另外，注音字典 [ZhuyinDictionary.txt](https://github.com/huanlin/nchinese/blob/master/src/NChinese/Phonetic/ZhuyinDictionary.txt) 是以新酷音 [libchewing](https://github.com/chewing/libchewing) 的檔案（tsi.src）為藍本，再經過工具加工之後所產生的。新酷音的授權協議是 [GNU LGPL v2.1](https://github.com/chewing/libchewing/blob/master/COPYING)。

