using System.Collections.Generic;
using System.Text;

namespace RiggVar.FR
{

    public class TAdapterBufferNCP
    {
        public char StartChar = TBaseServer.STX;
        public char EndChar = TBaseServer.ETX;

        public TBaseServer Server;

        protected object lastSender = null;
        protected StringBuilder sb = new StringBuilder();
        private Dictionary<object, StringBuilder> bufferList = new Dictionary<object, StringBuilder>();

        public TAdapterBufferNCP(TBaseServer ts)
        {
            Server = ts;
            Server.OnHandleMsg = new TInjectMsgEvent(InjectMsg);
        }

        private void DeleteBuffer(object sender)
        {
            if (bufferList.ContainsKey(sender))
            {
                bufferList.Remove(sender);
            }
        }

        [System.Diagnostics.DebuggerStepThrough]
        private void GetBuffer(object sender)
        {
            if (sender == lastSender)
            {
                return;
            }

            if (sender == null)
            {
                sb = new StringBuilder();
                return;
            }

            //write back
            if (lastSender != null)
            {
                if (bufferList.ContainsKey(lastSender))
                {
                    bufferList[lastSender] = sb;
                }
            }

            //add new if not present
            if (!bufferList.ContainsKey(sender))
            {
                bufferList.Add(sender, new StringBuilder());
            }

            //read out
            sb = bufferList[sender] as StringBuilder;

            //remember Sender
            lastSender = sender;
        }

        protected void InjectMsg(object sender, TMsgSource ms, string s)
        {
            GetBuffer(sender);
            char ch;
            for (int i = 0; i < s.Length; i++)
            {
                ch = s[i];
                if (ch == StartChar)
                {
                    sb = new StringBuilder();
                }
                else if (ch == EndChar)
                {
                    TContextMsg cm = new TContextMsg
                    {
                        MsgSource = ms,
                        Sender = sender,
                        msg = sb.ToString()
                    };
                    HandleMsg(cm);
                    DeleteBuffer(sender);
                    break;
                }
                else
                {
                    sb.Append(ch);
                }
            }
        }

        protected virtual void HandleMsg(TContextMsg cm)
        {
            //virtual;
        }
    }

}
