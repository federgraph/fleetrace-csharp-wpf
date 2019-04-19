using System;
using System.Text;
using System.Windows;

namespace RiggVar.FR
{

    public class TMain : IDrawNotifier, IBridgeBO, ISwitchBO, IBORecreator
    {
        public static TMain Controller;
        public static Window MainForm;
        public static Action IdleAction;

        public static string AppName = "FR62";
        public static string ContextPath = @"/";

        public static bool IsWebApp;
        public static bool IsService;
        public static bool IsWinGUI;
        public static bool WantAutoSync = true;
        public static bool WantBridgeMsgBroadcast = true; //true for EventType = 600
        public static bool UseDB;
        public static bool ReadWorkspaceInfo;
        public static bool AutoPlugin;
        public static bool AutoUpload;

        public static TBaseIniImage BaseIniImage;
        public static TMsgFactory MsgFactory;
        public static TAdapterBO AdapterBO;

        public static TBaseFormAdapter FormAdapter;
        public static TDBStringListFactory DBStringListFactory;
        public static TWorkspaceInfo WorkspaceInfo;
        public static TWorkspaceManager WorkspaceManager;
        public static TRedirector Redirector;
        public static TFolderInfo FolderInfo;

        public static TLogger Logger;
        public static TIniImage IniImage;
        public static TDocManager DocManager;
        public static TPeerManager PeerManager;
        public static TBOContainer BOManager;
        public static TGuiManager GuiManager;
        public static TSoundManager SoundManager;

        public static ISwitchBO SwitchBO;
        public static IBridgeBO BridgeBO;
        public static IDrawNotifier DrawNotifier;
        public static IBOConnector BOConnector;
        public static IBORecreator BORecreator;

