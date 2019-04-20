using System;

namespace RiggVar.FR
{

    // TimeConst is from other sport, not sailing, but I will leave it in here.
    public class TimeConst
    {
        public static int cRun1 = 1;
        public static int cRun2 = 2;
        public static int cRun3 = 3;
        public static int eaDSQGate = 1;
        public static int eaStatus = 2;
        public static int eaOTime = 3;

        public static int TimeStatus_None = 0;  //no time present, may still arrive
        public static int TimeStatus_Auto = 1; //time presentn, automatically set
        public static int TimeStatus_Man = 2; //time present, manually set
        public static int TimeStatus_TimePresent = 3; //time present, don't know how
        public static int TimeStatus_Penalty = 4; //penalty time, automatically set

        public static int TimeNull = int.MaxValue;

        public const int channel_QA_ST = 0;
        public const int channel_QA_IT = 1;
        public const int channel_QA_FT = 2;
        public const int channel_QB_ST = 3;
        public const int channel_QB_IT = 4;
        public const int channel_QB_FT = 5;
        public const int channel_QC_ST = 6;
        public const int channel_QC_IT = 7;
        public const int channel_QC_FT = 8;
        
        //public static string [] TimeStatusStrings  = { "", "Auto", "Man", "Time", "Pen" };
        public static TimeStatusStruct TimeStatusStrings;
    }    

    public struct TimeStatusStruct
    {
        public string this [TTimeStatus status]
        {
            get 
            {
                switch (status)
                {
                    case TTimeStatus.tsNone: return "";
                    case TTimeStatus.tsAuto: return "Auto";
                    case TTimeStatus.tsMan: return "Man";
                    case TTimeStatus.tsTimePresent: return "Time";
                    case TTimeStatus.tsPenalty: return "Pen";
                    default: return string.Empty;
                }
            }
        }
    }

    public enum TTimeStatus 
    {
        tsNone,
        tsAuto,
        tsMan,
        tsTimePresent,
        tsPenalty
    }


    public class TTimeSplit
    {
        private string FormatNumber2(int aNumber)
        {
            return aNumber < 10 ? "0" + aNumber.ToString() : aNumber.ToString();
        }

        private string FormatNumber4(int aNumber)
        {
            //result := Format('%.4d', [aNumber]); //this is Pascal code
            return aNumber.ToString("D4");
        }

        public void Split (out bool minus, out int h, out int m, out int s, out int ss)
        {
            int TimeVal = Value;
            minus = false;
            if (TimeVal < 0)
            {
                minus = true;
                TimeVal = Math.Abs(TimeVal);
            }
            ss = Convert.ToInt32(TimeVal % 10000);
            TimeVal = TimeVal / 10000;
            s = Convert.ToInt32(TimeVal % 60);
            TimeVal = TimeVal / 60;
            m = Convert.ToInt32(TimeVal % 60);
            TimeVal = TimeVal / 60;
            h = Convert.ToInt32(TimeVal);
        }

        public void SplitToStrings(out string sSign, out string sHour, out string sMin, out string sSec, out string sSubSec)
        {
            Split(out bool minus, out int h, out int m, out int s, out int ss);

            sSign = minus ? "M" : "P";

            sHour = FormatNumber2(h);
            sMin = FormatNumber2(m);
            sSec = FormatNumber2(s);
            sSubSec = FormatNumber2(ss / 100); // 2 subsecond digits
        }

        public string AsString()
        {
            string result = string.Empty;
            int m, s, ss;
            Split(out bool minus, out int h, out m, out s, out ss);
            ss = ss / 100;
            result =
                FormatNumber2(h) + ":" +
                FormatNumber2(m) + ":" +
                FormatNumber2(s) + "." +
                FormatNumber2(ss);
            if (minus)
            {
                result = "-" + result;
            }

            return result;
        }

        public string AsString3()
        {
            string result = string.Empty;
            int m, s, ss;
            Split(out bool minus, out int h, out m, out s, out ss);
            ss = ss / 100;
            result =
                FormatNumber2(h) + ":" +
                FormatNumber2(m) + ":" +
                FormatNumber2(s) + "." +
                FormatNumber2(ss) + "0";
            if (minus)
            {
                result = "-" + result;
            }

            return result;
        }

        public string AsString4()
        {
            string result = string.Empty;
            int m, s, ss;
            Split(out bool minus, out int h, out m, out s, out ss);
            result =
                FormatNumber2(h) + ":" +
                FormatNumber2(m) + ":" +
                FormatNumber2(s) + "." +
                FormatNumber4(ss);
            if (minus)
            {
                result = "-" + result;
            }

            return result;
        }

        public int Value { get; set; }

