namespace RiggVar.Scoring
{

    /// <summary> 
    /// Interface that any scoring system must implement
    /// so that it can be managed by ScoringManager
    /// </summary>
    public interface IScoringModel
    {
        /// <summary>
        /// returns the name of this scoring system
        /// </summary>
        string Name
        {            
            get;                
        }

        /// <summary> 
        /// pulls parameters from the sourceModel
        /// </summary>
        /// <param name="sourceModel">
        /// the originating model from which parameters are drawn
        /// </param>
        IScoringModel Attributes
        {
            set;            
        }

        /// <summary>
        /// Given a Race and a list of Entries, calculates the RacePoints objects
        /// The entries should be assumed to represent a single class within the Race
        /// scoreRace can assume that an Entry without a finish in the Race is DNC
        /// but should recognize that the Race may well have finishers not in the Entries.
        /// <P>
        /// Can assume:
        /// (1) that any "NoFinish penalties" have been properly passed
        ///     thru to the FinishPosition.
        /// (2) FinishPosition is otherwise sound and matches finishtimes if any
        /// (3) All items in Entry list should be valid racers in the Race
        /// (4) None of race, entries, or points is null
        /// </P>
        /// </summary>
        /// <param name="race">the Race to be scored</param>
        /// <param name="points">a list of racepoints in which the points should be stored</param>
        /// <param name="positionOnly">when true do NOT recalculate race points, do race position only</param>
        void ScoreRace(TRace race, TRacePointsList points, bool positionOnly);

        /// <summary> 
        /// Given a list or races, entries, points lists.. 
        /// calculates the overall series points.
        /// Assume that each individual race has already been calculated, 
        /// and that throwouts have already been designated in the points objects.
        /// Do not worry about tiebreakers.
        /// </summary>
        /// <param name="races">list of races involved in the series</param>
        /// <param name="entries">list of entries whose series totals are to be calculated</param>
        /// <param name="points">list of points for all races and entries (and maybe more)</param>
        /// <param name="series">map in which (key=entry, value=Double) 
        /// series points are to be calculated.</param>
        void ScoreSeries(TRaceList races, TEntryList entries, TRacePointsList points, TSeriesPointsList series);
        
        /// <summary> 
        /// resolve ties among a group of tied boats.  A tie that is breakable
        /// should have .01 point increments added as appropriate.
        /// Assume that each individual race and series points have calculated, and that
        /// throwouts have already been designated in the points objects.
        /// </summary>
        /// <param name="races">list of involved races, in the order they were sailed</param>
        /// <param name="entries">list of tied entries</param>
        /// <param name="points">list of points for all races and entries (and maybe more!)</param>
        /// <param name="series">map containing series points for the entries, 
        /// prior to handling ties (and maybe more)</param>
        void CalculateTieBreakers(TRaceList races, TEntryList entries, TRacePointsList points, TSeriesPointsList series);
        
        /// <summary> 
        /// Given a penalty, returns the number of points to be assigned.
        /// Do NOT handle AVG, it will be dealt with by ScoringManager.
        /// Note that the race could be null, and basePts might be 0 or NaN
        /// </summary>
        /// <param name="p">the Penalty to be calculated</param>
        /// <param name="rpList">the RacePointsList of the points being calculated</param>
        /// <param name="basepts">the points calculated before applying a penalty</param>
        double GetPenaltyPoints(TRSPenalty p, TRacePointsList rpList, double basePts);
        
        /// <summary> 
        /// Calculates throwouts... its also the responsibility of the ScoringModel
        /// to manage the setting of throwout criteria
        /// </summary>
        /// <param name="list">list of the RacePoints for all races of one entry 
        /// for which throwouts should be considered
        /// </param>
        void CalcThrowouts(TRacePointsList pointsList);
        
        /// <summary> 
        /// Sorts a list of series points from best to worst
        /// </summary>
        void SortSeries(TSeriesPointsList seriesPoints);
        
    }

}