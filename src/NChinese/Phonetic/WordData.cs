using System.Collections.Generic;
using System.Text;

namespace NChinese.Phonetic
{
    public class WordData
    {      
        public int Frequency { get; set; } = 0;

        public List<string> ZhuyinList { get; private set; }

        public WordData()
        {
            Frequency = 0;
            ZhuyinList = new List<string>();
        }

        public WordData(int frequency, List<string> bopomofoList)
        {
            Frequency = frequency;
            ZhuyinList = bopomofoList;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var target = obj as WordData;
            if (target == null)
                return false;

            if (Frequency != target.Frequency || ZhuyinList.Count != target.ZhuyinList.Count)
                return false;

            for (int i = 0; i < ZhuyinList.Count; i++)
            {
                if (ZhuyinList[i].Equals(target.ZhuyinList[i]) == false)
                    return false;
            }
            return true;
        }

        public override string ToString()
        {
            // note: 不輸出字詞頻率
            var sb = new StringBuilder();
            foreach (var zy in ZhuyinList)
            {
                sb.Append(zy);
                sb.Append(" ");
            }
            return sb.ToString().Trim();
        }
    }

}
