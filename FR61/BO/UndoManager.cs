//#define Desktop - see Project Configuration

namespace RiggVar.FR
{
    public class TUndoManager
    {
        private TBaseBO BaseBO;
        private TStringList BaseList;
        private TStringList LogList;
        private TStringList CurrentList;
        private TStringList UndoList;
        private TStringList RedoList;

        public TStringList ProxyXML; //buffer xml for debugging purpose only

        public TUndoManager(TBaseBO abo)
        {
            BaseBO = abo;

            ProxyXML = new TStringList();
            BaseList = new TStringList();
            LogList = new TStringList();
            CurrentList = new TStringList();
            UndoList = new TStringList();
            RedoList = new TStringList();
        }

        public void Clear()
        {
            UndoList.Clear();
            RedoList.Clear();
            LogList.Clear();
            CurrentList.Clear();
            UndoCount = 0;
        }

        public void AddMsg(string UndoMsg, string RedoMsg)
        {
            Trim();
            UndoList.Add(UndoMsg);
            RedoList.Add(RedoMsg);
            LogList.Add(RedoMsg);
            UndoCount = UndoList.Count;
#if Desktop
            BaseBO.Watches.Undo = UndoMsg;
            BaseBO.Watches.Redo = RedoMsg;
#endif
            //Avoid endless loop: do not broadcast messages from Switch.
            if (! MsgContext.SwitchLocked)
            {
                BaseBO.OutputServer.InjectMsg(LookupKatID.FR, TMsgSource.UndoRedo, RedoMsg);
            }
        }
        private bool BOF()
        {
            return (UndoCount < 1);
        }
        private bool EOF()
        {
            return UndoCount >= UndoList.Count; 
        }
        public int RedoCount
        {
            get { return RedoList.Count - UndoCount; }
        }
        public int UndoCount { get; private set; }
        public int LogCount => LogList.Count;

        public string Redo()
        {
            if (UndoCount >= 0 && UndoCount < RedoList.Count)
            {
                string result = RedoList[UndoCount];
                UndoCount = UndoCount + 1;
                LogList.Add(result);
                return result;
            }
            return "";
        }

        public string Undo()
        {
            if (UndoCount > 0 && UndoCount <= UndoList.Count)
            {
                UndoCount = UndoCount - 1;
                string result = UndoList[UndoCount];                
                LogList.Add(result);
                return result;
            }
            return "";
        }

        public void UpdateBase(string s)
        {
            BaseList.Text = s;
        }
        public void UpdateCurrent(string s)
        {
            CurrentList.Text = s;
        }
        public string GetBase()
        {
            return BaseList.Text;
        }
        public string GetLog()
        {
            string result = LogList.Text;
            return result == "" ? "Log list is empty." : result;
        }
        public string GetRedo()
        {
            string result = SubList(RedoList, UndoCount, RedoList.Count - 1);
            return result == "" ? "Redo sublist is empty." : result;
        }
        public string GetUndo()
        {
            string result = SubList(UndoList, 0, UndoCount-1);
            return result == "" ? "Undo sublist is empty." : result;
        }
        public string SubList(TStrings SL, int Index1, int Index2)
        {
            if (Index1 >= 0 && Index2 < SL.Count)
            {
                TStringList TL = new TStringList();
                for (int i = Index1; i <= Index2; i++)
                {
                    TL.Add(SL[i]);
                }
                return TL.Text;
            }
            return "";
        }
        public string GetUndoRedo()
        {
            string result = "";
            if (UndoList.Count > 0)
            {
                TStringList TL = new TStringList();
                for (int i = 0; i < UndoList.Count; i++)
                {
                    TL.Add(UndoList[i] + " | " + RedoList[i]);
                }
                result = TL.Text;
            }
            return result == "" ? "Undo/Redo combined list is empty." : result;
        }

        private void Trim()
        {
            for (int i = UndoList.Count - 1; i >= UndoCount; i--)
            {
                UndoList.Delete(i);
                RedoList.Delete(i);
            }
        }

    }

}
