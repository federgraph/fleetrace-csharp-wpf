//#define Sailtime

using System.Xml;
using System.Globalization;

namespace RiggVar.FR
{

    /* 
    Part 7 Race Organisation << | >> Appendix B Sailboard Racing Rules

    Contents | Index | Definitions

    APPENDIX A - SCORING
    See rule 88.3.

    A1 NUMBER OF RACES
    The number of races scheduled and the number required to be completed to
    constitute a series shall be stated in the sailing instructions.

    A2 SERIES SCORES
    Each boat's series score shall be the total of her race scores excluding her
    worst score. (The sailing instructions may make a different arrangement by
    providing, for example, that no score will be excluded, that two or more
    scores will be excluded, or that a specified number of scores will be
    excluded if a specified number of races are completed.) If a boat has two
    or more equal worst scores, the score( s) for the race( s) sailed earliest
    in the series shall be excluded. The boat with the lowest series score wins
    and others shall be ranked accordingly.

    A3 STARTING TIMES AND FINISHING PLACES
    The time of a boat's starting signal shall be her starting time, and the order
    in which boats finish a race shall determine their finishing places. However,
    when a handicap system is used a boat's elapsed time, corrected to the nearest
    second, shall determine her finishing place.

    A4 LOW POINT AND BONUS POINT SYSTEMS
    Most series are scored using either the Low Point System or the Bonus Point
    System. The Low Point System uses a boat's finishing place as her race score.
    The Bonus Point System benefits the first six finishers because of the greater
    difficulty in advancing from fourth place to third, for example, than from
    fourteenth place to thirteenth. The system chosen may be made to apply by
    stating in the sailing instructions that, for example, 'The series will be
    scored as provided in Appendix A of the racing rules using the
    [Low] [Bonus] Point System.'

    A4.1 Each boat starting and finishing and not thereafter retiring, being
    penalized or given redress shall be scored points as follows:


    Finishing place Low Point System Bonus Point System
    First 1 0
    Second 2 3
    Third 3 5.7
    Fourth 4 8
    Fifth 5 10
    Sixth 6 11.7
    Seventh 7 13
    Each place thereafter Add 1 point Add 1 point


    A4.2 A boat that did not start, did not finish, retired after finishing or was
    disqualified shall be scored points for the finishing place one more than the
    number of boats entered in the series. A boat penalized under rule 30.2 or 44.3
    shall be scored points as provided in rule 44.3(c).

    A5 SCORES DETERMINED BY THE RACE COMMITTEE
    A boat that did not start, comply with rule 30.2 or 30.3, or finish, or that
    takes a penalty under rule 44.3 or retires after finishing, shall be scored
    accordingly by the race committee without a hearing. Only the protest committee
    may take other scoring actions that worsen a boat's score.

    A6 CHANGES IN PLACES AND SCORES OF OTHER BOATS
    (a) If a boat is disqualified from a race or retires after finishing, each boat
    that finished after her shall be moved up one place.

    (b) If the protest committee decides to give redress by adjusting a boat's
    score, the scores of other boats shall not be changed unless the protest
    committee decides otherwise.

    A7 RACE TIES
    If boats are tied at the finishing line or if a handicap system is used and
    boats have equal corrected times, the points for the place for which the boats
    have tied and for the place(s) immediately below shall be added together and
    divided equally. Boats tied for a race prize shall share it or be given
    equal prizes.

    A8 SERIES TIES
    A8.1 If there is a series score tie between two or more boats, each boat's
    race scores shall be listed in order of best to worst, and at the first point(s)
    where there is a difference the tie shall be broken in favour of the boat(s)
    with the best score(s). No excluded scores shall be used.

    A8.2 If a tie remains between two boats, it shall be broken in favour of the
    boat that scored better than the other boat in more races. If more than two
    boats are tied, they shall be ranked in order of the number of times each boat
    scored better than another of the tied boats. No race for which a tied boat's
    score has been excluded shall be used. [Rule A8.2 is deleted and rule A8.3 is
    renumbered as A8.2. Change effective as from 1 June 2002.]

    A8.3 2 If a tie still remains between two or more boats, they shall be ranked
    in order of their scores in the last race. Any remaining ties shall be broken
    by using the tied boats' scores in the next-to-last race and so on until all
    ties are broken. These scores shall be used even if some of them are excluded
    scores.

    A9 RACE SCORES IN A SERIES LONGER THAN A REGATTA
    For a series that is held over a period of time longer than a regatta, a boat
    that came to the starting area but did not start, did not finish, retired after
    finishing or was disqualified shall be scored points for the finishing place
    one more than the number of boats that came to the starting area. A boat that
    did not come to the starting area shall be scored points for the finishing
    place one more than the number of boats entered in the series.

    A10 GUIDANCE ON REDRESS
    If the protest committee decides to give redress by adjusting a boat's score
    for a race, it is advised to consider scoring her

    (a) points equal to the average, to the nearest tenth of a point
    (0.05 to be rounded upward), of her points in all the races in the series
    except the race in question;

    (b) points equal to the average, to the nearest tenth of a point (0.05 to be
    rounded upward), of her points in all the races before the race in question;

    or

    (c) points based on the position of the boat in the race at the time of the
    incident that justified redress.

    A11 SCORING ABBREVIATIONS
    These abbreviations are recommended for recording the circumstances described:

    DNC Did not start; did not come to the starting area
    DNS Did not start (other than DNC and OCS)
    OCS Did not start; on the course side of the starting line and broke rule 29.1 or 30.1
    ZFP 20% penalty under rule 30. 2
    BFD Disqualification under rule 30.3
    SCP Took a scoring penalty under rule 44.3
    DNF Did not finish
    RAF Retired after finishing
    DSQ Disqualification
    DNE Disqualification not excludable under rule 88.3(b)
    RDG Redress given
    DGM Disqualification great misconduct
    DPI Descretionary penalty imposed


    --------------------------------------------------------------------------------

    Contents | Index | Definitions

    Part 7 Race Organisation << | >> Appendix B Sailboard Racing Rules
    
    */

