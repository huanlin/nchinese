using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
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

			if (m_CoInitialized)
			{
				Ole32.CoUninitialize();
			}
		}

		~MsImeFacade()
		{
			Cleanup();
		}

		#endregion

		private bool m_CoInitialized = false;
		private ImeClass m_ImeClass;
		private MsIme.IFELanguage m_IfeLanguage;
		private bool m_IfeLangOpened = false;
		private int m_ConvModeCaps = 0;

		private static string[] s_ImeClassNames =
		{
			"MSIME.China",
			"MSIME.Japan",
			"MSIME.Taiwan",
			"MSIME.Taiwan.ImeBbo"
		};

		public MsImeFacade(ImeClass imeClass)
		{
			if (!m_CoInitialized)
			{
				WinApi.Ole32.CoInitialize(IntPtr.Zero);
				m_CoInitialized = true;
			}

			m_ImeClass = imeClass;

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
			if (m_IfeLangOpened)
			{
				CloseIFELanguage();
			}

			Guid imeGuid;

			int errCode = Ole32.CLSIDFromString(s_ImeClassNames[(int)m_ImeClass], out imeGuid);
			WinBase.CheckError(errCode);

			Guid feLangIID = new Guid(MsIme.Constants.IID_IFELanguage);

			IntPtr ppv;

			errCode = Ole32.CoCreateInstance(imeGuid, IntPtr.Zero, Ole32.CLSCTX_SERVER,
				feLangIID, out ppv);
			WinBase.CheckError(errCode);

			m_IfeLanguage = Marshal.GetTypedObjectForIUnknown(ppv, typeof(MsIme.IFELanguage)) as MsIme.IFELanguage;

			errCode = m_IfeLanguage.Open();
			WinBase.CheckError(errCode);
			m_IfeLangOpened = true;

			IntPtr cmodeCaps;
			errCode = m_IfeLanguage.GetConversionModeCaps(out cmodeCaps);
			WinBase.CheckError(errCode);
			m_ConvModeCaps = cmodeCaps.ToInt32();

			// TODO: Error processing and release COM object.
		}

		private void CloseIFELanguage()
		{
			if (m_IfeLangOpened)
			{
				try
				{
					m_IfeLanguage.Close();
				}
				catch
				{
					// ignore error.
				}
			}
		}

		public bool IsReady
		{
			get
			{
				return m_IfeLangOpened;
			}
		}

		private delegate T MorphResultHandler<T>(MsIme.MorphResult mr);

        private bool m_Is64Bit = (IntPtr.Size == 8);

        public bool Is64Bit
        {
            get { return m_Is64Bit; }
        }

		/// <summary>
		/// This method always return a simple output string, but you can provide a 
		/// MorphResultHandler to get what you want.
		/// </summary>
		/// <param name="text">Input string.</param>
		/// <param name="req">.</param>
		/// <param name="cmodes"></param>
		/// <param name="mrHandler"></param>
		/// <returns></returns>
		private T GetJMorphResult<T>(string text, MsIme.Req req, MsIme.Cmodes cmodes,
			MorphResultHandler<T> mrHandler)
		{
			if (mrHandler == null)
			{
				throw new Exception("GetJMorphResult needs a MorphResultHandler.");
			}

			IntPtr jmResult = IntPtr.Zero;

			try
			{
				int errCode = m_IfeLanguage.GetJMorphResult(
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
		/// Get output string from unmanaged MORRSLT struct.
		/// </summary>
		/// <param name="text"></param>
		/// <param name="req"></param>
		/// <param name="cmodes"></param>
		/// <returns>Output string.</returns>
		public string GetJMorphResultString(string text, MsIme.Req req, MsIme.Cmodes cmodes)
		{
			MorphResultHandler<string> mrHandler = MorphResultHelper.GetOutputString;
			return GetJMorphResult(text, req, cmodes, mrHandler);
		}


		/// <summary>
		/// Get Bopomofo (Ju-yin-fu-hao) of input text. Bopomofo is also known as Mandarin Phonetic Symbols.
		/// </summary>
		/// <param name="text">Input text.</param>
		/// <returns>BoPoMoFo (Ju-Yin).</returns>
		public string[] GetBopomofo(string text)
		{
			if (m_ImeClass == ImeClass.Taiwan ||
				m_ImeClass == ImeClass.TaiwanBbo ||
				m_ImeClass == ImeClass.China)
			{
				MorphResultHandler<string[]> mrHandler = MorphResultHelper.GetMonoRubyArray;
				return GetJMorphResult(text, MsIme.Req.Rev, MsIme.Cmodes.BoPoMoFo, mrHandler);
			}
			throw new Exception("GetBoPoMoFo can be used only when ImeClassName is Taiwan or China.");
		}

		/// <summary>
		/// Note: This method has not tested yet!
		/// </summary>
		/// <param name="text"></param>
		/// <returns></returns>
		public string GetPinYin(string text)
		{
			if (m_ImeClass == ImeClass.Taiwan ||
				m_ImeClass == ImeClass.TaiwanBbo ||
				m_ImeClass == ImeClass.China)
			{
				MorphResultHandler<string> mrHandler = MorphResultHelper.GetOutputString;
				return GetJMorphResult(text, MsIme.Req.Rev, MsIme.Cmodes.PinYin, mrHandler);
			}
			throw new Exception("GetPinYin can be used only when ImeClassName is Taiwan or China!");
		}

		public string GetPhonetic(string text)
		{
			//TODO: Implemet GetPhonetic.
			throw new Exception("ImeEngine.GetPhonetic has not implemented yet!");
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
				MsIme.Cmodes cmodes = (MsIme.Cmodes)this.m_ConvModeCaps;
				return (cmodes.ToString());
			}
		}
	}
}
