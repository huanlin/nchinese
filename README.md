# NChinese

[![Build status](https://ci.appveyor.com/api/projects/status/mtuddcaa62v7kmta/branch/master?svg=true)](https://ci.appveyor.com/project/huanlin/nchinese/branch/master) [![NuGet Badge](https://buildstats.info/nuget/nchinese)](https://www.nuget.org/packages/NChinese/)

NChinese 是一套用來處理中文字詞的函式庫。目前具備的功能主要是反查一串中文字的注音或拼音。

## 開發工具

 * Visual Studio 2017

### 相依套件

 * [NUnit](http://nunit.org/) 
 * [Serilog](https://serilog.net/)
 
### 建置工具
 
 * [Nuke Build](https://nuke.build/)
 * [GitVersion](https://github.com/GitTools/GitVersion)
 * [Appveyor](https://www.appveyor.com/) for Github 建置徽章（略過單元測試，因為 Appvevor 主機上無法啟動 IFELanguage COM 元件。）

## 授權協議

Copyright(c) 2018 Michael Tsai.

此開源專案是採用 MIT 授權。

[libchewing](https://github.com/chewing/libchewing) 則是採用 GNU LGPL 2.1 授權。
