namespace RiggVar.FR
{
    public class TBridgeClient
    {
        public const int StatusConnected = 0;
        public const int StatusDisconnected = 1;
        public const int StatusUnknown = 2;
        
        public THandleMsgEvent HandleMsgMethod;
        public THandleMsgEvent HandleStatusMethod;
        private TBridgeConnection cs;
        private int FStatus;

        MemoLogger Logger => TMain.PeerManager.Logger;

        public TBridgeClient(THandleMsgEvent handler)
        {
            HandleMsgMethod = handler;
            FStatus = StatusDisconnected;
        }

        public bool Connected => FStatus == StatusConnected;

        public void Connect(string host, int port)
        {
            if (Active)
            {
                return;
            }

            Active = true;
            cs = new TBridgeConnection(this);
            Logger.AppendLine(string.Format("BridgeClient.Connect() to {0}:{1}", host, port));
            cs.Open(host, port);
        }

        public void Disconnect()
        {
            Active = false;
            if (cs != null)
            {
                cs.Close();
                cs = null;
            }
        }

        internal bool Active { get; set; }

        public bool SendMsg(string s)
        {
            if (Active && cs != null)
            {
                return cs.SendMsg(s); //sende an den Eingang vom Server
            }

            return false;
        }

        internal void HandleMsg(TBridgeConnection sc, string s)
        {                    
            if (HandleMsgMethod != null)
            {
                if (FStatus != StatusConnected)
                {
                    Logger.AppendLine(string.Format("BridgeClient.HandleMsg(), s.Length = {0}", s.Length));
                }

                HandleMsgMethod(this, s);
            }
        }

        internal void HandleStatus(TBridgeConnection sc, int status)
        {
            switch (status)
            {
                case StatusConnected: 
                    FStatus = StatusConnected; 
                    StatusCache.PeerConnected = true;
                    Logger.AppendLine("BridgeClient.HandleStatus(), Connected");
                    Logger.Notify();
                    break;
                case StatusDisconnected: 
                    Active = false;
                    FStatus = StatusDisconnected; 
                    StatusCache.PeerConnected = false;
                    Logger.AppendLine("BridgeClient.HandleStatus(), Disconnected");
                    break;
                default: 
                    FStatus = StatusUnknown; 
                    Logger.AppendLine("BridgeClient.HandleStatus(), Unknown");
                    break;
            }

            HandleStatusMethod?.Invoke(this, FStatus.ToString());
        }

    }

}