        public string Hour
        {
            get 
            {
                Split(out bool minus, out int h, out int m, out int s, out int ss);
                return FormatNumber2(h);
            }
        }

        public string Min
        {
            get 
            {
                Split(out bool minus, out int h, out int m, out int s, out int ss);
                return FormatNumber2(m);
            } 
        }

        public string Sec
        {
            get 
            {
                Split(out bool minus, out int h, out int m, out int s, out int ss);
                return FormatNumber2(s);
            }
        }

        public string SubSec
        {
            get 
            {
                Split(out bool minus, out int h, out int m, out int s, out int ss);
                return FormatNumber2(ss);
            }
        }

        public string Sign => Value < 0 ? "M" : "P";
    }

    public class TNTime : TBOPersistent
    {
        protected int FTime;
        protected TTimeStatus FStatus;
        private int FDisplayPrecision = 2;
        private readonly int FPrecision = 2;
        
        public TNTime()
        {
        }
        
        private string EnsureLeadingZero(int Value)
        {
            // put a 0 in front of one digit numbers
            if (Value < 10)
            {
                return "0" + Value.ToString();
            }
            else
            {
                return Value.ToString();
            }
        }

        private string LeadingZeros(int anz, string sIn)
        {
            //ensure that the string has the right number of digits (anz = Anzahl = Count)
            string hs; //help string
            hs = string.Empty;
            //make help string of zeros, which is long enough
            for (int i = 1; i <= anz; i++)
            {
                hs = hs + "0";
            }
            //put it in front
            hs = hs + sIn;
            //and take/copy from the right
            return Utils.Copy(hs, hs.Length - anz + 1, anz);
        }

        private string CheckTime(string TimeStr)
        {
            int dotpos;
            string TimeStr2;
            int lastdd;
            string sNachkomma;
            string sNachkommaChecked;

            if (TimeStr == string.Empty)
            {
                return string.Empty;
            }
            TimeStr2 = string.Empty;
            //find first dot
            dotpos = Utils.Pos(".", TimeStr);
            lastdd = dotpos;
            //also check for comma, just in case 
            dotpos = Utils.Pos(",", TimeStr);

            if ((lastdd == 0) && (dotpos == 0))
            {
                TimeStr = TimeStr + ".0";
                dotpos = Utils.Pos(".", TimeStr);
            }
            if ((lastdd == 0) && (dotpos > 0))
            {
                lastdd = dotpos; //comma
            }
            else if (lastdd > 0)
            {
                dotpos = lastdd; //dot
            }

            // first check characters before the decimal separator (comma or dot)
            for (int i = dotpos - 1; i >= 1; i--)
            {
                //check characters
                char c = TimeStr[i-1];
                if (c == ':')
                {
                    if (lastdd > 0) //decimal char already found
                    {
                        if (lastdd - i < 3) // we want 2 digits
                        {
                            TimeStr2 = "0" + TimeStr2; //use leading zero
                        }
                    }

                    lastdd = i; //save position
                }
                else if ((c >= '0') && (c <= '9'))
                {
                    //if it was a number (at least)
                    TimeStr2 = TimeStr[i-1] + TimeStr2; //use it
                }
            }
            TimeStr2 = LeadingZeros(6, TimeStr2);

            // then check characters after the decimal (after comma or dot)
            sNachkommaChecked = string.Empty;
            sNachkomma = Utils.Copy(TimeStr, dotpos, TimeStr.Length);
            for (int i = 2; i <= sNachkomma.Length; i++)
            {
                char c = sNachkomma[i-1];
                if ((c >= '0') && (c <= '9'))
                {
                    sNachkommaChecked = sNachkommaChecked + sNachkomma[i-1];
                }
            }
            if (sNachkommaChecked == "")
            {
                sNachkommaChecked = "0";
            }

            return TimeStr2 + "." + sNachkommaChecked;
        }

