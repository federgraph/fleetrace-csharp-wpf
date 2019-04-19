using System.Collections.Generic;

namespace RiggVar.FR
{
    public class TEventNode : TBaseNode<
        TEventColGrid,
        TEventBO,
        TEventNode,
        TEventRowCollection,
        TEventRowCollectionItem,
        TEventColProps,
        TEventColProp
        >
    {
        public const int Layout_Points = 0;
        public const int Layout_Finish = 1;
        private int FShowPoints;
        public bool ShowPLZColumn;
        public bool ShowPosRColumn = true;
        public bool UseFleets;
        public int TargetFleetSize = 8;
        public int PartialCalcLastRace;
        private int FFirstFinalRace = 20;

        public TColorMode ColorMode = TColorMode.ColorMode_Error;

        public TStammdatenRowCollection StammdatenRowCollection;
        public TOTimeErrorList ErrorList;
        public int WebLayout;

        public TEventNode()
            : base()
        {
            ErrorList = new TOTimeErrorList(TMain.BO);
        }

        public void Load()
        {
            TEventRowCollectionItem o;
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
            TEventRowCollectionItem o;
            Collection.Clear();

            for (int i = 0; i < RowCount; i++)
            {
                o = Collection.AddRow();
                o.BaseID = i + 1;
                o.SNR = 999 + i + 1;
                o.Bib = i + 1;
            }
        }

        public void ClearRace(int r)
        {
            if (r >= 1 && r < RCount)
            {
                TEventRaceEntry ere;
                int f;
                foreach (TEventRowCollectionItem cr in Collection)
                {
                    ere = cr.Race[r];
                    f = ere.Fleet;
                    ere.Clear();
                    ere.Fleet = f;
                }
                Modified = true;
            }
        }
                
        public void CopyFleet(int r)
        {
            UseFleets = true;
            foreach (TEventRowCollectionItem cr in Collection)
            {
                if (r > 1 && r < RCount)
                {
                    cr.Race[r].Fleet = cr.Race[r - 1].Fleet;
                }
            }
        }

        public void DisableFleet(int r, int f, bool b)
        {
            if (r > 0 && r < RCount && UseFleets)
            {
                foreach (TEventRowCollectionItem cr in Collection)
                {
                    if (cr.Race[r].Fleet == f)
                    {
                        cr.Race[r].IsRacing = b;
                    }
                }
            }
        }

        public bool IsFinalRace(int r) => (FirstFinalRace > 0 && r >= FirstFinalRace);

        public int FirstFinalRace
        {
            get => FFirstFinalRace == 0 ? RCount : FFirstFinalRace;
            set => FFirstFinalRace = value;
        }

        public int FleetMaxProposed(int r)
        {
            int fc = 0;
            if (r > 0 && r < RCount && TargetFleetSize > 0)
            {
                TEventRowCollection cl = Collection;
                fc = cl.Count / TargetFleetSize; //cl.Count div TargetFleetSize; 
                if (TargetFleetSize > 0 && cl.Count > 0)
                {
                    while (TargetFleetSize * fc < cl.Count)
                    {
                        fc++;
                    }
                }
            }
            return fc;
        }

        public int FleetMax(int r)
        {
            int result = 0;
            if (r > 0 && r < RCount)
            {
                foreach (TEventRowCollectionItem cr in Collection)
                {
                    if (cr.Race[r].Fleet > result)
                    {
                        result = cr.Race[r].Fleet;
                    }
                }
            }
            return result;
        }

        public int FillFleetList(int r, int f, List<TEventRowCollectionItem> L)
        {
            int result = 0;
            if (r > 0 && r < RCount)
            {
                foreach (TEventRowCollectionItem cr in Collection)
                {
                    if (cr.Race[r].Fleet == f)
                    {
                        L.Add(cr);
                    }
                }
            }
            return result;
        }

        public void PartialCalc(int r)
        {
            PartialCalcLastRace = r;
            TMain.BO.CalcEV.Calc(this);
            Modified = false;
            OnCalc?.Invoke(this);
            ErrorList.CheckAll(this);
            PartialCalcLastRace = 0;
        }

        public void InitFleet(int r)
        {
            UseFleets = true;

            int fc = FleetMaxProposed(r);
            int f, c;
            bool upPhase;

            TEventRowCollection cl = Collection;
            TEventRowCollectionItem cr;
            for (int i = 0; i < cl.Count; i++)
            {
                cr = cl[i];
                c = i % fc;

                //upPhase = not Odd(i div fc);
                upPhase = ((i / fc) % 2) >= 0;

                if (upPhase)
                {
                    f = c + 1;
                }
                else
                {
                    f = fc - c;
                }

                if (r == 1)
                {
                    cr.Race[r].Fleet = f;
                }
                else if (r > 1 && r < RCount)
                {
                    cr = cl[cr.PLZ];
                    cr.Race[r].Fleet = f;
                }
            }
        }

        public void InitFleetByFinishHack(int r)
        {
            TEventRowCollection cl;
            TEventRowCollectionItem cr;
            TEventRaceEntry ere;

            int fc = FleetMaxProposed(r);
            if (r > 0 && r < RCount && TargetFleetSize > 0 && fc > 0)
            {
                UseFleets = true;
                cl = Collection;
                //clear fleet assignment
                for (int j = 0; j < cl.Count; j++)
                {
                    cr = cl[j];
                    ere = cr.Race[r];
                    ere.Fleet = 0;
                }
                //gererate new from existing finish position info
                //Fleet f, FinishPosition fp
                int f;
                for (int fp = 1; fp <= TargetFleetSize; fp++)
                {
                    f = 1;
                    for (int j = 0; j < cl.Count; j++)
                    {
                        cr = cl[j];
                        ere = cr.Race[r];
                        if (ere.OTime == fp && ere.Fleet == 0)
                        {
                            ere.Fleet = f;
                            f++;
                        }
                        if (f == fc + 2)
                        {
                            break;
                        }
                    }
                }
            }
        }

        public override void Calc()
        {
            TMain.BO.CalcEV.Calc(this);
            Modified = false;
            ErrorList.CheckAll(this);
            OnCalc?.Invoke(this);
        }

        public TNotifyEvent OnCalc { get; set; }

        public int ShowPoints
        {
            get => WebLayout > 0 ? WebLayout : FShowPoints;
            set => FShowPoints = value;
        }

        public int RCount => RaceCount + 1;

        public int RaceCount => TMain.BO != null ? TMain.BO.BOParams.RaceCount : -1;

    }

}
