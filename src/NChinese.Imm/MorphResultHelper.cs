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

			string output = Marshal.PtrToStringUni(mr.PtrToOutputString, mr.OutputLength);

            // 當輸入字串包含非中文字的字元時，OutputLength 可能大於 0，但輸出字串的第一個字元卻是 '\0'
            if (String.IsNullOrEmpty(output) || output[0] == '\0')
            {                
                return new string[] { }; 
            }

            List<string> results = new List<string>();

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
