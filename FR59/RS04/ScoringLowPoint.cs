using System.Collections.Generic;
using System.Diagnostics;

namespace RiggVar.Scoring
{

    /// <summary> 
    /// ISAF LowPoint scoring system
    /// </summary>
    public class TScoringLowPoint: IScoringModel
    {
        public const string NAME_LowPoint = "ISAF Low Point 2001-2004";

        public bool FirstIs75 = false;
        public bool ReorderRAF = true;
        public bool HasFleets;
        public int TargetFleetSize;
        public bool IsFinalRace;
        public int TiebreakerMode = TIE_RRS_DEFAULT;
                
        /// <summary> 
        /// option per RRS2001 A9 for different penalties for "long" series
        /// If true, the penalties as per A9 will be applied
        /// </summary>
        public bool IsLongSeries = false;

        public int FixedNumberOfThrowouts = 0;
        
        /// <summary> 
        /// Default percentage penalty for failure to check-in
        /// </summary>
        public  int CheckinPercent = 20;
        
        public const int TLE_DNF = 0;
        public const int TLE_FINISHERSPLUS1 = 1;
        public const int TLE_FINISHERSPLUS2 = 2;
        public const int TLE_AVERAGE = 3;
        
        public int TimeLimitPenalty;
                        
        protected internal const double TIEBREAK_INCREMENT = 0.0001;

        public const int TIE_RRS_DEFAULT = 1;
        public const int TIE_RRS_A82_ONLY = 2;

        public TScoringLowPoint()
        {
            TimeLimitPenalty = TLE_DNF;
        }

        public virtual string Name => NAME_LowPoint;

        public virtual IScoringModel Attributes
        {
            set
            {
                try
                {
                    TScoringLowPoint that = (TScoringLowPoint) value;
                    
                    CheckinPercent = that.CheckinPercent;
                    IsLongSeries = that.IsLongSeries;
                    TimeLimitPenalty = that.TimeLimitPenalty;
                    FixedNumberOfThrowouts = that.FixedNumberOfThrowouts;
                    FirstIs75 = that.FirstIs75;
                }
                catch
                {
                }
            }            
        }

        protected void SetTiedPoints(List<TRacePoints> value)
        {
            double pts = 0;
            foreach (TRacePoints rp in value)
            {
                pts += rp.Points;
            }
            pts = pts / value.Count;

            foreach (TRacePoints rp in value)
            {
                rp.Points = pts;
            }
        }
                                                                                
        /// <summary> 
        /// trivial implementation, doesn't really sort at all
        /// </summary>
        public int CompareTo(object obj)
        {
            return this.ToString().CompareTo(obj.ToString());
        }

        public override int GetHashCode()
        {
            return base.GetHashCode(); //dummy, to avoid warning
        }

        /// <summary> 
        /// compares two lowpoint systems for equality of their optional settings
        /// </summary>
        public override bool Equals(object obj)
        {
            if (!(obj is TScoringLowPoint))
            {
                return false;
            }

            if (this == obj)
            {
                return true;
            }

            TScoringLowPoint that = (TScoringLowPoint) obj;
            
            if (!this.ToString().Equals(that.ToString()))
            {
                return false;
            }

            if (this.CheckinPercent != that.CheckinPercent)
            {
                return false;
            }

            if (this.TimeLimitPenalty != that.TimeLimitPenalty)
            {
                return false;
            }

            return FixedNumberOfThrowouts == that.FixedNumberOfThrowouts;
        }

        public override string ToString()
        {
            return "ScoringLowPoint";
        }
        
        public virtual void ScoreRace(TRace r, TRacePointsList points, bool positionOnly)
        {
            if (r.HasFleets)
            {
                ScoreRace2(r, points, FirstIs75, positionOnly);
            }
            else
            {
                ScoreRace(r, points, FirstIs75, positionOnly);
            }
        }

        protected internal virtual void ScoreRace2(
            TRace race, TRacePointsList points, bool firstIs75, bool positionOnly)
        {
            HasFleets = race.HasFleets;
            TargetFleetSize = race.TargetFleetSize;
            IsFinalRace = race.IsFinalRace;


            //find the number of fleets in the race
            int fc = 0;
            foreach (TRacePoints rp in points)
            {
                if (rp.Finish != null && rp.Finish.Fleet > fc)
                {
                    fc = rp.Finish.Fleet;
                }
            }

            TRacePointsList rpl = new TRacePointsList();

            //call scoreRace for each fleet
            for (int j = 0; j <= fc; j++)
            {
                foreach (TRacePoints rp in points)
                {
                    if (rp.Finish != null && rp.Finish.Fleet == j)
                    {
                        rpl.Add(rp);
                    }
                }
                if (rpl.Count > 0)
                {
                    ScoreRace(race, rpl, FirstIs75, positionOnly);
                }

                rpl.Clear();
            }
        }
        
