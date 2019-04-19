using RiggVar.Scoring;

namespace RiggVar.FR
{

    /// <summary>
    /// proxy for using internal scoring code directly, 
    /// kein Umweg �ber TFRProxy and TProxyLoader
    /// </summary>
    public class TCalcEventProxy06 : TCalcEventProxy
    {
        private TStrings FSL = new TStringList();

        public TRegatta regatta;
        public TEventNode qn;
        public TEventProps EventProps;

        public TCalcEventProxy06()
        {
        }

        private bool InitThrowoutScheme()
        {
            //precondition
            if (regatta == null)
            {
                return false;
            }

            if (qn == null)
            {
                return false;
            }

            if (qn.Collection.Count == 0)
            {
                return false;
            }

            //count of sailed races
            int IsRacingCount = 0;
            for (int r = 1; r <= qn.RaceCount; r++)
            {
                if (TMain.BO.RNode[r].IsRacing)
                {
                    IsRacingCount++;
                }
            }

            if (IsRacingCount < 1)
            {
                return false;
            }

            //number of throwouts
            int t = TMain.BO.EventProps.Throwouts;
            if (t >= IsRacingCount)
            {
                t = IsRacingCount - 1;
            }

            //update scoring-system-attributes in javascore
            TScoringLowPoint sm = new TScoringLowPoint();

            if (t < 2)
            {
                sm.SetFixedNumberOfThrowouts(0);
            }
            else
            {
                sm.SetFixedNumberOfThrowouts(t);
            }

            sm.FirstIs75 = EventProps.FirstIs75;
            sm.ReorderRAF = EventProps.ReorderRAF;

            if (EventProps.ScoringSystem == TScoringSystem.BonusPoint)
            {
                regatta.ScoringManager.Model = TScoringBonusPoint.NAME_BonusPointSystem;
            }
            else if (EventProps.ScoringSystem == TScoringSystem.BonusPointDSV)
            {
                regatta.ScoringManager.Model = TScoringBonusPointDSV.NAME_BonusPointSystemDSV;
            }
            else
            {
                regatta.ScoringManager.Model = TScoringLowPoint.NAME_LowPoint;
            }

            IScoringModel m = regatta.ScoringManager.GetModel();
            m.Attributes = sm;

            return true;
        }

        public void LoadProxy()
        {
            TEntry e;
            TRace r;
            TFinish f;
            TFinishPosition fp;
            TRSPenalty p;

            TEventRowCollectionItem cr;
            TEventRaceEntry er;

#if Sailtime
            long ft;
            long NowMillies = Utils.DateTimeToMillies(DateTime.Now);
#endif
            if (regatta == null)
            {
                return;
            }

            if (qn == null)
            {
                return;
            }

            TEventRowCollection cl = qn.Collection;

            if (qn.RaceCount >= qn.FirstFinalRace)
            {
                TRegatta.IsInFinalPhase = true;
            }

            for (int i = 0; i < cl.Count; i++)
            {
                cr = cl[i];
                cr.isGezeitet = false;
                e = new TEntry
                {
                    SailID = cr.SNR
                };
                regatta.AddEntry(e);

                for (int j = 1; j < cr.RCount; j++)
                {
                    if (qn.PartialCalcLastRace > 0 && j > qn.PartialCalcLastRace)
                    {
                        break;
                    }

                    if (i == 0)
                    {
                        r = new TRace(j);
                        regatta.Races.Add(r);
                        r.IsRacing = TMain.BO.RNode[j].IsRacing;
                        r.HasFleets = cr.Ru.UseFleets;
                        r.TargetFleetSize = cr.Ru.TargetFleetSize;
                        if (j >= qn.FirstFinalRace)
                        {
                            r.IsFinalRace = true;
                        }

#if Sailtime
                        r.DivInfo.setStartTime(d, NowMillies + (j * 1000));
                        r.StartDate = DateTime.Now + TimeSpan.FromDays(1);
#endif
                    }
                    else
                    {
                        r = regatta.Races[j-1] as TRace;
                    }

                    er = cr.Race[j];
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
                        cr.isGezeitet = true;
                    }

#if Sailtime
                    //need Finishtime ft for raceties
                    if (fp.isValidFinish())
                        ft = NowMillies + (1000 * er.OTime);
                    else
                        ft = SailTime.NOTIME;
#endif

                    if (er.QU == 0)
                    {
                        p = null;
                    }
                    else
                    {
                        p = new TRSPenalty(er.QU)
                        {
                            Points = er.Penalty.Points,
                            Percent = er.Penalty.Percent
                        };
#if Sailtime
                        p.Note = er.Penalty.Note;
                        p.TimePenalty = er.Penalty.TimePenalty;
#endif
                    }
                    f = new TFinish(r, e, fp, p)
                    {
                        Fleet = er.Fleet,
                        IsRacing = er.IsRacing
                    };
                    r.FinishList.Add(f);
                }
            }
            //Gezeitet ermitteln
            int a = 0;
            for (int i = 0; i < cl.Count; i++)
            {
                if (cl[i].isGezeitet)
                {
                    a++;
                }
            }
            TMain.BO.Gezeitet = a;                    
        }

