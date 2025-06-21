﻿using System.IO;
using System.Linq;
using NChinese.Phonetic;
using NUnit.Framework;
using Serilog;

namespace Test.NChinese.Phonetic
{
    [TestFixture]
    public class ZhuyinReverseConversionProviderTests
    {
        [SetUp]
        public void SetUp()
        {
            string logFile = Path.Combine(TestContext.CurrentContext.TestDirectory, "log-Test.NChinese.txt");
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File(logFile)
                .CreateLogger();
        }

        [TearDown]
        public void TearDown()
        {
            Log.CloseAndFlush();
        }

        public static object[] TestData =
        {
            new object[]
            {
                "班",
                new string[] { "ㄅㄢ" }
            },
            new object[]
            {
                "礦",
                new string[] { "ㄎㄨㄤˋ" }
            },
            new object[]
            {
                "什麼",
                new string[] { "ㄕㄣˊ", "ㄇㄜ˙" }
            },
            new object[]
            {
                "便宜又方便得不得了",
                new string[] { "ㄆㄧㄢˊ", "ㄧˊ", "ㄧㄡˋ", "ㄈㄤ", "ㄅㄧㄢˋ", "ㄉㄜ˙", "ㄅㄨˋ", "ㄉㄜˊ", "ㄌㄧㄠˇ" }
            }
        };

        [TestCaseSource("TestData")]
        public void Should_Convert_Succeed(string input, string[] expected)
        {
            var zhuyinProvider = new ZhuyinReverseConversionProvider();

            var actual = zhuyinProvider.Convert(input);

            Assert.That(actual.SequenceEqual(expected), Is.True);
        }

        [Test]
        public void Should_Convert_With_AutoFillSpaces()
        {
            var zhuyinProvider = new ZhuyinReverseConversionProvider();
            zhuyinProvider.AutoFillSpaces = true;

            var expected = new string[] { "ㄅ　ㄢˉ" }; // 注意有聲調符號（一聲）
            var actual = zhuyinProvider.Convert("班");

            Assert.That(actual.SequenceEqual(expected), Is.True);
        }
    }
}
