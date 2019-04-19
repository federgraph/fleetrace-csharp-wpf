using System.Collections;
using System.Text;

namespace RiggVar.FR
{
    class MergeMsgEntry
    {
        public string Msg;

        public int SwitchID; //gets updated as messages are merged
        public int MsgID; //gets updated as messages are merged
        public string Key;

        public MergeMsgEntry(int switchID, int msgID, string msg)
        {
            SwitchID = switchID;
            MsgID = msgID;
            Msg = msg.Trim();

            int i = Msg.IndexOf('=');

            if (i > 0)
            {
                //Extract Key
                Key = Msg.Substring(0, i).Trim();

                //Trim Value
                if (i + 1 < Msg.Length)
                {
                    string v = Msg.Substring(i + 1).Trim();
                    Msg = Key + '=' + v;
                }
            }
            else
            {
                Key = string.Empty;
            }
        }

    }

    public class MergeMsgRing : IMsgRing
    {
        protected string crlf = "\r\n";
        private bool FLogValid = true;
        protected int FCapacity;
        protected ArrayList SL;
        private Hashtable Keys;
        public int FMsgID;

        public MergeMsgRing(int Capacity)
        {
            FCapacity = Capacity;
            SL = new ArrayList(Capacity);
            Keys = new Hashtable(Capacity);
        }

        public int MsgID => FMsgID;

        public bool LogValid()
        {
            return FLogValid;
        }

        public void Clear()
        {
            SL.Clear();
            Keys.Clear();
            FLogValid = true;
        }

        public void AddMsg(int SwitchID, string s)
        {
            FMsgID++;
            if (FMsgID == int.MaxValue)
            {
                FMsgID = 0;
            }

            MergeMsgEntry me = new MergeMsgEntry(SwitchID, MsgID, s);

            //keep size of list within capacity
            //todo: try to delete least recently updated key first
            if (SL.Count >= FCapacity)
            {
                FLogValid = false;
                MergeMsgEntry me1 = (MergeMsgEntry)SL[0];
                if (Keys.ContainsKey(me1.Key))
                {
                    Keys.Remove(me1.Key);
                }
                SL.RemoveAt(0);
            }

            if (Keys.ContainsKey(me.Key))
            {
                MergeMsgEntry me2 = (MergeMsgEntry)Keys[me.Key];

                //key is equal, do not update
                if (me2.Key != me.Key)
                {
                    System.Diagnostics.Debug.WriteLine("error");
                }

                me2.Msg = me.Msg; //update Msg
                me2.MsgID = me.MsgID; //update MsgID
                me2.SwitchID = me.SwitchID; //update SwitchID
            }
            else
            {
                SL.Add(me);
                Keys.Add(me.Key, me);
            }
        }

        public int GetDiffCount(int SwitchID, int StartMsgID)
        {
            int diffCount = 0;
            if (LogValid())
            {
                if (MsgID - StartMsgID <= 0)
                {
                    return 0;
                }

                for (int i = 0; i < SL.Count; i++)
                {
                    MergeMsgEntry me = (MergeMsgEntry)SL[i];
                    if (me.SwitchID != SwitchID && me.MsgID > StartMsgID)
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
                for (int i = 0; i < SL.Count; i++)
                {
                    MergeMsgEntry me = (MergeMsgEntry)SL[i];
                    if (me.SwitchID != SwitchID && me.MsgID > StartMsgID)
                    {
                        sb.Append(me.Msg);
                        sb.Append(crlf);
                    }
                }
            }
        }

    }

}
