namespace RiggVar.FR
{

    public class TRaceEntry : TBaseEntry
    {
        public int Bib;

        public int SNR;
        public string FN;
        public string LN;
        public string SN;
        public string NC;
        public string GR;
        public string PB;

        public string QU;
        public int DG;
        public string ST;
        public TTimePointEntry[] IT;
        public TTimePointEntry FT;

        public int MRank;

        public TRaceEntry(int aITCount)
        {
            IT = new TTimePointEntry[aITCount + 1];
            for (int i = 0; i < ITCount; i++)
            {
                IT[i] = new TTimePointEntry();
            }

            FT = IT[0];
        }

        public virtual string RunID
        {
            get => string.Empty;
            set
            {
            }
        }

        public int ITCount => IT.Length;

    }

}
