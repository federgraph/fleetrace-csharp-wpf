namespace RiggVar.Scoring
{

    public class TFinish
    {
        public static int NextFinishID = 1;
        private readonly int FinishID;
        public int Fleet;
        public bool IsRacing = true;

        public TRace Race;
        public TEntry Entry;
        private TFinishPosition fPosition;
        private TRSPenalty fPenalty;

        public TFinish(TRace inRace, TEntry inEntry) : this(inRace, inEntry, new TFinishPosition(Constants.NOF), new TRSPenalty(Constants.NOF))
        {
        }

        public TFinish(TRace inRace, TEntry inEntry, TFinishPosition inOrder, TRSPenalty inPenalty)
        {
            FinishID = NextFinishID;
            NextFinishID++;
            Race = inRace;
            Entry = inEntry;
            fPosition = inOrder;
            fPenalty = inPenalty ?? new TRSPenalty();
            if (fPenalty.IsFinishPenalty())
            {
                fPosition = new TFinishPosition(fPenalty.Penalty);
            }
        }

        public virtual TFinishPosition FinishPosition
        {
            get => fPosition;
            set
            {
                fPosition = value;
                if (TRSPenalty.IsFinishPenalty(value.IntValue()))
                {
                    Penalty.FinishPenalty = value.IntValue();
                }
            }
        }
        public virtual TRSPenalty Penalty
        {
            get => fPenalty;
            set
            {
                if (value == null)
                {
                    value = new TRSPenalty();
                }

                fPenalty = value;
            }
        }

        /// <summary>
        /// sorts based on finishes WITHOUT regard to penalites
        /// except for non-finishing penalties
        /// </summary>
        public int CompareTo(object obj)
        {
            TFinish that = (TFinish)obj;
            return fPosition.CompareTo(that.fPosition);
        }

        public override int GetHashCode()
        {
            return FinishID; //base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is TFinish))
            {
                return false;
            }

            if (this == obj)
            {
                return true;
            }

            TFinish that = (TFinish)obj;

            if ((Entry == null) ? (that.Entry != null) : !Entry.Equals(that.Entry))
            {
                return false;
            }

            if ((fPenalty == null) ? (that.fPenalty != null) : !fPenalty.Equals(that.fPenalty))
            {
                return false;
            }

            return true;
        }

        public override string ToString()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            if (Entry == null)
            {
                sb.Append("<null entry>");
            }
            else
            {
                sb.Append(Entry.ToString());
                sb.Append("/ ");
                if (fPosition != null)
                {
                    sb.Append(fPosition.ToString());
                }

                if (fPenalty != null)
                {
                    sb.Append("[");
                    sb.Append(fPenalty.ToString());
                    sb.Append("]");
                }
            }
            return sb.ToString();
        }

        public virtual bool IsNoFinish()
        {
            return fPosition.IsNoFinish();
        }

        public virtual bool HasPenalty()
        {
            return fPenalty.Penalty != Constants.NOP;
        }

        public virtual bool HasPenalty(TRSPenalty pen)
        {
            return fPenalty.Penalty == pen.Penalty;
        }

        public virtual bool HasPenalty(int ipen)
        {
            return fPenalty.Penalty == ipen;
        }

    }
}