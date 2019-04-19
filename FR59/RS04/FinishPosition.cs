namespace RiggVar.Scoring
{
    
    /// <summary> 
    /// Class for storing finish position numbers.
    /// 
    /// This is mostly an integer with the raw finish order, 
    /// but also handles non-finish values such as DNC, DNS and DNF.
    /// 
    /// NOTE this class is responsible only for specifying the finish numbers, 
    /// NOT for determining the points to be assigned. 
    /// 
    /// <see cref="ScoringSystems"/> for changing penalties into points.
    /// 
    /// <p>See also the @Penalty class for handling of penalties assigned to boats
    /// (whether or not they have a valid finish).</p>
    /// 
    /// to set a new finish position, recreate the instance
    /// </summary>
    public class TFinishPosition
    {    
        public static int NextFinishPositionID = 1;
        private readonly int FinishPositionID;

        private int fFinishPosition;
        
        public TFinishPosition(int value)
        {
            FinishPositionID = NextFinishPositionID;
            NextFinishPositionID++;

            if ((value & Constants.NF) != 0)
            {
                // setting to non-finish penalty... mask out other bits
                fFinishPosition = value & Constants.NOF;
            }
            else
            {
                fFinishPosition = value;
            }
        }
        
        public static int ParseString(string s)
        {
            try
            {
                return int.Parse(s);
            }
            catch
            {
                try
                {
                    return TRSPenalty.ParsePenalty(s).Penalty;
                }
                catch
                {
                    return Constants.NOF;
                }
            }
        }
                        
        public int CompareTo(object obj)
        {
            if (!(obj is TFinishPosition))
            {
                return - 1;
            }

            TFinishPosition that = (TFinishPosition) obj;
            if (fFinishPosition < that.fFinishPosition)
            {
                return - 1;
            }
            else if (fFinishPosition > that.fFinishPosition)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
        
        public virtual bool IsFinisher()
        {
            return (fFinishPosition < Constants.HF) && (fFinishPosition > 0);
        }
        
        public virtual bool IsNoFinish()
        {
            return fFinishPosition == Constants.NOF;
        }
        
        public override int GetHashCode()
        {
            return FinishPositionID; //base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return !(obj is TFinishPosition) ? false : fFinishPosition == ((TFinishPosition) obj).fFinishPosition;
        }

        public virtual int IntValue()
        {
            return fFinishPosition;
        }
        
        public virtual bool IsValidFinish()
        {
            return IsValidFinish(fFinishPosition);
        }
            
        public static bool IsValidFinish(int order)
        {
            return order <= Constants.HF;
        }
        
        public override string ToString()
        {
            return ToString(fFinishPosition);
        }
                
        public static string ToString(int order)
        {
            switch (order)
            {
                case Constants.NOF: return "No Finish";
                case Constants.DNC: return "dnc";
                case Constants.DNS: return "dns";
                case Constants.DNF: return "dnf";
                case Constants.TLE: return "tle";
                default: return order.ToString();                    
            }
        }
    }
}