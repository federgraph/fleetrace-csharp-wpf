namespace RiggVar.FR
{
  public interface IWatchGUI 
  {
    void Show();
    void Hide();
    bool IsNew();
    bool IsVisible();
    void UpdateFormCaption(string Title, string EventName);
    void InitLabel(int LabelID, string Caption);
    void UpdateValue(int LabelID, string Content);
  }

    public class TLocalWatches : MsgContext
    {
        private IWatchGUI FormWatches;
        private string FUndo;
        private string FRedo;

        public TLocalWatches()
        {
            FMsgOffset = 4;
            TGlobalWatches.Instance.Subscribe(this);
        }
        public void Dispose()
        {
            TGlobalWatches.Instance.UnSubscribe(this);
        }
        public override void Clear()
        {
            base.Clear();
            FUndo = "";
            FRedo = "";
        }
        public IWatchGUI WatchGUI
        {
            get => FormWatches;
            set => FormWatches = value;
        }
        public void Init()
        {
            if (FormWatches != null)
            {
                FormWatches.InitLabel(1, "Undo");
                FormWatches.InitLabel(2, "Redo");
                FormWatches.InitLabel(3, "AdapterMsgIn");
                FormWatches.InitLabel(4, "AdapterMsgInCount");
                FormWatches.InitLabel(5, "AdapterMsgOut");
                FormWatches.InitLabel(6, "AdapterMsgOutCount");
                FormWatches.InitLabel(7, "MsgIn");
                FormWatches.InitLabel(8, "MsgInCount");
                FormWatches.InitLabel(9, "MsgOut");
                FormWatches.InitLabel(10, "MsgOutCount");
            }
        }
        public void Show(string EventName)
        {
            if (FormWatches != null)
            {
                FormWatches.Show();
                if (FormWatches.IsNew())
                {
                    Init();
                    if (EventName != "")
                    {
                        FormWatches.UpdateFormCaption("FR Watches", EventName);
                    }
                }
                UpdateAll();
            }
        }        
        public override void Update(int LabelID)
        {
            if (FormWatches != null && FormWatches.IsVisible())
            {
                switch (LabelID)
                {
                    case 1: 
                        FormWatches.UpdateValue(1, Undo);
                        break;
                    case 2: 
                        FormWatches.UpdateValue(2, Redo);
                        break;
                    case 3:    
                        FormWatches.UpdateValue(3, TGlobalWatches.Instance.MsgIn);
                        FormWatches.UpdateValue(4, TGlobalWatches.Instance.FMsgInCount.ToString());
                        break;   
                    case 5:    
                        FormWatches.UpdateValue(5, TGlobalWatches.Instance.MsgOut);
                        FormWatches.UpdateValue(6, TGlobalWatches.Instance.FMsgOutCount.ToString());
                        break;   
                    case 7:    
                        FormWatches.UpdateValue(7, MsgIn);
                        FormWatches.UpdateValue(8, FMsgInCount.ToString());
                        break;
                    case 9:    
                        FormWatches.UpdateValue(9, MsgOut);
                        FormWatches.UpdateValue(10, FMsgOutCount.ToString());
                        break;
                }
            }
        }
        public void UpdateAll()
        {
            for (int i = 1; i <= 10; i++)
            {
                Update(i);
            }
        }
        public string Undo
        {
            get => FUndo;
            set
            {
                FUndo = value;
                Update(1);
            }
        }
        public string Redo
        {
            get => FRedo;
            set
            {
                FRedo = value;
                Update(2);
            }
        }
    }

}
