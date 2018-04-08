using System;

namespace NChinese.Imm
{
    public class ImmZhuyinConversionProvider : IReverseConversionProvider, IDisposable
    {
        private MsImeService _imeService;

        public ImmZhuyinConversionProvider()
        {
            _imeService = new MsImeService(ImeClass.Taiwan);
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

        ~ImmZhuyinConversionProvider()
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
            using (MsImeService ime = new MsImeService(ImeClass.China))
            {
                string[] result = ime.GetZhuyin(input);
                return result;
            }
        }
    }
}
