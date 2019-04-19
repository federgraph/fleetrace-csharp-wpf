namespace RiggVar.Scoring
{

    /// <summary> 
    /// Contains points information on an entry in a race
    /// This is separated from the Finish object because when fleet scoring gets
    /// implemented an entry could have more than one score for a single finish
    /// </summary>
    public class TRacePoints: TPoints
    {        
        public static int NextRacePointID = 1;
        public int RacePointID;

        public TRace Race;
        public bool IsThrowout;

        private TFinish fFinish;

        protected internal int aSailID; // for debugging so boat's SailID pops up in debugger listing
        
        public TRacePoints(TRace race, TEntry entry, double points, bool throwout):base(entry, points, 0)
        {
            RacePointID = NextRacePointID;
            NextRacePointID++;

            if (entry != null)
            {
                aSailID = entry.SailID;
            }
            Race = race;
            IsThrowout = throwout;
            fFinish = null;
        }

        public virtual bool IsTied(TRacePoints lastrp)
        {
            //can throw NullpointerException
            TFinish f = Finish;
            return (lastrp != null 
                && f.FinishPosition.IntValue() != 0 //&& f.CorrectedTime != SailTime.NOTIME 
                && !f.HasPenalty() 
                && !lastrp.Finish.HasPenalty() 
                && f.FinishPosition.IntValue() == lastrp.Finish.FinishPosition.IntValue() //&& lastrp.Finish.CorrectedTime == f.CorrectedTime
                );
        }                

        public virtual TFinish Finish
        {
            get
            {
                if (fFinish == null)
                {
                    if (Race == null)
                    {
                        return null;
                    }

                    if (Entry == null)
                    {
                        return null;
                    }

                    fFinish = Race.GetFinish(Entry);
                }
                return fFinish;
            }            
        }
        
        /// <summary> 
        /// compares based on the points, ignores the throwout
        /// </summary>
        public int CompareTo(object obj)
        {
            if (obj == null)
            {
                return - 1;
            }

            try
            {
                TPoints that = (TPoints) obj;
                if (Points < that.Points)
                {
                    return - 1;
                }
                else if (Points > that.Points)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
            catch
            {
                return - 1;
            }
        }
                        
        public override int GetHashCode()
        {
            return RacePointID;
        }

        public override bool Equals(object obj)
        {
            if (this == obj)
            {
                return true;
            }

            if (!(obj is TRacePoints))
            {
                return false;
            }

            if (!base.Equals(obj))
            {
                return false;
            }

            TRacePoints that = (TRacePoints) obj;
            if (this.IsThrowout != that.IsThrowout)
            {
                return false;
            }

            if (!EqualsWithNull(this.Race, that.Race))
            {
                return false;
            }

            return true;
        }

        public override string ToString()
        {
            return ToString(true);
        }
        
        public virtual string ToString(bool showPts)
        {
            TFinish finish = Finish;
            TRSPenalty penalty = finish.Penalty;
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            
            bool didPts = false;
            if (showPts || !finish.HasPenalty() || (penalty.IsOtherPenalty()))
            {
                string s;
                s = Points.ToString("f2");
                sb.Append(s);
                didPts = true;
            }
            
            if (penalty.IsDsqPenalty())
            {
                TRSPenalty ptemp = (TRSPenalty) finish.Penalty.Clone();
                ptemp.ClearPenalty(Constants.NF);
                if (didPts)
                {
                    sb.Append("/");
                }

                sb.Append(ptemp.ToString(false));
            }
            else if (finish.HasPenalty())
            {
                if (didPts)
                {
                    sb.Append("/");
                }

                sb.Append(penalty.ToString(false));
            }
            
            if (IsThrowout)
            {
                sb.Insert(0, "[");
                sb.Append("]");
            }
            return sb.ToString();
        }

        public bool IsMedalRace
        {
            get
            {
                if (Finish == null || this.Race == null)
                {
                    return false;
                }

                return Finish.Fleet == 0 && Race.IsFinalRace;
            }
        }
    }
}