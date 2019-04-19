namespace RiggVar.FR
{

    public class TBaseBO
    {
        //atoms
        public string cTokenA = "FR";
        public string cTokenB = "Division";
        
        //composite
        public string cTokenModul = "A.";
        public string cTokenSport = "A.B.";
        public string cTokenCC = "A.B.CC.";
        public string cTokenRequest = "A.B.Request.";
        public string cTokenAnonymousRequest = "A.*.Request.";
        public string cTokenAnonymousOutput = "A.*.Output.";
        public string cTokenOutput = "A.B.Output.";
        public string cTokenOutputXML = "A.B.Output.XML.";
        public string cTokenOutputCSV = "A.B.Output.CSV.";
        public string cTokenOutputHTM = "A.B.Output.HTM.";

        //special
        public string cTokenID = "SNR";
        public string cTokenRace = "W";
        public string cTokenOption = "Graph";

        public string DivisionName
        {
            set
            {
                cTokenB = value;
                cTokenModul = cTokenA + ".";
                cTokenSport = cTokenModul + cTokenB + ".";
                cTokenCC = cTokenSport + "CC.";
                cTokenRequest = cTokenSport + "Request.";
                cTokenAnonymousRequest = cTokenModul + "*.Request.";
                cTokenOutput = cTokenSport + "Output.";
                cTokenOutputXML = cTokenOutput + "XML.";
                cTokenOutputCSV = cTokenOutput + "CSV.";
                cTokenOutputHTM = cTokenOutput + "HTM.";
            }
        }

        protected bool FLoading;
        public bool Loading => FLoading;

        public TAdapterParams AdapterParams;

        public TInputNCP InputServer;
        public TOutputNCP OutputServer;
        public TBaseOutput Output;
        public int CounterMsgHandled;
        public string BackupDir = "";
        public TLocalWatches Watches;

        public TBaseBO(TAdapterParams aParams) : base()
        {
            AdapterParams = aParams;
        }

        public virtual void OnIdle()
        {
            //virtual;
        }

        public virtual bool Calc()
        {
            return false; //virtual
        }

        public virtual void Backup()
        {
            //virtual;
        }

        public virtual void BackupToSL(TStrings SL, bool CompactFormat)
        {
            //virtual;
        }

        public virtual void BackupToSL(TStrings SL)
        {
            //virtual;
        }

        public virtual void Restore()
        {
            //virtual;
        }

        public void Connect()
        {
            BOConnector.ConnectBO();
        }

        public void Disconnect()
        {
            BOConnector.DisconnectBO();
        }

        public bool Connected
        {
            get => (BOConnector.Connected && (BOConnector.ConnectedBO));
            set
            {
                if (!value)
                {
                    Disconnect();
                }
                else if (value && (!Connected))
                {
                    Connect();
                }
            }
        }

        public virtual TBaseBO FindDestinationBO(string Prot)
        {
            return this;
        }

        private IBOConnector BOConnector => TMain.BOConnector;

        public void Dispose(bool disposing)
        {
            if (disposing)
            {
                //...
            }
        }

        public virtual TBaseMsg NewMsg()
        {
            return new TBaseMsg();
        }

    }

}
