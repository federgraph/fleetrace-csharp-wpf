using System;
using System.Text;

namespace RiggVar.FR
{
    /// <summary>
    /// see Bridge\ClientBridge
    /// </summary>
    public class TOutputController : TPeerController
    {
        public const int clRed = 0;
        public const int clLime = 1;
        protected int LED_Brush_Color = clRed;

        private TStringList SL;
        private int MsgCounterIn;
        private int MsgCounterOut;

        private TBridgeClient ResultClient; //see BridgeClient

        public TOutputController(TBaseIniImage aIniImage) : base(aIniImage)
        {
            SL = new TStringList();
            CreateClient();
            LED_Brush_Color = clRed;
        }

        private void CreateClient()
        {
            ResultClient = new TBridgeClient(new THandleMsgEvent(this.HandleMsg));
        }

        //private void HandleConnectedChanged(object Sender)
        //{
        //    PaintLED();
        //}

        private void PaintLED()
        {
            LED_Brush_Color = ResultClient.Connected ? clLime : clRed;
        }

        public override void Close()
        {
            Plugout();
        }

        public override bool Connected => LED_Brush_Color == clLime;

        private void ConnectBtn_actionPerformed()
        {
            ResultClient.Connect(TMain.IniImage.OutputHost, TMain.IniImage.OutputPort);
            PaintLED();
        }

        private void DisconnectBtn_actionPerformed()
        {
            ResultClient.Disconnect();
            PaintLED();
        }

        public override void EditProps()
        {
            TMain.FormAdapter.EditConnectionProps(TConfigSection.csOutput);
        }

        public override void Plugin()
        {
            ConnectBtn_actionPerformed();
        }

        public override void Plugout()
        {
            DisconnectBtn_actionPerformed();
        }

        public override string Download()
        {
            if (ResultClient.Connected)
            {
                ResultClient.SendMsg("FR.*.Output.CSV.Data.B");
                MsgCounterOut++;
            }
            return ""; //asynchron, do nothing now, call DoOnBackup later
        }

        public override bool IsEnabled(SwitchOp Op)
        {
            switch (Op)
            {
                case SwitchOp.Plugin:
                    return LED_Brush_Color != clLime; //not ResultClient.Connected;
                case SwitchOp.Plugout:
                    return LED_Brush_Color == clLime; //ResultClient.Connected;
                case SwitchOp.Synchronize:
                    return  false;
                case SwitchOp.Upload:
                    return false;
                case SwitchOp.Download:
                    return LED_Brush_Color == clLime; //ResultClient.Connected;
                default:
                    return false;
            }
        }

        public override void GetStatusReport(StringBuilder sb)
        {
            sb.Append("ClassType: TOutputController" + Environment.NewLine);
            //sb.Append("Host: " + ResultClient.Client.Host);
            //sb.Append("Port: " + Utils.IntToStr(ResultClient.Client.Port));
            sb.Append("In: " + Utils.IntToStr(MsgCounterIn) + Environment.NewLine);
            sb.Append("Out: " + Utils.IntToStr(MsgCounterOut) + Environment.NewLine);
        }

        public /*override*/ void UpdateStatusBar(TStrings Memo)
        {
            Memo.Add("In: " + Utils.IntToStr(MsgCounterIn));
            Memo.Add("Out: " + Utils.IntToStr(MsgCounterOut));
        }

        protected void HandleMsg(object Sender, string msg)
        {
            TContextMsg cm = new TContextMsg();
            cm.MsgSource = TMsgSource.Bridge;
            cm.Sender = Sender;
            cm.msg = msg;
            cm.DecodeHeader();

            SL.Text = cm.msg;

            if (SL.Count == 1)
            {
                TMain.GuiManager.PlaySound(SoundID.Click02);
                TMain.BridgeBO.InjectClientBridgeMsg(cm.msg);
                MsgCounterIn++;
            }
            else if (CheckBackup())
            {
                DoOnBackup(cm.msg);
                MsgCounterIn++;
            }

            SL.Clear();
        }

        protected bool CheckBackup()
        {
            string line;

            //plausibility test - params must be present in Backup
            bool p1 = false;
            bool p2 = false;
            int i = 0;
            while (i < SL.Count - 1 && !(p1 && p2))
            {
                i++;
                line = SL[i];
                if (Utils.Pos("StartlistCount", line) > 0)
                {
                    p1 = true;
                }

                if (Utils.Pos("RaceCount", line) > 0)
                {
                    p2 = true;
                }
            }

            return p1 && p2;
        }

    }

}
