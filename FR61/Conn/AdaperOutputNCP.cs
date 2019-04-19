namespace RiggVar.FR
{

    public class TAdapterOutputNCP : TAdapterBaseNCP
    {

        public TAdapterOutputNCP(TBaseServer ts) : base(ts)
        {
        }

        public override void HandleMsg(TContextMsg cm)
        {
            //direkt antworten auf Anfragen am Ausgang (ohne Berechnung)
            Server.Reply(cm.Sender, TMain.BO.Output.GetMsg(cm.msg));
        }

        //public void SendMsg(int KatID, TContextMsg cm)
        //{
        //    TGlobalWatches.Instance.MsgOut = cm.msg;
        //    cm.MsgType = TSwitchController.MsgTypeInput;
        //    Server.SendMsg(KatID, cm); //broadcast
        //}

    }
}
