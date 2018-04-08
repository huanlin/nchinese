using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using NChinese.Imm.WinApi;

namespace NChinese.Imm
{
    /// <summary>
    /// This class provides some helper methods for IMM32.DLL.
    /// </summary>
    public static class ImmHelper
	{
		private static List<KeyboardLayoutInfo> _keyboardLayoutList;

		// Class constructor
		static ImmHelper()
		{
			_keyboardLayoutList = new List<KeyboardLayoutInfo>();
			InitializeKeyboardLayoutList();
		}

		/// <summary>
		/// 取得指定輸入法的 keyboard layout handle。
		/// </summary>
		/// <param name="imeName">輸入法名稱。</param>
		/// <returns>若成功取得輸入法 handle，則傳回 handle 指標，否則傳回空指標。</returns>
		public static void InitializeKeyboardLayoutList()
		{
			const int MaxHkl = 20;

			IntPtr[] hklList = new IntPtr[MaxHkl];
			int cntHkl;
			int i;
			IntPtr imeNamePtr;
			string imeName;

			cntHkl = Imm32.GetKeyboardLayoutList(hklList.Length, hklList);

			if (cntHkl < 1)
			{
				return;
			}

			imeNamePtr = Marshal.AllocHGlobal(256);
			try
			{
				for (i = 0; i < cntHkl; i++)
				{
					if (Imm32.ImmEscape(hklList[i], IntPtr.Zero, Imm32.IME_ESC_IME_NAME, imeNamePtr) == 0)
					{
						continue;
					}
					imeName = Marshal.PtrToStringUni(imeNamePtr);

					KeyboardLayoutInfo kli = new KeyboardLayoutInfo(imeName, hklList[i]);
					_keyboardLayoutList.Add(kli);
				}
			}
			finally
			{
				Marshal.FreeHGlobal(imeNamePtr);
			}
		}

		/// <summary>
		/// 檢查某個輸入法是否有安裝。
		/// </summary>
		/// <param name="imeNameToCheck"></param>
		/// <returns></returns>
		public static bool IsImeInstalled(string imeNameToCheck)
		{
			foreach (KeyboardLayoutInfo kli in _keyboardLayoutList)
			{
				if (kli.ImeName.Equals(imeNameToCheck))
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// 是否有安裝注音輸入法。
		/// </summary>
		/// <returns></returns>
		public static bool IsPhoneticImeInstalled()
		{
			return IsImeInstalled("注音");
		}

		/// <summary>
		/// 是否有安裝微軟新注音。
		/// </summary>
		/// <returns></returns>
		public static bool IsNewPhoneticImeInstalled()
		{
			return IsImeInstalled("新注音");
		}       
	}

	/// <summary>
	/// 此類別用來儲存某個輸入法的名稱以及 keyboard layout handle。
	/// </summary>
	public class KeyboardLayoutInfo
	{
		public string ImeName;	// 輸入法名稱
		public IntPtr Handle;	// Keyboard layout handle.

		public KeyboardLayoutInfo(string imeName, IntPtr handle)
		{
			this.ImeName = imeName;
			this.Handle = handle;
		}
	}
}
