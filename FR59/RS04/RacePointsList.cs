using System.Collections.Generic;

namespace RiggVar.Scoring
{

    public class TRacePointsList : List<TRacePoints>
    {
        public TRacePointsList() : base()
        {
        }

        public virtual int NumberFinishers
        {
            /// <summary> 
            /// calculates number of valid finishers in this list of race points;
            /// NOTE: if any of the finishes are null, returns 0;
            /// NOTE: this is computationally intensive, if you can go straight
            /// to the raw finish list, that is better
            /// </summary>
            get
            {
                int n = 0;
                foreach (TRacePoints pts in this)
                {
                    if (pts.Race == null)
                    {
                        // if race is null, then must be series standings, assume all valid
                        n++;
                    }
                    else
                    {
                        TFinish f = pts.Race.GetFinish(pts.Entry);
                        if (f != null && f.FinishPosition != null && f.FinishPosition.IsValidFinish())
                        {
                            n++;
                        }
                    }
                }
                return n;
            }            
        }

        public virtual int NumberStarters
        {
            /// <summary> 
            /// calculates number of valid starters in this list of race points;
            /// NOTE: if any of the finishes are null, returns 0;
            /// NOTE: this is computationally intensive, if you can go straight;
            /// to the raw finish list, that is better
            /// </summary>
            get
            {
                int n = 0;
                foreach (TRacePoints pts in this)
                {
                    if (pts.Race == null)
                    {
                        // if race is null, then must be series standings, assume all valid
                        n++;
                    }
                    else
                    {
                        TFinish f = pts.Race.GetFinish(pts.Entry);
                        if (f != null && f.FinishPosition != null)
                        {
                            if (f.FinishPosition.IsValidFinish())
                            {
                                n++;
                            }
                            else if (!(f.Penalty.HasPenalty(Constants.DNC) || f.Penalty.HasPenalty(Constants.DNS)))
                            {
                                n++;
                            }
                        }
                    }
                }
                return n;
            }            
        }
                                            
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (this == obj)
            {
                return true;
            }

