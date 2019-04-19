using System.Text;

namespace RiggVar.Scoring
{

    /// <summary> 
    /// Contains total points information of an entry in a regatta
    /// </summary>
    public class TSeriesPoints: TPoints
    {        
        public static int NextSeriesPointID = 1;
        public int SeriesPointID;

        public bool Tied;

        protected internal int aSailID; // for debugging so boat's SailID pops up in debugger listing
        protected internal int EntryID; // for preserving order when sorting equals
    
        public TSeriesPoints(TEntry entry) : this(entry, double.NaN, int.MaxValue, false)
        {
        }
        
        public TSeriesPoints(TEntry entry, double points, int pos, bool tied) : base(entry, points, pos)
        {
            SeriesPointID = NextSeriesPointID;
            NextSeriesPointID++;

            if (entry != null)
            {
                aSailID = entry.SailID;
                EntryID = entry.EntryID;
            }
            Tied = tied;
        }
                
        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (this == obj)
            {
                return true;
            }

            if (!(obj is TSeriesPoints))
            {
                return false;
            }

            if (!base.Equals(obj))
            {
                return false;
            }

            TSeriesPoints that = (TSeriesPoints) obj;
            return (Tied == that.Tied);
        }

        public int CompareTo(object obj)
        {
            if (obj == null)
            {
                return - 1;
            }

            try
            {
                TSeriesPoints that = (TSeriesPoints) obj;
                if (this.Points < that.Points)
                {
                    return - 1;
                }
                else if (Points > that.Points)
                {
                    return 1;
                }
                else
                {
                    //need to compare by EntryID also, because Sort is not a stable sort,
                    //order of elements is not preserved if elements are equal
                    if (EntryID < that.EntryID)
                    {
                        return -1;
                    }
                    else if (EntryID > that.EntryID)
                    {
                        return 1;
                    }
                }
                return 0;
            }
            catch
            {
                return - 1;
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Points.ToString("f2"));
            if (Tied)
            {
                sb.Append("T");
            }

            return sb.ToString();
        }                
                
    }
}