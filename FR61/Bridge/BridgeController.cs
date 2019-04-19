using System;
using System.Text;

namespace RiggVar.FR
{
    public enum BridgeProxyType
    {
        Mock=0,
        Asmx=1,
        Php=2,
        Client=3,
        Server=4,
        Synchron=5,
        REST=6,
        Proxy=7,
        Combi=8
    }

    public class TBridgeController : TPeerController
    {
        private bool FEnabled;

        private TStringList Log;
        private TStringList SL;
        private TStringList SLMerge;
        private int FSwitchID;
        private int FLastBackupID;
        private int FBackupMsgID;
        private int FLastMsgID;
        private DateTime FLastOutTime;
        private DateTime FLastInTime;

        private readonly int FLogCapacity;

        private int FTaktIn;
        private int FTaktOut;

        public TConnection InputConnection;
        public TConnection OutputConnection;

        private TBaseIniImage IniImage;
        public TGridUpdate BridgeUpdate;
        public IBridgeBO BridgeBO;
        public BridgeProxyType ProxyType;

        public TBridgeController(IBridgeBO aBridgeBO) : base(aBridgeBO.GetBaseIniImage()) 
        {
            BridgeBO = aBridgeBO;
            IniImage = BridgeBO.GetBaseIniImage();

            FTaktOut = 0;
            FTaktIn = 0;
            FEnabled = false;
            FLogCapacity = 2000;
            FLastOutTime = DateTime.Now;
            FLastInTime = FLastOutTime;

            Log = new TStringList();
            SL = new TStringList();
            SLMerge = new TStringList();

            ProxyType = IniImage.BridgeProxy;

            if (ProxyType == BridgeProxyType.Client)
            {
                Bridge = new TClientBridge(IniImage);
            }
            else if (ProxyType == BridgeProxyType.Proxy)
            {
                Bridge = new TProxyBridge(IniImage);
            }
            else if (ProxyType == BridgeProxyType.Combi)
            {
                Bridge = new TCombiBridge(IniImage);
            }
            else if (ProxyType == BridgeProxyType.REST)
            {
                Bridge = new TRESTBridge(IniImage.EventType);
            }
            else if (ProxyType == BridgeProxyType.Server)
            {
                Bridge = new TServerBridge(FLogCapacity);
            }
            else
            {
                Bridge = new TAsynchronBridge(IniImage);
            }

            if (Bridge is TClientBridge)
            {
                (Bridge as TClientBridge).BridgeController = this;
            }

            Bridge.SetServerUrl(IniImage.BridgeUrl);

            SetTaktIn(IniImage.TaktIn);
            SetTaktOut(IniImage.TaktOut);

            BridgeUpdate = new TGridUpdate
            {
                OnUpdate = new EventHandler(this.ActionPerformed)
            };

        }

        public void Destroy()
        {
            SetEnabled(false);
            Plugout();
        }

        private void SendMessages()
        {
            if (Log.Count > 1)
            {
                Bridge.SendDiffLog(FSwitchID, Log.Text);
                Log.Clear();
            }
            else if (Log.Count == 1)
            {
                Bridge.SendMsg(FSwitchID, Log[0]);
                Log.Clear();
            }
            FLastOutTime = DateTime.Now;
        }

        private void GetNewMessages()
        {
            int temp = FLastMsgID;
            FLastMsgID = Bridge.GetLastMsgID();
            string s = Bridge.GetNewMessages(FSwitchID, temp);
            if (s != "")
            {
                SL.Text = s;
                for (int i = 0; i < SL.Count; i++)
                {
                    s = SL[i];
                if (BridgeBO != null)
                    {
                        BridgeBO.Broadcast(s);
                    }
                }
            }
            FLastInTime = DateTime.Now;
        }

        public override bool Connected => FSwitchID > 0;

        public override void Connect()
        {
            if (BridgeBO == null)
            {
                return;
            }

            InputConnection = BridgeBO.GetInputConnection();
            OutputConnection = BridgeBO.GetOutputConnection();

            OutputConnection.SetOnSendMsg(new THandleContextMsgEvent(AddMsg));
            if (FSwitchID > 0)
            {
                SetEnabled(true);
            }
        }

        public override void Disconnect()
        {
            InputConnection = null;
            OutputConnection = null;
            SetEnabled(false);
        }

        public override void DoOnIdle()
        {
            if (FTaktIn == 0)
            {
                //process msgQueue of asynchronous tcp client connection
                Bridge.DoOnIdle();
            }

            if (GetEnabled())
            {
                int Sec;

                //Out
                Sec = DecodeTime(FLastOutTime);
                if (FTaktOut > 0 && Sec >= FTaktOut) 
                {
                    SendMessages();
                }

                //In
                Sec = DecodeTime(FLastInTime);
                if (FTaktIn > 0 && Sec >= FTaktIn) 
                {
                    GetNewMessages();
                }
            }
        }

