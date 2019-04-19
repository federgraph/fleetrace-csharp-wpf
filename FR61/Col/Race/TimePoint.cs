namespace RiggVar.FR
{

    public class TTimePoint : TBOPersistent
    {
        private TPTime FOTime = new TPTime();
        private TPTime FBehind = new TPTime();
        private TQTime FBFT = new TQTime();
        private TQTime FBPL = new TQTime();
        
        public int ORank;
        public int Rank;
        public int PosR;
        public int PLZ;
        
        public TTimePoint()
        {
        }

        public override void Assign(object source)
        {
            if (source is TTimePoint o)
            {
                OTime.Assign(o.OTime);
                Behind.Assign(o.Behind);
                BFT.Assign(o.BFT);
                BPL.Assign(o.BPL);
                ORank = o.ORank;
                Rank = o.Rank;
                PosR = o.PosR;
                PLZ = o.PLZ;
            }
            else if (source is TTimePointEntry e)
            {
                OTime.Parse(e.OTime);
                Behind.Parse(e.Behind);
                BFT.Parse(e.BFT);
                BPL.Parse(e.BPL);
                ORank = e.ORank;
                Rank = e.Rank;
                PosR = e.PosR;
                PLZ = e.PLZ;
            }
            else
            {
                base.Assign(source);
            }
        }

        public void Clear()
        {
            OTime.Clear();
            Behind.Clear();
            BFT.Clear();
            BPL.Clear();
            ORank = 0;
            Rank = 0;
            PosR = 0;
            PLZ = 0;
        }

        public TPTime OTime
        {
            get => FOTime;
            set
            {
                if (value != null)
                {
                    FOTime.Assign(value);
                }
            }
        }

        public TPTime Behind
        {
            get => FBehind;
            set
            {
                if (value != null)
                {
                    FBehind.Assign(value);
                }
            }
        }

        public TQTime BFT
        {
            get => FBFT;
            set
            {
                if (value != null)
                {
                    FBFT.Assign(value);
                }
            }
        }

        public TQTime BPL
        {
            get => FBPL;
            set
            {
                if (value != null)
                {
                    FBPL.Assign(value);
                }
            }
        }

    }

}
