using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading;
using NChinese.Imm.WinApi;
using MsIme = NChinese.Imm.WinApi.MsIme;

namespace NChinese.Imm
{
    public enum ImeClass
    {
        China = 0,
        Japan = 1,
        Taiwan = 2,
        TaiwanBbo = 3
    }

    /// <summary>
    /// This class is a wrapper for IFELanguage.
    /// <para>Ref: http://www.mihai-nita.net/article.php?artID=20051215a</para>
    /// </summary>
    public sealed class MsImeService : IDisposable
    {
        #region Dispose pattern

        private bool _disposed;

        public bool Disposed
        {
            get
            {
                lock (this)
                {
                    return _disposed;
                }
            }
        }

        public void Dispose()
        {
            lock (this)
            {
                if (_disposed == false)
                {
                    Cleanup();
                    _disposed = true;
                    GC.SuppressFinalize(this);
                }
            }
        }

        public void Cleanup()
        {
            CloseIFELanguage();

            if (_coInitialized)
            {
                Ole32.CoUninitialize();
            }
        }

        ~MsImeService()
        {
            Cleanup();
        }

        #endregion

        private bool _coInitialized = false;
        private ImeClass _imeClass;
        private MsIme.IFELanguage _ifeLanguage;
        private bool _ifeLangOpened = false;
        private int _convModeCaps = 0;

        private static string[] s_ImeClassNames =
        {
            "MSIME.China",
            "MSIME.Japan",
            "MSIME.Taiwan",
            "MSIME.Taiwan.ImeBbo"
        };

        public MsImeService(ImeClass imeClass)
        {
            if (!_coInitialized)
            {
                WinApi.Ole32.CoInitialize(IntPtr.Zero);
                _coInitialized = true;
            }

            _imeClass = imeClass;

            try
            {
                OpenIFELanguage();
            }
            catch (Win32Exception ex)
            {
                throw new Exception(ex.Message + Environment.NewLine +
                    "Error code: 0x" + ex.ErrorCode.ToString("X2"));
            }
        }

        public static bool IsAvailable(ImeClass imeClass)
        {
            WinApi.Ole32.CoInitialize(IntPtr.Zero);

            if (Thread.CurrentThread.GetApartmentState() == ApartmentState.MTA)
            {
                throw new Exception("Cannot run under Multiple Threaded Apartment mode because it will fail while creating COM object IFELanguage.");
            }

            MsIme.IFELanguage ifeLanguage = null;
            try
            {
                int errCode = Ole32.CLSIDFromString(s_ImeClassNames[(int)imeClass], out Guid imeGuid);
                WinBase.CheckError(errCode);

                Guid feLangIID = new Guid(MsIme.Constants.IID_IFELanguage);

                IntPtr ppv;

                errCode = Ole32.CoCreateInstance(imeGuid, IntPtr.Zero, Ole32.CLSCTX_SERVER,
                    feLangIID, out ppv);
                WinBase.CheckError(errCode);

                ifeLanguage = Marshal.GetTypedObjectForIUnknown(ppv, typeof(MsIme.IFELanguage)) as MsIme.IFELanguage;

                errCode = ifeLanguage.Open();
                WinBase.CheckError(errCode);

                IntPtr cmodeCaps;
                errCode = ifeLanguage.GetConversionModeCaps(out cmodeCaps);
                WinBase.CheckError(errCode);
            }
            catch
            {
                try
                {
                    ifeLanguage?.Close();
                }
                catch
                {
                    // ignore error. 
                }
                return false;
            }
            return true;
        }

        private void OpenIFELanguage()
        {
            if (_ifeLangOpened)
            {
                CloseIFELanguage();
            }

            if (Thread.CurrentThread.GetApartmentState() == ApartmentState.MTA)
            {
                throw new Exception("Cannot run under Multiple Threaded Apartment mode because it will fail while creating COM object IFELanguage.");
            }

            Guid imeGuid;

            int errCode = Ole32.CLSIDFromString(s_ImeClassNames[(int)_imeClass], out imeGuid);
            WinBase.CheckError(errCode);

            Guid feLangIID = new Guid(MsIme.Constants.IID_IFELanguage);

            IntPtr ppv;

            errCode = Ole32.CoCreateInstance(imeGuid, IntPtr.Zero, Ole32.CLSCTX_SERVER,
                feLangIID, out ppv);
            WinBase.CheckError(errCode);

            _ifeLanguage = Marshal.GetTypedObjectForIUnknown(ppv, typeof(MsIme.IFELanguage)) as MsIme.IFELanguage;

            errCode = _ifeLanguage.Open();
            WinBase.CheckError(errCode);
            _ifeLangOpened = true;

            IntPtr cmodeCaps;
            errCode = _ifeLanguage.GetConversionModeCaps(out cmodeCaps);
            WinBase.CheckError(errCode);
            _convModeCaps = cmodeCaps.ToInt32();
        }

