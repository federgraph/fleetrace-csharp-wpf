using System;

namespace RiggVar.FR
{

    public class TMsgFactory
    {
        public virtual TBaseMsgList CreateMsg()
        {
            return new TBaseMsgList();
        }
    }

    public class TReplayMsg : TBOPersistent
    {
        protected string FDivision;
        protected string FRunID;
        protected int FBib;
        protected string FCmd;
        protected string FMsgValue;

        public TMsgType FMsgType;
        public string FMsgKey;

        public int DBID; //hat DB automatisch
        public DateTime LogTime;
        public int SeqNo;
        public bool Delivered;
        public int CheckInt;
        public int ReplayInterval;
        public bool Hidden;
        public int ReplayOrder;
        public bool IsError; //nicht speichern
        public bool IsCheckSumError; //nicht speichern

        public TReplayMsg()
        {
            ClearResult();
        }

        public virtual void ClearResult()
        {
            //Content
            FDivision = "*";
            FRunID = "RunID";
            FBib = 0;
            FCmd = "Cmd";
            FMsgValue = "00:00:00.000";
            FMsgKey = "MsgKey";
            FMsgType = TMsgType.None;

            //Verwaltung
            DBID = -1;
            LogTime = DateTime.Now;
            SeqNo = 1;
            IsError = false;
            IsCheckSumError = false;
            Delivered = false;
            CheckInt = 0;
            ReplayInterval = 1000;
            Hidden = false;
            ReplayOrder = 0;
        }

        public override void Assign(object source)
        {
            if (source is TReplayMsg cr)
            {
                //Content
                FDivision = cr.FDivision;
                FRunID = cr.FRunID;
                FBib = cr.FBib;
                FCmd = cr.FCmd;
                FMsgValue = cr.FMsgValue;
                FMsgKey = cr.FMsgKey;
                FMsgType = cr.FMsgType;

                //Verwaltung
                DBID = cr.DBID;
                LogTime = cr.LogTime;
                SeqNo = cr.SeqNo;
                IsError = cr.IsError;
                IsCheckSumError = cr.IsCheckSumError;
                Delivered = cr.Delivered;
                CheckInt = cr.CheckInt;
                ReplayInterval = cr.ReplayInterval;
                Hidden = cr.Hidden;
                ReplayOrder = cr.ReplayOrder;
            }
            else
            {
                base.Assign(source);
            }
        }

        public string Division
        {
            get => FDivision;
            set => FDivision = value;
        }

        public string RunID
        {
            get => FRunID;
            set => FRunID = value;
        }
        public int Bib
        {
            get => FBib;
            set => FBib = value;
        }
        public string Cmd
        {
            get => FCmd;
            set => FCmd = value;
        }
        public string MsgValue
        {
            get => FMsgValue;
            set => FMsgValue = value;
        }
        public string MsgKey
        {
            get => FMsgKey;
            set => FMsgKey = value;
        }
        public TMsgType MsgType
        {
            get => FMsgType;
            set => FMsgType = value;
        }
        public string AsString
        {
            get
            {
                string sDBID;
                if (DBID < 0)
                {
                    sDBID = "DBID";
                }
                else
                {
                    sDBID = Utils.IntToStr(DBID);
                }

                return Cmd + "," + MsgValue + "," + sDBID;
            }
            set
            {
                string s;
                string temp = string.Empty;
                s = Utils.Cut(",", value, ref temp);
                Cmd = temp;
                s = Utils.Cut(",", s, ref temp);
                MsgValue = temp; 
                s = Utils.Cut(",", s, ref temp);
                DBID = Utils.StrToIntDef(temp, -1);
            }
        }
        public virtual string DiskMsgHeader
        {
            get
            {
                return "Cmd,MsgValue,ReplayInterval";
            }
        }
        public virtual string DiskMsg
        {
            get 
            {
                string sep = ",";
                return Cmd + sep + MsgValue + sep + ReplayInterval.ToString() + sep;
            }
        }

    }

    public class TBaseMsg : TReplayMsg
    {
        public int MsgResult;

        public int KatID;
        public string Prot;

