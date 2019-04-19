using System.Collections;
using System.Text;

namespace RiggVar.FR
{

    public interface IMsgRing
    {
        void AddMsg(int SwitchID, string s);
        int GetDiffCount(int SwitchID, int StartMsgID);
        void GetDelta(int SwitchID, StringBuilder sb, int StartMsgID);
        bool LogValid();
        int MsgID { get; }
        void Clear();
    }

    class MsgEntry
    {
        public int SwitchID;
        public string Msg;

        public MsgEntry(int switchID, string msg)
        {
            SwitchID = switchID;
            Msg = msg;
        }

    }

    public class MsgRing : IMsgRing
    {
        protected string crlf = "\r\n";
        private bool FLogValid = true;
        protected int FCapacity;
        protected ArrayList SL;
        public int FMsgID;

        public MsgRing(int Capacity)
        {
            FCapacity = Capacity;
            SL = new ArrayList(Capacity); 
        }

        public int MsgID => FMsgID;

        public bool LogValid()
        {
            return FLogValid;
        }

        public void Clear()
        {
            SL.Clear();
            FLogValid = true;
        }

        public void AddMsg(int SwitchID, string s)
        {
            if (s == null || s == string.Empty)
            {
                return;
            }

            FMsgID++;
            if (FMsgID == int.MaxValue)
            {
                FMsgID = 0;
            }

            if (SL.Count > FCapacity)
            {
                SL.RemoveAt(0);
                FLogValid = false;
            }
            SL.Add(new MsgEntry(SwitchID, s));
        }

        public int GetDiffCount(int SwitchID, int StartMsgID)
        {
            int diffCount = 0;
            if (LogValid())
            {
                int msgCount = MsgID - StartMsgID;
                if (msgCount <= 0)
                {
                    return 0;
                }

                int startValue = SL.Count - msgCount;
                if (startValue < 0)
                {
                    startValue = 0;
                }

                if (startValue > SL.Count - 1)
                {
                    return 0;
                }

                for (int i = startValue; i < SL.Count; i++)
                {
                    MsgEntry me = (MsgEntry)SL[i];
                    if (me.SwitchID != SwitchID)
                    {
                        diffCount++;
                    }
                }
            }
            return diffCount;
        }

        public void GetDelta(int SwitchID, StringBuilder sb, int StartMsgID)
        {
            if (LogValid())
            {
                int msgCount = MsgID - StartMsgID;
                if (msgCount <= 0)
                {
                    return;
                }

                int startValue = SL.Count - msgCount;
                if (startValue < 0)
                {
                    startValue = 0;
                }

                if (startValue > SL.Count - 1)
                {
                    return;
                }

                for (int i = startValue; i < SL.Count; i++)
                {
                    MsgEntry me = (MsgEntry) SL[i];
                    if (me.SwitchID != SwitchID)
                    {
                        sb.Append(me.Msg);
                        sb.Append(crlf);
                    }
                }
            }
        }

    }

}