        private void CloseIFELanguage()
        {
            if (_ifeLangOpened)
            {
                try
                {
                    _ifeLanguage.Close();
                }
                catch
                {
                    // ignore error.
                }
            }
        }

        public bool IsReady { get => _ifeLangOpened; }

        private delegate T MorphResultHandler<T>(MsIme.MorphResult mr);

        public bool Is64Bit { get => IntPtr.Size == 8; }

        /// <summary>
        /// This method always return a simple output string, but you can provide a 
        /// MorphResultHandler to get what you want.
        /// </summary>
        /// <param name="text">Input string.</param>
        /// <param name="req">.</param>
        /// <param name="cmodes"></param>
        /// <param name="mrHandler"></param>
        /// <returns></returns>
        private T GetJMorphResult<T>(string text, MsIme.ConversionRequest req, MsIme.ConversionMode cmodes,
            MorphResultHandler<T> mrHandler)
        {
            if (mrHandler == null)
            {
                throw new Exception("GetJMorphResult needs a MorphResultHandler.");
            }

            IntPtr jmResult = IntPtr.Zero;

            try
            {
                int errCode = _ifeLanguage.GetJMorphResult(
                    (uint)req, (uint)cmodes,
                    text.Length, text, IntPtr.Zero, out jmResult);

                WinBase.CheckError(errCode);
            }
            catch (COMException comEx)
            {
                throw new Exception("GetJMorphResult failed!" + Environment.NewLine +
                    comEx.Message + Environment.NewLine + "Error code: 0x" + comEx.ErrorCode.ToString("X"));
            }
            catch (Exception ex)
            {
                throw new Exception("GetJMorphResult failed!" + Environment.NewLine + ex.Message);
            }

            try
            {
                // for debugging.
                /*
                int ptrSize = Marshal.SizeOf(typeof(IntPtr));
				string output = Marshal.PtrToStringUni(
					Marshal.ReadIntPtr(jmResult, 4),
					Marshal.ReadInt16(jmResult, 4 + IntPtr.Size));				
                System.IO.File.WriteAllText(@"C:\1.txt", output);
                */


                MsIme.MorphResult mr = (MsIme.MorphResult)Marshal.PtrToStructure(jmResult, typeof(MsIme.MorphResult));

                return mrHandler(mr);
            }
            finally
            {
                Ole32.CoTaskMemFree(jmResult);
            }
        }

        /// <summary>
        /// Get Zhuyin symbols (aka Bopomofo) of the input text. 
        /// </summary>
        /// <param name="text">Input text.</param>
        /// <returns>Zhuyin symbols in an array.</returns>
        public string[] GetZhuyin(string text)
        {
            if (_imeClass == ImeClass.Taiwan ||
                _imeClass == ImeClass.TaiwanBbo ||
                _imeClass == ImeClass.China)
            {
                string[] result = GetJMorphResult(
                    text,
                    MsIme.ConversionRequest.ReverseConversion,
                    MsIme.ConversionMode.Bopomofo,
                    (mr) => MorphResultHelper.GetMonoRubyArray(mr));
                return result;
            }
            throw new Exception("GetZhuyin can be used only when ImeClassName is Taiwan or China.");
        }

        /// <summary>
        /// 取得一串中文字的拼音。
        /// </summary>
        /// <param name="input">輸入字串。注意：輸入字串必須全部都是中文字。如果包含非中文字的字元，會因為轉換失敗而返回空陣列。</param>
        /// <returns></returns>
        public string[] GetPinyin(string input)
        {
            if (_imeClass == ImeClass.Taiwan ||
                _imeClass == ImeClass.TaiwanBbo ||
                _imeClass == ImeClass.China)
            {
                string[] result = GetJMorphResult(
                    input,
                    MsIme.ConversionRequest.ReverseConversion,
                    MsIme.ConversionMode.Pinyin,
                    (mr) => MorphResultHelper.GetMonoRubyArray(mr));
                return result;
            }
            throw new Exception("GetPinYin can be used only when ImeClassName is Taiwan or China!");
        }

        /// <summary>
        /// 取得日文平假名。NOT TESTED YET!!
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public string[] GetHiragana(string input)
        {
            string[] result = GetJMorphResult(
                input,
                MsIme.ConversionRequest.ReverseConversion,
                MsIme.ConversionMode.HiraganaOut,
                (mr) => MorphResultHelper.GetMonoRubyArray(mr));
            return result;
        }

        public static string[] ImeClassNames
        {
            get
            {
                string[] result = new string[s_ImeClassNames.Length];
                s_ImeClassNames.CopyTo(result, 0);
                return result;
            }
        }

        /// <summary>
        /// Returns conversion mode capabilities of current IME class.
        /// </summary>
        public string ConversionModeCaps
        {
            get
            {
                MsIme.ConversionMode cmodes = (MsIme.ConversionMode)_convModeCaps;
                return (cmodes.ToString());
            }
        }
    }
}
