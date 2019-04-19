namespace RiggVar.Scoring
{
    
    /// <summary> 
    /// abstract class, 
    /// is linked to an Entry,
    /// contains position and points information,
    /// implemented as RacePoints and SeriesPoints
    /// </summary>
    public abstract class TPoints
    {
        public TEntry Entry;
        public double Points;
        public int Position;
        
        protected internal TPoints(TEntry entry, double points, int pos)
        {
            Entry = entry;
            Points = points;
            Position = pos;
        }
                
        public override int GetHashCode()
        {
            //avoid warning here
            //RacePoint and SeriesPoints override GetHashCode in a meaningful way
            return base.GetHashCode();
        }

        public bool EqualsWithNull(object left, object right)
        {
            try
            {
                //static Equals method of object handles nulls without exception
                return object.Equals(left, right);
                //return left.Equals(right);
            }
            catch (System.NullReferenceException)
            { 
                return (right == null);
            }        
        }

        public override bool Equals(object obj)
        {
            if (this == obj)
            {
                return true;
            }

            if (!(obj is TPoints))
            {
                return false;
            }

            TPoints that = (TPoints) obj;
            if (Points != that.Points)
            {
                return false;
            }

            if (Position != that.Position)
            {
                return false;
            }

            if (!EqualsWithNull(this.Entry, that.Entry))
            {
                return false;
            }

            return true;
        }
                                        
    }
}