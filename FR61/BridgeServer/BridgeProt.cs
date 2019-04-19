namespace RiggVar.FR
{

    /// <summary>
    ///BridgeProt, alias ProxyProt, used in RA21 and also in FR62 when configured as ServerBridge.
    ///<list type="bullet">
    ///<item><description>marshalls calls on and off the wire from server perspective
    ///(deserializes messages sent from AsnchronBridge)</description></item>
    ///<item><description>a reference to the data (LocalBridge) is injected by the container</description></item>
    ///<item><description>public Calc method processes the msg</description></item>
    ///<item><description>protected DispatchMsg method deserializes the msg and delegates to LocalBridge</description></item>
    ///<item><description>an OnBroadcast event is used with SendMsg calls so msg is passed on to all 
    ///connected parties except the sender</description></item>
    ///<item><description>is base class for TProxyProt = class(TBridgeProt)</description></item>
    ///</list>
    /// </summary>
    public class TBridgeProt
    {

        public TLocalBridge LocalBridge;
        public THandleMsgEvent OnBroadcast;
        protected TStringList SL;
        protected int MsgCounterIn;
        protected string MsgText;
        protected TContextMsg CM;

        public TBridgeProt()
        {
            SL = new TStringList();
            CM = new TContextMsg();        
        }

        private int PopInt()
        {
            int result = -1;
            if (SL.Count > 0)
            {
                result = Utils.StrToIntDef(SL[0], -1);
                SL.Delete(0);
            }
            return result;
        }

        private string PopString()
        {
            string result = "";
            if (SL.Count > 0)
            {
                result = SL[0].Trim();
                SL.Delete(0);
            }
            return result;
        }

        public string Calc(object Sender, string msg)
        {
            string s;
            string result = "";
            MsgCounterIn++;
            LocalBridge.DoOnUpdateUI(UUIAction.MsgCounterIn, Utils.IntToStr(MsgCounterIn));
            MsgText = msg;
            if (msg != "")
            {
                SL.Text = msg;
                if (SL.Count > 0)
                {
                    s = SL[0];
                    SL.Delete(0);
                    result = DispatchMsg(Sender, s);
                }
            }
            return result;
        }

        public string DispatchMsg(object Sender, string s)
        {
            string m;
            int MsgID;
            int SwitchID;
            int BackupID;
            int BackupMsgID;
            int StartMsgID;
            int StartBackupID;
        
            string result = "";
            if (s == "Plugin")
            {
                SwitchID = LocalBridge.Plugin();
                SL.Clear();
                SL.Add(s);
                SL.Add(Utils.IntToStr(SwitchID));
                result = SL.Text;
            }

            else if (s == "Plugout")
            {
                SwitchID = PopInt();
                if (SwitchID > 0)
                {
                    LocalBridge.Plugout(SwitchID);
                }
            }

            else if (s == "SendBackup")
            {
                SwitchID = PopInt();
                s = PopString();
                if (SwitchID > -1)
                {
                    BackupMsgID = LocalBridge.SendBackupAndLog(SwitchID, SL.Text, "");
                    SL.Clear();
                    SL.Add(s);
                    SL.Add(Utils.IntToStr(BackupMsgID));
                    result = SL.Text;
                }
            }

            else if (s == "SendDiffLog")
            {
                SwitchID = PopInt();
                if (SwitchID > -1)
                {
                    LocalBridge.SendDiffLog(SwitchID, SL.Text);
                }
            }

            else if (s == "SendMsg")
            {
                SwitchID = PopInt();
                if (SwitchID > -1)
                {
                    m = PopString();
                    if (m != "" && m.Length < 200)
                    {

                        //pass msg on to all except the sender
                        if (OnBroadcast != null)
                        {
                            SL.Clear();
                            SL.Add("GetNewMessages");
                            SL.Add(m);
                            OnBroadcast(Sender, SL.Text);
                            LocalBridge.DoOnUpdateUI(UUIAction.Msg, m);
                        }

                        //add to MsgRing, update UI, update local App 
                        CM.msg = m;
                        CM.IsIncomingMsg = true;
                        LocalBridge.SendContextMsg(SwitchID, CM);
                        CM.Clear();
                    }
                }
            }

            else if (s == "GetBackup")
            {
                SL.Text = LocalBridge.GetBackup();
                SL.Insert(0, s);
                result = SL.Text;
            }

            else if (s == "GetNewMessages")
            {
                SwitchID = PopInt();
                if (SwitchID > 0)
                {
                    StartMsgID = PopInt();
                    if (StartMsgID > -1)
                    {
                        SL.Text = LocalBridge.GetNewMessages(SwitchID, StartMsgID);
                        SL.Insert(0, s);
                        result = SL.Text;
                    }
                }
            }

            else if (s == "LogValid")
            {
                SL.Text = s;
                SL.Add(Utils.BoolStr[LocalBridge.LogValid()]);
                result = SL.Text;
            }

            else if (s == "GetLastMsgID")
            {
                MsgID = LocalBridge.GetLastMsgID();
                SL.Text = s;
                SL.Add(Utils.IntToStr(MsgID));
                result = SL.Text;
            }

            else if (s == "GetLastBackupID")
            {
                BackupID = LocalBridge.GetLastBackupID();
                SL.Text = s;
                SL.Add(Utils.IntToStr(BackupID));
                result = SL.Text;
            }

            else if (s == "GetBackupSwitchID")
            {
                SwitchID = LocalBridge.GetBackupSwitchID();
                SL.Text = s;
                SL.Add(Utils.IntToStr(SwitchID));
                result = SL.Text;
            }

            else if (s == "CheckForBackup")
            {
                SwitchID = PopInt();
                if (SwitchID > 0)
                {
                    StartBackupID = PopInt();
                    if (StartBackupID > -1)
                    {
                        SL.Text = s;
                        SL.Add(Utils.BoolStr[LocalBridge.CheckForBackup(SwitchID, StartBackupID)]);
                        result = SL.Text;
                    }
                }
            }

            else if (s == "CheckForLog")
            {
                SwitchID = PopInt();
                if (SwitchID > 0)
                {
                    StartMsgID = PopInt();
                    if (StartMsgID > -1)
                    {
                        SL.Text = s;
                        SL.Add(Utils.BoolStr[LocalBridge.CheckForBackup(SwitchID, StartMsgID)]);
                        result = SL.Text;
                    }
                }
            }
            return result;
        }

    }
}
