namespace RiggVar.FR
{
    public class TUndoAgent
    {
        private bool FUndoLock;
        public bool FUndoFlag;

        protected string UndoMsg;
        protected string RedoMsg;

        internal TMsgTree MsgTree;

        public TUndoAgent()
        {
            TInputAction InputAction = new TInputAction
            {
                OnSend = new TInputAction.ActionEvent(InputActionHandler)
            };
            TInputActionManager.UndoActionRef = InputAction;
            MsgTree = new TMsgTree(TMain.BO.cTokenA, TInputActionManager.UndoActionID);
        }        

        protected void InputActionHandler(object sender, string s)
        {
            if (UndoFlag)
            {
                UndoMsg = s;
                UndoFlag = false;
            }
            else
            {
                RedoMsg = s;
                TMain.BO.UndoManager.AddMsg(UndoMsg, RedoMsg);
            }
        }

        public bool UndoLock
        {
            get => FUndoLock || TMain.BO.Loading;
            set => FUndoLock = value;
        }

        public bool UndoFlag
        {
            get => FUndoFlag;
            set => FUndoFlag = value;
        }

    }

}
