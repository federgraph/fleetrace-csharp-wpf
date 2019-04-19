using System;
using System.Text;

namespace RiggVar.FR
{
    /// <summary>
    ///ServerBridge, alias ProxyController
    ///<list type="bullet">
    ///<item><term>1</term><description>is a 'Bridge'</description></item>
    ///<item><term>2</term><description>the inherited LocalBridge  the Data and implements the Bridge-Interface</description></item>
    ///<item><term>3</term><description>the BridgeServer add connectivity via Socket</description></item>
    ///<item><term>4</term><description>the BridgeWeb adds connectivity via Http (diagnostics/monitoring)</description></item>
    ///<item><term>5</term><description>the BridgeProt serializes and deserializes messages</description></item>
    ///</list>
    /// </summary>
    public class TServerBridge : TLocalBridge
    {
        private DateTime StartupTime;
        public TStringList SL; //wie bei AsynchronBridge f�r ausgehende msg
        TBridgeProt BridgeProt;
        TBridgeNCP BridgeNCP;
        public TBridgeWeb BridgeWeb;

        public TServerBridge(int Capacity) : base(Capacity)
        {
            StartupTime = DateTime.Now;
            SL = new TStringList();

            BridgeProt = new TBridgeProt
            {
                LocalBridge = this
            };

            TBaseServer ts;
            try
            {
                ts = ServerFactory.CreateServer(TMain.IniImage.BridgePort, TServerFunction.Bridge);
                BridgeNCP = new TBridgeNCP(ts);
                TMain.Logger.Log("BridgeNCP.Port = " + Utils.IntToStr(TMain.IniImage.BridgePort));
                BridgeNCP.BridgeProt = BridgeProt;
            }
            catch
            {
                BridgeNCP = null;
            }
            BridgeWeb = new TBridgeWeb
            {
                LocalBridge = this
            };

            BridgeProt.OnBroadcast = new THandleMsgEvent(BridgeNCP.Broadcast);
        }


        public string GetStatusString()
        {
            SLReport.Clear();
            try
            {
                SLReport.Add("StartupTime: " + StartupTime.ToString());
                SLReport.Add("DateTimeToStr(Now): " + DateTime.Now.ToString());
                SLReport.Add("BridgeWeb.Url: " + BridgeWeb.Url);
                int i = BridgeNCP.Server.ConnectionCount();
                SLReport.Add("Connections: " + Utils.IntToStr(i));
                return SLReport.Text;
            }
            catch
            {
                return "";
            }
        }

        public override void GetStatusReport(StringBuilder sb)
        {
            string crlf = Environment.NewLine;
            sb.Append("-- TServerBridge" + crlf);
            sb.Append("StartupTime: " + StartupTime.ToString() + crlf);
            sb.Append("DateTimeToStr(Now): " + DateTime.Now.ToString() + crlf);
            sb.Append("BridgeWeb.Url: " + BridgeWeb.Url + crlf);
            int i = BridgeNCP.Server.ConnectionCount();
            sb.Append("Connections: " + Utils.IntToStr(i) + crlf);
        }

        public override bool IsSameBridge()
        {
            bool result = base.IsSameBridge();

            //parameter BridgeHost is not checked because
            //the Delphi TServerSocket opens BridgePort on ip_address_any

            //return true if Port maches
            //this will prevent error: Port already open
            if (BridgeNCP.Server != null)
            {
                if (BridgeNCP.Server.Port() == TMain.IniImage.BridgePort)
                {
                    result = true;
                }
            }
            return result;
        }

        public override void SendContextMsg(int SwitchID, TContextMsg cm)
        {
            //incoming and outgoing messages are added to the msg-ring
            //this step is direction-agnostic
            //it is accomplished by calling the inherited SendMsg method
            SendMsg(SwitchID, cm.msg);

            //in addition, the embedded server-bridge must also
            //- process incoming messages and broadcast them via normal output
            //- broadcast outgoing messages to connected bridge-clients
            if (cm.IsIncomingMsg)
            {
                //process cm.msg in 'application'
                if (cm.MsgSource != TMsgSource.Bridge)
                {
                    cm.MsgSource = TMsgSource.Bridge;
                }

                TMain.Controller.InjectServerBridgeMsg(cm);
            }
            else if (cm.IsOutgoingMsg)
            {
                //broadcast client bridges
                SL.Clear();
                SL.Add("GetNewMessages");
                SL.Add(cm.msg);
                BridgeNCP.Broadcast(null, SL.Text);
                DoOnUpdateUI(UUIAction.Msg, cm.msg);
                SL.Clear();
            }
        }

        public override bool IsEnabled(SwitchOp Op)
        {
            bool isPluggedIn = PC.IsBridgeServerPluggedIn;
            switch (Op)
            {
                case SwitchOp.Plugin:
                    return ! isPluggedIn;
                case SwitchOp.Plugout:
                    return isPluggedIn;
                case SwitchOp.Synchronize:
                    return isPluggedIn;
                case SwitchOp.Upload:
                    return isPluggedIn && PC.IsMaster;
                case SwitchOp.Download:
                    return this.FBackup != null; //return isPluggedIn;
                default:
                    return false;
            }
        }

        public override int Plugin()
        {
            PC.IsBridgeServerPluggedIn = true;
            return base.Plugin();
        }

        public override void Plugout(int SwitchID)
        {
            PC.IsBridgeServerPluggedIn = false;
            base.Plugout(SwitchID);
        }

        public override void DoOnIdle()
        {
            BridgeNCP.Server.ConsumeAll();
        }

        public override void Close()
        {
            PC.Plugout();
        }

        private TPeerController PC => TMain.PeerController;

    }
}
