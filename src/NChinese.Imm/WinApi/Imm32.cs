using System;
using System.Runtime.InteropServices;

namespace NChinese.Imm.WinApi
{
    public static class Imm32
    {
        private const string ImmDll = "Imm32.dll";
		private const string UserDll = "user32.dll";

		[DllImport(UserDll, EntryPoint="GetKeyboardLayout", CharSet=CharSet.Auto, SetLastError=true)]
		public static extern IntPtr GetKeyboardLayout(int dwLayout);

        [DllImport(UserDll, EntryPoint="GetKeyboardLayoutList", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern int GetKeyboardLayoutList(int nBuff, IntPtr[] list);

        [DllImport(ImmDll, EntryPoint = "ImmEscapeW", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int ImmEscape(IntPtr aHkl, IntPtr aHimc, int aEscape, IntPtr aData);

        [DllImport("Imm32.dll", EntryPoint="ImmGetConversionListW", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int ImmGetConversionList(IntPtr hKL, IntPtr hImc, string lpSrc, IntPtr lpDst, int dwBufLen, int uFlag);

        [DllImport("Imm32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr ImmGetContext(IntPtr hWnd);

        [DllImport("Imm32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool ImmReleaseContext(IntPtr hWnd, IntPtr hImc);

        const int GCL_REVERSECONVERSION = 0x0002;
        [StructLayout(LayoutKind.Sequential)]
        private class CANDIDATELIST
        {
            public int dwSize;
            public int dwStyle;
            public int dwCount;
            public int dwSelection;
            public int dwPageStart;
            public int dwPageSize;
            public int dwOffset;
        }

        // Dialog mode of ImmEscape 
        public const int IME_ESC_QUERY_SUPPORT = 0x0003;
		public const int IME_ESC_RESERVED_FIRST = 0x0004;
		public const int IME_ESC_RESERVED_LAST = 0x07FF;
		public const int IME_ESC_PRIVATE_FIRST = 0x0800;
		public const int IME_ESC_PRIVATE_LAST = 0x0FFF;
		public const int IME_ESC_SEQUENCE_TO_INTERNAL = 0x1001;
		public const int IME_ESC_GET_EUDC_DICTIONARY = 0x1003;
		public const int IME_ESC_SET_EUDC_DICTIONARY = 0x1004;
		public const int IME_ESC_MAX_KEY = 0x1005;
		public const int IME_ESC_IME_NAME = 0x1006;
		public const int IME_ESC_SYNC_HOTKEY = 0x1007;
		public const int IME_ESC_HANJA_MODE = 0x1008;
		public const int IME_ESC_AUTOMATA = 0x1009;
		public const int IME_ESC_PRIVATE_HOTKEY = 0x100A;
    }
}