    public enum TISAFPenaltyDSQ
    {
        NoDSQ,
        DSQ,
        DNE,
        RAF,
        OCS,
        BFD,
        DGM
    }
    public enum TISAFPenaltyNoFinish
    {
        NoFinishBlank,
        TLE,
        DNF,
        DNS,
        DNC
        //NoFinish
    }
    public enum TISAFPenaltyOther
    {
#if Sailtime
        TIM, //$0010; //time limit
#endif
        ZFP, //$0020;
        AVG, //$0040; //average
        SCP, //$0080; //scoring penalty, pct (percent) of finish position
        RDG, //$0100; //redress given
        MAN, //$0200; //manual
        CNF, //$0400; //check-in failure
#if Sailtime
        TMP, //$0800; //scoring time penalty, pct (percent) of time
#endif
        DPI  //$1000; //descretionary penalty imposed
    }

    public class TPenaltyISAF : TPenalty
    {
        public static string PenaltyDSQString(TISAFPenaltyDSQ o)
        {
            switch (o)
            {
                case TISAFPenaltyDSQ.NoDSQ: return "";
                case TISAFPenaltyDSQ.DSQ: return "DSQ";
                case TISAFPenaltyDSQ.DNE: return "DNE";
                case TISAFPenaltyDSQ.RAF: return "RAF";
                case TISAFPenaltyDSQ.OCS: return "OCS";
                case TISAFPenaltyDSQ.BFD: return "BFD";
                case TISAFPenaltyDSQ.DGM: return "DGM";
            }
            return string.Empty;
        }
        public static string PenaltyNoFinishString(TISAFPenaltyNoFinish o)
        {
            switch (o)
            {
                case TISAFPenaltyNoFinish.NoFinishBlank: return "";
                case TISAFPenaltyNoFinish.TLE: return "TLE";
                case TISAFPenaltyNoFinish.DNF: return "DNF";
                case TISAFPenaltyNoFinish.DNS: return "DNS";
                case TISAFPenaltyNoFinish.DNC: return "DNC";
                //case TISAFPenaltyNoFinish.NoFinish: return "NOFINISH";
            }
            return string.Empty;
        }
        public static string PenaltyOtherString(TISAFPenaltyOther o)
        {
            switch (o)
            {
#if Sailtime
                case TISAFPenaltyOther.TIM: return "TIM";
#endif
                case TISAFPenaltyOther.ZFP: return "ZFP";
                case TISAFPenaltyOther.AVG: return "AVG";
                case TISAFPenaltyOther.SCP: return "SCP";
                case TISAFPenaltyOther.RDG: return "RDG";
                case TISAFPenaltyOther.MAN: return "MAN";
                case TISAFPenaltyOther.CNF: return "CNF";
#if Sailtime
                case TISAFPenaltyOther.TMP: return "TMP";
#endif
                case TISAFPenaltyOther.DPI: return "DPI";
            }
            return string.Empty;
        }

