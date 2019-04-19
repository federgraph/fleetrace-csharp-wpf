#define Switch

using System;
using System.Collections.Generic;

namespace RiggVar.FR
{

    public enum TServerFunction
    {
        Unknown,
        Input,
        Output,
        Bridge
    }

    public enum TMsgSource
    {
        Unknown,
        InternalInput,
        InternalOutput,
        UndoRedo,
        TCP,
        Switch,
        Bridge,
        ServerBridge
    }

    public enum TMsgDirection
    {
        Unknown,
        Outgoing,
        Incoming
    }

    public class TContextMsg
    {

        public const char STH = (char)4;
        public static string HeaderSepString = Convert.ToString(STH);

        public int KatID;
        public object Sender;
        public bool IsQueued;
        public bool IsAdapterMsg;
        public TMsgSource MsgSource;
        public TMsgDirection MsgDirection;
        public char MsgType;
        public string msg;
        public bool HasRequest;
        public string RequestString;
        public TStringList OutputRequestList; //initially null
        public string Answer;

        public void Clear()
        {
            Sender = null;
            IsQueued = false;
            MsgSource = TMsgSource.Unknown;
            MsgDirection = TMsgDirection.Unknown;
            MsgType = '-';
            msg = "";
            HasRequest = false;
            RequestString = "";
            OutputRequestList = null;
        }

        public void DecodeHeader()
        {
            if (msg == null)
            {
                return;
            }

            char[] sep;
            sep = new char[1];
            sep[0] = STH;
            string[] split = msg.Split(sep, 2);
            if (split.Length == 2)
            {
                string MsgHeader = split[0];
                if (MsgHeader.Length > 0)
                {
                    MsgType = MsgHeader[0];
                }

                msg = split[1];
            }
        }

        public string EncodedMsg => MsgType + HeaderSepString + msg;

        public bool IsSwitchMsg => MsgSource == TMsgSource.Switch; //&& SwitchSender != null //is plugged in//&& cm.Sender == SwitchSender

        public bool IsBridgeMsg => MsgSource == TMsgSource.Bridge;

        public bool IsOutgoingMsg
        {
            get => MsgDirection == TMsgDirection.Outgoing;
            set => MsgDirection = TMsgDirection.Outgoing;
        }

        public bool IsIncomingMsg
        {
            get => MsgDirection == TMsgDirection.Incoming;
            set => MsgDirection = TMsgDirection.Incoming;
        }
    }

    public class TContextMsgQueue
    {
        private Queue<TContextMsg> FList;

        public TContextMsgQueue()
        {
            FList = new Queue<TContextMsg>();
        }

        public void Enqueue(TContextMsg msg)
        {
            FList.Enqueue(msg);
        }

        public TContextMsg Dequeue()
        {
            if (FList.Count > 0)
            {
                return FList.Dequeue() as TContextMsg;
            }
            else
            {
                return null;
            }
        }

        public int Count()
        {
            return FList.Count;
        }

    }

    /// <summary>
    /// for routing Messages back from specific Connection to InputServer or OutputServer
    /// </summary>
    public delegate void THandleMsgEvent(object sender, string s);

    public delegate void THandleContextMsgEvent(object sender, TContextMsg cm);

    public delegate void TInjectMsgEvent(object sender, TMsgSource msgSource, string s);

    /// <summary>
    /// Abstract base class implementing IServer.
    /// Has SetMsg() method which activates OnHandleMsg delegate.
    /// A bool property IsOutput indicates if the Server class belongs to an InputServer or OutputServer.
    /// </summary>
    public class TBaseServer
    {
        public const char STX = (char)2;
        public const char ETX = (char)3;

        public static string StartString = Convert.ToString(STX);
        public static string EndString = Convert.ToString(ETX);

        public static int Status_Active = 1;
        private readonly int FPort;
        public TMsgSource MsgSource;

        public TBaseServer(int aPort, TServerFunction aServerFunction)
        {
            FPort = aPort;
            ServerFunction = aServerFunction;
            MsgSource = TMsgSource.Unknown;
        }

        public virtual int Port()
        {
            return FPort;
        }

        public virtual void SendMsg(int KatID, TContextMsg cm)
        {
        }

        public virtual void Reply(object Connection, string s)
        {
        }

        public virtual int Status()
        {
            return Active ? Status_Active : 0;
        }

        /// <summary>
        /// Actual Count of connections for reporting purposes only,
        /// may be unknown or hidden behind an interface.
        /// </summary>
        /// <returns>Count of active connections, -1 if unknown</returns>
        public virtual int ConnectionCount()
        {
            return -1;
        }

        public virtual bool IsDummy()
        {
            return true;
        }

        public virtual void ConsumeAll()
        {
        }

        public TInjectMsgEvent OnHandleMsg { get; set; }
        public TServerFunction ServerFunction { get; }

        public bool IsBridge => ServerFunction == TServerFunction.Bridge;
        public bool IsOutput => ServerFunction == TServerFunction.Output;
        public bool IsInput => ServerFunction == TServerFunction.Input;
        public bool Active { get; set; }

        public void SetMsg(object sender, string s)
        {
            OnHandleMsg?.Invoke(sender, MsgSource, s);
        }

        public void Trace(string s)
        {
            FRTrace.Trace(s);
        }
    }

}
