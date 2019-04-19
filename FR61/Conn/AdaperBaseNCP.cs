namespace RiggVar.FR
{

    public class TAdapterBaseNCP
    {
        public TBaseServer Server;

        public TAdapterBaseNCP(TBaseServer ts)
        {
            Server = ts;
            Server.OnHandleMsg = new TInjectMsgEvent(InjectMsg);
        }

        public virtual void InjectMsg(object sender, TMsgSource ms, string s)
        {            
            TContextMsg cm = new TContextMsg();
            cm.MsgSource = ms;
            cm.Sender = sender;
            cm.IsAdapterMsg = true;
            cm.msg = s;
            HandleMsg(cm);
        }

        public virtual void HandleMsg(TContextMsg cm)
        {
        }

    }
}
