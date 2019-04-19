using System.Collections.Generic;

namespace RiggVar.Scoring
{
    /// <summary>
    /// altes DSV Punktsystem
    /// 1.6 - 2.9 - 4.0 - 5.0 - ...
    /// </summary>
    public class TScoringBonusPointDSV : TScoringLowPoint
    {
        public const string NAME_BonusPointSystemDSV = "Bonus Point DSV";

        public TScoringBonusPointDSV() : base()
        {
        }

        public override string Name => NAME_BonusPointSystemDSV;

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
                        pts = 16;
                    }
                    else if (pts == 16)
                    {
                        pts = 29;
                    }
                    else if (pts == 29)
                    {
                        pts = 40;
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
                result = nEntries;
            }
            else if (p.HasPenalty(Constants.DNC))
            {
                result = nEntries;
            }
            else if (p.HasPenalty(Constants.DNF))
            {
                result = nEntries;
            }
            else if (p.IsDsqPenalty())
            {
                result = nEntries;
            }

            return result * 10;
        }

    }
}
