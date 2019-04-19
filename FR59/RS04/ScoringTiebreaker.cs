namespace RiggVar.Scoring
{

    /// <summary> 
    /// covering class to run tied boats through a tiebreaker
    /// looks thru seriespoints lists, gathers groups of tied boats and
    /// calls an breakTies method to handle that group of tied boats
    /// which delegates to concrete implementation
    /// </summary>
    public class TScoringTiebreaker
    {
        private IScoringModel fModel;
        private readonly TRacePointsList racePoints;
        private TSeriesPointsList seriesPoints;

        public TScoringTiebreaker(IScoringModel model, TRaceList rlist, TRacePointsList rpl, TSeriesPointsList spl)
        {
            fModel = model;
            Races = rlist;
            racePoints = rpl;
            seriesPoints = spl;
        }

        protected TRaceList Races { get; }

        public void Process()
        {
            TEntryList tiedBunch = new TEntryList();
            TSeriesPoints basePoints = (TSeriesPoints) seriesPoints[0];
            
            for (int i = 1; i < seriesPoints.Count; i++)
            {
                TSeriesPoints newPoints = seriesPoints[i];
                
                if (basePoints.Points == newPoints.Points)
                {
                    // have a tie, see if starting a new group
                    if (tiedBunch.Count == 0)
                    {
                        tiedBunch.Add(basePoints.Entry);
                    }
                    tiedBunch.Add(newPoints.Entry);
                }
                else
                {
                    // this one not tie, send bunch to tiebreaker resolution
                    if (tiedBunch.Count > 0)
                    {
                        BreakTies(tiedBunch);
                        tiedBunch.Clear();
                    }
                    basePoints = newPoints;
                }
            }
            
            // at end of loop, see if we are tied at the bottom
            if (tiedBunch.Count > 0)
            {
                BreakTies(tiedBunch);
            }
        }

        //public virtual void breakTies(EntryList tiedBunch)
        public void BreakTies(TEntryList tiedBunch)
        {
            fModel.CalculateTieBreakers(Races, tiedBunch, racePoints, seriesPoints);
        }

    }
}