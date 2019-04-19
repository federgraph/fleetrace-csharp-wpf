using System;
using System.Collections.Generic;
using System.Reflection;
using System.Diagnostics;

namespace RiggVar.Scoring
{
    
    /// <summary> 
    /// Parent holder of information about scoring the races in a regatta.
    /// Provides all the covering information calculating race and series points
    /// for a set of entries (a division or fleet) in a set of races.
    /// <p>One instance of this class will exist for every "series" in a regatta
    /// For now there is only one class in a regatta, so all boats are in all race and
    /// there will only be one ScoringManager instance.</p>
    /// <p>But when multi-classes and possibly overall fleet results come in, then there
    /// will be one of these for each scored class, and each scored fleet.</p>
    /// </summary>
    public class TScoringManager
    {
        public static bool testing = true;
        private static readonly bool sTraceStatus = false;

        private static Dictionary<string, string> SupportedSystems;
            
        public IScoringModel fScoringModel;
        
        /// <summary> 
        /// contains RacePoints objects, one for each entry in each race
        /// </summary>
        protected internal TRacePointsList fPointsList;
        
        /// <summary> 
        /// contains SeriesPoints, one for each entry in the regatta
        /// </summary>
        protected internal TSeriesPointsList fSeries;

        public TScoringManager() : base()
        {
            fScoringModel = new TScoringLowPoint();
            fPointsList = new TRacePointsList();
            fSeries = new TSeriesPointsList();
        }

        static TScoringManager()
        {
            {
                SupportedSystems = new Dictionary<string, string>();
                AddSupportedModel(TScoringLowPoint.NAME_LowPoint, "RiggVar.Scoring.TScoringLowPoint");
                AddSupportedModel(TScoringBonusPoint.NAME_BonusPointSystem, "RiggVar.Scoring.TScoringBonusPoint");
                AddSupportedModel(TScoringBonusPointDSV.NAME_BonusPointSystemDSV, "RiggVar.Scoring.TScoringBonusPointDSV");
            }
        }

        public static List<string> SupportedModels
        {
            get
            {
                List<string> l = new List<string>();
                foreach (string o in SupportedSystems.Keys)
                {
                    l.Add(o);
                }    
                return l;
            }            
        }

        public virtual string Model
        {
            set
            {
                string modelClass = SupportedSystems[value];
                IScoringModel newModel = null;
                
                if (modelClass == null)
                {
                    Console.Out.WriteLine("WARNING!!! Unsupported scoring system requested, set to lowpoint, request=" + value);
                    newModel = new TScoringLowPoint();
                }
                else
                {
                    try
                    {
                        Type tempType = Type.GetType(modelClass);

                        // Get the constructor that takes an integer as a parameter.
                        ConstructorInfo constructorInfoObj = tempType.GetConstructor(Type.EmptyTypes);
                        object tempObject = null;
                        if (constructorInfoObj != null)
                        {
                            tempObject = constructorInfoObj.Invoke(null);
                            if (tempObject is IScoringModel)
                            {
                                newModel = tempObject as IScoringModel;
                            }
                            else
                            {
                                newModel = new TScoringLowPoint();
                            }
                        }
                        else
                        {
                            newModel = new TScoringLowPoint();
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e.StackTrace);
                    }
                    
                    if (newModel == null)
                    {
                        Console.Out.WriteLine("WARNING!!! Unsupported scoring class requested, set to lowpoint, request=" + modelClass);
                        newModel = new TScoringLowPoint();
                    }
                }
                
                //copy over the throwouts and other stuff
                if (fScoringModel != null && newModel != null)
                {
                    newModel.Attributes = fScoringModel;
                }

                fScoringModel = newModel;
            }            
        }

        public virtual IScoringModel GetModel()
        {
            return fScoringModel;            
        }

        public virtual TRacePointsList RacePointsList => fPointsList;
        public virtual TSeriesPointsList SeriesPointsList => fSeries;

        public static void AddSupportedModel(string modelName, string modelClass)
        {
            SupportedSystems[modelName] = modelClass;
        }

        /// <summary> returns the list of series points for an entry</summary>
        /// <param name="entry">entry whose points are wanted</param>
        /// <param name="div">Division</param>
        /// <returns>list of seriespoints</returns>
        public virtual TSeriesPoints GetSeriesPoints(TEntry entry)
        {
            return fSeries.FindPoints(entry);
        }
                        
        /// <summary>calculates a racepoints array for specified race;
        /// NOTE that this instance is automatically scored in the hashmap;
        /// returns if the Scoring system is not defined.
        /// </summary>
        /// <param name="race">race to be scored</param>
        /// <param name="entries">entries in the race</param>
        /// <param name="points">points list in which race's points are stored</param>
        /// <exception cref="ScoringException">throws ScoringException if problem occurs</exception>
        public virtual void ScoreRace(TRace race, TRacePointsList points)
        {
            ScoreRace(race, points, false);
        }
        
