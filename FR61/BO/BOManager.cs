namespace RiggVar.FR
{

    public class TBOManager : IBOFactory, IBOConnector
    {
        public TAdapterBO AdapterBO;
        public TBO BO;

        public TBOManager()
        {
        }
    
        public void CreateAdapterBO()
        {
            TBOParams BOParams;

            BOParams = new TBOParams
            {
                IsAdapter = true
            };

             AdapterBO = new TAdapterBO();
            
            FRTrace.Trace("InputServer.Port := " + Utils.IntToStr(AdapterBO.InputServer.Server.Port()) + ";");
            FRTrace.Trace("OutputServer.Port := " + Utils.IntToStr(AdapterBO.OutputServer.Server.Port()) + ";");
        }

        #region IBOFactory

        public void CreateBO(TAdapterParams aBOParams)
        {
            TBOParams BOParams = new TBOParams();
            BOParams.Assign(aBOParams);
            BOParams.ForceWithinLimits();
            BOParams.IsAdapter = false;
            BO = new TSDIBO(BOParams);
            TMain.BO = BO;
        }

        public void DeleteBO()
        {
            if (BO != null)
            {
                if (ConnectedBO)
                {
                    DisconnectBO();
                }

                BO.Dispose(true);
                BO = null;
                TMain.BO = null;
            }
        }

        #endregion

        #region IBOConnector

        public void ConnectBO()
        {
            if (AdapterBO != null)
            {
                AdapterBO.AdapterInputConnection = BO.InputServer.Server.Connect("Adapter.Input");
                AdapterBO.AdapterOutputConnection = BO.OutputServer.Server.Connect("Adapter.Output");
                AdapterBO.AdapterOutputConnection.SetOnSendMsg(new THandleContextMsgEvent(AdapterBO.ReceiveMsg));
                ConnectedBO = true;
            }
            else
            {
                DisconnectBO();
            }
        }

        public void DisconnectBO()
        {
            if (AdapterBO != null)
            {
                if (AdapterBO.AdapterInputConnection != null)
                {
                    AdapterBO.AdapterInputConnection.Delete();
                }

                if (AdapterBO.AdapterOutputConnection != null)
                {
                    AdapterBO.AdapterOutputConnection.Delete();
                }

                AdapterBO.AdapterInputConnection = null;
                AdapterBO.AdapterOutputConnection = null;
            }
            ConnectedBO = false;
        }

        public bool Connected
        {
            get
            {
                if (AdapterBO != null)
                {
                    return ((AdapterBO.InputServer.Server.Status() == TBaseServer.Status_Active)
                        && (AdapterBO.OutputServer.Server.Status() == TBaseServer.Status_Active));
                }
                else
                {
                    return false;
                }
            }
        }

        public bool ConnectedBO { get; private set; }

        #endregion

    }

}
