using System;

namespace RiggVar.FR
{
    public class TSLIterator
    {
        private int i = -1;
        private readonly int c;
        private TStrings SL;
        readonly char SpaceChar = ' ';

        public TSLIterator(TStrings aSL)
        {
            SL = aSL;
            c = SL.Count;
        }
        public int NextI()
        {
            i++;            
            if (i < c)
            {
                return Utils.StrToIntDef(SL[i], -1);
            }
            else
            {
                return -1;
            }
        }
        public string NextS()
        {
            i++;
            if (i < c)
            {
                return SL[i];
            }
            else
            {
                return string.Empty;
            }
        }
        public char NextC()
        {
            i++;
            if ((i < c) && (SL[i].Length == 1))
            {
                return SL[i][0];
            }
            else
            {
                return SpaceChar;
            }
        }
    }
}