        /// <summary> 
        /// calculates a racepoints array for specified race;
        /// NOTE that this instance is automatically scored in the hashmap;
        /// does nothing if the Scoring system is not defined.
        /// </summary>
        /// <param name="race">race to be scored</param>
        /// <param name="entries">entries in the race</param>
        /// <param name="points">points list in which race's points are stored</param>
        /// <exception cref="ScoringException">throws ScoringException if problem occurs</exception>
        public virtual void ScoreRace(TRace race, TRacePointsList points, bool positionOnly)
        {
            if (fScoringModel == null || race == null)
            {
                return ;
            }

            fScoringModel.ScoreRace(race, points, positionOnly);
        }

        public virtual void ScoreRegatta(TRegatta regatta, TRaceList inRaces)
        {
            if (sTraceStatus)
            {
                Debug.WriteLine("ScoringManager: scoring started...");
            }

            if (fScoringModel == null ||
                regatta == null ||
                inRaces.Count == 0 ||
                regatta.Entries.Count == 0)
            {
                if (sTraceStatus)
                {
                    Debug.WriteLine("ScoringManager: (empty) done.");
                }

                return;
            }

            fPointsList.Clear();
            fSeries.Clear();

            ScoreDivision(regatta);

            if (sTraceStatus)
            {
                Debug.WriteLine("ScoringManager: scoring completed.");
            }

        }
                
        private void ScoreDivision(TRegatta regatta)
        {
            if (sTraceStatus)
            {
                Console.Out.WriteLine("ScoringManager: scoring races...");
            }

            TEntryList entries = regatta.Entries;
            
            // create list of races for this division
            TRaceList divRaces = new TRaceList();
            foreach (TRace r in regatta.Races)
            {
                if (r.IsRacing)
                {
                    divRaces.Add(r);
                }
            }

            TRacePointsList divPointsList = new TRacePointsList();
            
            // calc race points for each race (and each division in a race)
            foreach (TRace r in divRaces)
            {                                
                TRacePointsList racePoints = new TRacePointsList();
                foreach (TEntry e in entries)
                {
                    racePoints.Add(new TRacePoints(r, e, double.NaN, false));
                }
                ScoreRace(r, racePoints);
                divPointsList.AddRange(racePoints);
            }
            
            // calc series points
            fPointsList.AddRange(divPointsList);
            ScoreSeries(divRaces, entries, divPointsList);
        }

        private void ScoreSeries(TRaceList divRaces, TEntryList entries, TRacePointsList divPointsList)
        {
            TRace r;
            if (divRaces.Count > 0)
            {
                r = divRaces[0];
                if (r.HasFleets && TRegatta.IsInFinalPhase) // && r.Regatta.IsInFinalPhase
                {
                    ScoreQualiFinalSeries(divRaces, entries, divPointsList);
                }
                else
                {
                    ScoreSeries1(divRaces, entries, divPointsList);
                }
            }
        }

        private void ScoreQualiFinalSeries(TRaceList divRaces, TEntryList entries, TRacePointsList points)
        {
            TRacePoints rp;
            //find the number of fleets in the race
            int fc = 0;
            for (int i = 0; i < points.Count; i++)
            {
                rp = points[i];
                if ((rp.Finish != null) && (rp.Finish.Fleet > fc))
                {
                    fc = rp.Finish.Fleet;
                }
            }

            TRace fr = null;
            if ((divRaces != null) && (divRaces.Count > 0))
            {
                fr = divRaces[divRaces.Count - 1];
            }

            if (fr == null)
            {
                return;
            }

            if (!fr.IsFinalRace)
            {
                ScoreSeries1(divRaces, entries, points);
            }
            else
            {
                TRacePointsList rpl = new TRacePointsList();
                TEntryList el = new TEntryList();

                int posOffset = 0;
                TFinish f;
                TEntry e;

                // call ScoreSeries2 for each fleet
                for (int j = 0; j <= fc; j++)
                {
                    // get the entries for the fleet
                    for (int i = 0; i < fr.FinishList.Count; i++)
                    {
                        f = fr.FinishList[i];
                        e = f.Entry;
                        if (f.Fleet == j)
                        {
                            el.Add(e);
                        }
                    }

                    // get the racepoints for the fleet
                    for (int i = 0; i < points.Count; i++)
                    {
                        rp = points[i];
                        if (rp.Finish != null) //&& (rp.Finish.Fleet == j)
                        {
                            rpl.Add(rp);
                        }
                    }

                    if ((el.Count > 0) && (rpl.Count > 0))
                    {
                        ScoreSeries2(divRaces, el, rpl, posOffset);
                        posOffset = posOffset + el.Count;
                    }

                    rpl.Clear();
                    el.Clear();
                }
            }
        }