        public void UnLoadProxy()
        {
            TSeriesPointsList seriesPoints;
            TSeriesPoints sp;

            TRace race;
            TRacePoints racepts;

            TEventRowCollectionItem cr;
            TEventRowCollectionItem crPLZ;
            
            if (regatta == null)
            {
                return;
            }

            if (qn == null)
            {
                return;
            }

            try
            {
                seriesPoints = regatta.ScoringManager.SeriesPointsList;
                regatta.ScoringManager.GetModel().SortSeries(seriesPoints);

                TEventRowCollection cl = qn.Collection;
                for (int e = 0; e < seriesPoints.Count; e++)
                {
                    sp = seriesPoints[e] as TSeriesPoints;
                    cr = cl.FindKey((int)sp.Entry.SailID);
                    if ((cr == null) || (sp == null))
                    {
                        continue;
                    }

                    cr.isTied = sp.Tied;
                    cr.Race[0].CPoints = sp.Points;
                    cr.Race[0].Rank = sp.Position;
                    cr.Race[0].PosR = e + 1;

                    crPLZ = cl[e];
                    if (crPLZ != null)
                    {
                        crPLZ.Race[0].PLZ = cr.IndexOfRow;
                    }

                    // loop thru races
                    for (int i = 0; i < regatta.Races.Count; i++)
                    {
                        if (qn.PartialCalcLastRace > 0 && i > qn.PartialCalcLastRace - 1)
                        {
                            break;
                        }

                        race = regatta.Races[i];
                        racepts = regatta.ScoringManager.RacePointsList.FindPoints(race, sp.Entry);
                        if (racepts != null)
                        {                                     
                            cr.Race[i+1].CPoints = racepts.Points;
                            cr.Race[i+1].Drop = racepts.IsThrowout;
                        }
                        else if (!race.IsRacing)
                        {
                            cr.Race[i+1].CPoints = 0;
                            cr.Race[i+1].Drop = false;
                        }
                    }
                }
            }
            catch
            {
            }                    
        }

        public void ClearAllWithoutCalc()
        {
            try
            {
                TEventRowCollection cl = qn.Collection;
                int posr = 0;
                foreach (TEventRowCollectionItem cr in cl)
                {
                    cr.isTied = true;
                    cr.Race[0].CPoints = 0;
                    cr.Race[0].Rank = 0;
                    cr.Race[0].PosR = posr++;
                    cr.Race[0].PLZ = cr.IndexOfRow;

                    // loop thru races
                    for (int i = 0; i < qn.RaceCount; i++)
                    {
                        cr.Race[i + 1].CPoints = 0;
                        cr.Race[i + 1].Drop = false;
                    }
                }
            }
            catch
            {
            }
        }

        public override void Calc(TEventNode aqn)
        {
            qn = aqn;
            EventProps = TMain.BO.EventProps;
            if (qn.Collection.Count == 0)
            {
                return;
            }

            //  Note: wird beim Laden eines events zweimal aufgerufen
            //  einmal bei Clear, hier wird Gezeitet 0, da noch keine Daten da sind,
            //  dann mit OnIdle/Modified = false.
            //  wenn Gezeitet 0 ist k�nnen keine Ranglistenpunkte ausgerechnet werden.

            regatta = new TRegatta();
            try
            {
                if (EventProps.ScoringSystem == TScoringSystem.BonusPoint)
                {
                    regatta.ScoringManager.Model = TScoringBonusPoint.NAME_BonusPointSystem;
                }
                else if (EventProps.ScoringSystem == TScoringSystem.BonusPointDSV)
                {
                    regatta.ScoringManager.Model = TScoringBonusPointDSV.NAME_BonusPointSystemDSV;
                }

                if (InitThrowoutScheme())
                {
                    LoadProxy();
                    regatta.ScoreRegatta();
                    UnLoadProxy();
                }
                else
                {
                    ClearAllWithoutCalc();
                }
            }
            finally
            {
                regatta = null;
            }
        }

        public override void GetScoringNotes(TStrings SL)
        {
            for (int i = 0; i < FSL.Count; i++)
            {
                SL.Add(FSL[i]);
            }
        }
    }
}
