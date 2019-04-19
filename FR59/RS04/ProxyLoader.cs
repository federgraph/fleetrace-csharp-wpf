namespace RiggVar.Scoring
{

    /// <summary>
    /// Loads and unloads proxy data to and from a new regatta object.
    /// </summary>
    public class TProxyLoader {

        public const int THROWOUT_NONE = 4;

        protected TRegatta regatta; //temporary regatta object
        internal TFRProxy proxyNode; //passed via parameter to calc
        internal TJSEventProps EventProps; //shortcut to proxyNode.EventProps;

        public TProxyLoader( ) 
        {
        }

        public void Calc(TFRProxy proxy)
        {
            proxyNode = proxy;
            EventProps = proxyNode.EventProps;
            if (proxyNode.EntryInfoCollection.Count == 0)
            {
                return;
            }

            regatta = new TRegatta();
            try
            {
                if (EventProps.ScoringSystem == TJSEventProps.Bonus)
                {
                    regatta.ScoringManager.Model = TScoringBonusPoint.NAME_BonusPointSystem;
                }
                else if (EventProps.ScoringSystem == TJSEventProps.BonusDSV)
                {
                    regatta.ScoringManager.Model = TScoringBonusPointDSV.NAME_BonusPointSystemDSV;
                }

                InitThrowoutScheme();
                LoadProxy();
                regatta.ScoreRegatta();

                UnLoadProxy();
            }
            finally
            {
                regatta = null;
            }
        }

        private void InitThrowoutScheme()
        {
            //precondition
            if (regatta == null)
            {
                return;
            }

            if (proxyNode == null)
            {
                return;
            }

            if (proxyNode.EntryInfoCollection.Count == 0)
            {
                return;
            }

            //count of sailed races
            int IsRacingCount = 0;
            for (int r = 1; r < proxyNode.RCount; r++)
            {
                if (proxyNode.IsRacing[r])
                {
                    IsRacingCount++;
                }
            }

            //number of throwouts
            int t = proxyNode.EventProps.Throwouts;
            if (t >= IsRacingCount)
            {
                t = IsRacingCount - 1;
            }

            TScoringLowPoint sm = new TScoringLowPoint();

            sm.SetFixedNumberOfThrowouts(t);

            sm.FirstIs75 = EventProps.FirstIs75;            
            sm.ReorderRAF = EventProps.ReorderRAF;

            if (EventProps.ScoringSystem == TJSEventProps.Bonus)
            {
                regatta.ScoringManager.Model = TScoringBonusPoint.NAME_BonusPointSystem;
            }
            else if (EventProps.ScoringSystem == TJSEventProps.BonusDSV)
            {
                regatta.ScoringManager.Model = TScoringBonusPointDSV.NAME_BonusPointSystemDSV;
            }
            else
            {
                regatta.ScoringManager.Model = TScoringLowPoint.NAME_LowPoint;
            }

            IScoringModel m = regatta.ScoringManager.GetModel();
            m.Attributes = sm;
        }

        private void LoadProxy()
        {
            TEntry e;
            TRace r;
            TFinish f;
            TFinishPosition fp;
            TRSPenalty p;

            TEntryInfo cr;
            TRaceInfo er;

            if (regatta == null)
            {
                return;
            }

            if (proxyNode == null)
            {
                return;
            }

            if (proxyNode.RCount > proxyNode.FirstFinalRace)
            {
                TRegatta.IsInFinalPhase = true;
            }

            TEntryInfoCollection cl = proxyNode.EntryInfoCollection;
            for (int i = 0; i < cl.Count; i++)
            {
                cr = cl[i];
                cr.IsGezeitet = false;
                e = new TEntry
                {
                    SailID = cr.SNR
                };
                regatta.AddEntry(e);

                for (int j = 1; j < cr.RCount; j++)
                {
                    if (i == 0)
                    {
                        r = new TRace(j);
                        regatta.Races.Add(r);
                        r.IsRacing = proxyNode.IsRacing[j];
                        r.HasFleets = proxyNode.UseFleets;
                        r.TargetFleetSize = proxyNode.TargetFleetSize;
                        if (j >= proxyNode.FirstFinalRace)
                        {
                            r.IsFinalRace = true;
                        }
                    }
                    else
                    {
                        r = regatta.Races[j-1] as TRace;
                    }

                    er = cr[j];
                    if (er.OTime == 0)
                    {
                        fp = new TFinishPosition(Constants.DNC);
                    }
                    else
                    {
                        fp = new TFinishPosition(er.OTime);
                    }

                    if (fp.IsValidFinish())
                    {
                        cr.IsGezeitet = true;
                    }

                    if (er.QU == 0)
                    {
                        p = null;
                    }
                    else
                    {
                        p = new TRSPenalty(er.QU)
                        {
                            Points = er.Penalty_Points,
                            Percent = er.Penalty_Percent
                        };
                    }
                    f = new TFinish(r, e, fp, p)
                    {
                        Fleet = er.Fleet
                    };
                    if (!er.IsRacing)
                    {
                        f.IsRacing = er.IsRacing;
                    }

                    r.FinishList.Add(f);
                }
            }
            //count Gezeitet
            int a = 0;
            for (int i = 0; i < cl.Count; i++)
            {
                if (cl[i].IsGezeitet)
                {
                    a++;
                }
            }
            proxyNode.Gezeitet = a;
        }

        private void UnLoadProxy()
        {
            TSeriesPointsList seriesPoints;
            TSeriesPoints sp;

            TRace race;
            TRacePoints racepts;

            TEntryInfo cr;
            TEntryInfo crPLZ;
            
            if (regatta == null)
            {
                return;
            }

            if (proxyNode == null)
            {
                return;
            }

            try
            {
                seriesPoints = regatta.ScoringManager.SeriesPointsList;
                regatta.ScoringManager.GetModel().SortSeries(seriesPoints);

                TEntryInfoCollection cl = proxyNode.EntryInfoCollection;
                for (int e = 0; e < seriesPoints.Count; e++)
                {
                    sp = seriesPoints[e] as TSeriesPoints;
                    cr = cl.FindKey(sp.Entry.SailID);
                    if ((cr == null) || (sp == null))
                    {
                        continue;
                    }

                    cr.IsTied = sp.Tied;
                    cr[0].CPoints = sp.Points;
                    cr[0].Rank = sp.Position;
                    cr[0].PosR = e + 1;

                    crPLZ = cl[e];
                    if (crPLZ != null)
                    {
                        crPLZ[0].PLZ = cr.Index;
                    }

                    // loop thru races display points for each race
                    for (int i = 0; i < regatta.Races.Count; i++)
                    {
                        race = regatta.Races[i];
                        racepts = regatta.ScoringManager.RacePointsList.FindPoints(race, sp.Entry);
                        if (racepts != null)
                        {                                     
                            cr[i+1].CPoints = racepts.Points;
                            cr[i+1].Drop = racepts.IsThrowout;
                        }
                        else if (!race.IsRacing)
                        {
                            cr[i+1].CPoints = 0;
                            cr[i+1].Drop = false;
                        }
                    }
                }
            }
            catch
            {
            }
        }
    }
}
