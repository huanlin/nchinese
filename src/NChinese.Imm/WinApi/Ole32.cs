using System;
using System.Runtime.InteropServices;

namespace NChinese.Imm.WinApi
{
    /// <summary>
    /// A simple wrapper for OLE32.dll.
    /// </summary>
    public static class Ole32
	{
		public const int CLSCTX_INPROC_SERVER = 1;
		public const int CLSCTX_INPROC_HANDLER = 2;
		public const int CLSCTX_LOCAL_SERVER = 4;
		public const int CLSCTX_SERVER = CLSCTX_INPROC_SERVER | CLSCTX_LOCAL_SERVER;

		[DllImport("ole32.dll")]
		public static extern int CLSIDFromString(

		[MarshalAs(UnmanagedType.LPWStr)] string lpsz, out Guid clsid);
		[DllImport("ole32.dll")]
		public static extern int CoCreateInstance(
			[In, MarshalAs(UnmanagedType.LPStruct)] Guid clsid,
			IntPtr pUnkOuter,
			uint dwClsContext,
			[In, MarshalAs(UnmanagedType.LPStruct)] Guid iid,
			out IntPtr pv);

		[DllImport("ole32.dll", CallingConvention = CallingConvention.StdCall)]
		public static extern int CoInitialize(IntPtr pvReserved);

		[DllImport("ole32.dll", CallingConvention = CallingConvention.StdCall)]
		public static extern void CoUninitialize();

		[DllImport("ole32.dll")]
		public static extern void CoTaskMemFree(IntPtr ptr);

	}
}
