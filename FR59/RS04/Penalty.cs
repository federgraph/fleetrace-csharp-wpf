using System.Globalization;

namespace RiggVar.Scoring
{

    /// <summary> 
    /// These are the constants that defined the penalty values.
    /// They come in three groups:
    /// 
    /// Non-Finish penalty values are for boats that do not have a valid Finish.
    /// These are used in BOTH the FinishPosition class and in the Penalty class.
    /// These are not-bitwise penalties and a boat cannot have more than one of these at a time.  
    /// See FinishPosition class
    /// 
    /// Disqualification penalties are the various ways a boat can be disqualified.
    /// These also are not bitwise, as a boat can only be disqualified once.
    /// But a boat can be disqualified with or without a valid finish.
    /// So, a boat can carry both a non-finish penalty and a disqualification penalty.  
    /// See Penalty class
    /// 
    /// Other penalties are the various other ways a boat can be "dinged".  This includes
    /// check-in penalties, redress, percentage penalties, etc.  These ARE Bit-wise
    /// penalties, and a boat can have more than one of them.  Also, a boat CAN have a
    /// non-finish penalty and an other penalty, but a boat may not have a disqualification
    /// penalty and "other" penalty.
    /// See Penalty class
    /// </summary>
    public struct Constants
    {
        public const int NOP = 0x0000; //no penalty (mask)

        //group of dsq penalties, not bitwise, only one out of these, lower 3 bits
        public const int DSQ = 0x0001;
        public const int DNE = 0x0002;
        public const int RAF = 0x0003; //retired after finish
        public const int OCS = 0x0004; //on course side
        public const int BFD = 0x0005; //black flag disqualification
        public const int DGM = 0x0006; //-

        public const int DM = 0x0007; //dsq penalty mask

        //group / set of other penalties, bitwise, can have more the one of them
        public const int ZFP = 0x0020; //z flag penalty
        public const int AVG = 0x0040; //average
        public const int SCP = 0x0080; //scoring penalty
        public const int RDG = 0x0100; //redress given
        public const int MAN = 0x0200; //manual
        public const int CNF = 0x0400; //checkin failure
        public const int DPI = 0x1000; //redress given
        public const int NOF = 0xE000; //nofinish (value)

        public const int OM = 0x1FF8; //other (penalty) mask
        public const int HF = 0x1FFF; //highest (possible) finish

        //group of no-finish penalties, not bitwise, upper 3 bits
        public const int TLE = 0x6000; //tle, time limit expired (finish time window)
        public const int DNF = 0x8000; //dnf, did not finish
        public const int DNS = 0xA000; //dns, did not start
        public const int DNC = 0xC000; //dnc, did not come to the starting area

        public const int NF = 0xE000; //no finish (mask)
    }


    /// <summary> 
    /// Class for storing penalty settings. NOTE this class is responsible only
    /// for specifying the penalty assignments: NOT for determining the points to
    /// be assigned. See <see cref="ScoringSystems"/> for changing penalties into points.
    /// <p>
    /// There are three sets of penalties supported in this class:</p>
    /// <list type="">
    /// <item>NonFinishPenalties: penalties that can be assigned to boats that
    /// have not finished a race. Examples include DNC, DNS</item>
    /// <item>Disqualification Penalties: penalties that override other penalties
    /// and involve some variant of causing a boat's finish to be ignored. Examples
    /// include, DSQ, OCS</item>
    /// <item>ScoringPenalties: penalties that may accumulate as various "hits"
    /// on a boat's score. Examples include SCP, ZFP</item>
    /// </list>
    /// Although it is unusual a boat may have more than one penalty applied. For
    /// example a boat may get a Z Flag penalty and a 20 Percent penalty.  Or a boat
    /// may miss a finish time window and still be scored with a 20 Percent penalty.
    /// <p>
    /// In general, a boat can have a Non-Finish penalty AND any other penalty applied
    /// And the scoring penalties can accumulate. But the disqualification penalties do
    /// not accumulate and will override other penalties assigned</p>
    /// </summary>
    public class TRSPenalty
    {
        /// <summary> contains the percentage assigned if a SCP penalty is set </summary>
        private int fPercent;

