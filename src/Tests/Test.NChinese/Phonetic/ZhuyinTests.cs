using NChinese.Phonetic;
using NUnit.Framework;

namespace Test.NChinese.Phonetic
{
    [TestFixture]
    public class ZhuyinTests
    {
        [SetUp]
        public void SetUp()
        {

        }

        [Test]
        public void Should_ParseKeyString_Succeed()
        {
            string zhuyinKeys = "wu0";
            Zhuyin expected = new Zhuyin("ㄊㄧㄢ");
            Zhuyin actual;
            actual = Zhuyin.ParseKeyString(zhuyinKeys);
            Assert.That(actual, Is.EqualTo(expected));

            zhuyinKeys = "2k7";
            expected = new Zhuyin("ㄉㄜ" + Zhuyin.Tone0Char);  // 的
            actual = Zhuyin.ParseKeyString(zhuyinKeys);
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void Should_ToString_Succeed()
        {
            Zhuyin target = new Zhuyin("ㄊㄧㄢ");
            string expected = "ㄊㄧㄢ" + Zhuyin.Tone1Char;
            string actual;
            actual = target.ToString();
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void Should_GetTone_Succeed()
        {
            string zhuyinKeys = "ql3";
            ZhuyinTone expected = ZhuyinTone.Tone3;
            ZhuyinTone actual;
            actual = Zhuyin.GetTone(zhuyinKeys);
            Assert.That(actual, Is.EqualTo(expected));
        }


    }
}
