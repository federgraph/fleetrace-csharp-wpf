namespace RiggVar.FR
{
    public class TRaceNode : TBaseNode<
        TRaceColGrid,
        TRaceBO,
        TRaceNode,
        TRaceRowCollection,
        TRaceRowCollectionItem,
        TRaceColProps,
        TRaceColProp
        >
    {
        private TNotifyEvent FOnCalc;
        public TBOParams BOParams;
        public TStammdatenRowCollection StammdatenRowCollection; //shortcut
        public int Index;
        public int[] BestTime;
        public int[] BestIndex;
        public TPTime ST = new TPTime();
        public bool IsRacing = true;

        public TRaceNode()
            : base()
        {
            BestTime = new int[1];
            BestIndex = new int[1];

            TBO o = TMain.BO;
            BOParams = o.BOParams; //nicht erzeugt, nur Referenz kopiert
            StammdatenRowCollection = o.StammdatenNode.Collection;
            //..einiges wird in TBO.Init gesetzt, wo der RaceNode erzeugt wird
            BestTime = new int[BOParams.ITCount + 1];
            BestIndex = new int[BOParams.ITCount + 1];
        }

        public void Load()
        {
            TRaceRowCollectionItem o;
            Collection.Clear();

            o = Collection.AddRow();
            o.SNR = 1001;
            o.Bib = 1;
            o.BaseID = 1;

            o = Collection.AddRow();
            o.SNR = 1002;
            o.Bib = 2;
            o.BaseID = 2;

            o = Collection.AddRow();
            o.SNR = 1003;
            o.Bib = 3;
            o.BaseID = 3;
        }

        public void Init(int RowCount)
        {
            TRaceRowCollectionItem o;
            Collection.Clear();

            for (int i = 0; i < RowCount; i++)
            {
                o = Collection.AddRow();
                o.BaseID = i + 1;
                o.SNR = 1000 + i + 1;
                o.Bib = i + 1;
            }
        }

        public TRaceRowCollectionItem FindSNR(int SNR)
        {
            if (SNR == 0)
            {
                return null;
            }

            TRaceRowCollection cl = Collection;
            TRaceRowCollectionItem cr;
            for (int i = 0; i < cl.Count; i++)
            {
                cr = cl[i];
                if (cr.SNR == SNR)
                {
                    return cr;
                }
            }
            return null;
        }

        public TRaceRowCollectionItem FindBib(int Bib)
        {
            if (Bib == 0)
            {
                return null;
            }

            TRaceRowCollection cl = Collection;
            TRaceRowCollectionItem cr;
            for (int i = 0; i < cl.Count; i++)
            {
                cr = cl[i];
                if (cr.Bib == Bib)
                {
                    return cr;
                }
            }
            return null;
        }

        public void CopyToMRank()
        {
            TRaceRowCollection cl = Collection;
            TRaceRowCollectionItem cr;
            for (int i = 0; i < cl.Count; i++)
            {
                cr = cl[i];
                cr.MRank = cr.FT.PosR;
            }
        }

        public void CopyFromMRank()
        {
            TRaceRowCollection cl = Collection;
            TRaceRowCollectionItem cr;
            for (int i = 0; i < cl.Count; i++)
            {
                cr = cl[i];
                if (cr.MRank == 0)
                {
                    cr.FT.OTime.Clear();
                }
                else
                {
                    cr.FT.OTime.Parse(cr.MRank.ToString());
                }

                cr.Modified = true;
            }
        }

        public override void Calc()
        {
            TMain.BO.CalcTP.Calc(this);
            Modified = false;
            if (OnCalc != null)
            {
                OnCalc(this);
            }
        }

        public TNotifyEvent OnCalc
        {
            get => FOnCalc;
            set => FOnCalc = value;
        }

    }

}