        /// <summary> contains the points to be awarded for RDG and MAN penalties </summary>
        private double fPoints;

        /// <summary> 
        /// contains the penalties assigned. This is a "bit-wise" field,
        /// each bit represents a different penalty.
        /// </summary>
        private int fPenalty;

        /// <summary> default constructor, creates and empty penalty </summary>
        public TRSPenalty() : this(Constants.NOP)
        {
        }

        /// <summary>
        /// created new penalty object with fPercent and fPoints zero.
        /// </summary>
        /// <param name="pen">combined penalty value</param>
        public TRSPenalty(int pen)
        {
            fPenalty = pen;
            fPercent = 0;
            fPoints = 0;
        }

        public static TRSPenalty[] AllNonFinishPenalties
        {
            get
            {
                return new TRSPenalty[] {
                                         new TRSPenalty(Constants.DNC),
                                         new TRSPenalty(Constants.DNS),
                                         new TRSPenalty(Constants.DNF),
                                         new TRSPenalty(Constants.TLE)
                                         //new TRSPenalty( NOFINISH) 
                };
            }
        }

        /// <summary> replaces the finish penalty leaving others alone
        /// </summary>
        public virtual int FinishPenalty
        {
            set
            {
                fPenalty = fPenalty & (Constants.NF ^ 0xFFFF); // clear finish bits (upper 3 bits)
                fPenalty = fPenalty | (value & Constants.NF); // add in new finish penalty bit
            }
        }

        /// <summary> replaces the disqualification penalty leaving others alone
        /// </summary>
        public virtual int DsqPenalty
        {
            set
            {
                fPenalty = fPenalty & (Constants.DM ^ 0xFFFF); // clear dsq bits (lower 3 bits)
                fPenalty = fPenalty | (value & Constants.DM); // add in new dsq penalty bit
            }
        }

        public virtual int Penalty
        {
            get => fPenalty;
            /// <summary> Replaces the current penalty settings 
            /// with the specified penalty.
            /// Resets percentage and manual points to 0.
            /// </summary>
            set
            {
                fPenalty = value;
                fPercent = 0; //percentage
                fPoints = 0; //manual points
            }
        }
        public virtual int Percent
        {
            get => fPercent;
            /// <summary> Replaces the current percentage penalty amount 
            /// with the specified amount
            /// leaves other penalty settings alone, 
            /// does NOT light the SCP penalty flag
            /// </summary>        
            set
            {
                // addOtherPenalty(Constants.SCP);
                fPercent = value;
        }
        }
        public virtual double Points
        {
            get => fPoints;
            /// <summary> replaces the current manual points with the specified points,
            /// does NOT light the MAN or RDG flag
            /// </summary>                
            set => fPoints = value;
        }

