namespace RiggVar.FR
{

    public class TRaceRowCollectionItem : TBaseRowCollectionItem<
        TRaceColGrid,
        TRaceBO,
        TRaceNode, 
        TRaceRowCollection,
        TRaceRowCollectionItem, 
        TRaceColProps,
        TRaceColProp
        >
    {
        private readonly TPenalty FQU;
        private TTimePoint[] FIT;

        public TRaceRowCollectionItem()
            : base()
        {
            FQU = new TPenaltyISAF();
            FIT = new TTimePoint[TMain.BO.BOParams.ITCount + 1];
            for (int i = 0; i < ITCount; i++)
            {
                FIT[i] = new TTimePoint();
            }

            FT = FIT[0];
        }

        public override void Assign(TRaceRowCollectionItem Source)
        {
            if (Source != null)
            {
                TRaceRowCollectionItem o = Source;
                Bib = o.Bib;
                SNR = o.SNR;
                QU.Assign(o.QU);
                DG = o.DG;
                MRank = o.MRank;
                ST.Assign(o.ST);
                for (int i = 0; i < ITCount; i++)
                {
                    this[i].Assign(o[i]);
                }
            }
        }

        public void Assign(TRaceEntry Source)
        {
            if (Source != null)
            {
                TRaceEntry e = Source;
                Bib = e.Bib;
                SNR = e.SNR;
                QU.FromString(e.QU);
                DG = e.DG;
                MRank = e.MRank;
                ST.Parse(e.ST);
                for (int i = 0; i < ITCount; i++)
                {
                    this[i].Assign(e.IT[i]);
                }
            }
        }

        public override void ClearList()
        {
            base.ClearList();
            SNR = 0;
            Bib = BaseID;
        }

        public override void ClearResult()
        {
            base.ClearResult();
            ST.Clear();
            DG = 0;
            MRank = 0;
            QU.Clear();
            for (int i = 0; i < ITCount; i++)
            {
                this[i].Clear();
            }
        }

        private TStammdatenRowCollectionItem GetSDItem()
        {
            return Ru.StammdatenRowCollection.FindKey(SNR);
        }

        public TPTime ST
        {
            get => Ru.ST;
            set
            {
                if (value != null)
                {
                    Ru.ST.Assign(value);
                }
            }
        }

        public TTimePoint this[int Index] => (Index >= 0) && (Index <= ITCount) ? FIT[Index] : null;

        public TTimePoint FT { get; }

        public int ITCount => FIT.Length;

        public int Bib { get; set; }
        public int SNR { get; set; }

        public string FN
        {
            get
            {
                TStammdatenRowCollectionItem sd = GetSDItem();
                return sd != null ? sd.FN : string.Empty;
            }
        }

        public string LN
        {
            get
            {
                TStammdatenRowCollectionItem sd = GetSDItem();
                return sd != null ? sd.LN : string.Empty;
            }
        }

        public string SN
        {
            get
            {
                TStammdatenRowCollectionItem sd = GetSDItem();
                return sd != null ? sd.SN : string.Empty;
            }
        }

        public string NC
        {
            get
            {
                TStammdatenRowCollectionItem sd = GetSDItem();
                return sd != null ? sd.NC : string.Empty;
            }
        }

        public string GR
        {
            get
            {
                TStammdatenRowCollectionItem sd = GetSDItem();
                return sd != null ? sd.GR : string.Empty;
            }
        }

        public string PB
        {
            get
            {
                TStammdatenRowCollectionItem sd = GetSDItem();
                return sd != null ? sd.PB : string.Empty;
            }
        }

        public TPenalty QU
        {
            get => FQU;
            set
            {
                if (value != null)
                {
                    QU.Assign(value);
                }
            }
        }

        public int DG { get; set; }

        public int MRank { get; set; }

    }

}