        public TMain()
        {
            Controller = this;

            DBStringListFactory = new TDBStringListCreator();
            Redirector = new TRedirector();
            Logger = new TLogger();

            if (WorkspaceInfo == null)
            {
                WorkspaceInfo = new TWorkspaceInfo();
                WorkspaceInfo.Load();
            }
            if (SoundManager == null)
            {
                SoundManager = new TSoundManager();
            }

            if (WorkspaceManager == null)
            {
                WorkspaceManager = new TWorkspaceManager();
            }

            if (FolderInfo == null)
            {
                FolderInfo = new TFolderInfo();
            }

            IniImage = new TIniImage();
            BaseIniImage = IniImage;
            DrawNotifier = this;
            BridgeBO = this;
            SwitchBO = this;

            BOManager = new TBOContainer();
            BOConnector = BOManager;
            BORecreator = BOManager;

            //Init AdapterBO
            if (IniImage.WantAdapter && IniImage.WantSockets)
            {
                BOManager.CreateAdapterBO();
                AdapterBO = BOManager.AdapterBO;
            }

            //Init BO
            MsgFactory = new TBOMsgFactory();
            BOManager.InitBO();

            DocManager = new TDocManager(IniImage.DBInterface);

            PeerManager = new TPeerManager();

            //BO.StatusFeedback.Enabled = IniImage.FeedbackEnabled;

            if (IniImage.AutoConnect)
            {
                BO.Connect();
            }

            if (FormAdapter == null)
            {
                FormAdapter = new TBaseFormAdapter();
            }

            GuiManager = new TGuiManager();

            if (AutoPlugin)
            {
                PeerController.Plugin();
                if (AutoUpload)
                {
                    PeerController.Upload(BO.Save());
                }
            }

        }

        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                //...Peer.PeerController.Close() called from MainForm.Dispose()
                //...BO.StatusFeedback.Dispose();
            }
        }
        public static TPeerController PeerController
        {
            get
            {
                if (PeerManager != null)
                {
                    return PeerManager.Peer;
                }

                return null;
            }
        }

        public static TBO BO;

        public bool HaveSocketPermission => true;
        public bool HaveWebPermission => true;
        public bool HaveFilePermission
        {
            get
            {
                if (!IniImage.WantLocalAccess)
                {
                    return false;
                }

                return true;
            }
        }
        public string StatusString
        {
            get
            {
                try
                {
                    string crlf = Environment.NewLine;
                    StringBuilder sb = new StringBuilder();

                    sb.Append(DateTime.Now.ToString() + crlf);

                    sb.Append(crlf);
                    sb.Append("--- InputNCP" + crlf);
                    BO.InputServer.Server.StatusReport(sb);

                    sb.Append(crlf);
                    sb.Append("--- OutputNCP" + crlf);
                    BO.OutputServer.Server.StatusReport(sb);

                    int i;
                    if (AdapterBO != null)
                    {
                        sb.Append(crlf);
                        sb.Append("--- AdapterBO" + crlf);
                        i = AdapterBO.InputServer.Server.ConnectionCount();
                        sb.Append("AdapterInputConnections: " + i + crlf);
                        i = AdapterBO.OutputServer.Server.ConnectionCount();
                        sb.Append("AdapterOutputConnections: " + i + crlf);
                    }

                    sb.Append(crlf);
                    sb.Append("--- WorkspaceInfo" + crlf);
                    WorkspaceInfo.WorkspaceReport(sb);

                    sb.Append(crlf);
                    sb.Append("--- PeerController" + crlf);
                    PeerController.GetStatusReport(sb);

                    return sb.ToString();
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
        }

        #region IBORecreator

        public void CreateNew(TStrings ml)
        {
            BOManager.CreateNew(ml);
        }
        public void LoadNew(string data)
        {
            BOManager.LoadNew(data);
        }
        public void RecreateBOFromBackup()
        {
            BOManager.RecreateBOFromBackup();
        }
        public void RecreateBO(TAdapterParams p)
        {
            BOManager.RecreateBO(p);
        }
        public string GetTestData()
        {
            return BOManager.GetTestData();
        }

        #endregion

        #region IDrawNotifier Member

        public void ScheduleDraw(object sender, DrawNotifierEventArgs e)
        {
            if (GuiManager != null)
            {
                GuiManager.ScheduleDraw(sender, e);
            }
        }

        public void ScheduleFullUpdate(object sender, DrawNotifierEventArgs e)
        {
            if (GuiManager != null)
            {
                GuiManager.ScheduleFullUpdate(sender, e);
            }
        }

        #endregion

        #region IBridgeBO + ISwitchBO

        public TConnection GetInputConnection()
        {
            return BO.InputServer.Server.Connect("BridgeBO.Input");
        }

        public TConnection GetOutputConnection()
        {
            return BO.OutputServer.Server.Connect("BridgeBO.Output");
        }

        public void InjectClientBridgeMsg(string s)
        {
            TBaseMsg msg = BO.NewMsg();
            try
            {
                MsgContext.BridgeLocked = true;
                msg.Prot = s;
                if (msg.DispatchProt())
                {
                    if (WantAutoSync)
                    {
                        GuiManager.CacheMotor.SynchronizeIfNotActive();
                    }

                    if (WantBridgeMsgBroadcast)
                    {
                        BO.Calc();
                        BO.OutputServer.InjectMsg(LookupKatID.FR, TMsgSource.Bridge, msg.Prot);
                    }

                    if (s.IndexOf("IT") > 0)
                    {
                        ScheduleDraw(
                            this,
                            new DrawNotifierEventArgs(DrawNotifierEventArgs.DrawTargetRace));
                    }
                    else
                    {
                        ScheduleDraw(
                            this,
                            new DrawNotifierEventArgs(DrawNotifierEventArgs.DrawTargetEvent));
                    }
                    GuiManager.PlaySound(SoundID.Click03);
                }
                else
                {
                    GuiManager.PlaySound(SoundID.Recycle);
                }

                }
            finally
            {
                MsgContext.BridgeLocked = false;
            }
      }

        public void InjectServerBridgeMsg(TContextMsg cm)
        {
            TBaseMsg msg = BO.NewMsg();
            try
            {
                MsgContext.BridgeLocked = true;
                msg.Prot = cm.msg;
                if (msg.DispatchProt())
                {
                    if (TMain.WantAutoSync)
                    {
                        TMain.GuiManager.CacheMotor.SynchronizeIfNotActive();
                    }

                    BO.OnIdle();
                    BO.OutputServer.SendMsg(0, cm);

                    if (cm.msg.IndexOf("IT") > 0)
                    {
                        ScheduleFullUpdate(
                            this,
                            new DrawNotifierEventArgs(DrawNotifierEventArgs.DrawTargetRace));
                    }
                    else
                    {
                        ScheduleFullUpdate(
                            this,
                            new DrawNotifierEventArgs(DrawNotifierEventArgs.DrawTargetEvent));
                    }
                }
            }
            finally
            {
                MsgContext.BridgeLocked = false;
            }

        }

        public void Broadcast(string s)
        {
            TBaseMsg msg = new TBOMsg();
            msg.Prot = s;
            if (msg.DispatchProt())
            {
                BO.Calc();
                TContextMsg cm = new TContextMsg();
                cm.msg = s;
                BO.OutputServer.SendMsg(600, cm);
                if (s.IndexOf("IT") > 0)
                {
                    ScheduleFullUpdate(
                        this,
                        new DrawNotifierEventArgs(DrawNotifierEventArgs.DrawTargetRace));
                }
                else
                {
                    ScheduleFullUpdate(
                        this,
                        new DrawNotifierEventArgs(DrawNotifierEventArgs.DrawTargetEvent));
                }
            }
        }

        public string GetReport(string sRequest)
        {
            return BO.Output.GetMsg(sRequest);
        }

        public TBaseIniImage GetBaseIniImage()
        {
            return IniImage;
        }

        public TAdapterBO GetAdapterBO()
        {
            return BOManager.AdapterBO;
        }

        #endregion

        public virtual string ChooseNewEventName()
        {
            return FormAdapter.ChooseNewEventName();
        }
        public virtual string GetNewDocName()
        {
            return FormAdapter.GetNewDocName();
        }
        public virtual string ChooseDocAvail(TStringList SL)
        {
            return FormAdapter.ChooseDocAvail(SL);
        }
        public virtual bool ChooseDB()
        {
            return FormAdapter.ChooseDB();
        }

        public string WebStatusString
        {
            get
            {
                TStringList SL;
                int i;
                string s;

                SL = new TStringList();
                try
                {
                    i = IniImage.PortIn;
                    SL.Add(i.ToString());

                    i = IniImage.PortOut;
                    SL.Add(i.ToString());

                    i = IniImage.WebServerPort;
                    SL.Add(i.ToString());

                    i = GuiManager.Race;
                    SL.Add(i.ToString());

                    i = GuiManager.IT;
                    SL.Add(i.ToString());

                    if (PeerController.Connected)
                    {
                        SL.Add("Plugged");
                    }
                    else
                    {
                        SL.Add("Unplugged");
                    }

                    if (BO.Connected)
                    {
                        SL.Add("Connected");
                    }
                    else
                    {
                        SL.Add("Disconnected");
                    }

                    s = DocManager.DBInterface;
                    SL.Add(s);

                    s = WorkspaceInfo.WorkspaceTypeName;
                    SL.Add(s);

                    i = WorkspaceInfo.WorkspaceID;
                    SL.Add(i.ToString());

                    if (WorkspaceInfo.WorkspaceType == 5)
                    {
                        s = WorkspaceInfo.WorkspaceUrl;
                        SL.Add(s);
                    }

                    s = SL[0];
                    for (int t = 1; t < SL.Count; t++)
                    {
                        s = s + " | " + SL[t];
                    }

                    return s;

                }
                catch
                {
                    return "";
                }
            }
        }

        public void UpdateWorkspace(int WorkspaceType, int WorkspaceID)
        {
            if (WorkspaceType != WorkspaceInfo.WorkspaceType
              || WorkspaceID != WorkspaceInfo.WorkspaceID)
            {
                InitWorkspace(WorkspaceType, WorkspaceID);
            }
        }

        public void InitWorkspace(int WorkspaceType, int WorkspaceID)
        {
            if (WorkspaceManager != null)
            {
                WorkspaceManager.Init(WorkspaceType, WorkspaceID);
            }
        }

        public bool IsServerBridge => ServerBridge != null;
        public bool IsClientBridge => ClientBridge != null;

        public TServerBridge ServerBridge
        {
            get
            {
                TBridgeController BridgeController;
                IBridgeService Bridge;

                TServerBridge result = null;
                if (PeerManager.ProviderID == 2) //Bridge
                {
                    if (PeerManager.Peer is TBridgeController)
                    {
                        BridgeController = PeerController as TBridgeController;
                        if (BridgeController.ProxyType == BridgeProxyType.Server) //ServerBridge
                        {
                            Bridge = BridgeController.Bridge;
                            if (Bridge is TServerBridge)
                            {
                                result = Bridge as TServerBridge;
                            }
                        }
                    }
                }
                return result;
    }
        }

        public TClientBridge ClientBridge
        {
            get
            {
                TBridgeController BridgeController;
                IBridgeService Bridge;

                TClientBridge result = null;
                if (PeerManager.ProviderID == 2) //Bridge
                {
                    if (PeerManager.Peer is TBridgeController)
                    {
                        BridgeController = PeerController as TBridgeController;
                        //AsynchronBridge, ClientBridge, ProxyBridge, CombiBridge
                        Bridge = BridgeController.Bridge;
                        if (Bridge is TClientBridge)
                        {
                            result = Bridge as TClientBridge;
                        }
                    }
                }
                return result;
            }
        }

        public void InitPeer()
        {
            if (IsServerBridge)
            {
                AutoPlugin = true;
                AutoUpload = true;
                PeerController.Plugin();
                PeerController.Upload(BO.Save());
            }
            else
            {
                AutoPlugin = false;
                AutoUpload = false;
            }
        }

    }

}
