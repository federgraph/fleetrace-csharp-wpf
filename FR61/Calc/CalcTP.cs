namespace RiggVar.FR
{
    public class TCalcTP
    {
        public TBO BO;
        private TRaceNode qn;
        private TQProxy FProxy;

        public TCalcTP(TBO abo)
        {
            BO = abo;
            FProxy = new TQProxy1();
        }

        private void LoadProxy(TRaceNode qn, int channel)
        {
            TRaceRowCollection cl = qn.Collection;
            FProxy.Count = cl.Count;
            for (int i = 0; i < cl.Count; i++)
            {
                TRaceRowCollectionItem cr = cl[i];
                FProxy.Bib[i] = cr.Bib;
                FProxy.DSQGate[i] = cr.DG;
                FProxy.Status[i] = cr.QU.AsInteger;
                FProxy.OTime[i] = cr[channel].OTime.AsInteger;
            }
        }

        private void UnLoadProxy(TRaceNode qn, int channel)
        {
            TRaceRowCollection cl = qn.Collection;
            if (FProxy.Count != cl.Count)
            {
                return;
            }

            for (int i = 0; i < cl.Count; i++)
            {
                TRaceRowCollectionItem cr = cl[i];
                cr[channel].Behind.AsInteger = FProxy.TimeBehind[i];
                cr[channel].ORank = FProxy.ORank[i];
                cr[channel].Rank = FProxy.Rank[i];
                cr[channel].PosR = FProxy.PosR[i];
                cr[channel].PLZ = FProxy.PLZ[i];
                cr.Ru.BestTime[channel] = FProxy.BestOTime;
                cr.Ru.BestIndex[channel] = FProxy.BestIndex;
            }
        }

        private void CalcQA()
        {
            for (int i = 0; i <= BO.BOParams.ITCount; i++)
            {
                LoadProxy(qn, i);
                FProxy.Calc();
                UnLoadProxy(qn, i);
            }
        }

        public void Calc(TRaceNode aqn)
        {
            qn = aqn;
            CalcQA();
        }

        public void UpdateDynamicBehind(TRaceBO bo, TRaceRowCollectionItem cr, int channel)
        {
            TRaceRowCollection cl;
            TRaceNode rd;
            TPTime refTime;

            if (cr == null)
            {
                return;
            }

            rd = cr.Ru;

            //Zwischenzeiten
            if (channel > 0)
            {
                //TimeBehind in Bezug auf die Zwischenzeit des IT-Besten
                cr[channel].BPL.UpdateQualiTimeBehind(
                    rd.BestTime[channel],
                    cr[channel].OTime.AsInteger);

                //TimeBehind in Bezug auf die Zwischenzeit des FT-Besten
                cl = rd.Collection;
                refTime = cl[rd.BestIndex[0]][channel].OTime;
                if ((rd.BestIndex[0] != cr.IndexOfRow) && refTime.TimePresent)
                {
                    cr[channel].BFT.UpdateQualiTimeBehind(
                        refTime.AsInteger,
                        cr[channel].OTime.AsInteger);
                }
                else
                {
                    cr[channel].BFT.Clear();
                }
            }

            //Zielzeit
            else
            {
                cr.FT.BPL.UpdateQualiTimeBehind(
                    rd.BestTime[0],
                    cr.FT.OTime.AsInteger);
            }
        }

        public bool HighestBibGoesFirst
        {
            get => FProxy.HighestBibGoesFirst;
            set => FProxy.HighestBibGoesFirst = value;
        }

    }

}