        /// <summary> 
        /// <p>Given a Race, and a list of Entries calculates the RacePoints object
        /// The entries should be assumed to represent a single class within the Race
        /// calculateRace can assume that an Entries without a finish in the Race is DNC
        /// but should recognize that the Race may well have finishers not in the Entries.</p>
        /// <p>Also assumes that points is pre-populated, just needs to have finish points 
        /// assigned throws ScoringException if there is a problem with the scoring</p>
        /// </summary>
        /// <param name="r">race to be scored</param>
        /// <param name="points">racepointslist in which to store the results</param>
        /// <param name="firstIs75">true if first place should be .75</param>
        protected internal virtual void ScoreRace(
            TRace r, TRacePointsList points, bool firstIs75, bool positionOnly)
        {
            // sort points on finishposition sorted top to bottom by finish
            points.SortFinishPosition();
            
            double pts = firstIs75?.75:1.0;
            
            // position within the divsion (as opposed to within the fleet)
            int divPosition = 1;

            bool valid;
            bool isdsq;
            bool israf;
            // loop thru the race's finishes, for each finish in the list, set the points            
            foreach (TRacePoints rp in points)
            {
                TFinish f = rp.Finish;
                double basePts = pts;
                rp.Position = divPosition++;

                valid = f.FinishPosition.IsValidFinish();
                isdsq = f.Penalty.IsDsqPenalty();
                israf = f.Penalty.HasPenalty(Constants.RAF);

                bool isNormalCountup = valid && (!isdsq);

                if (!ReorderRAF)
                {
                    isNormalCountup = valid && (israf || !isdsq);
                }

                if (isNormalCountup)
                {
                    //RAF does not alter finish places

                    // increment points to be assigned to next guy if this
                    // one is a valid finisher and not disqualified
                    if (pts == .75)
                    {
                        pts = 1.0;
                    }

                    pts++;
                }
                else
                {
                    rp.Position = f.FinishPosition.IntValue(); //has penalty type ecoded
                }
                if (f.HasPenalty())
                {
                    basePts = GetPenaltyPoints(f.Penalty, points, basePts);
                }
                if (!positionOnly)
                {
                    if (rp.IsMedalRace)
                    {
                        rp.Points = basePts * 2;
                    }
                    else if (!rp.Finish.IsRacing)
                    {
                        rp.Points = 0.0;
                    }
                    else
                    {
                        rp.Points = basePts;
                    }
                }
            }

            if (!positionOnly)
            {
                // look for ties - must be done with correctedtime
                TRacePoints lastrp = null;
                List<TRacePoints> tied = new List<TRacePoints>();
                foreach (TRacePoints rp in points)
                {
                    if (rp.IsTied(lastrp))
                    {
                        // boats are tied if neither has a penalty and the current boat
                        // has a valid corrected time, and its the same as the last corrected time
                        if (tied.Count == 0)
                        {
                            tied.Add(lastrp);
                        }

                        tied.Add(rp);
                    }
                    else if (tied.Count > 0)
                    {
                        // coming out of set of tied boats, reset their points and clear out
                        SetTiedPoints(tied);
                        tied.Clear();
                    }
                    lastrp = rp;
                }
                // if processing tieds at end of loop
                if (tied.Count > 0)
                {
                    SetTiedPoints(tied);
                }
            }
        }
                
        /// <summary> 
        /// calculates the overall series points.
        /// Assume that each individual race has already been calculated, and that
        /// throwouts have already be designated in the points objects
        /// </summary>
        /// <param name="races">list of races involved in the series</param>
        /// <param name="entries">to be considered in this series</param>
        /// <param name="points">
        /// list of points for all races and entries (and maybe more)
        /// </param>
        /// <param name="series">
        /// map in which (key=entry, value=Double) series points are to be calculated.
        /// </param>
        /// <exception cref="ScoringException">
        /// throws ScoringException if there is a problem with the scoring
        /// </exception>
        public virtual void ScoreSeries(
            TRaceList races, TEntryList entries, TRacePointsList points, TSeriesPointsList series)
        {
            foreach (TSeriesPoints sp in series)
            {
                TEntry e = sp.Entry;
                TRacePointsList ePoints = points.FindAllPointsForEntry(e); // list of e's finishes
                double tot = 0;
                foreach (TRacePoints p in ePoints)
                {
                    if (!p.IsThrowout)
                    {
                        tot += p.Points;
                    }
                }
                sp.Points = tot;
            }
        }

