using System;
using System.Text;

namespace RiggVar.FR
{
    public class TClientBridge : TAsynchronBridge
    {
        public const int clRed = 0;
        public const int clLime = 1;
        protected int LED_Color = clRed;

        protected TBridgeClient ResultClient;

        public TClientBridge(TBaseIniImage aIniImage) : base(aIniImage)
        {
            ResultClient = new TBridgeClient(new THandleMsgEvent(this.SetOutputMsg));
        }

        public override int Plugin()
        {
            Logger.AppendLine("TClientBridge.Plugin()");
            ConnectBtn_actionPerformed();
            if (ResultClient.Connected)
            {
                int temp = base.Plugin();
                BridgeController.DoOnPlugin(0);
                return 0;
            }
            else
            {
                return 0;
            }
        }

        public override void Plugout(int SwitchID)
        {
            base.Plugout(SwitchID);
            DisconnectBtn_actionPerformed();
        }

        public override bool IsEnabled(SwitchOp Op)
        {
            switch (Op)
            {
                case SwitchOp.Plugin:
                    return LED_Color != clLime;
                case SwitchOp.Plugout:
                    return LED_Color == clLime;
                case SwitchOp.Synchronize:
                    return true;
                case SwitchOp.Upload:
                    return LED_Color == clLime;
                case SwitchOp.Download:
                    return LED_Color == clLime;
                default:
                    return false;
            }
        }

        protected override void Send(string s)
        {
            if (ResultClient.Connected)
            {
                MsgCounterOut++;
                ResultClient.SendMsg(s);
            }
            else
            {
                Logger.AppendLine("  TClientBridge.Send() skipped (not connected)");
            }
        }

        public override void Close()
        {
            BridgeController.Plugout();
            //DisconnectBtn_actionPerformed(); //<-wird indirekt aufgerufen
        }

        void ConnectBtn_actionPerformed()
        {
            Logger.AppendLine(string.Format("-IniImage.BridgeHost = {0}", IniImage.BridgeHost));
            Logger.AppendLine(string.Format("-IniImage.BridgePort = {0}", IniImage.BridgePort));
            ResultClient.Connect(IniImage.BridgeHost, IniImage.BridgePort);
            paintLED();
        }

        void DisconnectBtn_actionPerformed()
        {
            ResultClient.Disconnect();
            paintLED();
        }

        private void paintLED()
        {
            LED_Color = ResultClient.Connected ? clLime : clRed;
        }

        private void StatusChanged(object sender, string s)
        {
            paintLED();
        }

        public override void GetStatusReport(StringBuilder sb)
        {
            sb.Append("-- TClientBridge" + Environment.NewLine);           
            base.GetStatusReport(sb);
        }

    }
}
