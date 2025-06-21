using System.Globalization;
using NChinese;
using NUnit.Framework;

namespace Test.NChinese
{
    /// <summary>
    /// 注意：此檔案包含 UTF-16 編碼的中文字，故存檔時必須採用 Unitcode 編碼格式（即 UTF-16 Little Endian）。
    /// </summary>
    [TestFixture]
    public class StringExtensionsTests
    {
        [SetUp]
        public void SetUp()
        {

        }

        [TestCase("王𤭢𪚥誤吳吴")]
        [TestCase("𠑗㒨𨉼")] // CJK 擴展 B 區
        [TestCase("𪜎𪡀𪞫")] // CJK 擴展 C 區
        [TestCase("𫞦𫟁𫠇")] // CJK 擴展 D 區
        public void Should_IsCJK_Return_True_For_CJK_Ideographs(string s)
        {
            var charEnum = StringInfo.GetTextElementEnumerator(s);
            while (charEnum.MoveNext())
            {
                Assert.That(charEnum.GetTextElement().IsCJK(), Is.True);
            }           
        }

        [TestCase("2%-*")]
        [TestCase("ㄅㄆㄇㄈ")] // 注音符號不算表意文字
        [TestCase("【」〇")]   // 中文全形符號
        [TestCase("あえかァィ")]   // 日文平假名和片假名
        public void Should_IsCJK_Return_False_For_Non_CJK_Ideographs(string s)
        {
            var charEnum = StringInfo.GetTextElementEnumerator(s);
            while (charEnum.MoveNext())
            {
                Assert.That(charEnum.GetTextElement().IsCJK(), Is.False);
            }
        }

        [TestCase("哈囉！你好。", 0, 1)]
        [TestCase(" 哈囉 你好。", 1, 2)]
        [TestCase("𠑗㒨𨉼", 0, 3)] // CJK 擴展 B 區
        public void Should_FindConsecutiveUnihan_FindChinese(string input, int expectedStartIndex, int expectedStopIndex)
        {
            var result = input.FindConsecutiveUnihan();
            Assert.That(expectedStartIndex, Is.EqualTo(result.StartIndex));
            Assert.That(expectedStopIndex, Is.EqualTo(result.StopIndex));
        }

    }
}
