using System.ComponentModel;

namespace NChinese.Imm.WinApi
{
    public static class WinBase
	{
		const int S_OK = 0x00000000;

		public static void CheckError(int errorCode)
		{
			if (errorCode != S_OK)
			{
				throw new Win32Exception(errorCode);
			}
		}
	}
}
