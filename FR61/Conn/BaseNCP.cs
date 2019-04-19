namespace RiggVar.FR
{

    public class TBaseNCP
    {
        public TServerIntern Server;
        public int KatID = LookupKatID.FR;

        public TBaseNCP(TServerIntern ts)
        {
            Server = ts;
            Server.OnHandleMsg = new TInjectMsgEvent(InjectMsg);
        }

        public virtual void InjectMsg(object sender, TMsgSource ms, string s)
        {
        }

        public virtual void HandleMsg(TContextMsg cm)
        {
        }
    }

}
