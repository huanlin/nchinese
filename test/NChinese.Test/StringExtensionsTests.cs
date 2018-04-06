using System.Globalization;
using NUnit.Framework;
using SharpChinese;

namespace ChineseTest
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
        public void Should_IsUnihan_Return_True_For_CJK_Ideographs(string s)
        {
            var charEnum = StringInfo.GetTextElementEnumerator(s);
            while (charEnum.MoveNext())
            {
                Assert.IsTrue(charEnum.GetTextElement().IsUnihan());
            }           
        }

        [TestCase("2%-*")]
        [TestCase("ㄅㄆㄇㄈ")] // 注音符號不算表意文字
        [TestCase("【」〇")]   // 中文全形符號
        [TestCase("あえかァィ")]   // 日文平假名和片假名
        public void Should_IsUnihan_Return_False_For_Non_CJK_Ideographs(string s)
        {
            var charEnum = StringInfo.GetTextElementEnumerator(s);
            while (charEnum.MoveNext())
            {
                Assert.IsFalse(charEnum.GetTextElement().IsUnihan());
            }
        }

    }
}
