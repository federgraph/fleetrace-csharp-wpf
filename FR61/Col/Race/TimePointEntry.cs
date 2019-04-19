namespace RiggVar.FR
{

    public class TTimePointEntry : TBOPersistent
    {
        public string OTime;
        public int ORank;
        public int Rank;
        public int PosR;
        public string Behind;
        public string BFT;
        public string BPL;
        public int PLZ;

        public override void Assign(object source)
        {
            if (source is TTimePointEntry)
            {
                TTimePointEntry e = source as TTimePointEntry;
                OTime = e.OTime;
                Behind = e.Behind;
                BFT = e.BFT;
                BPL = e.BPL;
                ORank = e.ORank;
                Rank = e.Rank;
                PosR = e.PosR;
                PLZ = e.PLZ;
            }
            else if (source is TTimePoint)
            {
                TTimePoint o = source as TTimePoint;
                OTime = o.OTime.ToString();
                Behind = o.Behind.ToString();
                BFT = o.BFT.ToString();
                BPL = o.BPL.ToString();
                ORank = o.ORank;
                Rank = o.Rank;
                PosR = o.PosR;
                PLZ = o.PosR;
            }
            else
            {
                base.Assign(source);
            }
        }
    }

}
