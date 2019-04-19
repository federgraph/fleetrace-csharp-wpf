namespace RiggVar.FR
{
    public class TRouterNCP : TAdapterBaseNCP
    {

        public TRouterNCP(TBaseServer ts) : base(ts)
        {
        }

        private void HandleRouterMsg(TContextMsg cm)
        {
            if (DispatchProt(cm.msg))
            {
                if (TMain.BO != null)
                {
                    cm.Answer = TMain.AdapterBO.AdapterInputConnection.HandleMsg(cm.msg);
                    //auf mehrzeilige msg (mit request) antworten
                    if (cm.msg.IndexOf(".Request.") > -1)
                    {
                        Server.Reply(cm.Sender, cm.Answer);
                    }
                }
            }
        }

        private bool DispatchProt(string s)
        {
            return true;
        }

    }
}
