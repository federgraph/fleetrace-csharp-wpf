namespace RiggVar.FR
{

    public class TOutputNCP : TBaseNCP
    {
        public TOutputNCP(TServerIntern ts) : base(ts)
        {
        }

        public override void HandleMsg(TContextMsg cm)
        {
            //direkt antworten auf Anfragen am Ausgang (ohne Berechnung)
            Server.Reply(cm.Sender, TMain.BO.Output.GetMsg(cm.msg));
        }

        public override void InjectMsg(object sender, TMsgSource ms, string s)
        {
            TContextMsg cm = new TContextMsg
            {
                MsgSource = ms,
                msg = s
            };
            SendMsg(KatID, cm);
        }

        public void SendMsg(int KatID, TContextMsg cm)
        {
            TMain.BO.Watches.MsgOut = cm.msg;
            Server.SendMsg(KatID, cm); //broadcast
        }

    }
}
