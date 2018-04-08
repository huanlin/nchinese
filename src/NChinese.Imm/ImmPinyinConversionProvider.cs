using System;

namespace NChinese.Imm
{
    public class ImmPinyinConversionProvider : IReverseConversionProvider, IDisposable
    {
        private MsImeService _imeService;

        public ImmPinyinConversionProvider()
        {
            _imeService = new MsImeService(ImeClass.China);
        }

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
            string[] result = _imeService.GetPinyin(input);
            return result;
        }
    }
}
