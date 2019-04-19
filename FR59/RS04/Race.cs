using System;

namespace RiggVar.Scoring
{

    public class TRace : IComparable
    {
        public static int NextRaceID = 1;
        public int RaceID;

        public int NameID;
        public bool IsRacing;

        public TFinishList FinishList;

        public bool HasFleets;
        public int TargetFleetSize;
        public bool IsFinalRace;

        public TRace(int inNameID)
        {
            RaceID = NextRaceID;
            NextRaceID++;
            NameID = inNameID;
            FinishList = new TFinishList();
        }

        public virtual TFinish Finish
        {
            /// <summary>
            /// adds or replaces the finish for value.Entry in this race.
            /// Ignores the finish if value.Entry is not valid entrant.
            /// </summary>
            set
            {
                TEntry e = value.Entry;
                if (e == null || !IsSailing(e))
                {
                    return ;
                }

                TFinish oldFinish = FinishList.FindEntry(e);
                if (oldFinish != null)
                {
                    FinishList.Remove(oldFinish);
                }
                FinishList.Add(value);
            }
            
        }
                                                    
        public int CompareTo(object obj)
        {
            if (!(obj is TRace))
            {
                return - 1;
            }

            if (Equals(obj))
            {
                return 0;
            }

            TRace that = (TRace) obj;
            
            if (this.NameID < that.NameID)
            {
                return -1;
            }
            else if (this.NameID == that.NameID)
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }
        
        public override bool Equals(object obj)
        {
            if (this == obj)
            {
                return true;
            }

            try
            {
                return (RaceID == ((TRace) obj).RaceID);
            }
            catch
            {
                return false;
            }
        }
        
        public override int GetHashCode()
        {
            return RaceID;
        }
                                        
        /// <summary> 
        /// return true if the specified entry should be sailing in the race
        /// </summary>
        public virtual bool IsSailing(TEntry e)
        {
            return true;
        }
                
        public override string ToString()
        {
            if (NameID == 0)
            {
                return "R?"; //Util.getString("noname");
            }
            else
            {
                return "R" + NameID.ToString();
            }
        }

        /// <summary> 
        /// returns the finish for entry e in this race
        /// May return null if entry e was not a valid entrant in this race
        /// If e is valid entrant but does not hae a finish, a finish
        /// with FinishPosition of NOFINISH is created and returned
        /// </summary>
        public virtual TFinish GetFinish(TEntry e)
        {
            if (!IsSailing(e))
            {
                return null;
            }

            TFinish f = FinishList.FindEntry(e);
            if (f == null)
            {
                f = new TFinish(this, e)
                {
                    Penalty = new TRSPenalty(Constants.NOF)
                };
            }
            return f;
        }

    }
}