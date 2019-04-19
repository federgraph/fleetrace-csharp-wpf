namespace RiggVar.FR
{
    /// <summary>
    /// Zusammenfassung für WebReceiver.
    /// </summary>
    public class TWebReceiverBase
    {
        protected TStringList SL;
        protected int MsgCounterIn;
        protected int MsgCounterOut;

        public TWebReceiverBase()
        {
            SL = new TStringList();
        }

        public string Receive(string msg)
        {
            string result = "";
            MsgCounterIn++;
            if (msg != "")
            {
                SL.Text = msg;
                int Action = PopInt();
                if (Action != -1)
                {
                    result = DispatchMsg(Action);
                }
            }
            return result;
        }

        protected virtual string DispatchMsg(int Action)
        {
            //virtual
            return "";
        }

        protected void AddOp(int op)
        {
            SL.Clear();
            AddInt(op);
        }

        protected void AddStr(string s)
        {
            SL.Add(s);
        }

        protected void AddInt(int i)
        {
            SL.Add(Utils.IntToStr(i));
        }

        protected void AddBool(bool b)
        {
            SL.Add(Utils.BoolStr[b]);
        }

        protected int PopInt()
        {
            int result = -1;
            if (SL.Count > 0)
            {
                result = Utils.StrToIntDef(SL[0], -1);
                SL.Delete(0);
            }
            return result;
        }

        protected string PopStr()
        {
            string result = "";
            if (SL.Count > 0)
            {
                result = SL[0];
                SL.Delete(0);
            }
            return result;
        }

        protected bool PopBool()
        {
            bool result = false;
            if (SL.Count > 0)
            {
                result = Utils.IsTrue(SL[0]);
                SL.Delete(0);
            }
            return result;
        }

    }


}
