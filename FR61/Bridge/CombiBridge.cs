using System;
using System.Text;

namespace RiggVar.FR
{
    public class TCombiBridge : TProxyBridge
    {
        public TCombiBridge(TBaseIniImage aIniImage) : base(aIniImage)
        {
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

        public override void GetStatusReport(StringBuilder sb)
        {
            sb.Append("-- TCombiBridge" + Environment.NewLine);
            base.GetStatusReport(sb);
        }

    }
}
