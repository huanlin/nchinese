using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using MsIme = NChinese.Imm.WinApi.MsIme;

namespace NChinese.Imm
{
    internal static class MorphResultHelper
	{
		public static string GetOutputString(MsIme.MorphResult mr)
		{
			return Marshal.PtrToStringUni(mr.PtrToOutputString, mr.OutputLength);
		}

		public static string[] GetMonoRubyArray(MsIme.MorphResult mr)
		{
            // TODO: Couldn't work when build with x64 platform.
			if (mr.PtrToMonoRubyPosArray.ToInt32() == 0)
			{
				throw new Exception("Monoruby position array is NULL!");
			}

			List<string> results = new List<string>();

			string output = Marshal.PtrToStringUni(mr.PtrToOutputString, mr.OutputLength);

			int offset = 0;
			int lastIndex = -1;
			int currIndex;
			int totalCount = 0;

			while (totalCount < output.Length) 
			{
				currIndex = Marshal.ReadInt16(mr.PtrToMonoRubyPosArray, offset);
				if (lastIndex >= 0) 
				{
					int length = currIndex - lastIndex;
					results.Add(output.Substring(lastIndex, length));
					totalCount += length;
				}
				lastIndex = currIndex;
				offset += 2;			
			}
			return results.ToArray();
		}
	}
}
