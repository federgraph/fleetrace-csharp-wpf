namespace RiggVar.FR
{
    public class StatusConst
    {
        public const int Status_OK = 0;
        public const int Status_DNS = 1;
        public const int Status_DNF = 2;
        public const int Status_DSQ = 3;
        public const int Status_DSQPending = 4;

        //public const string [] StatusEnumStrings  = { "ok", "dns", "dnf", "dsq", "*" };
        public static StatusStruct StatusEnumStrings;

        public const int crsNone = 0;
        public const int crsStarted = 1;
        public const int crsResults = 2;
    }
    public struct StatusStruct
    {
        public string this [TStatusEnum status]
        {
            get 
            {
                switch (status)
                {
                    case TStatusEnum.csOK: return "ok";    
                    case TStatusEnum.csDNS: return "dns";
                    case TStatusEnum.csDNF: return "dnf";
                    case TStatusEnum.csDSQ: return "dsq";
                    case TStatusEnum.csDSQPending: return "*";            
                }
                return string.Empty;
            }
        }
    }
    public enum TStatusEnum 
    {
        csOK,
        csDNS,
        csDNF,
        csDSQ,
        csDSQPending
    }
    public class TPenalty : TBOPersistent
    {
        public TPenalty()
        {
        }
        protected virtual bool GetIsDSQPending()
        {
            return false;
        }
        protected virtual bool GetIsOK()
        {
            return true;
        }
        protected virtual bool GetIsOut()
        {
            return false;
        }
        protected virtual void SetIsDSQPending(bool Value)
        {}
        protected virtual int GetAsInteger()
        {
            return 0;
        }
        protected virtual void SetAsInteger(int Value)
        {
        }

        public virtual void Clear()
        {}
        public virtual bool Parse(string Value)
        {
            return false;
        }
        public virtual bool FromString(string Value)
        {
            return false;
        }

        public int AsInteger
        {
            get => GetAsInteger();
            set => SetAsInteger(value);
        }
        public bool IsOK
        {
            get { return GetIsOK(); }
        }
        public bool IsOut
        {
            get { return GetIsOut(); }
        }
        public bool IsDSQPending
        {
            get => GetIsDSQPending();
            set => SetIsDSQPending(value);
        }
    }

    public class TStatus : TPenalty
    {
        protected override bool GetIsDSQPending()
        {
            return (Status == TStatusEnum.csDSQPending);
        }
        protected override bool GetIsOK()
        {
            return (Status == TStatusEnum.csOK) || (Status == TStatusEnum.csDSQPending);
        }
        protected override bool GetIsOut()
        {
            return (Status == TStatusEnum.csDSQ) || (Status == TStatusEnum.csDNF) || (Status == TStatusEnum.csDNS);
        }
        protected override void SetIsDSQPending(bool Value)
        {
            Status = Value ? TStatusEnum.csDSQPending : TStatusEnum.csOK;
        }
        protected override int GetAsInteger()
        {
            switch (Status)
            {
                case TStatusEnum.csOK: return StatusConst.Status_OK;
                case TStatusEnum.csDSQ: return StatusConst.Status_DSQ;
                case TStatusEnum.csDNF: return StatusConst.Status_DNF;
                case TStatusEnum.csDNS: return StatusConst.Status_DNS;
                case TStatusEnum.csDSQPending: return StatusConst.Status_DSQPending;
                default: return StatusConst.Status_OK;
            }
        }
        protected override void SetAsInteger(int value)
        {
            switch (value)
            {
                case StatusConst.Status_OK: 
                    Status = TStatusEnum.csOK;
                    break;
                case StatusConst.Status_DSQ: 
                    Status = TStatusEnum.csDSQ;
                    break;
                case StatusConst.Status_DNF: 
                    Status = TStatusEnum.csDNF;
                    break;
                case StatusConst.Status_DNS: 
                    Status = TStatusEnum.csDNS;
                    break;
                case StatusConst.Status_DSQPending: 
                    Status = TStatusEnum.csDSQPending;
                    break;
                default: 
                    Status = TStatusEnum.csOK;
                    break;
            }
        }

        public TStatusEnum Status { get; set; }
        public override void Assign(object source)
        {
            if (source is TStatus)
            {
                TStatus o = source as TStatus;
                Status = o.AsEnum;
            }
            else
            {
                base.Assign(source);
            }
        }
        public override void Clear()
        {
            Status = TStatusEnum.csOK;
        }
        public override bool Parse(string Value)
        {
            string temp = Value.ToLower();
            bool result = true;            
            if (temp == "dsq")
            {
                Status = TStatusEnum.csDSQ;
            }
            else if (temp == "dns")
            {
                Status = TStatusEnum.csDNS;
            }
            else if (temp == "dnf")
            {
                Status = TStatusEnum.csDNF;
            }
            else if (temp == "ok")
            {
                Status = TStatusEnum.csOK;
            }
            else if (temp == "*")
            {
                Status = TStatusEnum.csDSQPending;
            }
            else
            {
                result = false;
            }

            return result;
        }
        public override string ToString()
        {
            return StatusConst.StatusEnumStrings[Status];
        }

        public int CompareStatus(TStatus A, TStatus B, int GateA, int GateB)
        {
            //beide gleich:
            if (A.Status == B.Status)
            {
                if ( 
                    ((A.Status == TStatusEnum.csDNF) && (B.Status == TStatusEnum.csDNF))
                    || ((A.Status == TStatusEnum.csDSQ) && (B.Status == TStatusEnum.csDSQ))
                    )
                {
                    if (GateA > GateB)
                    {
                        return 1;
                    }
                    else if (GateB > GateA)
                    {
                        return 2;
                    }
                }
                else
                {
                    return 0;
                }
            }

            //beide ok, niemand besser:
            else if (A.IsOK && B.IsOK)
            {
                return 0;
            }

            //A ok, B out:
            else if (A.IsOK && B.IsOut)
            {
                return 1;
            }

            //A out, B ok:
            else if (A.IsOut && B.IsOK)
            {
                return 2;
            }

            //beide Out aber nicht gleich:
            else //if (A.IsOut && B.IsOut)
            {
                if (A.Status == TStatusEnum.csDNF)
                {
                    return 1;
                }
                else if (B.Status == TStatusEnum.csDNF)
                {
                    return 2;
                }
                else if (A.Status == TStatusEnum.csDSQ)
                {
                    return 1;
                }
                else if (B.Status == TStatusEnum.csDSQ)
                {
                    return 2;
                }
                //else //if ((A.Status == csDNS) && (B.Status == cdDNS))
                //    return 0;
            }
            return 0;
        }
        public bool IsBetter(TStatus Partner)
        {
            return CompareStatus(this, Partner, 0, 0) == 1;
        }
        public bool IsBetter(TStatus Partner, int GateA, int GateB)
        {
            return CompareStatus(this, Partner, GateA, GateB) == 1;
        }
        public bool IsEqual(TStatus Partner, int GateA, int GateB)
        {
            return CompareStatus(this, Partner, GateA, GateB) == 0;
        }
        public bool IsWorse(TStatus Partner)
        {
            return CompareStatus(this, Partner, 0, 0) == 2;
        }
        public bool IsWorse(TStatus Partner, int GateA, int GateB)
        {
            return CompareStatus(this, Partner, GateA, GateB) == 2;
        }
        public TStatusEnum AsEnum
        {
            get { return Status; }
            set { Status = value; }
        }
    }

}