        public virtual void CalculateTieBreakers(
            TRaceList races, TEntryList entries, TRacePointsList racepoints, TSeriesPointsList seriespoints)
        {
            TRace r;
            TRacePoints rp;
            TiebreakerMode = TIE_RRS_DEFAULT;

            int i = races.Count;
            while (i > 0)
            {
                i--;
                r = races[i];
                if (r.IsFinalRace)
                {
                    foreach (TEntry e in entries)
                    {
                        rp = racepoints.FindPoints(r, e);
                        if (rp != null)
                        {
                            if (rp.Finish != null)
                            {
                                TiebreakerMode = rp.Finish.Fleet == 0 ? TIE_RRS_A82_ONLY : TIE_RRS_DEFAULT;
                            }
                        }
                    }
                }
                break; //only look into last race
            }

            if (TiebreakerMode == TIE_RRS_DEFAULT)
            {
                CalculateTieBreakersDefault(races, entries, racepoints, seriespoints);
            }
            else
            {
                CalculateTieBreakersAlternate(races, entries, racepoints, seriespoints);
            }
        }

        /// <summary> 
        /// resolve ties among a group of tied boats.  
        /// A tie that is breakable should have .01 point increments added as appropriate.
        /// Assume that each individual race and series points have calculated, and that
        /// throwouts have already been designated in the points objects.
        /// </summary>
        /// <param name="races">
        /// races involved
        /// </param>
        /// <param name="entriesIn">
        /// list of tied entries
        /// </param>
        /// <param name="points">
        /// list of points for all races and entries (and maybe more!)
        /// </param>
        /// <param name="series">
        /// map containing series points for the entries, prior to
        /// handling ties (and maybe more than just those entries)
        /// </param>
        public void CalculateTieBreakersDefault(
            TRaceList races, TEntryList entriesIn, TRacePointsList points, TSeriesPointsList series)
        {
            TEntryList entries = entriesIn.CloneEntries();

            if (entries == null)
            {
                return;
            }

            // first create a lists of finishes for each of the tied boats.
            //elist is a list of RacePointLists.
            //each elist item is a sorted list of racepoints that are not throwouts
            //1 elist item per tied entry, 
            List<TRacePointsList> eLists = new List<TRacePointsList>();
            foreach (TEntry e in entries)
            {
                TRacePointsList ePoints = points.FindAllPointsForEntry(e);
                eLists.Add(ePoints);
            }
            
            List<TRacePointsList> tiedWithBest = new List<TRacePointsList>();
            // pull out best of the bunch one at a time
            // after each scan, best is dropped with no more change
            // in points.  Each remaining gets .01 added to total.
            // continue til no more left to play
            while (eLists.Count > 1)
            {
                TRacePointsList bestPoints = eLists[0];
                tiedWithBest.Clear();
                
                // loop thru entries, apply tiebreaker method
                // keep the best (winner)
                for (int i = 1; i < eLists.Count; i++)
                {
                    TRacePointsList leftPoints = eLists[i];
                    
                    // compares for ties by A8.1
                    int c = ComparePointsBestToWorst(leftPoints, bestPoints);
                    if (c < 0)
                    {
                        bestPoints = leftPoints; //remember the best
                        tiedWithBest.Clear(); //start new group
                    }
                    else if (c == 0)
                    {
                        tiedWithBest.Add(leftPoints); //tie found
                    }
                }
                if (tiedWithBest.Count > 0)
                {
                    // have boats tied after applying A8.1 - so send them into
                    // next tiebreakers clauses
                    tiedWithBest.Add(bestPoints);
                    CompareWhoBeatWhoLast(tiedWithBest, series);
                }
                double inc = (tiedWithBest.Count + 1) * TIEBREAK_INCREMENT;
                eLists.Remove(bestPoints);

                //eLists.removeAll(tiedWithBest); //bestPoint may be part of it, but not always
                foreach (TRacePointsList o in tiedWithBest)
                {
                    int i = eLists.IndexOf(o);
                    if (i > -1)
                    {
                        eLists.RemoveAt(i);
                    }
                }

                IncrementSeriesScores(eLists, inc, series); //prepare for next iteration
            }
        }