        protected string ConvertTimeToStr3(int TimeVal)
        {
            // Format (-)HH:MM:SS.mm
            // without leading hours/minutes: 674326 -> '1:07.43'
            // without leading zeros
            // with no more than 2 digits after the decimal
            int hours, min, sec, msec;
            string temp;
            bool minus = false;
  
            if (TimeVal < 0)
            {
                minus = true;
                TimeVal = Math.Abs(TimeVal);
            }

            msec = Convert.ToInt32(TimeVal % 10000);
            TimeVal = TimeVal / 10000;
            sec = Convert.ToInt32(TimeVal % 60);
            TimeVal = TimeVal / 60;
            min = Convert.ToInt32(TimeVal % 60);
            TimeVal = TimeVal / 60;
            hours = Convert.ToInt32(TimeVal);

            // skip what is not present (hour, minute)
            temp = string.Empty;
            if (hours > 0)
            {
                temp = temp + EnsureLeadingZero(hours) + ":";
            }

            if (min + hours > 0)
            {
                temp = temp + EnsureLeadingZero(min) + ":";
            }

            if (sec + min + hours > 0)
            {
                temp = temp + EnsureLeadingZero(sec);
            }
            else
            {
                temp = temp + "00"; //never start with decimal, always print seconds
            }

            // append a 'template' with four digits after the decimal
            // later we will only use the part up to the desired precision
            temp = temp + "." + LeadingZeros(4, msec.ToString());

            // skip a leading zero
            if (temp[0] == '0')
            {
                temp = Utils.Copy(temp, 2, temp.Length);
            }

            if (minus)
            {
                temp = "-" + temp;
            }

            // cut off third an fourth digit after the decimal
            temp = Utils.Copy(temp, 1, temp.Length - (4-FDisplayPrecision));

            return temp;
        }

        private int ConvertStrToTime1(string TimeStr)
        {
            // returns time as count of hundredth of a second
            // example: 1:02.03
            // 00010200 --> 6200

            // note that ConvertStrToTime2 will only pass the pre-decimal part as param to this function

            int i, j;
            int k;

            if (TimeStr[2] == ':') // ignore colon
            {
                k = 1;
            }
            else
            {
                k = 0;
            }

            i = Utils.StrToIntDef(Utils.Copy(TimeStr, 1, 2), 0); // hours
            j = i;
            i = Utils.StrToIntDef(Utils.Copy(TimeStr, 3 + k, 2), 0); // minutes
            j = j * 60 + i;
            i = Utils.StrToIntDef(Utils.Copy(TimeStr, 5 + k * 2, 2), 0); // seconds
            j = j * 60 + i;
            i = Utils.StrToIntDef(Utils.Copy(TimeStr, 7 + k * 3, 2), 0); // hundredth of a second
            j = j * 100 + i;
            return j;
        }

        private int ConvertStrToTime2(string TimeStr)
        {
            int V; // part before decimal
            int N; // part after decimal
            int pos1, pos2, posi;
            string str;

            pos1 = Utils.Pos(".", TimeStr);
            pos2 = Utils.Pos(",", TimeStr);
            posi = pos1 + pos2;
            if (posi > 0)
            {
                // V
                str = "000000" + Utils.Copy(TimeStr, 1, posi - 1) + "00"; // ensure there are enough digits
                str = Utils.Copy(str, str.Length - 7, 8); // take last 8 characters
                V = ConvertStrToTime1(str) * 100; // convert those
                // V has now a precision of 4

                // N
                str = Utils.Copy(TimeStr, 8, TimeStr.Length);
                if (str.Length == 0)
                {
                    str = "0";
                }
                // round to desired precision                
                double d = Utils.StrToIntDef(str, 0) * Math.Pow(10, FPrecision - str.Length);
                N = Convert.ToInt32(Math.Round(d)); //Convert.ToInt64(Math.Round(d));
                // but internally always keep 4 digits after the decimal
                for (int i = FPrecision; i <= 3; i++)
                {
                    N = N * 10;
                }

                return V + N;
            }
            else
            {
                return 0;
            }
        }
        protected int Time
        {
            get => FTime; set => FTime = value;
        }

        public override void Assign(object source)
        {
            if (source is TNTime)
            {
                TNTime o = (TNTime) source;
                AsInteger = o.AsInteger;
            }
            else
            {
                base.Assign(source);
            }
        }

        public virtual void Clear()
        {
            Status = TTimeStatus.tsNone;
            Time = 0;
        }

        public bool Parse(string Value)
        {
            if (!IsValidTimeString(Value))
            {
                return false;
            }

            Status = TTimeStatus.tsAuto;
            Time = ConvertStrToTime2(CheckTime(Value));
            return true;
        }

        public override string ToString()
        {
            if ((FTime == 0) && (!TimePresent))
            {
                return string.Empty;
            }
            else if (FTime == 0)
            {
                switch (DisplayPrecision)
                {
                    case 1: return "0.0";
                    case 2: return "0.00";
                    case 3: return "0.000";
                    case 4: return "0.0000";
                    default: return "0";
                }
            }
            else
            {
                return ConvertTimeToStr3(FTime);
            }
        }

        public void UpdateQualiTimeBehind(int aBestTime, int aOTime)
        {
            if (aBestTime == TimeConst.TimeNull)
            {
                AsInteger = TimeConst.TimeNull;
            }
            else if (aOTime > 0)
            {
                AsInteger =  aOTime - aBestTime;
            }
            else
            {
                AsInteger = TimeConst.TimeNull;
            }
        }

