using System;
using System.Text;

namespace RiggVar.FR
{

    public class TConnection : TCollectionItem<TConnections, TConnection>
    {
        private string FAnswer;
        private THandleContextMsgEvent FOnSendMsg;
        public string Name;

        public TConnection() : base()
        {
        }

        public void InjectMsg(string s)
        {
            Server.SetMsg(this, s);
        }

        public string HandleMsg(string s)
        {
            FAnswer = string.Empty;
            Server.SetMsg(this, s);
            if (!IsOutput)
            {
                TMain.BO.OnIdle(); //--> Calc, ProcessQueue --> SetAnswer
            }

            return FAnswer;
        }

        public void HandleContextMsg(TContextMsg cm)
        {
            TMain.BO.InputServer.HandleMsg(cm);
            //Server.HandleContextMsg(cm);
            if (!IsOutput)
            {
                TMain.BO.OnIdle(); //--> Calc, ProcessQueue --> SetAnswer
            }
        }

        public void SendMsg(TContextMsg cm)
        {
            if (IsOutput && (OnSendMsg != null))
            {
                OnSendMsg(this, cm);
            }
        }

        public void SetOnSendMsg(THandleContextMsgEvent Value)
        {
            FOnSendMsg = Value;
        }

        public void SetAnswer(string s)
        {
            FAnswer = s;
        }

        public TServerIntern Server => (Collection as TConnections).Server;
        public int Port => ID;
        public bool IsOutput => Server.IsOutput;

        public THandleContextMsgEvent OnSendMsg
        {
            get => FOnSendMsg;
            set => SetOnSendMsg(value);
        }

    }

    public class TConnections : TCollection<TConnections, TConnection>
    {
        public TServerIntern Server;
        public TConnections()
            : base()
        {
        }
    }

    public class TServerIntern : TBaseServer
    {
        public TServerIntern(int aPort, TServerFunction aServerFunction)
            : base(aPort, aServerFunction)
        {
            if (aServerFunction == TServerFunction.Input)
            {
                MsgSource = TMsgSource.InternalInput;
            }
            else if (aServerFunction == TServerFunction.Output)
            {
                MsgSource = TMsgSource.InternalOutput;
            }
            else if (aServerFunction == TServerFunction.Bridge)
            {
                MsgSource = TMsgSource.Bridge;
            }
            else
            {
                MsgSource = TMsgSource.Unknown;
            }

            Connections = new TConnections
            {
                Server = this
            };
            Active = true;
        }

        public override void SendMsg(int KatID, TContextMsg cm)
        {
            for (int i = 0; i < Connections.Count; i++)
            {
                Connections[i].SendMsg(cm);
            }
        }

        public override void Reply(object Connection, string s)
        {
            for (int i = 0; i < Connections.Count; i++)
            {
                TConnection so = Connections[i];
                if (so == Connection)
                {
                    so.SetAnswer(s); //selective
                    break;
                }
            }
        }

        public void HandleContextMsg(TContextMsg cm)
        {
            TMain.BO.InputServer.HandleMsg(cm);
        }

        public TConnection Connect(string name)
        {
            TConnection c = Connections.AddRow();
            c.Name = name;
            return c;
        }

        public TConnections Connections { get; }

        public override int ConnectionCount()
        {
            return Connections.Count;
        }

        public void StatusReport(StringBuilder sb)
        {
            foreach (TConnection c in Connections)
            {
                sb.Append(c.Name + Environment.NewLine);
            }

        }
    }

}