        /*
        Penalty values are/should be used only internally to the program and NOT
        written out to persistent storage (that is done by string name). 
        Therefore it should be safe to reset the values, provided the orders are 
        not changed and the types of penalties keep their bit-boundaries straight.
        
        Disqualification penalties are the various ways a boat can be disqualified.
        These are not bitwise, as a boat can only be disqualified once.  
        But a boat can be disqualified with or without a valid finish.  
        So a boat can carry both a non-finish penalty and a disqualification penalty.
        See Penalty class.
        */
        public const int ISAF_DSQ = 0x0001; //Disqualification
        public const int ISAF_DNE = 0x0002; //Disqualification not excludable under rule 88.3(b)
        public const int ISAF_RAF = 0x0003; //Retired after finishing
        public const int ISAF_OCS = 0x0004; //Did not start; on the course side of the starting line and broke rule 29.1 or 30.1
        public const int ISAF_BFD = 0x0005; //Disqualification under rule 30.3 - black flag
        public const int ISAF_DGM = 0x0006; //Disqualification Great Miscoduct

        /*
        Other scoring penalties
        
        are the various other ways a boat can be "dinged".  This includes
        check-in penalties, redress, percentage penalties, etc.  
        These ARE Bit-wise penalties, and a boat can have more than one of them.  
        Also, a boat CAN have a non-finish penalty and "other" penalty, 
        but a boat may not have a disqualification penalty and "other" penalty. 
        See Penalty class.
        */
#if Sailtime
        //public const int available = 0x0008;
        public const int ISAF_TIM = 0x0010; //time limit
#endif
        public const int ISAF_ZFP = 0x0020; //20% penalty under rule 30. 2
        public const int ISAF_AVG = 0x0040; //average
        public const int ISAF_SCP = 0x0080; //Took a scoring penalty under rule 44.3
        public const int ISAF_RDG = 0x0100; //Redress given
        public const int ISAF_MAN = 0x0200; //manual
        public const int ISAF_CNF = 0x0400; //check-in failure
#if Sailtime
        public const int ISAF_TMP = 0x0800; //scoring time penalty, pct (percent) of time
#endif
        public const int ISAF_DPI = 0x1000; //descretionary penalty imposed

        //highest possible real finish
        public const int ISAF_HIGHEST_FINISH = 0x1FFF; //8191

        
        /*
        Non-finishing penalties,    
            
        show up in the finish order column
        and can get set as Finish "Positions" - means no finish recorded yet.
        Non-Finish penalty values are for boats that do not have a valid Finish.
        these are used in BOTH the FinishPosition class and in the Penalty class
        These are not-bitwise penalties and a boat cannot have more than one
        of these at a time. See FinishPosition class
        */
        //available = 0x2000; //8192
        //          = 0010000000000000
        //available = 0x4000;
        //          = 0100000000000000
        public const int ISAF_TLE = 0x6000; //an amount of time applied to elapsed time?
        //          = 0110000000000000
        public const int ISAF_DNF = 0x8000; //Did not finish
        //          = 0100000000000000
        public const int ISAF_DNS = 0xA000; //Did not start (other than DNC and OCS)
        //          = 1010000000000000
        public const int ISAF_DNC = 0xC000; //Did not start; did not come to the starting area
        //          = 1100000000000000
        public const int ISAF_NOFINISH = 0xE000; //
        //          = 1110000000000000 //57344


        //These masks break up the integer into the portions reserved for each penalty type.
        public const int ISAF_NO_PENALTY = 0x0000;
        public const int ISAF_DSQ_MASK = 0x0007;
        public const int ISAF_OTHER_MASK = 0x1FF8;
        public const int ISAF_NOFINISH_MASK = 0xE000; //57344

        private bool FDSQPending;

        public TEnumSet PenaltyOther = new TEnumSet(typeof(TISAFPenaltyOther));

        public TStringList SLPenalty = new TStringList();

