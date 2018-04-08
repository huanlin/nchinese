using System;
using System.Text;

namespace NChinese.Imm
{
    public class ImmPinyinConversionProvider : IReverseConversionProvider, IDisposable
    {
        private MsImeService _imeService;

        public ImmPinyinConversionProvider()
        {
            _imeService = new MsImeService(ImeClass.China);
        }

        public bool FixForTraditionalChinese { get; set; } = true;

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // dispose managed state (managed objects).
                }

                _imeService.Dispose();

                disposedValue = true;
            }
        }

        ~ImmPinyinConversionProvider()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(false);
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

        public string[] Convert(string input)
        {
            if (FixForTraditionalChinese)
            {
                var sb = new StringBuilder(input);
                sb.Replace("內", "内"); // 注意兩個字元不一樣! 前者的拼音會讀作「納」。
                sb.Replace("過", "过"); // 若不作此調整，「過」拼音會讀作一聲的「郭」。
                input = sb.ToString();
            }

            string[] result = _imeService.GetPinyin(input);
            return result;
        }
    }
}
