namespace RiggVar.FR
{

    ///<summary>
    ///<para>a ServerSocket connection point for a ServerBridge</para>
    ///<para>used by ServerBridge in unit RiggVar.BridgeServer</para>
    ///<para>the Container (ServerBridge) contains
    ///      BridgeProt: TBridgeProt; (has Data/LocalBridge and implements Protocol)
    ///      BridgeServer: TBridgeServer; (for connectivity)
    ///      BridgeWeb: TBridgeWeb; (for web access, references a LocalBridge)
    ///</para>
    ///<para>the reference to BridgeProt in BridgeServer (here) is the same (instance)
    ///as that of the containing class</para>
    ///<para>BridgeServer adds connectivity to the ServerBridge</para>
    /// </summary>
    public class TBridgeNCP : TAdapterBaseNCP
    {
        public  TBridgeProt BridgeProt;

        public TBridgeNCP(TBaseServer ts) : base(ts)
        { 
        }

        public override void HandleMsg(TContextMsg cm)
        {
            if (BridgeProt != null)
            {
                cm.Answer = BridgeProt.Calc(cm.Sender, cm.msg);
                if (cm.Answer != "")
                {
                    Server.Reply(cm.Sender, cm.Answer);
                }
            }
        }

        public void Broadcast(object Sender, string s)
        {
            TContextMsg cm = new TContextMsg
            {
                MsgDirection = TMsgDirection.Outgoing,
                MsgSource = TMsgSource.ServerBridge,
                msg = s,
                Sender = Sender
            };
            //broadcast to all mit cm.Sender == null funktioniert nur, wenn von innen gesendet
            //cm.Sender = null;
            Server.SendMsg(LookupKatID.FR, cm);
        }

    }
}