        public void AddMsg(object sender, TContextMsg cm)
        {
            if (MsgContext.BridgeLocked == false && cm.msg != "")
            {
                if (TaktOut == 0)
                {
                    if (FSwitchID > 0)
                    {
                        if (!cm.IsOutgoingMsg)
                        {
                            cm.IsOutgoingMsg = true;
                        }
                    }

                    Bridge.SendContextMsg(FSwitchID, cm);
                }
                else
                {
                    if (IniImage.EventType == 400)
                    {
                        AddLine(cm.msg);
                    }
                    else
                    {
                        MergeLine(cm.msg);
                    }
                }
            }
        }

        private void AddLine(string s)
        {
            if (Log.Count > FLogCapacity)
            {
                Log.Delete(0);
            }

            Log.Add(s);
        }

        private void MergeLine(string s)
        {
            string temp;
            SLMerge.Clear();
            int i = Utils.Pos("=", s);
            if (i > 0)
            {
                temp = Utils.Copy(s, 1, i - 1).Trim();
                temp += "=";
                temp += Utils.Copy(s, i + 1, s.Length).Trim();
            }
            else
            {
                temp = s.Trim();
                temp = temp.Replace(' ', '_');
            }

            if (Utils.Pos("=", temp) == 0)
            {
                temp = temp + "=";
            }
            SLMerge.Add(temp);
            string sK = SLMerge.Names(0);
            string sV = SLMerge.Values(sK);

            int LogMsgIndex = Log.IndexOfName(sK);
            if (LogMsgIndex >= 0) 
            {
                Log[LogMsgIndex] = sK + '=' + sV;
            }
            else 
            {
                if (Log.Count > FLogCapacity)
                {
                    Log.Delete(0);
                }

                Log.Add(sK + '=' + sV);
            }
        }

        public void Clear()
        {
            Log.Clear();
            FBackupMsgID = Bridge.SendBackupAndLog(FSwitchID, "", "");
            FLastOutTime = DateTime.Now;
            FLastInTime = FLastOutTime;
        }

        public override void EditProps()
        {
            if (Bridge is TClientBridge)
            {
                TMain.FormAdapter.EditConnectionProps(TConfigSection.csBridge);
                //...change will automatically be detected at plugin time, and new ResultClient created...    
            }
            else
            {
                //edit ServerName, TaktIn, TaktOut
                //TFormBridgeProps.EditBridgeProps(this);
                TMain.FormAdapter.EditBridgeProps(this);
            }
        }

        public override void Plugin()
        {
            if (FSwitchID == 0)
            {
                int temp = Bridge.Plugin();
                if (temp > 0)
                {
                    FSwitchID = temp;
                }
            }
            if (FSwitchID > 0)
            {
                SetEnabled(true);
            }
        }

        public override void Plugout()
        {
            if (FSwitchID > 0)
            {
                SetEnabled(false);
            }

            Bridge.Plugout(FSwitchID);
            FSwitchID = 0;
        }

        public override void Synchronize()
        {
            if (FSwitchID < 1)
            {
                FSwitchID = Bridge.Plugin();
            }

            if (FSwitchID > 0)
            {
                SendMessages();
                GetNewMessages();
            }
        }

        public override void Upload(string s)
        {
            if (s != "")
            {
                Log.Clear();
                FBackupMsgID = Bridge.SendBackupAndLog(FSwitchID, s, "");
            }
        }

        public override string Download()
        {
            FLastBackupID = Bridge.GetLastBackupID();
            string s = Bridge.GetBackup();
            if (s != "")
            {
                Log.Clear();
            }

            return s;
        }

        public override void Close()
        {
            Bridge.Close();
        }

        public override bool IsMaster => IniImage.IsMaster;

        public override bool IsEnabled(SwitchOp Op)
        {
            if (Bridge == null)
            {
                return false;
            }

            bool result = false;

            switch (Op)
            {
                case SwitchOp.Plugin: 
                    result = true; 
                    break;
                case SwitchOp.Plugout: 
                    result = true; 
                    break;
                case SwitchOp.Synchronize: 
                    result = IsMaster; 
                    break;
                case SwitchOp.Upload: 
                    result = Connected && IsMaster; 
                    break;
                case SwitchOp.Download: 
                    result = true; 
                    break;
                default: 
                    return false;
            }

            return result && Bridge.IsEnabled(Op);
        }
        public override bool AllowRecreate
        {
            get
            {                
                bool result = true;

                //Main.IniImage.BridgeProxy is the new BridgeProxyType
                if (TMain.IniImage.BridgeProxy == BridgeProxyType.Server)
                {
                //cannot recreate ServerBridge,
                //because in PeerManager PortOpenCheck returns 'Port already open'
                //even short after destruction of old ServerBridge
                    if (ProxyType == BridgeProxyType.Server) //ProxyType is current (old) BridgeProxyType
                {             
                    if (Bridge.IsSameBridge())
                        {
                            result = false;
                        }
                    }
                }
                return result;
            }
        }