        public virtual bool IsValidTimeString(string TimeStr)
        {
            return StaticIsValidTimeString(TimeStr);
        }

        public static bool StaticIsValidTimeString(string TimeStr)
        {
            if (TimeStr == string.Empty)
            {
                return false;
            }

            // find a dot, this is the default
            int dotpos = TimeStr.LastIndexOf('.'); //decimal point position
            int lastcolon = dotpos;

            // find last comma
            dotpos = TimeStr.LastIndexOf(',');

            // we must have a decimal point and a valid dotpos
            if ((lastcolon == -1) && (dotpos == -1))
            {
                TimeStr = TimeStr + ".0";
                dotpos = TimeStr.LastIndexOf('.');
            }

            // lastcolon and dotpos will be set to same value
            if ((lastcolon == -1) && (dotpos > 0))
            {
                lastcolon = dotpos; //decimal was comma
            }
            else if (lastcolon > 0)
            {
                dotpos = lastcolon; //decimal was dot
            }

            // check the part in front of the decimal:
            // start at the decimal and read towards the left
            // first read seconds, then minutes, then hours
            // remove colons as you go
            for (int i = dotpos - 1; i >= 0; i--)
            {
                char c = TimeStr[i];
                if (c == ':')
                { 
                    // ok, skip the colon and continue, all is good
                }
                else if ((c >= '0') && (c <= '9'))
                {
                     // ok, skip over digit and continue, all is good
                }
                else
                {
                    return false; // no, this is not valid
                }
            }

            // check the part after the decimal
            // start at the decimal and read towards the right
            string sNachkomma = Utils.Copy(TimeStr, dotpos+2, TimeStr.Length);
            for (int i = 0; i < sNachkomma.Length; i++)
            {
                char c = sNachkomma[i];
                if ((c < '0') || (c > '9'))
                {
                    return false;
                }
            }
            return true;
        }

        public virtual string StatusAsString()
        {
            return TimeConst.TimeStatusStrings[Status];
        }

        public TTimeSplit TimeSplit
        { 
            get 
            {
                TTimeSplit o = new TTimeSplit();
                o.Value = Time;
                return o;
            }
        }

        public bool TimePresent
        { 
            get 
            {
                return (FStatus != TTimeStatus.tsNone);
            }
        }

        public string AsString
        {
            get => ToString();
            set => Parse(value);
        }

        public int DisplayPrecision
        {
            get => FDisplayPrecision;
            set
            {
                if ((DisplayPrecision > 0) && (DisplayPrecision <= 4))
                {
                    FDisplayPrecision = value;
                }
            }
        }

        public virtual int AsInteger
        {
            get => FStatus == TTimeStatus.tsNone ? 0 : FTime;
            set
            {
                if (value == TimeConst.TimeNull)
                {
                    Status = TTimeStatus.tsNone;
                    Time = 0;
                }
                else
                {
                    Status = TTimeStatus.tsAuto;
                    Time = value;
                }
            }
        }

        public TTimeStatus Status
        {
            get
            {
                // Assert(FTime >= 0, "time cannot be negative");
                if (FTime < 0)
                {
                    FTime = 0;
                    FStatus = TTimeStatus.tsNone;
                }
                return FStatus;
            }
            set => FStatus = value;
        }
    }

    public class TQTime : TNTime {}

    public class TPTime : TNTime
    {
        public override int AsInteger
        {
            get => FStatus == TTimeStatus.tsNone ? -1 : FTime;
            set
            {
                if ((value == TimeConst.TimeNull) || (value < 0))
                {
                    Status = TTimeStatus.tsNone;
                    Time = 0;
                }
                else
                {
                    Status = TTimeStatus.tsAuto;
                    Time = value;
                }
            }
        }

        public override string ToString()
        {
            if (FTime < 0)
            {
                return string.Empty;
            }
            else if ((FTime == 0) && (!TimePresent))
            {
                return string.Empty;
            }
            else if (FTime == 0)
            {
                switch (DisplayPrecision)
                {
                    case 1: return "0.0";
                    case 2: return "0.00";
                    case 3: return "0.000";
                    case 4: return "0.0000";
                    default: return "0";
                }
            }
            else
            {
                return ConvertTimeToStr3(FTime);
            }
        }

        public void SetPenalty(int PenaltyTime)
        {
            if (PenaltyTime < 100)
            {
                PenaltyTime = 100;
            }

            if (PenaltyTime > 595900)
            {
                PenaltyTime = 595900;
            }

            Status = TTimeStatus.tsPenalty;
            Time = PenaltyTime;
        }

    }

}