        private void ScoreSeries2(
            TRaceList divRaces, 
            TEntryList entries, 
            TRacePointsList divPointsList,
            int posOffset)
        {
            // calc throwouts
            foreach (TEntry e in entries)
            {
                fScoringModel.CalcThrowouts(divPointsList.FindAllPointsForEntry(e));
            }

            // run thru looking for average points
            CalcAveragePoints(divPointsList);

            TSeriesPointsList divSeriesPoints = new TSeriesPointsList();
            divSeriesPoints.InitPoints(entries);

            // no finishes go to next division
            if (divSeriesPoints.Count == 0)
            {
                return;
            }

            fScoringModel.ScoreSeries(divRaces, entries, divPointsList, divSeriesPoints);

            // now run through looking for clumps of tied boats
            // pass the clumps of tied boats on to scoringmodel for resolution
            fScoringModel.SortSeries(divSeriesPoints);

            TScoringTiebreaker doties = new TScoringTiebreaker(
              fScoringModel, divRaces, divPointsList, divSeriesPoints);
            doties.Process();

            // now set series position
            divSeriesPoints.SortPoints();
            int position = 1;
            double lastpoints = 0;
            bool tied = false;
            for (int e = 0; e < divSeriesPoints.Count; e++)
            {
                TSeriesPoints sp = divSeriesPoints[e];
                double thispoints = sp.Points;
                double nextpoints = (e + 1 < divSeriesPoints.Count) ? divSeriesPoints[e + 1].Points : 99999999.0;
                tied = !((thispoints != lastpoints) && (thispoints != nextpoints));
                if (!tied)
                {
                    position = e + 1;
                }
                else
                {
                    // position is same if tied with last
                    if (thispoints != lastpoints)
                    {
                        position = e + 1;
                    }
                }
                sp.Position = position + posOffset;
                sp.Tied = tied;
                lastpoints = thispoints;
            }

            fSeries.AddRange(divSeriesPoints);
        }

        private void ScoreSeries1(TRaceList divRaces, TEntryList entries, TRacePointsList divPointsList)
        {
            ScoreSeries2(divRaces, entries, divPointsList, 0);
        }
                
        /// <summary> 
        /// calculates average points as per RRS2001 A10(a) (throwouts included):
        /// "points equal to the average, to the nearest tenth of a point (0.05 
        /// to be rounded upward), of her points in all the races in the series 
        /// except the race in question;"
        /// <p>NOTE: this formula assumes that "the race in question" really wants
        /// to say the "race(s) in question"</p>
        /// </summary>
        /// <param name="regatta">
        /// regatta to be scored.  All instances of AVG in all races in
        /// all divisions in the regatta will be scanned and AVG points calculated
        /// </param>
        public virtual void CalcAveragePoints(TRacePointsList divRacePoints)
        {
            CalcAveragePoints(divRacePoints, true);
        }
        
        /// <summary> 
        /// calculates average points as per RRS2001 A10(a) except that including the
        /// throwout (or not) is an optional
        /// </summary>
        /// <param name="regatta">
        /// regatta to be scored.  All instances of AVG in all races (in
        /// all divisions in the regatta) will be scanned and AVG points calculated
        /// </param>
        /// <param name="throwoutIsIncluded">
        /// true if throwouts are to be included in calculation
        /// </param>
        public virtual void CalcAveragePoints(TRacePointsList divRacePoints, bool throwoutIsIncluded)
        {
            if (sTraceStatus)
            {
                Console.Out.WriteLine("ScoringManager: calculating average points...");
            }

            TEntryList eWithAvg = new TEntryList();
            for (int i = 0; i < divRacePoints.Count; i++)
            {
                TRacePoints rp = divRacePoints[i];
                if (rp.Finish.HasPenalty(Constants.AVG))
                {
                    TEntry e = rp.Entry;
                    if (!eWithAvg.Contains(e))
                    {
                        eWithAvg.Add(e);
                    }
                }
            }
            
            foreach (TEntry e in eWithAvg)
            {
                TRacePointsList list = divRacePoints.FindAllPointsForEntry(e);
                double pts = 0;
                double n = 0;
                bool hasAvg = false;
                
                double[] tempPts = new double[list.Count];
                int[] tempPen = new int[list.Count];
                int t = 0;
                
                foreach (TRacePoints p in list)
                {                     
                    TFinish finish = p.Race.GetFinish(p.Entry);
                    
                    tempPts[t] = p.Points;
                    tempPen[t++] = finish.Penalty.Penalty;
                    
                    if ((!p.IsThrowout || throwoutIsIncluded) && finish != null && !finish.Penalty.HasPenalty(Constants.AVG))
                    {
                        pts = pts + p.Points;
                        n++;
                    }
                    else if (finish != null && finish.Penalty.HasPenalty(Constants.AVG))
                    {
                        hasAvg = true;
                    }
                }
                
                if (hasAvg)
                {
                    double avg = pts / n;
                    avg = (long)Math.Round(avg * 10);
                    avg = avg / 10.0;
                    foreach (TRacePoints p in list)
                    {
                        TFinish finish = p.Race.GetFinish(p.Entry);
                        if (finish != null && finish.Penalty.HasPenalty(Constants.AVG))
                        {
                            p.Points = avg;
                        }
                    }
                }
            }
        }
    }
}