        public override bool Equals(object obj)
        {
            if (this == obj)
            {
                return true;
            }

            try
            {
                TRSPenalty that = (TRSPenalty)obj;
                if (fPenalty != that.fPenalty)
                {
                    return false;
                }

                if (fPercent != that.fPercent)
                {
                    return false;
                }

                if (this.fPoints != that.fPoints)
                {
                    return false;
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }

        public int CompareTo(object obj)
        {
            //if (!(obj instanceof Penalty)) return -1;
            if (Equals(obj))
            {
                return 0;
            }

            TRSPenalty that = (TRSPenalty)obj;

            // so far all penalties are equal
            if (that.fPenalty > this.fPenalty)
            {
                return -1;
            }
            else if (that.fPenalty < this.fPenalty)
            {
                return 1;
            }
            else if (that.fPercent > this.fPercent)
            {
                return -1;
            }
            else if (that.fPercent < this.fPercent)
            {
                return 1;
            }
            else if (that.fPoints > this.fPoints)
            {
                return -1;
            }
            else if (that.fPoints < this.fPoints)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        public object Clone()
        {
            return MemberwiseClone();
        }

        /// <summary> Adds the specified penalty to the set of other penalties applied
        /// All other penalties remain
        /// </summary>
        public virtual int AddOtherPenalty(int newPen)
        {
            newPen = newPen & Constants.OM; // mask out stray bits out of Other area (in the middle between dsq bits and nofinish bits)
            fPenalty = fPenalty | newPen;
            return fPenalty;
        }

        /// <summary> Clears the specified penalty in the set of penalties applied
        /// </summary>
        public virtual int ClearPenalty(int newPen)
        {
            int notPen = 0xFFFF ^ newPen;
            fPenalty = (fPenalty & notPen);
            return fPenalty;
        }

        /// <summary>
        /// tests for the presence of the bit, 
        /// presumes that inPen is a simple penalty, 
        /// not a combination penalty
        /// </summary>
        /// <param name="inPen">a "simple" penalty, not a combination</param>
        /// <returns>true if penalty bit is set</returns>
        public virtual bool HasPenalty(int inPen)
        {
            if (IsOtherPenalty(inPen))
            {
                return (fPenalty & inPen & Constants.OM) != 0;
                // AND of the two in the other range should return good
            }
            else if (IsDsqPenalty(inPen))
            {
                return (fPenalty & Constants.DM) == (inPen & Constants.DM);
            }
            else if (IsFinishPenalty(inPen))
            {
                return (fPenalty & Constants.NF) == (inPen & Constants.NF);
            }
            return inPen == fPenalty;
        }

        /// <summary>
        /// clears the penalty integer, percent and points
        /// </summary>
        public virtual void Clear()
        {
            fPoints = 0;
            fPercent = 0;
            Penalty = Constants.NOP;
        }

        public virtual bool IsFinishPenalty()
        {
            return IsFinishPenalty(fPenalty);
        }

        public virtual bool IsDsqPenalty()
        {
            return IsDsqPenalty(fPenalty);
        }

        public virtual bool IsOtherPenalty()
        {
            return IsOtherPenalty(fPenalty);
        }

        public static bool IsFinishPenalty(int pen)
        {
            return (pen & Constants.NF) != 0;
        }

        public static bool IsDsqPenalty(int pen)
        {
            return (pen & Constants.DM) != 0;
        }

        public static bool IsOtherPenalty(int pen)
        {
            return (pen & Constants.OM) != 0;
        }

        public override string ToString()
        {
            return ToString(this, true);
        }

        public virtual string ToString(bool showPts)
        {
            return ToString(this, showPts);
        }

        public static string ToString(TRSPenalty inP)
        {
            return ToString(inP, true);
        }

        public static string ToString(TRSPenalty inP, bool showPts)
        {
            int pen = inP.Penalty;
            if (pen == 0)
            {
                return "";
            }

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            if ((inP.Penalty & Constants.NF) != 0)
            {
                sb.Append(TFinishPosition.ToString(pen & Constants.NF));
                sb.Append(",");
            }

            pen = inP.Penalty & Constants.DM;
            if (pen == Constants.DSQ)
            {
                sb.Append("DSQ,");
            }

            if (pen == Constants.DNE)
            {
                sb.Append("DNE,");
            }

            if (pen == Constants.RAF)
            {
                sb.Append("RAF,");
            }

            if (pen == Constants.OCS)
            {
                sb.Append("OCS,");
            }

            if (pen == Constants.BFD)
            {
                sb.Append("BFD,");
            }

            if (pen == Constants.DGM)
            {
                sb.Append("DGM,");
            }

            pen = inP.Penalty & Constants.OM;
            if ((pen & Constants.CNF) != 0)
            {
                sb.Append("CNF,");
            }

            if ((pen & Constants.ZFP) != 0)
            {
                sb.Append("ZFP,");
            }

            if ((pen & Constants.AVG) != 0)
            {
                sb.Append("AVG,");
            }

            if ((pen & Constants.SCP) != 0)
            {
                sb.Append(System.Convert.ToString(inP.Percent));
                sb.Append("%,");
            }
            if ((pen & Constants.MAN) != 0)
            {
                sb.Append("MAN");
                if (showPts)
                {
                    sb.Append("/");
                    sb.Append(inP.Points.ToString());
                }
                sb.Append(",");
            }
            if ((pen & Constants.RDG) != 0)
            {
                sb.Append("RDG");
                if (showPts)
                {
                    sb.Append("/");
                    sb.Append(inP.Points.ToString());
                }
                sb.Append(",");
            }
            if ((pen & Constants.DPI) != 0)
            {
                sb.Append("DPI");
                if (showPts)
                {
                    sb.Append("/");
                    sb.Append(inP.Points.ToString());
                }
                sb.Append(",");
            }

            string r = sb.ToString();
            if ((r.Length > 0) && (r.Substring(r.Length - 1).Equals(",")))
            {
                r = r.Substring(0, (r.Length - 1) - (0));
            }
            return r;
        }

        public static string ToString(int pen)
        {
            return ToString(pen, true);
        }

        public static string ToString(int pen, bool doShort)
        {
            return ToString(new TRSPenalty(pen), doShort);
        }

        public static void ParsePenalty(TRSPenalty pen, string penString)
        {
            TRSPenalty newpen = ParsePenalty(penString);
            pen.fPoints = newpen.fPoints;
            pen.fPenalty = newpen.fPenalty;
            pen.fPercent = newpen.fPercent;
        }

        /// <summary>
        /// string parameter is a list of comma separated tokens
        /// each token has the form of Key[/Value]
        /// a points value is expected for RDG, MAN (parsed as double)
        /// a percent value is expected for SCP (parsed as int)
        /// without a key, a percent value can be specified as P+Value or  Value+%
        /// may throw ArgumentException
        /// </summary>
        /// <param name="origPen">Comma separated list of tokens of key/value pairs</param>
        /// <returns>new penalty object</returns>
        public static TRSPenalty ParsePenalty(string origPen)
        {
            string pen = origPen.ToUpper();

            if (pen.Length == 0)
            {
                return new TRSPenalty(Constants.NOP);
            }

            //foreach comma separated token, call this same method (recursively)
            if (pen.IndexOf(",") >= 0)
            {
                int leftc = 0;
                TRSPenalty newpen = new TRSPenalty();

                while (leftc <= pen.Length)
                {
                    int rightc = pen.IndexOf(",", leftc);
                    if (rightc < 0)
                    {
                        rightc = pen.Length;
                    }

                    string sub = pen.Substring(leftc, (rightc) - (leftc));
                    TRSPenalty addpen = ParsePenalty(sub);
                    if (addpen.IsOtherPenalty())
                    {
                        newpen.AddOtherPenalty(addpen.Penalty);

                        if (addpen.HasPenalty(Constants.MAN) || addpen.HasPenalty(Constants.RDG))
                        {
                            newpen.Points = addpen.Points;
                        }
                        if (addpen.HasPenalty(Constants.SCP))
                        {
                            newpen.Percent = addpen.Percent;
                        }
                    }
                    else if (addpen.IsDsqPenalty())
                    {
                        newpen.DsqPenalty = addpen.Penalty;
                    }
                    else if (addpen.IsFinishPenalty())
                    {
                        newpen.FinishPenalty = addpen.Penalty;
                    }
                    leftc = rightc + 1;
                }
                return newpen;
            }

            //the individual tokens should have the general form of <pen>/<number>
            string[] divided = pen.Split('/');
            pen = divided[0];
            string val = (divided.Length > 1) ? divided[1] : "";

            if (pen.Equals("DSQ"))
            {
                return new TRSPenalty(Constants.DSQ);
            }

            if (pen.Equals("DNE"))
            {
                return new TRSPenalty(Constants.DNE);
            }

            if (pen.Equals("DND"))
            {
                return new TRSPenalty(Constants.DNE);
            }

            if (pen.Equals("RAF"))
            {
                return new TRSPenalty(Constants.RAF);
            }

            if (pen.Equals("RET"))
            {
                return new TRSPenalty(Constants.RAF);
            }

            if (pen.Equals("OCS"))
            {
                return new TRSPenalty(Constants.OCS);
            }

            if (pen.Equals("PMS"))
            {
                return new TRSPenalty(Constants.OCS);
            }

            if (pen.Equals("BFD"))
            {
                return new TRSPenalty(Constants.BFD);
            }

            if (pen.Equals("DGM"))
            {
                return new TRSPenalty(Constants.DGM);
            }

            if (pen.Equals("CNF"))
            {
                return new TRSPenalty(Constants.CNF);
            }

            if (pen.Equals("ZPG"))
            {
                return new TRSPenalty(Constants.ZFP);
            }

            if (pen.Equals("ZFP"))
            {
                return new TRSPenalty(Constants.ZFP);
            }

            if (pen.Equals("AVG"))
            {
                return new TRSPenalty(Constants.AVG);
            }

            if (pen.Equals("DNC"))
            {
                return new TRSPenalty(Constants.DNC);
            }

            if (pen.Equals("DNS"))
            {
                return new TRSPenalty(Constants.DNS);
            }

            if (pen.Equals("DNF"))
            {
                return new TRSPenalty(Constants.DNF);
            }

            if (pen.Equals("WTH"))
            {
                return new TRSPenalty(Constants.DNF);   // 2001            
            }

            if (pen.Equals("TLE"))
            {
                return new TRSPenalty(Constants.TLE);
            }

            if (pen.Equals("TLM"))
            {
                return new TRSPenalty(Constants.TLE);
            }

            if (pen.EndsWith("%"))
            {
                TRSPenalty pctPen = new TRSPenalty(Constants.SCP);
                try
                {
                    int pct = int.Parse(pen.Substring(0, (pen.Length - 1)));
                    pctPen.Percent = pct;
                }
                catch //(Exception dontcare)
                {
                }
                return pctPen;
            }

            if (pen.StartsWith("P"))
            {
                TRSPenalty pctPen = new TRSPenalty(Constants.SCP);
                try
                {
                    int pct = int.Parse(pen.Substring(1));
                    pctPen.Percent = pct;
                    return pctPen;
                }
                catch //(Exception dontcare)
                {
                }
            }

            if (pen.Equals("RDG") || pen.Equals("RDR") || pen.Equals("MAN") || pen.Equals("DPI"))
            {
                TRSPenalty penalty;
                if (pen.StartsWith("MAN"))
                {
                    penalty = new TRSPenalty(Constants.MAN);
                }
                else if (pen.StartsWith("DPI"))
                {
                    penalty = new TRSPenalty(Constants.DPI);
                }
                else //if (pen.StartsWith("RDG"))
                {
                    penalty = new TRSPenalty(Constants.RDG);
                }
                // assume is form "MAN/<pts>"
                try
                {
                    double pts = double.Parse(val, NumberFormatInfo.InvariantInfo);
                    penalty.Points = pts;
                }
                catch //(Exception e)
                {
                }
                return penalty;
            }

            if (pen.Equals("SCP") || pen.Equals("PCT"))
            {
                TRSPenalty penalty = new TRSPenalty(Constants.SCP);
                // assume is form "SCP/<pts>"
                try
                {
                    int pct = int.Parse(val);
                    penalty.Percent = pct;
                }
                catch //(Exception e)
                {
                }
                return penalty;
            }

            throw new System.ArgumentException("Unable to parse penalty, pen=" + pen);
        }
    }
}