        public void CalculateTieBreakersAlternate(
    TRaceList races, TEntryList entries, TRacePointsList racepoints, TSeriesPointsList seriespoints)
        {
            if (entries == null)
            {
                return;
            }

            List<TRacePointsList> rpLists = new List<TRacePointsList>();
            foreach (TEntry e in entries)
            {
                TRacePointsList ePoints = racepoints.FindAllPointsForEntry(e);
                rpLists.Add(ePoints);
            }
            CompareWhoBeatWhoLast(rpLists, seriespoints);
        }
        
        private TRacePointsList prepBestToWorst(TRacePointsList rpList)
        {
            TRacePointsList ePoints = rpList.CloneEntries();

            // delete throwouts from the list
            for (int i = ePoints.Count-1; i >= 0; i--)
            {
                TRacePoints p = ePoints[i];
                if (p != null)
                {
                    if (p.IsThrowout)
                    {
                        ePoints.Remove(p);
                    }
                }
            }            
            ePoints.SortPoints();
            return ePoints;
        }
        
        /// <summary> 
        /// <p>compares two sets of race points for tie breaker resolution.</p>
        /// <p>RRS2001 A8.1: "If there is a series score tie between two or more boats, 
        /// each boat�s race scores shall be listed in order of best to worst, 
        /// and at the first point(s) where there is a difference the tie 
        /// shall be broken in favour of the boat(s) with the best score(s).
        /// No excluded scores shall be used."</p>
        /// </summary>
        /// <param name="races">races involved</param>
        /// <param name="inLeft">racepointslist of lefty</param>
        /// <param name="inRight">racepointslist of righty</param>
        /// <returns>
        /// -1 if "lefty" wins tiebreaker, 1 if righty wins, 0 if tied.
        /// </returns>
        protected internal virtual int ComparePointsBestToWorst(
            TRacePointsList inLeft, TRacePointsList inRight)
        {
            TRacePointsList left = prepBestToWorst(inLeft);
            TRacePointsList right = prepBestToWorst(inRight);
            
            double lp = 0;
            double rp = 0;
            
            // we know they are sorted by finish points, look for first non-equal finish
            for (int i = 0; i < left.Count; i++)
            {
                lp = (left[i]).Points;
                rp = (right[i]).Points;
                if (lp < rp)
                {
                    return - 1;
                }
                else if (rp < lp)
                {
                    return 1;
                }
            }
            return 0;
        }
        
        /// <summary> 
        /// applying the remaining tiebreakers of RRS2001 A8 to set of boats tied after
        /// comparing their list of scores.  This is the new 2002+ formula after ISAF
        /// deleted 8.2 and renumbered 8.3 to 8.2
        /// <p>
        /// RRS2001 modified A8.2 (old 8.3): "If a tie still remains between two or more 
        /// boats, they shall be ranked in order of their scores in the last race. 
        /// Any remaining ties shall be broken by using the tied boats� scores in the 
        /// next-to-last race and so on until all ties are broken. 
        /// These scores shall be used even if some of them are excluded scores."</p>
        /// </summary>
        /// <param name="stillTied">
        /// list of boat scores of the group for which A8.1 does not resolve the tie
        /// </param>
        /// <param name="series">list of series scores</param>
        protected internal virtual void CompareWhoBeatWhoLast(
            List<TRacePointsList> stillTied, TSeriesPointsList series)
        {
            int nRaces = stillTied[0].Count;
            int nTied = stillTied.Count;
            TEntryList tiedEntries = new TEntryList();
            double[] beatenCount = new double[ nTied];
            foreach (TRacePointsList list in stillTied)
            {
                if (list.Count == 0)
                {
                    continue;
                }

                list.SortRace();
                tiedEntries.Add(list[0].Entry);
            }
            // now look to see if anyone is STILL tied, applying A8.3 now
            // now have beatenCount, can increment an entries score TIEBREAK_INCREMENT for each
            // boat in the list with a higher beaten count
            for (int e = 0; e < nTied; e++)
            {
                //otherLoop: 
                for (int o = 0; o < nTied; o++)
                {
                    if ( (e != o) && (beatenCount[e] == beatenCount[o]))
                    {    
                        for (int r = nRaces-1; r >= 0; r--)
                        {
                            double ePts = stillTied[e][r].Points;
                            double oPts = stillTied[o][r].Points;
                            if (ePts > oPts)
                            {
                                IncrementSeriesScore(tiedEntries[e], TIEBREAK_INCREMENT, series);
                            }

                            if (ePts != oPts)
                            {
                                goto otherLoop;
                            }
                        }
                    }
                }
                otherLoop: ; 
            }
        }
            
