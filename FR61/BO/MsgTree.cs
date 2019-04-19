namespace RiggVar.FR
{

    public class TMsgTree : TBaseMsgTree
    {
        public TMsgTree(string aNameID, int aActionID) : base(aNameID)
        {
            NewActionID = aActionID;
            Division = new TDivision(this, TMain.BO.cTokenB);
        }

        public TDivision Division { get; }

        public override string LongToken(string t)
        {
            return TMsgParser.FRLongToken(t);
        }
    }

    public class TInputValue : TBaseInputValue
    {
        public TInputValue() : base()
        { 
        }
        public TInputValue(TBaseToken aOwner, string aNameID) 
          : base(aOwner, aNameID)
        {
        }
        protected bool IsValidBoolean(string s)
        {
            return true;
        }
        protected bool IsValidCount(string s)
        {
            return true;
        }
        protected bool IsValidTime(string s)
        {
            return true;
        }
        protected bool IsValidStatus(string s)
        {
            return true;

            /*
            bool b = false;
            if (s == "ok") b = true;
            else if (s == "dnf") b = true;
            else if (s == "dsq") b = true;
            else if (s == "dns") b = true;
            else if (s == "*") b = true;
            return b;
            */
        }
        protected bool IsValidBib(string s)
        {
            return true;
        }
        protected bool IsValidCourse(string s)
        {
            return true;
        }
        protected bool IsValidSNR(string s)
        {
            return true;
        }
        protected bool IsValidNC(string s)
        {
            return true;
        }
        protected bool IsValidName(string s)
        {
            return true;
        }
        protected bool IsValidDSQGate(string s)
        {
            return true;
        }
        protected bool IsValidGR(string s)
        {
            return true;
        }
        protected bool IsValidPB(string s)
        {
            return true;
        }
        protected bool IsValidRace(string s)
        {
            if (s != null)
            {
                return s.Length < 13;
            }
            else
            {
                return false;
            }
        }
        protected bool IsValidRadius(string s)
        {
            return true;
        }
        protected bool IsValidKoord(string s)
        {
            return true;
        }
        protected bool IsValidProp(string Key, string Value)
        {
            return true;
        }
    }

    public class TPos : TInputValue
    {
        public TPos()
        { 
        }
        public void Bib(string Value)
        {
            SendMsg(IsValidBib(Value), "Bib", Value);
        }
        public void SNR(string Value)
        {
            SendMsg(IsValidSNR(Value), "SNR", Value);            
        }
    }

    public class TBib : TInputValue
    {
        public TBib() : base()
        { 
            //no parameterized constructor, always used within a Tokenlist
        }
        /// <summary>
        /// Test Message
        /// </summary>
        /// <param name="Value">value for the right side of the equal sign</param>
        public void XX(string Value)
        {
            SendMsg(true, "XX", Value);
        }
        /// <summary>
        /// 'Quitt' Message
        /// </summary>
        /// <param name="Value">value of the QU message</param>
        public void QU(string Value)
        {
            SendMsg(IsValidStatus(Value), "QU", Value);
        }
        public void DG(string Value)
        {
            SendMsg(IsValidDSQGate(Value), "DG", Value);            
        }
        public void ST(string Value)
        {
            SendMsg(IsValidTime(Value), "ST", Value);
        }
        public void IT(int channel, string Value)
        {
            SendMsg(IsValidTime(Value), "IT" + channel.ToString(), Value);
        }
        public void FT(string Value)
        {
            SendMsg(IsValidTime(Value), "FT", Value);
        }
        public void Rank(string Value)
        {
            SendMsg(IsPositiveInteger(Value), "Rank", Value);
        }
        public void RV(string Value)
        {
            SendMsg(IsValidRace(Value), "RV", Value);
        }
        public void FM(string Value)
        {
            SendMsg(IsValidRace(Value), "FM", Value);
        }
    }

    public class TStartlist : TInputValue
    {
        private readonly TTokenList FPosStore;

        public TStartlist(TBaseToken aOwner, string aNameID) : base (aOwner, aNameID)
        {
            FPosStore = new TTokenList(this, "Pos", new TPos());
        }
        public TPos Pos(int index)
        {
            return FPosStore[index] as TPos;
        }
        public void Count(string Value)
        {
            SendMsg(IsPositiveInteger(Value), "Count", Value);
        }
    }

    public class TRun : TInputValue
    {
        private readonly TTokenList FBibStore;

        public TRun() : base()
        {
            FBibStore = new TTokenList(this, "Bib", new TBib());
        }
        public TRun(TBaseToken aOwner, string aNameID) : base (aOwner, aNameID)
        {
            FBibStore = new TTokenList(this, "Bib", new TBib());
        }
        public void IsRacing(string Value)
        {
            SendMsg(IsValidBoolean(Value), "IsRacing", Value);
        }
        public TBib Bib(int index)
        {
            return FBibStore[index] as TBib;
        }
    }

    public class TRun1 : TRun
    {
        public TStartlist Startlist;
        public TRun1(TBaseToken aOwner, string aNameID) : base (aOwner, aNameID)
        {
              Startlist = new TStartlist(this, "STL");
        }
    }

    public class TDivision : TInputValue
    {
        private readonly TTokenList FAthleteStore;
        private readonly TTokenList FRaceStore;
        public TRun1 Race1;

        public TDivision(TBaseToken aOwner, string aNameID) : base (aOwner, aNameID)
        {
            FAthleteStore = new TTokenList(this, TMain.BO.cTokenID, new TAthlete(aOwner, aNameID));
            FRaceStore = new TTokenList(this, TMain.BO.cTokenRace, new TRun());
            Race1 = new TRun1(this, TMain.BO.cTokenRace + "1");
        }

        public TRun Race(int index)
        {
            return FRaceStore[index] as TRun;
        }

        public TAthlete Athlete(int index)
        {
            return FAthleteStore[index] as TAthlete;
        }
    }


    public class TAthlete : TInputValue
    {
        public TAthlete(TBaseToken aOwner, string aNameID) : base (aOwner, aNameID)
        {
        }
        public void SNR(string Value)
        {
            SendMsg(IsValidSNR(Value), "SNR", Value);
        }
        public void FN(string Value)
        {
            SendMsg(IsValidName(Value), FieldNames.FN, Value);
        }
        public void LN(string Value)
        {
            SendMsg(IsValidName(Value), FieldNames.LN, Value);
        }
        public void SN(string Value)
        {
            SendMsg(IsValidName(Value), FieldNames.SN, Value);            
        }
        public void NC(string Value)
        {
            SendMsg(IsValidNC(Value), FieldNames.NC, Value);
        }
        public void GR(string Value)
        {
            SendMsg(IsValidGR(Value), FieldNames.GR, Value);
        }
        public void PB(string Value)
        {
            SendMsg(IsValidPB(Value), FieldNames.PB, Value);
        }
        public void Prop(string Key, string Value)
        {
            SendMsg(IsValidProp(Key, Value), "Prop_" + Key, Value);
        }
        public void FieldN(int index, string Value)
        {
            SendMsg(IsValidName(Value), "N" + index.ToString(), Value);
        }

    }

}