        public TPenaltyISAF()
        {
            SLPenalty.QuoteChar = '#';            
        }
        public override void Assign(object source)
        {
            if (source.Equals(this))
            {
                return;
            }

            if (source is TPenaltyISAF)
            {
                TPenaltyISAF o = source as TPenaltyISAF;
                PenaltyDSQ = o.PenaltyDSQ;
                PenaltyNoFinish = o.PenaltyNoFinish;
                PenaltyOther.Assign(o.PenaltyOther);
                FDSQPending = o.FDSQPending;
                //
                Points = o.Points;
                Percent = o.Percent;
#if Sailtime
                TimePenalty = o.TimePenalty;
                Note = o.Note;
#endif
            }
            else
            {
                base.Assign(source);
            }
        }
        public override void Clear()
        {            
            PenaltyDSQ = TISAFPenaltyDSQ.NoDSQ;
            PenaltyNoFinish = TISAFPenaltyNoFinish.NoFinishBlank;
            PenaltyOther.Clear();
            FDSQPending = false;
            //
            Points = 0;
            Percent = 0;
#if Sailtime
            TimePenalty = 0;
            Note = "";
#endif
        }

        public static int TISAFPenaltyOtherLow = TEnumSet.Low(typeof(TISAFPenaltyOther));
        public static int TISAFPenaltyOtherHigh = TEnumSet.High(typeof(TISAFPenaltyOther));