            try
            {
                TRacePointsList that = (TRacePointsList) obj;
                if (that.Count != Count)
                {
                    return false;
                }

                foreach (TRacePoints rpThis in this)
                {
                    TRacePoints rpThat = that.FindPoints(rpThis.Race, rpThis.Entry);
                    if (rpThat == null)
                    {
                        return false;
                    }

                    if (!rpThis.Equals(rpThat))
                    {
                        return false;
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        
        /// <summary> 
        /// returns first (and hopefully only) item in list for specified race and entry
        /// </summary>
        public virtual TRacePoints FindPoints(TRace r, TEntry e)
        {
            foreach (TRacePoints p in this)
            {
                if (((e == null) || ((p.Entry != null) && (p.Entry.Equals(e)))) 
                    && ((r == null) || ((p.Race != null) && (p.Race.Equals(r)))) )
                {
                    return p;
                }
            }
            return null;
        }
        
        public virtual TRacePointsList FindAllPointsForEntry(TEntry entry)
        {
            TRacePointsList list = new TRacePointsList();
            foreach (TRacePoints p in this)
            {
                if ((p.Entry != null) && p.Entry.Equals(entry))
                {
                    list.Add(p);
                }
            }
            return list;
        }
        
        public virtual TRacePointsList FindAllPointsForRace(TRace race)
        {
            TRacePointsList list = new TRacePointsList();
            foreach (TRacePoints p in this)
            {
                if ((p.Race != null) && p.Race.Equals(race))
                {
                    list.Add(p);
                }
            }
            return list;
        }

        /// <summary> 
        /// generates a string of elements
        /// </summary>
        public override string ToString()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder("rplist=(");

            int i = 0;
            foreach (TRacePoints p in this)
            {
                i++;
                sb.Append(p.ToString());
                if (i < (Count))
                {
                    sb.Append(',');
                }
            }
            sb.Append(')');
            return sb.ToString();
        }
                
        /// <summary> 
        /// calculates number of racers with specified penalty;
        /// NOTE: if any of the finishes are null, returns 0;
        /// NOTE: this is computationally intensive, if you can go straight;
        /// to the raw finish list, that is better
        /// </summary>
        public virtual int GetNumberWithPenalty(int pen)
        {
            int n = 0;
            foreach (TRacePoints pts in this)
            {
                try
                {
                    TFinish f = pts.Race.GetFinish(pts.Entry);
                    if (f.HasPenalty(pen))
                    {
                        n++;
                    }
                }
                catch
                {
                }
            }
            return n;
        }
        
        public virtual void ClearAll(TEntry e)
        {
            foreach (TRacePoints p in this)
            {
                if ((p.Entry != null) && p.Entry.Equals(e))
                {
                    Remove(p);
                }
            }
        }
        
        public virtual void ClearAll(TRace r)
        {
            foreach (TRacePoints p in this)
            {
                if ((p.Race != null) && (p.Race.Equals(r)))
                {
                    Remove(p);
                }
            }
        }
        
        /// <summary> 
        /// clears old points for race, and creates a new set of them, 
        /// returns a RacePointsList of points for this race.
        /// AND autoamtically adds DNC finishes for entries without finishes
        /// </summary>
        public virtual TRacePointsList InitPoints(TRace r, TEntryList entries)
        {
            ClearAll(r);
            TRacePointsList rList = new TRacePointsList();
            foreach (TEntry e in entries)
            {
                TFinish f = r.GetFinish(e);
                if (f == null)
                {
                    f = new TFinish(r, e)
                    {
                        FinishPosition = new TFinishPosition(Constants.DNC),
                        Penalty = new TRSPenalty(Constants.DNC)
                    };
                    r.Finish = f;
                }
                TRacePoints rp = new TRacePoints(f.Race, f.Entry, double.NaN, false);
                Add(rp);
                rList.Add(rp);
            }
            return rList;
        }
                    
        public virtual void SortPosition()
        {
            Sort(new ComparatorPosition());
        }
                
        public class ComparatorPosition: IComparer<TRacePoints>
        {
            public virtual int Compare(TRacePoints left, TRacePoints right)
            {
                if (left == null && right == null)
                {
                    return 0;
                }

                if (left == null)
                {
                    return - 1;
                }

                if (right == null)
                {
                    return 1;
                }

                return left.Finish.FinishPosition.CompareTo(right.Finish.FinishPosition);
            }
        }
                
        public virtual void SortFinishPosition()
        {
            this.Sort(new ComparatorFinishPosition());
        }
                
        public class ComparatorFinishPosition: IComparer<TRacePoints>
        {
            public virtual int Compare(TRacePoints left, TRacePoints right)
            {
                if (left == null && right == null)
                {
                    return 0;
                }

                if (left == null)
                {
                    return - 1;
                }

                if (right == null)
                {
                    return 1;
                }

                if (left.Finish == null)
                {
                    return - 1;
                }

                if (right.Finish == null)
                {
                    return 1;
                }

                return left.Finish.FinishPosition.CompareTo(right.Finish.FinishPosition);
            }
        }
                        
        public virtual void SortRace()
        {
            Sort(new ComparatorRace());
        }
        
        public class ComparatorRace: IComparer<TRacePoints>
        {
            public virtual int Compare(TRacePoints left, TRacePoints right)
            {
                if (left == null && right == null)
                {
                    return 0;
                }

                if (left == null)
                {
                    return - 1;
                }

                if (right == null)
                {
                    return 1;
                }

                try
                {
                    return left.Race.CompareTo(right.Race);
                }
                catch
                {
                    return 0;
                }
            }
        }
        
        public virtual void SortPoints()
        {
            Sort(new ComparatorPoints());
        }
        
        public class ComparatorPoints: IComparer<TRacePoints>
        {
            public virtual int Compare(TRacePoints left, TRacePoints right)
            {
                if (left == null && right == null)
                {
                    return 0;
                }

                if (left == null)
                {
                    return - 1;
                }

                if (right == null)
                {
                    return 1;
                }

                int c1 = left.CompareTo(right);
                if (c1 != 0)
                {
                    return c1;
                }

                return left.Finish.FinishPosition.CompareTo(right.Finish.FinishPosition);            
            }
        }

        public TRacePointsList CloneEntries()
        {
            TRacePointsList result = new TRacePointsList();
            foreach (TRacePoints e in this)
            {
                result.Add(e);
            }
            return result;
        }

    }
}