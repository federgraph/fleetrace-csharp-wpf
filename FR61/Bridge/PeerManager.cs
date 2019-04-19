namespace RiggVar.FR
{
    public class TPeerManager
    {
        public MemoLogger Logger;
        private int FProviderID;
        private readonly int FMax;

        private TBaseIniImage iniImage;
        private readonly IBridgeBO bridgeBO;
        private readonly ISwitchBO switchBO;

        public TPeerManager()
        {
            Logger = new MemoLogger();
            bridgeBO = TMain.BridgeBO;
            switchBO = TMain.SwitchBO;
            iniImage = TMain.BaseIniImage;
            FMax = 3;
            Peer = new TPeerController(iniImage);
            InitPeer(iniImage.BridgeProvider);
        }

        public void Close()
        {
            Peer.Close();
        }

        public virtual TPeerController CreatePeer(int BridgeProvider)
        {
            switch (BridgeProvider)
            {
                case 1: return new TSwitchController(switchBO);
                case 2: return new TBridgeController(bridgeBO);
                case 3: return new TOutputController(iniImage);
                default: return new TPeerController(iniImage);
            }
        }

        public virtual void EditBridgeProvider()
        {
        }

        public TPeerController Peer { get; private set; }

        private void InitPeer(int value)
        {
            if (value >= 0 && value <= FMax)
            {
                Peer = CreatePeer(value);
                FProviderID = value;
            }
        }

        public int ProviderID
        {
            get => FProviderID;
            set
            {
                if (value < 0 || value > FMax)
                {
                    return;
                }

                bool NeedRecreateController = false;

                //e.g ServerBridge cannot recreated because PortCheck,  port already open
                NeedRecreateController = Peer.AllowRecreate;

                if (value == 3) //always recreate if Controller is Output
                {
                    NeedRecreateController = true;
                }

                //always recreate if ControllerType changed
                if (value != FProviderID)
                {
                    NeedRecreateController = true;
                }

                if (NeedRecreateController)
                {
                    TMain.GuiManager.DisposePeer(); //disconnect
                    Peer = CreatePeer(value);
                    FProviderID = value;
                    iniImage.BridgeProvider = value;
                    TMain.GuiManager.InitPeer(); //connect, setup OnBackup
                }
            }
        }

    }
}