        public int TaktIn
        {
            get => GetTaktIn();
            set => SetTaktIn(value);
        }
        public int TaktOut
        {
            get => GetTaktOut();
            set => SetTaktOut(value);
        }

        public int GetTaktIn()
        {
            return FTaktIn;
        }

        public string BridgeUrl
        {
            get => GetBridgeUrl();
            set => SetBridgeUrl(value);
        }

        protected string GetBridgeUrl()
        {
            return Bridge.GetServerUrl();
        }

        protected void SetBridgeUrl(string value)
        {
            Bridge.SetServerUrl(value);
            IniImage.BridgeUrl = value;
        }

        int CheckTakt(int Value)
        {
            int result = Value;

            if (Value < -1)
            {
                result = -1;
            }
            else if (ProxyType == BridgeProxyType.Asmx)
            {
                if (Value < 2)
                {
                    result = 2;
                }
            }

            else if (ProxyType == BridgeProxyType.Php)
            {
                //bei php auf Takt nicht kleiner als 10 Sekunden
                if (Value < 10)
                {
                    result = 10;
                }
            }

            else if (ProxyType == BridgeProxyType.Client)
            {
                result = 0; // immer asynchron
            }

            else if (ProxyType == BridgeProxyType.Server)
            {
                result = 0; // immer asynchron
            }

            return result;
        }

        public void SetTaktIn(int value)
        {
            if (FTaktIn != value)
            {
                FTaktIn = CheckTakt(value);
            }

            IniImage.TaktIn = FTaktIn;
        }

        public int GetTaktOut() 
        {
            return FTaktOut;
        }

        public void SetTaktOut(int value) 
        {
            if (FTaktOut != value)
            {
                FTaktOut = CheckTakt(value);
            }

            IniImage.TaktOut = FTaktOut;
        }

        public IBridgeService Bridge { get; }

        private int DecodeTime(DateTime c)
        {
            TimeSpan ts = DateTime.Now - c;
            return ts.Seconds;
        }

        public void ActionPerformed(object sender, EventArgs args)
        {
            DoOnIdle();
        }

        public bool GetEnabled()
        {
            return FEnabled && ! Bridge.GetHasError();
        }

        public void SetEnabled(bool value)
        {
            FEnabled = value;
            BridgeUpdate.Enabled = value;
        }

        public override void DoOnBackup(string s)
        {
            if (s != "") 
            {
                Log.Clear();
                base.DoOnBackup(s);
            }
        }
        public void DoOnPlugin(int SwitchID)
        {
            if (SwitchID > 0)
            {
                FSwitchID = SwitchID;
            }

            if (SwitchID >= 0)
            {
                SetEnabled(true); //start processing input-msgQueue
                //to receive the PugIn callback!

                if (SwitchID > 0) 
                {
                    //only if real return message from remote bridge server
                    //was received - update the StatusBar display
//                    if (Globals.ConnectionNotifier != null) 
//                    {
//                        Globals.ConnectionNotifier.ConnectionStatusChanged(this,
//                            new ConnectionNotifierEventArgs(
//                            ConnectionNotifierEventArgs.SenderTypeExternalPlug,
//                            ConnectionNotifierEventArgs.ConnectStatusConnect));
//                    }
                }
            }
        }
        public void DoOnNewMessages(string NewMessages)
        {
            string s;
            SL.Text = NewMessages;
            for (int i = 0; i < SL.Count; i++)
            {
                s = SL[i];
                BridgeBO.InjectClientBridgeMsg(s);
            }
        }
        public void DoOnGetLastMsgID(int Value)
        {
            FLastMsgID = Value;
        }
        public void DoOnGetLastBackupID(int Value)
        {
            FLastBackupID = Value;
        }

        public void DoOnRequest(int Target, string Request)
        {
            string s = BridgeBO.GetReport(Request);
            Bridge.SendAnswer(Target, s);
        }

        public int getLastMsgID()
        {
            return FLastMsgID;
        }

        public override void GetStatusReport(StringBuilder sb)
        {
            sb.Append("ClassType: TBridgeController" + Environment.NewLine);
            TBridgeItem cr = new TBridgeItem();
            cr.InitFromCurrent();
            cr.Show(sb);
            Bridge.GetStatusReport(sb);
        }

    }
}
