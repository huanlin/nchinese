using NChinese.Phonetic;
using NUnit.Framework;

namespace Test.NChinese.Phonetic
{
    [TestFixture]
    public class ZhuyinQueryHelperTests
    {
        [SetUp]
        public void SetUp()
        {

        }

        [Test]
        public void Should_IsPolyphonic_Work()
        {
            string aChar = "的";
            bool expected = true;
            bool actual;
            actual = ZhuyinQueryHelper.IsPolyphonic(aChar);
            Assert.That(actual, Is.EqualTo(expected)); // Replace Assert.AreEqual with Assert.That

            aChar = "料";
            expected = false;
            actual = ZhuyinQueryHelper.IsPolyphonic(aChar);
            Assert.That(actual, Is.EqualTo(expected)); // Replace Assert.AreEqual with Assert.That
        }

        [Test]
        public void Should_GetZhuyinSymbols_Work()
        {
            string aChar = "料";
            string[] expected = { "ㄌㄧㄠˋ" };
            string[] actual;
            actual = ZhuyinQueryHelper.GetZhuyinSymbols(aChar, false);
            Assert.That(actual, Is.EqualTo(expected)); // Replace CollectionAssert with Assert.That

            aChar = "們";
            expected = new string[] { "ㄇㄣˊ", "ㄇㄣ˙" };
            actual = ZhuyinQueryHelper.GetZhuyinSymbols(aChar, false);
            Assert.That(actual, Is.EqualTo(expected)); // Replace CollectionAssert with Assert.That
        }

        [Test]
        public void Should_GetZhuyinKeys_Work()
        {
            string aChar = "料";
            string[] expected = { "xul4" };
            string[] actual;
            actual = ZhuyinQueryHelper.GetZhuyinKeys(aChar);
            Assert.That(actual, Is.EqualTo(expected)); // Replace CollectionAssert with Assert.That
        }
    }
}
