using System;
using System.Text;

namespace RiggVar.FR
{
    public class TProxyBridge : TClientBridge
    {
        public TProxyBridge(TBaseIniImage aIniImage) : base(aIniImage)
        {
        }

        public override bool IsEnabled(SwitchOp Op)
        {
            switch (Op) 
            {
                case SwitchOp.Upload:
                    return false;
                case SwitchOp.Download:
                    return false;
                default:
                    return base.IsEnabled(Op);
            }
        }

        protected override void DispatchMsg(string s)
        {
            if (s == "Request") 
            {
                int Target = PopInt();
                BridgeController.DoOnRequest(Target, SL.Text);
            }
            else
            {
                base.DispatchMsg(s);
            }
        }

        public override void SendAnswer(int Target, string Answer)
        {
            SL.Text = Answer;
            SL.Insert(0, "" + Target);
            SL.Insert(0, "Response");
            Send(SL.Text);
        }

        public override void GetStatusReport(StringBuilder sb)
        {
            sb.Append("-- TProxyBridge" + Environment.NewLine);
            base.GetStatusReport(sb);
        }

    }
}