        public TBaseMsg() : base()
        {
        }
        public virtual bool DispatchProt()
        {
            return false;
        }
    }

    public class TBaseMsgList : TBaseMsg
    {
        private TStrings SL = new TStringList();
        public TStrings OutputRequestList = new TStringList();

        public TBaseMsgList() : base()
        {
        }

        protected virtual TBaseMsg NewMsg()
        {
            return new TBaseMsg();
        }
        private void ProcessRequestHeader()
        {
            string s;
            bool b = false;
            
            int l = TMain.BO.cTokenRequest.Length;
            do
            {
                if (SL.Count > 0)
                {
                    b = (SL[0].StartsWith(TMain.BO.cTokenRequest));
                }
                else
                {
                    b = false;
                }

                if (b)
                {
                    s = Utils.CopyRest(SL[0], l + 1);
                    OutputRequestList.Add(TMain.BO.cTokenOutput + s);
                    SL.Delete(0);
                }
            }
            while (b);
        }

        private void ProcessRequestInput()
        {
            if (SL.Count > 0)
            {                
                TBaseMsg msg = NewMsg(); //NewMsg is virtual! //TBOMsg msg = new TBOMsg(BO);
                for (int i = 0; i < SL.Count; i++)
                {
                    if (IsComment(SL[i]))
                    {
                        continue;
                    }

                    msg.Prot = SL[i];
                    msg.DispatchProt();
                }
                SL.Clear();
            }
        }
        private bool IsComment(string s)
        {
            if ((s == "") || 
                (Utils.Copy(s, 1, 2) == "//") || 
                (Utils.Copy(s, 1, 1) == "#"))
            {
                return true;
            }
            return false;
        }

        public override bool DispatchProt()
        {        
            ClearResult();

            SL.Text = Prot;

            //if erste Zeile ist Request
            if (Prot.StartsWith(TMain.BO.cTokenRequest) || 
                Prot.StartsWith("RiggVar.Request.") || 
                Prot.StartsWith(TMain.BO.cTokenAnonymousRequest))
            {
                bool result = false;
                if (SL.Count > 0)
                {
                    //eine Request-Zeile, wie bisher
                    int l;
                    if (Prot.StartsWith(TMain.BO.cTokenRequest))
                    {
                        l = TMain.BO.cTokenRequest.Length;
                    }
                    else if (Prot.StartsWith(TMain.BO.cTokenAnonymousRequest))
                    {
                        l = TMain.BO.cTokenAnonymousRequest.Length;
                    }
                    else
                    {
                        l = "RiggVar.Request.".Length;
                    }

                    MsgValue = Utils.CopyRest(SL[0], l + 1);
                    Cmd = TMain.BO.cTokenOutput;
                    MsgValue = TMain.BO.cTokenOutput + MsgValue;
                    SL.Delete(0);
                    OutputRequestList.Add(MsgValue);

                    //alle weiteren Output-Request-Zeilen
                    ProcessRequestHeader();

                    //process body of message
                    ProcessRequestInput();

                    Prot = ""; //wird nicht mehr gebraucht
                    result = true; //sp�ter die Antwort senden
                    //Msg wird anschlie�end zur Queue hinzugef�gt,
                    //nach Neuberechnung wird Msg von der Queue geholt,
                    //der Output generiert und gesendet.
                }
                MsgResult = 1; //do not reply with MsgID now
                return result;
            }

            //Mehrzeilig ohne RequestHeader
            if (SL.Count > 1)
            {
                ProcessRequestInput();
                MsgResult = 1; //do not reply with MsgID
                return false; 
            }
            
            //Einzeilig
            if (SL.Count == 1)
            {
                TBaseMsg msg = NewMsg();
                msg.Prot = SL[0];
                bool result = msg.DispatchProt();
                MsgResult = msg.MsgResult;            
                return result;
            }
            
            //empty Msg
            MsgResult = 1;
            return false;
        }
    }

    public class TAdapterMsg : TBaseMsg
    {
        public TAdapterMsg() : base()
        {
        }
        public override bool DispatchProt()
        {
            ClearResult();
            return true;
        }
    }

}
