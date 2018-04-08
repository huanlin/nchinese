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
    public sealed class MsImeFacade : IDisposable
    {
        #region Dispose pattern

        private bool m_Disposed;

        public bool Disposed
        {
            get
            {
                lock (this)
                {
                    return m_Disposed;
                }
            }
        }

        public void Dispose()
        {
            lock (this)
            {
                if (m_Disposed == false)
                {
                    Cleanup();
                    m_Disposed = true;
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

        ~MsImeFacade()
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

        public MsImeFacade(ImeClass imeClass)
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
        /// Note: This method has not tested yet!
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public string[] GetPinyin(string text)
        {
            if (_imeClass == ImeClass.Taiwan ||
                _imeClass == ImeClass.TaiwanBbo ||
                _imeClass == ImeClass.China)
            {
                string[] result = GetJMorphResult(
                    text,
                    MsIme.ConversionRequest.ReverseConversion,
                    MsIme.ConversionMode.Pinyin,
                    (mr) => MorphResultHelper.GetMonoRubyArray(mr));
                return result;
            }
            throw new Exception("GetPinYin can be used only when ImeClassName is Taiwan or China!");
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