        protected internal virtual void  IncrementSeriesScore(
            TEntry e, double amount, TSeriesPointsList series)
        {
            // find all series points for e, should be exactly 1
            TSeriesPoints eSeries = series.FindAllPoints(e)[0];
            // add TIEBREAK_INCREMENT to its score
            eSeries.Points = eSeries.Points + amount;
        }
        
        protected internal virtual void IncrementSeriesScores(
            List<TRacePointsList> eLists, double amount, TSeriesPointsList series)
        {
            // add TIEBREAK_INCREMENT to series points of remaining tied boats
            for (int i = 0; i < eLists.Count; i++)
            {
                TRacePointsList pl = eLists[i];
                if (pl.Count == 0)
                {
                    Debug.WriteLine("ScoringMessageInvalidSeries");
                }
                else
                {
                    // pull entry from 1st element of the (i'th) eList
                    IncrementSeriesScore(pl[0].Entry, amount, series);
                }
            }
        }
        
        /// <summary>
        /// sorts a points list as on points ascending
        /// </summary>
        /// <param name="series">
        /// pointslist to be sorted
        /// </param>
        public virtual void SortSeries(TSeriesPointsList series)
        {
            series.SortPoints();
        }
        
        /// <summary> 
        /// Given a penalty, returns the number of points to be assigned (or added)
        /// </summary>
        /// <param name="p">penalty to be calculated, should never be null</param>
        /// <param name="rpList">racepointslist for whole race</param>
        /// <param name="basePts">
        /// starting points level in case penalty is based on non-penalty points
        /// </param>
        /// <returns>points to be assigned for the penalty</returns>
        public virtual double GetPenaltyPoints(
            TRSPenalty p, TRacePointsList rpList, double basePts)
        {
            int nEntries = 0;
            if (rpList != null)
            {
                nEntries = rpList.Count;
            }
            
            if (HasFleets && TargetFleetSize > nEntries && (! IsFinalRace))
            {
                nEntries = TargetFleetSize;
            }

            // if MAN, RDG, or DIP: return fixed points and be gone
            if (p.HasPenalty(Constants.MAN) || p.HasPenalty(Constants.RDG) || p.HasPenalty(Constants.DPI))
            {
                return p.Points;
            }
            
            //A9 RACE SCORES IN A SERIES LONGER THAN A REGATTA
            //For a series that is held over a period of time longer than a regatta, a boat that
            //came to the starting area but did not start (DNS), did not finish (DNF), retired after finishing (RAF)
            //or was disqualified (allDSQ) shall be scored points for the finishing place one more than
            //the number of boats that came to the starting area. A boat that did not come to
            //the starting area (DNC) shall be scored points for the finishing place one more than the
            //number of boats entered in the series.
            
            // if a DSQ penalty, return DSQ points a be gone (DSQ, DNE, OCS, BFD, RAF)
            if (p.IsDsqPenalty())
            {
                if (rpList == null)
                {
                    return 0;
                }

                return IsLongSeries ? (nEntries - rpList.GetNumberWithPenalty(Constants.DNC) + 1):(nEntries + 1);
            }

            //did come to the starting area but did not start
            if (p.HasPenalty(Constants.DNC))
            {
                return nEntries + 1;
            }

            // any non-finish penalty other than TLE, return entries + 1 and be gone
            //dnf, dns, (dnc already dealt with)
            if (p.IsFinishPenalty() && !p.HasPenalty(Constants.TLE))
            {
                if (rpList == null)
                {
                    return 0;
                }

                return IsLongSeries ? (nEntries - rpList.GetNumberWithPenalty(Constants.DNC) + 1):(nEntries + 1);
            }
            
            //time limit expired
            if (p.HasPenalty(Constants.TLE))
            {
                int nFinishers = (rpList == null)?0:rpList.NumberFinishers;
                // set the basepts to the appropriate TLE points
                switch (TimeLimitPenalty)
                {
                    case TLE_DNF: 
                        basePts = GetPenaltyPoints(new TRSPenalty(Constants.DNF), rpList, basePts); 
                        break;
                    
                    case TLE_FINISHERSPLUS1: 
                        basePts = nFinishers + 1; 
                        break;
                    
                    case TLE_FINISHERSPLUS2: 
                        basePts = nFinishers + 2; 
                        break;
                    
                    case TLE_AVERAGE: 
                        basePts = nFinishers + ((((double) nEntries) - nFinishers) / 2.0); 
                        break;
                    
                    default: 
                        basePts = GetPenaltyPoints(new TRSPenalty(Constants.DNF), rpList, basePts); 
                        break;                    
                }
            }
            
            // ADD in other penalties
            double dsqPoints = GetPenaltyPoints(new TRSPenalty(Constants.DSQ), rpList, basePts);
            if (p.HasPenalty(Constants.CNF))
            {
                basePts = (long) System.Math.Round(CalcPercent(CheckinPercent, basePts, nEntries, dsqPoints));
            }
            if (p.HasPenalty(Constants.ZFP))
            {
                basePts = (long) System.Math.Round(CalcPercent(20, basePts, nEntries, dsqPoints));
            }
            if (p.HasPenalty(Constants.SCP))
            {
                basePts = System.Math.Round(CalcPercent(p.Percent, basePts, nEntries, dsqPoints));
            }

            return basePts;
        }
        
