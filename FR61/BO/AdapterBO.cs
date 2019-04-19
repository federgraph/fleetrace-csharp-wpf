namespace RiggVar.FR
{

    public class TAdapterBO
    {
        public TConnection AdapterInputConnection;
        public TConnection AdapterOutputConnection;

        public TAdapterInputNCP InputServer;
        public TAdapterOutputNCP OutputServer;

        public TAdapterBO()
        {
            TBaseServer ts;
            try
            {
                ts = ServerFactory.CreateServer(TMain.IniImage.PortIn, TServerFunction.Input);
                InputServer = new TAdapterInputNCP(ts);

                ts = ServerFactory.CreateServer(TMain.IniImage.PortOut, TServerFunction.Output);
                OutputServer = new TAdapterOutputNCP(ts);
            }
            catch
            {
                InputServer = null;
                OutputServer = null;
            }
        }

        public void ReceiveMsg(object sender, TContextMsg  cm)
        {
            //todo: check, is param sender needed?

            if (AdapterOutputConnection != null)
            {
                //while processing a switch msg,
                //when multicasting to internally connected clients,
                //block the msg from going out
                if (!cm.IsSwitchMsg)
                {
                    //OutputServer.SendMsg(0, cm);

                    TGlobalWatches.Instance.MsgOut = cm.msg;
                    cm.MsgType = TSwitchController.MsgTypeInput;
                    OutputServer.Server.SendMsg(0, cm); //broadcast

                }
            }
         }
    }

}
