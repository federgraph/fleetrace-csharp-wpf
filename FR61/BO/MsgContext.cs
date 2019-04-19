using System.Collections.Generic;

namespace RiggVar.FR
{
    public class MsgContext
    {
        public static bool SwitchLocked = false;
        public static bool BridgeLocked = false;
        public static object SwitchSender = null;

        protected string FMsgIn;
        internal int FMsgInCount;
        protected string FMsgOut;
        internal int FMsgOutCount;
        protected int FMsgOffset;

        public MsgContext()
        {
        }

        public virtual void Clear()
        {
            FMsgIn = "";
            FMsgInCount = 0;
            FMsgOut = "";
            FMsgOutCount = 0;
        }

        public virtual void Update(int LabelID)
        {
        }

        public string MsgIn
        {
            get => FMsgIn;
            set
            {
                FMsgIn = value;
                FMsgInCount++;
                if (FMsgInCount == -1)
                {
                    FMsgInCount = 1;
                }

                Update(FMsgOffset + 3);
            }
        }

        public int MsgInCount
        {
            get { return FMsgInCount; }
        }

        public string MsgOut
        {
            get => FMsgOut;
            set
            {
                FMsgOut = value;
                FMsgOutCount++;
                if (FMsgOutCount == -1)
                {
                    FMsgOutCount = 1;
                }

                Update(FMsgOffset + 5);
            }
        }
    }

    /// <summary>
    /// Singleton
    /// </summary>
    public class TGlobalWatches : MsgContext
    {
        public static TGlobalWatches Instance = new TGlobalWatches();

        private List<MsgContext> FList;

        public TGlobalWatches()
        {
            FList = new List<MsgContext>();
        }
        public void Subscribe(MsgContext Subject)
        {
            FList.Add(Subject);
        }
        public void UnSubscribe(MsgContext Subject)
        {
            FList.Remove(Subject);
        }
        public override void Update(int LabelID)
        {
            for (int i = 0; i < FList.Count - 1; i++)
            {
                (FList[i]).Update(LabelID);
            }
        }
    }

}