        /// <summary> 
        /// returns percent of number of entries, to nearest 10th, .5 going up
        /// with a maximum points of those for DNC
        /// </summary>
        /// <param name="pct">the percent to be assigned</param>
        /// <param name="basePts">initial number of points</param>
        /// <param name="nEntries">number of entries in race</param>
        /// <param name="maxPoints">max points to be awarded</param>
        /// <returns>new points value</returns>
        protected internal virtual double CalcPercent(
            int pct, double basePts, double nEntries, double maxPoints)
        {
            // this gives points * 10
            double points = (long) System.Math.Round(nEntries * (((double) pct) / 10.0));
            points = points / 10.0;
            return System.Math.Min(basePts + points, maxPoints);
        }
        
        public void SetFixedNumberOfThrowouts(int value)
        {
            FixedNumberOfThrowouts = value;
        }

        /// <summary> 
        /// returns number of throwsout to be calculated in a race
        /// </summary>
        /// <param name="pointsList">racepointslist to be looked into</param>
        /// <returns>number of throwouts for races</returns>
        protected internal virtual int GetNumberThrowouts(TRacePointsList pointsList)
        {
            int nRaces = pointsList.Count;
            if (FixedNumberOfThrowouts < nRaces)
            {
                return FixedNumberOfThrowouts;
            }
            else
            {
                return nRaces - 1;
            }
        }
        
        /// <summary>
        /// <p>Calculates throwouts...; its also the responsibility of the ScoringSystem
        /// to manage the setting of throwout criteria.
        /// Assumes that prior throwout flags have been cleared prior to calling this method
        /// </p>
        /// <p>NOTE NOTE: if a boat has more that one race that is equal to their worse race;
        /// this will select their earliest "worst races" as their throwout;
        /// THIS CAN, IN RARE CASES, under 97 to 2000 rules, be a problem;
        /// But situation is clear in 2001-2004 rule</p> 
        /// </summary>
        /// <param name="pointsList">
        /// list of race points on which to calc throwouts
        /// </param>
        public virtual void CalcThrowouts(TRacePointsList pointsList)
        {
            // look through the fThrowouts array and determine how many throwouts to award
            int nThrows = GetNumberThrowouts(pointsList);
            
            for (int i = 0; i < nThrows; i++)
            {
                TRacePoints worstRace = null;
                foreach (TRacePoints thisRace in pointsList)
                {
                    if (thisRace.Finish.Fleet == 0 && thisRace.Race.IsFinalRace)
                    {
                        continue; //do not discard medal race scores
                    }

                    if (!thisRace.Finish.Penalty.HasPenalty(Constants.DNE)
                        && !thisRace.Finish.Penalty.HasPenalty(Constants.DGM)
                        && !thisRace.Finish.Penalty.HasPenalty(Constants.AVG))
                    {
                        if (!thisRace.IsThrowout
                            && ((worstRace == null)
                            || (thisRace.Points > worstRace.Points)))
                        {
                            worstRace = thisRace;
                        }
                    }
                }
                if (worstRace != null)
                {
                    worstRace.IsThrowout = true;
                }
            }
        }
                
    }    

}