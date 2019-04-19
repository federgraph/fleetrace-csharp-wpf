using System.Collections.Generic;

namespace RiggVar.Scoring

{
    /// <summary>
    /// Bonus - Punktsystem
    /// 0 - 3 - 5.7 - 8.0 - ...
    /// </summary>
    public class TScoringBonusPoint : TScoringLowPoint
    {
        public const string NAME_BonusPointSystem = "Bonus Point";

        public TScoringBonusPoint() : base()
        {
        }

        public override string Name => NAME_BonusPointSystem;

        protected internal override void ScoreRace(
            TRace r, TRacePointsList points, bool firstIs75, bool positionOnly)
        {
            // sort points on finishposition sorted top to bottom by finish
            points.SortFinishPosition();
            
            int pts = 0;
            
            // position within the divsion (as opposed to within the fleet)
            int divPosition = 1;

            // loop thru the race's finishes, for each finish in the list, set the points            
            foreach (TRacePoints rp in points)
            {
                TFinish f = rp.Finish;
                double basePts = pts;
                rp.Position = divPosition++;
                
                if (f.FinishPosition.IsValidFinish() && (!f.Penalty.IsDsqPenalty()))
                {
                    // increment points to be assigned to next guy if this
                    // guy is a valid finisher and not disqualified
                    if (pts == 0)
                    {
                        pts = 30;
                    }
                    else if (pts == 30)
                    {
                        pts = 57;
                    }
                    else if (pts == 57)
                    {
                        pts = 80;
                    }
                    else if (pts == 80)
                    {
                        pts = 100;
                    }
                    else if (pts == 100)
                    {
                        pts = 117;
                    }
                    else if (pts == 117)
                    {
                        pts = 130;
                    }
                    else if (pts == 130)
                    {
                        pts = 140;
                    }
                    else
                    {
                        pts = pts + 10;
                    }
                }
                else
                {
                    rp.Position = f.FinishPosition.IntValue();
                }
                if (f.HasPenalty())
                {
                    basePts = GetPenaltyPoints(f.Penalty, points, basePts);
                }
                if (!positionOnly)
                {
                    rp.Points = basePts / 10;
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

        public override double GetPenaltyPoints(
            TRSPenalty p, TRacePointsList rpList, double basePts)
        {
            int nEntries = 0;
            if (rpList != null)
            {
                nEntries = rpList.Count;
            }
            
            double result = base.GetPenaltyPoints(p, rpList, basePts);

            if (p.HasPenalty(Constants.DNS))
            {
                result = nEntries + 6;
            }
            else if (p.HasPenalty(Constants.DNC))
            {
                result = nEntries + 6;
            }
            else if (p.HasPenalty(Constants.DNF))
            {
                result = nEntries + 6;
            }
            else if (p.IsDsqPenalty())
            {
                result = nEntries + 6;
            }

            return result * 10;
        }

    }
}