        public override string ToString()
        {
            TISAFPenaltyOther po;
            string s;
            bool showPts = true;

            SLPenalty.Clear();

            if (PenaltyDSQ != TISAFPenaltyDSQ.NoDSQ)
            {
                SLPenalty.Add(PenaltyDSQString(PenaltyDSQ));
            }

            if (PenaltyNoFinish != TISAFPenaltyNoFinish.NoFinishBlank)
            {
                SLPenalty.Add(PenaltyNoFinishString(PenaltyNoFinish));
            }

            for (int i = TISAFPenaltyOtherLow; i <= TISAFPenaltyOtherHigh; i++)
            {
                po = (TISAFPenaltyOther) i; 
                if (PenaltyOther.IsMember(i))
                {                    
                    s = PenaltyOtherString(po);
                    if ((po == TISAFPenaltyOther.MAN) && showPts)
                    {
                        s += '/' + Points.ToString("0.0#", NumberFormatInfo.InvariantInfo);
                    }
                    else if ((po == TISAFPenaltyOther.RDG) && showPts)
                    {
                        s += '/' + Points.ToString("0.0#", NumberFormatInfo.InvariantInfo);
                    }
                    else if ((po == TISAFPenaltyOther.DPI) && showPts)
                    {
                        s += '/' + Points.ToString("0.0#", NumberFormatInfo.InvariantInfo);
                    }
                    else if ((po == TISAFPenaltyOther.SCP) && showPts)
                    {
                        s += '/' + Percent.ToString("0.0#", NumberFormatInfo.InvariantInfo) + '%';
                    }

#if Sailtime
                    else if ((po == TISAFPenaltyOther.TIM) && showPts)
                        s += '/' + RiggVar.JavaScore06.SailTime.ToString(TimePenalty);
                    else if ((po == TISAFPenaltyOther.TMP) && showPts)
                        s += '/' + Percent.ToString() + "%";
#endif

                    SLPenalty.Add(s);
                }
            }
            if (SLPenalty.Count > 0)
            {
                return SLPenalty.DelimitedText;
            }
            else
            {
                return string.Empty;
            }
        }
        //public override bool FromString(string Value)
        //{
        //    bool result = true;
        //    Value = Value.Replace('"', '#');
        //    SLPenalty.DelimitedText = Value;
        //    for (int i = 0; i < SLPenalty.Count; i++)
        //        result = result && Parse(SLPenalty[i]);
        //    return result;
        //}
        public string Invert(string Value)
        {
            string result = "ok";
            if (Value == null)
            {
                return result;
            }

            string pen = Value.ToLower();
            if (pen == "")
            {
                return result;
            }

            if (pen.Length < 1)
            {
                return result;
            }

            if (Value[0] == '-')
            {
                if (pen == "-dsq")
                {
                    result = TPenaltyISAF.PenaltyDSQString(PenaltyDSQ);
                }
                else if (pen == "-f")
                {
                    result = TPenaltyISAF.PenaltyNoFinishString(PenaltyNoFinish);
                }
#if Sailtime
                else if (pen == "-tim") result = TPenaltyISAF.PenaltyOtherString(TISAFPenaltyOther.TIM);
#endif
                else if (pen == "-zfp")
                {
                    result = TPenaltyISAF.PenaltyOtherString(TISAFPenaltyOther.ZFP);
                }
                else if (pen == "-avg")
                {
                    result = TPenaltyISAF.PenaltyOtherString(TISAFPenaltyOther.AVG);
                }
                else if (pen == "-scp")
                {
                    result = TPenaltyISAF.PenaltyOtherString(TISAFPenaltyOther.SCP);
                }
                else if (pen == "-rdg")
                {
                    result = TPenaltyISAF.PenaltyOtherString(TISAFPenaltyOther.RDG);
                }
                else if (pen == "-man")
                {
                    result = TPenaltyISAF.PenaltyOtherString(TISAFPenaltyOther.MAN);
                }
                else if (pen == "-cnf")
                {
                    result = TPenaltyISAF.PenaltyOtherString(TISAFPenaltyOther.CNF);
                }
#if Sailtime
                else if (pen == "-tmp") result = TPenaltyISAF.PenaltyOtherString(TISAFPenaltyOther.TMP);
#endif
                else if (pen == "-dpi")
                {
                    result = TPenaltyISAF.PenaltyOtherString(TISAFPenaltyOther.DPI);
                }
            }
            else
            {
                pen = Utils.Copy(pen, 1, 3);

                //disqualification penalty enum
                if (pen == "dsq")
                {
                    result = "-dsq";
                }
                else if (pen == "dne")
                {
                    result = "-dsq";
                }
                else if (pen == "raf")
                {
                    result = "-dsq";
                }
                else if (pen == "ocs")
                {
                    result = "-dsq";
                }
                else if (pen == "bfd")
                {
                    result = "-dsq";
                }
                else if (pen == "dgm")
                {
                    result = "-dsq";
                }

                //nofinish penalty enum
                else if (pen == "tle")
                {
                    result = "-f";
                }
                else if (pen == "dnf")
                {
                    result = "-f";
                }
                else if (pen == "dns")
                {
                    result = "-f";
                }
                else if (pen == "dnc")
                {
                    result = "-f";
                }

                //other (penalties set
#if Sailtime
                else if (pen == "tim") result = "-tim";
#endif
                else if (pen == "zfp")
                {
                    result = "-zfp";
                }
                else if (pen == "avg")
                {
                    result = "-avg";
                }
                else if (pen == "scp")
                {
                    result = "-scp";
                }
                else if (pen == "rdg")
                {
                    result = "-rdg";
                }
                else if (pen == "man")
                {
                    result = "-man";
                }
                else if (pen == "cnf")
                {
                    result = "-cnf";
                }
#if Sailtime
                else if (pen == "tmp") result = "-tmp";
#endif
                else if (pen == "dpi")
                {
                    result = "-dpi";
                }
            }

            return result;
        }
        public override bool Parse(string Value)
        {
            bool result = true;

            string val = string.Empty;
            string s = Value.ToLower();
            string pen = s;

            int i = Utils.Pos("/", s); //javascore delimiter, problem with xml-generator
            if (i < 1)
            {
                i = Utils.Pos(":", s); //alternative, normalized form
            }

            if (i > 0)
            {
                pen = Utils.Copy(s, 1, i-1).Trim();
                val = Utils.Copy(s, i+1, s.Length).Trim();
            }

            if (pen == string.Empty)
            {
                return result;
            }
            else if (pen == "ok")
            {
                Clear();
            }
            else if (pen == "*")
            {
                FDSQPending = true;
            }

            //disqualification penalty enum
            else if (pen == "-dsq")
            {
                PenaltyDSQ = TISAFPenaltyDSQ.NoDSQ;
            }
            else if (pen == "dsq")
            {
                PenaltyDSQ = TISAFPenaltyDSQ.DSQ;
            }
            else if (pen == "dne")
            {
                PenaltyDSQ = TISAFPenaltyDSQ.DNE;
            }
            else if (pen == "raf")
            {
                PenaltyDSQ = TISAFPenaltyDSQ.RAF;
            }
            else if (pen == "ocs")
            {
                PenaltyDSQ = TISAFPenaltyDSQ.OCS;
            }
            else if (pen == "bfd")
            {
                PenaltyDSQ = TISAFPenaltyDSQ.BFD;
            }
            else if (pen == "dgm")
            {
                PenaltyDSQ = TISAFPenaltyDSQ.DGM;
            }

            //nofinish penalty enum
            else if (pen == "-f")
            {
                PenaltyNoFinish = TISAFPenaltyNoFinish.NoFinishBlank;
            }
            else if (pen == "tle")
            {
                PenaltyNoFinish = TISAFPenaltyNoFinish.TLE;
            }
            else if (pen == "dnf")
            {
                PenaltyNoFinish = TISAFPenaltyNoFinish.DNF;
            }
            else if (pen == "dns")
            {
                PenaltyNoFinish = TISAFPenaltyNoFinish.DNS;
            }
            else if (pen == "dnc")
            {
                PenaltyNoFinish = TISAFPenaltyNoFinish.DNC;
            }

            //other penalties set, see also below for rdg, man, dpi etc.
            else if (pen == "zfp")
            {
                PenaltyOther.Include((int)TISAFPenaltyOther.ZFP);
            }
            else if (pen == "avg")
            {
                PenaltyOther.Include((int)TISAFPenaltyOther.AVG);
            }
            else if (pen == "cnf")
            {
                PenaltyOther.Include((int)TISAFPenaltyOther.CNF);
            }

#if Sailtime
            else if (pen == "-tim") PenaltyOther.Exclude((int)TISAFPenaltyOther.TIM);
#endif

            else if (pen == "-zfp")
            {
                PenaltyOther.Exclude((int)TISAFPenaltyOther.ZFP);
            }
            else if (pen == "-avg")
            {
                PenaltyOther.Exclude((int)TISAFPenaltyOther.AVG);
            }
            else if (pen == "-scp")
            {
                PenaltyOther.Exclude((int)TISAFPenaltyOther.SCP);
            }
            else if (pen == "-rdg")
            {
                PenaltyOther.Exclude((int)TISAFPenaltyOther.RDG);
            }
            else if (pen == "-man")
            {
                PenaltyOther.Exclude((int)TISAFPenaltyOther.MAN);
            }
            else if (pen == "-cnf")
            {
                PenaltyOther.Exclude((int)TISAFPenaltyOther.CNF);
            }
#if Sailtime
            else if (pen == "-tmp") PenaltyOther.Exclude((int)TISAFPenaltyOther.TMP);
#endif
            else if (pen == "-dpi")
            {
                PenaltyOther.Exclude((int)TISAFPenaltyOther.DPI);
            }

            //all of the rest should have <pen>/<number>

            else if (val.Length > 0 && val[val.Length-1] == '%')
            {
                PenaltyOther.Include((int)TISAFPenaltyOther.SCP);
                Percent = Utils.StrToIntDef(Utils.Copy(val, 1, pen.Length-1), 0);
            }

            else if (pen[0] == 'p')
            {
                PenaltyOther.Include((int)TISAFPenaltyOther.SCP);
                Percent = Utils.StrToIntDef(Utils.Copy(val, 0, val.Length), 0);
            }

#if Sailtime
            else if (pen == "tim")
            {
                PenaltyOther.Include((int)TISAFPenaltyOther.TIM);
                TimePenalty = RiggVar.JavaScore06.SailTime.forceToLong(val);
            }
#endif

            else if ((pen == "rdg") || (pen == "rdr") || (pen == "man") || (pen == "dpi"))
            {
                if (Utils.Copy(pen, 1, 3) == "man")
                {
                    PenaltyOther.Include((int)TISAFPenaltyOther.MAN);
                }
                else if (Utils.Copy(pen, 1, 3) == "dpi")
                {
                    PenaltyOther.Include((int)TISAFPenaltyOther.DPI);
                }
                else
                {
                    PenaltyOther.Include((int)TISAFPenaltyOther.RDG);
                }

                //assume is form "MAN/<pts>"
                val = val.Replace(',', '.');
                Points = Utils.StrToFloatDef(val, 0);
            }

#if Sailtime
            else if ((pen == "tmp"))
            {
                PenaltyOther.Include((int)TISAFPenaltyOther.TMP);
                //assume is form "MAN/<pts>"
                Percent = Utils.StrToIntDef(val, 0);
            }
#endif

            else if ((pen == "scp") || (pen == "pct"))
            {
                PenaltyOther.Include((int)TISAFPenaltyOther.SCP);
                //assume is form "MAN/<pts>"
                Percent = Utils.StrToIntDef(val, 0);
            }

            return result;
        }
        protected override bool GetIsDSQPending()
        {
            return FDSQPending;
        }
        protected override bool GetIsOK()
        {
            return (PenaltyDSQ == TISAFPenaltyDSQ.NoDSQ)
                && (PenaltyNoFinish == TISAFPenaltyNoFinish.NoFinishBlank)
                && (PenaltyOther.IsEmpty);
            //&& (FDSQPending == false);
        }
        protected override bool GetIsOut()
        {
            return !IsOK;
        }
        protected override void SetIsDSQPending(bool Value)
        {
            FDSQPending = Value;
        }
        protected override int GetAsInteger()
        {
            int result = 0;

            switch (PenaltyDSQ)
            {
                case TISAFPenaltyDSQ.DSQ: result = result | ISAF_DSQ; break; //$0001; //Disqualification
                case TISAFPenaltyDSQ.DNE: result = result | ISAF_DNE; break; //$0002; //Disqualification not excludable under rule 88.3(b)
                case TISAFPenaltyDSQ.RAF: result = result | ISAF_RAF; break; //$0003; //Retired after finishing
                case TISAFPenaltyDSQ.OCS: result = result | ISAF_OCS; break; //$0004; //Did not start; on the course side of the starting line and broke rule 29.1 or 30.1
                case TISAFPenaltyDSQ.BFD: result = result | ISAF_BFD; break; //$0005; //Disqualification under rule 30.3 - black flag
                case TISAFPenaltyDSQ.DGM: result = result | ISAF_DGM; break; //$0006;
            }
            switch (PenaltyNoFinish)
            {
                case TISAFPenaltyNoFinish.TLE: result = result | ISAF_TLE; break; //$6000; //an amount of time applied to elapsed time?
                case TISAFPenaltyNoFinish.DNF: result = result | ISAF_DNF; break; //$8000; //Did not finish
                case TISAFPenaltyNoFinish.DNS: result = result | ISAF_DNS; break; //$A000; //Did not start (other than DNC and OCS)
                case TISAFPenaltyNoFinish.DNC: result = result | ISAF_DNC; break; //$C000; //Did not start; did not come to the starting area
                case TISAFPenaltyNoFinish.NoFinishBlank: break;
            }
            for (int i = TISAFPenaltyOtherLow; i <= TISAFPenaltyOtherHigh; i++)
            {
                if (PenaltyOther.IsMember(i))
                {
                    TISAFPenaltyOther other = (TISAFPenaltyOther) i;
                    switch (other)
                    {
#if Sailtime
                        case TISAFPenaltyOther.TIM: result = result | ISAF_TIM; break; //$0010; //time limit
#endif
                        case TISAFPenaltyOther.ZFP: result = result | ISAF_ZFP; break; //$0020; //20% penalty under rule 30. 2
                        case TISAFPenaltyOther.AVG: result = result | ISAF_AVG; break; //$0040; //average
                        case TISAFPenaltyOther.SCP: result = result | ISAF_SCP; break; //$0080; //Took a scoring penalty under rule 44.3
                        case TISAFPenaltyOther.RDG: result = result | ISAF_RDG; break; //$0100; //Redress given
                        case TISAFPenaltyOther.MAN: result = result | ISAF_MAN; break; //$0200; //manual
                        case TISAFPenaltyOther.CNF: result = result | ISAF_CNF; break; //$0400; //check-in failure
#if Sailtime
                        case TISAFPenaltyOther.TMP: result = result | ISAF_TMP; break; //$0800; //scoring time penalty, pct (percent) of time
#endif
                        case TISAFPenaltyOther.DPI: result = result | ISAF_DPI; break; //$1000;
                        default:
                            break;
                    }
                }
            }

            return result;
        }
        protected override void SetAsInteger(int Value)
        {
            Clear();
            if (Value == ISAF_NO_PENALTY)
            {
                return;
            }

            int i = Value & ISAF_DSQ_MASK;
            switch (i)
            {
                case ISAF_DSQ: PenaltyDSQ = TISAFPenaltyDSQ.DSQ; break;
                case ISAF_DNE: PenaltyDSQ = TISAFPenaltyDSQ.DNE; break;
                case ISAF_RAF: PenaltyDSQ = TISAFPenaltyDSQ.RAF; break;
                case ISAF_OCS: PenaltyDSQ = TISAFPenaltyDSQ.OCS; break;
                case ISAF_BFD: PenaltyDSQ = TISAFPenaltyDSQ.BFD; break;
                case ISAF_DGM: PenaltyDSQ = TISAFPenaltyDSQ.DGM; break;
                default:
                    break;
            }

            i = Value & ISAF_NOFINISH_MASK;
            switch (i)
            {
                case ISAF_TLE: PenaltyNoFinish = TISAFPenaltyNoFinish.TLE; break;
                case ISAF_DNF: PenaltyNoFinish = TISAFPenaltyNoFinish.DNF; break;
                case ISAF_DNS: PenaltyNoFinish = TISAFPenaltyNoFinish.DNS; break;
                case ISAF_DNC: PenaltyNoFinish = TISAFPenaltyNoFinish.DNC; break;
                default:  PenaltyNoFinish = TISAFPenaltyNoFinish.NoFinishBlank; break;
            }

            i = Value & ISAF_OTHER_MASK;
#if Sailtime
            if ((i & ISAF_TIM) == ISAF_TIM) PenaltyOther.Include((int)TISAFPenaltyOther.TIM);
#endif
            if ((i & ISAF_ZFP) == ISAF_ZFP)
            {
                PenaltyOther.Include((int)TISAFPenaltyOther.ZFP);
            }

            if ((i & ISAF_AVG) == ISAF_AVG)
            {
                PenaltyOther.Include((int)TISAFPenaltyOther.AVG);
            }

            if ((i & ISAF_SCP) == ISAF_SCP)
            {
                PenaltyOther.Include((int)TISAFPenaltyOther.SCP);
            }

            if ((i & ISAF_RDG) == ISAF_RDG)
            {
                PenaltyOther.Include((int)TISAFPenaltyOther.RDG);
            }

            if ((i & ISAF_MAN) == ISAF_MAN)
            {
                PenaltyOther.Include((int)TISAFPenaltyOther.MAN);
            }

            if ((i & ISAF_CNF) == ISAF_CNF)
            {
                PenaltyOther.Include((int)TISAFPenaltyOther.CNF);
            }
#if Sailtime
            if ((i & ISAF_TMP) == ISAF_TMP) PenaltyOther.Include((int)TISAFPenaltyOther.TMP);
#endif
            if ((i & ISAF_RDG) == ISAF_DPI)
            {
                PenaltyOther.Include((int)TISAFPenaltyOther.DPI);
            }
        }
        public TISAFPenaltyDSQ PenaltyDSQ { get; private set; }
        public TISAFPenaltyNoFinish PenaltyNoFinish { get; private set; }
        public double Points { get; set; }
        public int Percent { get; set; }
#if Sailtime
        public long TimePenalty { get; set; }
        public string Note { get; set; }
#endif
        public void WriteXml(XmlWriter xw)
        {
            xw.WriteElementString("PenaltyDSQ", PenaltyDSQString(PenaltyDSQ));
            xw.WriteElementString("PenaltyNoFinish", PenaltyNoFinishString(PenaltyNoFinish));
            xw.WriteElementString("DSQPending", Utils.BoolStr[FDSQPending]);

            xw.WriteStartElement("PenaltyOther");
            TISAFPenaltyOther po;
            string s;
            for (int i = TISAFPenaltyOtherLow; i <= TISAFPenaltyOtherHigh; i++)
            {
                po = (TISAFPenaltyOther) i; 
                s = PenaltyOtherString(po);
                if (s == "")
                {
                    continue;
                }

                if (PenaltyOther.IsMember(i))
                {                    
                    xw.WriteAttributeString(s, Utils.BoolStr[true]);
                }
                else
                {
                    xw.WriteAttributeString(s, Utils.BoolStr[false]);
                }
            }
            xw.WriteEndElement();

            xw.WriteElementString("Points", Points.ToString());
            xw.WriteElementString("Percent", Percent.ToString());
#if Sailtime
            xw.WriteElementString("TimePenalty", TimePenalty.ToString());
            xw.WriteElementString("Note", Note);
#else
            xw.WriteElementString("TimePenalty", "0");
            xw.WriteElementString("Note", "");
#endif
        }
    }
}
