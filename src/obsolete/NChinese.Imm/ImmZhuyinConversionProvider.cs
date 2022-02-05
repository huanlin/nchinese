using System;
using System.Collections.Generic;
using NChinese.Phonetic;
using Serilog;

namespace NChinese.Imm
{
    /// <summary>
    /// 注音字根提供者。
    /// 此類別會自動根據作業環境來研判是要直接取得注音字根，還是先取得拼音字根之後再將拼音符號轉成注音符號。
    /// 之所以用到拼音，是因為 IFELanguage 在某些 Windows 環境中（例如 Windows 10）無法提供 MSIME.Taiwan API，也就是無法取得注音字根。
    /// 
    /// IFELanguage 的好處是可以一次傳入多個中文字，取得所有對應的注音字根，而且它會根據中文字的詞庫自動傳回正確的字根，例如：「一兵一卒」的兩個「一」都會傳回「　ㄧ　ˋ」。
    /// 
    /// 由於新注音 API 可一次取得多個中文字的整串注音字根，因此自然就無法取得多音字的所有字根。若需取得多音字的全部注音字根，請使用 ZhuyinQueryHelper 類別。
    /// </summary>
    public class ImmZhuyinConversionProvider : IReverseConversionProvider, IDisposable
    {
        private MsImeService _imeService;

        public bool IsPinyinProviderUsed { get; private set; }

        public ImmZhuyinConversionProvider()
        {
            IsPinyinProviderUsed = false;

            try
            {
                _imeService = new MsImeService(ImeClass.Taiwan);
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Failed to initialize IFELanguage COM objects for ImeClass.Taiwan.");
            }
            if (_imeService == null || _imeService.IsReady == false)
            {
                try
                {
                    _imeService = new MsImeService(ImeClass.China);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, $"Failed to initialize IFELanguage COM objects for ImeClass.China.");
                }

                if (_imeService.IsReady)
                {
                    IsPinyinProviderUsed = true;
                }
            }                      
        }

        public bool IsAvailable => _imeService != null && _imeService.IsReady;

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
            if (!IsAvailable)
            {
                throw new Exception("IFELanguage is not available!");
            }

            if (IsPinyinProviderUsed)
            {
                var pinyinArray = _imeService.GetPinyin(input);
                var zhuyinList = new List<string>();
                foreach (var pinyin in pinyinArray)
                {
                    var zhuyin = PinyinToZhuyin.Convert(pinyin);
                    zhuyinList.Add(zhuyin);
                }
                return zhuyinList.ToArray();
            }


            var zhuyinArray = _imeService.GetZhuyin(input);
            return zhuyinArray;
        }
    }
}
