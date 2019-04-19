namespace RiggVar.FR
{

    public class TBaseToken
    {
        internal static int NewActionID = 0; //used only during construction of MsgTree
        public static int MsgID = 0;
        public static bool UseLongNames = false;

        internal int FActionID;    
        internal TBaseToken Owner;
        internal string FNameID;
        internal int TokenID = -1;

        public TBaseToken()
        {
            FActionID = NewActionID;
        }

        public TBaseToken(TBaseToken aOwner, string aNameID)
        {
            Owner = aOwner;
            FNameID = aNameID;
            FActionID = NewActionID;
        }

        public int ActionID => FActionID;

        public string NameID
        {
            get
            {
                if (TokenID > -1)
                {
                    return UseLongNames ? LongToken(FNameID) + TokenID : FNameID + TokenID;
                }
                else
                {
                    return UseLongNames ? LongToken(FNameID) : FNameID;
                }
            }
        }

        public virtual string NamePath
        {
            get
            {
                string result = NameID;
                if (Owner != null)
                {
                    string s = Owner.NamePath;
                    if (s != string.Empty)
                    {
                        result = s + "." + NameID;
                    }
                }
                return result;
            }
        }

        public virtual string LongToken(string t)
        {
            return t;
        }

    }


    public class TTokenList
    {
        private TBaseToken IndexedChildToken;

        public TTokenList(TBaseToken aOwner, string aTokenName, TBaseToken aChildToken)            
        {
            IndexedChildToken = aChildToken;
            IndexedChildToken.Owner = aOwner;
            IndexedChildToken.FNameID = aTokenName;
        }

        /// <summary>
        /// the 'Token' property
        /// </summary>
        /// <param name="ID">the TokenID for an indexed Token</param>
        /// <returns>the Token with the TokenID dynamically set for immediate use</returns>
        public TBaseToken this [int ID]
        {
            get 
            {
                IndexedChildToken.TokenID = ID;
                return IndexedChildToken;
            }
        }
    }

    public class TInputAction
    {
        public delegate void ActionEvent(object sender, string s);

        public TInputAction()
        {
        }

        public ActionEvent OnSend { get; set; } = null;

        public virtual void Send(string sKey, string sValue)
        {
            OnSend?.Invoke(this, sKey + "=" + sValue);
        }

    }

    public class TInputActionManager
    {
        public const int DynamicActionID = 0;
        public const int UndoActionID = 1;

        public static TInputAction DynamicActionRef; // used for data serialization / message list
        public static TInputAction UndoActionRef; // used for creation of undo/redo messages
    }

    public class TBaseMsgTree : TBaseToken
    {
        public bool UseMsgID = false;
        public bool UsePrefix = true;

        public TBaseMsgTree(string aNameID) : base(null, aNameID)
        {            
        }

        public override string NamePath
        {
            get
            {
                string result = string.Empty;
                if (UseMsgID)
                {
                    if (Owner != null)
                    {
                        result = Owner.NamePath + "." + NameID;
                    }
                    else
                    {
                        result = NameID + ".Msg" + MsgID.ToString();
                    }
                }
                else if (UsePrefix)
                {
                    result = base.NamePath;
                }

                return result;
            }
        }

        public bool LongNames
        {
            get => UseLongNames;
            set => UseLongNames = value;
        }

    }


    public class TBaseInputValue : TBaseToken
    {
        public TBaseInputValue()
            : base()
        { 
        }

        public TBaseInputValue(TBaseToken aOwner, string aNameID)
            : base(aOwner, aNameID)
        {
        }

        private TInputAction InputAction
        {
            get
            {
                if (ActionID == 0)
                {
                    return TInputActionManager.DynamicActionRef;
                }
                else
                {
                    return TInputActionManager.UndoActionRef;
                }
            }
        }

        public void SendMsg(bool IsValid, string aCommand, string aValue)
        {
            string sKey;
            if (IsValid)
            {
                MsgID++;
                sKey = "";
            }
            else
            {
                sKey = "//";
            }

            sKey += NamePath + "." + aCommand;
            if (InputAction != null)
            {
                InputAction.Send(sKey, aValue);
            }
        }

        protected bool IsPositiveInteger(string s)
        {
            return true;
        }

    }

}
