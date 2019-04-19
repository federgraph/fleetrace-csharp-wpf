using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;
using System.Windows.Threading;

namespace RiggVar.FR
{

    public class ServerFactory
    {
        public static TBaseServer CreateServer(int Port, TServerFunction ServerFunction)
        {
            TBaseServer temp = new TSocketServer(Port, ServerFunction);
            if (!temp.Active)
            {
                //create Dummy if Socket cannot be opened
                temp = new TServerIntern(Port, ServerFunction);
            }
            return temp;
        }
    }

    public class TSocketServer : TBaseServer, IDisposable
    {
        public enum ServerMode
        {
            Free,
            Synchronized, //using Invoke via TMain.MainForm
            Queued //Producer/Consumer Queue to prevent thread context switch for each message
        }
        public ServerMode Mode = ServerMode.Queued;
        internal class TSocketMsg
        {
            public TSocketMsg(TSocketConnection connection, string message)
            {
                sender = connection;
                msg = message;
            }
            internal TSocketConnection sender;
            internal string msg;
        }
        internal Queue<TSocketMsg> MsgQueue;
        internal DispatcherOperation AsyncResult;

        internal void BeginInvokeIdleAction(TSocketConnection sender, string s)
        {
            MsgQueue.Enqueue(new TSocketMsg(sender, s));

            if (AsyncResult == null && TMain.IdleAction != null)
            {
                AsyncResult = TMain.MainForm.Dispatcher.BeginInvoke(TMain.IdleAction, DispatcherPriority.Normal, null);
            }
        }
        internal void EndInvokeIdleAction()
        {
            if (AsyncResult != null)
            {
                //TMain.MainForm.Dispatcher.EndInvoke(AsyncResult);
                AsyncResult = null;
            }
        }

        private TSocketConnections FConnections;
        private TcpListener FListener;
        private Thread FThread;
        public delegate void MsgHandler(string s);
        private bool disposed;

        public TSocketServer(int aPort, TServerFunction aServerFunction)
            : base(aPort, aServerFunction)
        {
            MsgSource = TMsgSource.TCP;
            FConnections = new TSocketConnections
            {
                Server = this
            };
            Active = StartListener();
            MsgQueue = new Queue<TSocketMsg>();
        }
        ~TSocketServer()
        {
            Dispose();
        }        
        public void Dispose()
        {        
            if (!disposed)
            {
                GC.SuppressFinalize(this);
                StopListener();
            }
            disposed = true;
        }
        /// <summary>
        /// broadcast
        /// </summary>
        /// <param name="s">msg to send</param>
        public override void SendMsg(int KatID, TContextMsg cm)
        {
            if (IsOutput || IsBridge)
            {
                TSocketConnection cr;
                for (int i = 0; i < FConnections.Count; i++)
                {
                    cr = FConnections[i];
                    if (KatID == 0 || cr.KatID == 0 || cr.KatID == KatID)
                    {
                        if (IsOutput)
                        {
                            //handled as Adapter Messages at destination
                            cr.SendMsg(cm.EncodedMsg);
                            //header is extracted only for Adapter messages
                        }
                        else
                        {
                            //no Header used for Bridge messages
                            if (cm.Sender != cr)
                            {
                                cr.SendMsg(cm.msg);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// send msg to one receiver
        /// </summary>
        /// <param name="Connection">receiver of msg</param>
        /// <param name="s">string to send</param>
        public override void Reply(object Connection, string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                s = "empty";
            }

            for (int i = 0; i < FConnections.Count; i++)
            {
                TSocketConnection so = FConnections[i];
                if (so == Connection)
                {
                    so.SendMsg(s); //selective
                }
            }
        }
        public override bool IsDummy()
        {
            return false;
        }
        private bool StartListener()
        {
            if (DoListen())
            {
                ThreadStart ts = new ThreadStart(DoAccept);
                FThread = new Thread(ts);
                if (this.IsOutput)
                {
                    FThread.Name = "SocketServer-Listen(Output)";
                }
                else
                {
                    FThread.Name = "SocketServer-Listen(Input)";
                }

                FThread.IsBackground = true;
                FThread.Start();
                UpdateStatus("Listener started");
                return true;
            }
            return false;
        }
        private void StopListener()
        {
            if (FListener != null)
            {
                FListener.Stop();
                FConnections.Clear();
                FListener = null;
            }
        }
        private bool DoListen()
        {
            try
            {
                IPAddress[] ipas = Dns.GetHostAddresses(Dns.GetHostName());
                try
                {
                    IPAddress ip = null;
                    //search for ipv4 first
                    foreach (IPAddress ipa in ipas)
                    {
                        if (ipa.AddressFamily == AddressFamily.InterNetwork)
                        {
                            ip = ipa;
                            break;
                        }
                    }
                    if (ip == null)
                    {
                        Debug.WriteLine("no ipv4 address available");
                        //search for ipv6
                        foreach (IPAddress ipa in ipas)
                        {
                            if (ipa.AddressFamily == AddressFamily.InterNetworkV6)
                            {
                                ip = ipa;
                                break;
                            }
                        }
                    }
                    if (ip == null)
                    {
                        return false;
                    }
                    int p = Port();
                    FListener = new TcpListener(ip, p);
                    FListener.Start();
                }
                catch(Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    return false;
                }
                return true;
            }
            catch
            {
            }
            return false;
        }
        /// <summary>
        /// DOListen will execute in a separate thread
        /// </summary>
        private void DoAccept()
        {
            try
            {
                while(true)
                {
                    TcpClient c = FListener.AcceptTcpClient();

                    TSocketConnection x = FConnections.AddRow();        
                    x.Connected += new TSocketConnection.SocketEvent(OnConnected);
                    x.Disconnected += new TSocketConnection.SocketEvent(OnDisconnected);
                    x.MsgReceived += new TSocketConnection.MsgEvent(OnMsgReceived);
                    x.Init(c);
        
                    //InvokeUpdateStatus("new Connection");
                }                
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
        private void InvokeUpdateStatus(string s)
        {
            UpdateStatus(s); //###

//            object [] p = new object[1];
//            p[0] = s;
//            this.Invoke(new MsgHandler(UpdateStatus), p);
        }
        private void UpdateStatus(string s)
        {
        }
        private void OnConnected(TSocketConnection sender)
        {
            UpdateStatus("Connected " +  sender.ID.ToString());
        }
        private void OnDisconnected(TSocketConnection sender)
        {
            UpdateStatus("DisConnected" +  sender.ID.ToString());
            sender.Delete();
        }
        private void OnMsgReceived(TSocketConnection sender, string s)
        {
            SetMsg(sender, s);
            if (!IsOutput && TMain.IsWebApp && TMain.BO != null)
            {
                TMain.BO.OnIdle(); //--> Calc, ProcessQueue --> SetAnswer
            }
        }
        public override void ConsumeAll()
        {
            TSocketMsg m;
            while (MsgQueue.Count > 0)
            {
                m = MsgQueue.Dequeue();
                OnMsgReceived(m.sender, m.msg);
            }
            EndInvokeIdleAction();
        }
        public override int ConnectionCount()
        {
            return FConnections.Count;
        }
    }

    public class TSocketConnection : TCollectionItem<TSocketConnections, TSocketConnection>
    {
        public byte StartByte;
        public byte EndByte;
        public string StartString;
        public string EndString;

        private bool useUnicode = false;
        private MemoryStream ms = new MemoryStream();
        private StringBuilder sb = new StringBuilder();
        private byte[] buffer = new byte[1024];
        private TcpClient FClient;
        private NetworkStream ns;
        private StreamWriter sw;

        public delegate void SocketEvent(TSocketConnection sender);
        public delegate void MsgEvent(TSocketConnection sender, string s);

        public event SocketEvent Connected;
        public event SocketEvent Disconnected;
        public event MsgEvent MsgReceived;

        public int KatID;
        bool isClosed = false;

        public TSocketConnection() : base()
        {
            StartByte = 2;
            EndByte = 3;
            StartString = TBaseServer.StartString;
            EndString = TBaseServer.EndString;
        }

        ~TSocketConnection()
        {
            Delete();
        }

        protected override void Dispose()
        {
            if (!isClosed)
            {
                isClosed = true;
                try
                {
                    if (sw != null)
                    {
                        sw.Close();
                        sw = null;
                    }
                    if (ns != null)
                    {
                        ns.Close();
                        ns = null;
                    }
                    if (FClient != null)
                    {
                    FClient.Close();
                    //Debug.WriteLine("~SC");
                }
                }
                catch (SocketException) 
                {
                    //Debug.WriteLine("~SE1");
                }
                catch (Exception)
                {
                    //Debug.WriteLine("~SE2");
                }
            }
            base.Dispose();
        }

        public void Init(TcpClient aClient)
        {
            FClient = aClient;
            Connected?.Invoke(this);
            ns = FClient.GetStream();
            ns.BeginRead(buffer, 0, buffer.Length, new AsyncCallback(DoReceive), null);            
        }

        public TSocketServer Server => (Collection as TSocketConnections).Server;

        public bool IsOutput => Server.IsOutput;

        private void DoReceive(IAsyncResult ar)
        {
            int intCount;
            try
            {
                lock(ns)
                {
                    intCount = ns.EndRead(ar);
                }
                if (intCount < 1)
                {
                    Disconnected(this);
                    return;
                }
                BuildString(buffer, intCount);
                lock(ns)
                {
                    ns.BeginRead(buffer, 0, buffer.Length, new AsyncCallback(DoReceive), null);
                }
            }
            catch (Exception)
            {
                Disconnected(this);
            }     
        }

        private void BuildString(byte [] bytes, int count)
        {
            byte b;
            for (int i = 0; i < count; i++)
            {
                b = bytes[i];
                if (b == StartByte)
                {
                    if (useUnicode)
                    {
                        ms = new MemoryStream(bytes);
                    }
                    else
                    {
                        sb = new StringBuilder();
                    }
                }
                else if (b == EndByte)
                {
                    try
                    {
                        string s;

                        if (!useUnicode)
                        {
                            s = sb.ToString();
                        }
                        else
                        {
                            //This constructor initializes the encoding to UTF8Encoding, 
                            //the BaseStream property using the stream parameter, 
                            //and the internal buffer to the default size.
                            //The detectEncodingFromByteOrderMarks parameter detects the 
                            //encoding by looking at the first three bytes of the stream. 
                            //It automatically recognizes UTF-8, little-endian Unicode, 
                            //and big-endian Unicode text 
                            //if the file starts with the appropriate byte order marks. 
                            //See the Encoding..::.GetPreamble method for more information.
                            using (StreamReader sr = new StreamReader(ms, true))
                            {
                                s = sr.ReadToEnd();
                            }
                        }

                        if (s.StartsWith("RiggVar.RegisterClient.SKK"))
                        {
                            KatID = LookupKatID.SKK;
                        }
                        else if (s.StartsWith("RiggVar.RegisterClient.Rgg"))
                        {
                            KatID = LookupKatID.Rgg;
                        }
                        else if (s.StartsWith("RiggVar.RegisterClient.FR"))
                        {
                            KatID = LookupKatID.FR;
                        }
                        else if (s.StartsWith("RiggVar.RegisterClient.SBPGS"))
                        {
                            KatID = LookupKatID.SBPGS;
                        }
                        else
                        {
                            switch (Server.Mode)
                            { 
                                case TSocketServer.ServerMode.Free:
                            MsgReceived(this, s);
                                    break;
                                case TSocketServer.ServerMode.Synchronized:
                                    InvokeOnMsgReceived(s);
                                    break;
                                case TSocketServer.ServerMode.Queued:
                                    //Server.MsgQueue.Enqueue(new TSocketServer.TSocketMsg(this, s));
                                    Server.BeginInvokeIdleAction(this, s);
                                    break;
                        }
                    }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                    }
                }
                else
                {
                    if (!useUnicode)
                    {
                        sb.Append(Convert.ToChar(b));
                    }
                    else
                    {
                        ms.WriteByte(b);
                    }
                }
            }
        }

        private void InvokeOnMsgReceived(string s)
        {
            object[] p = new object[1];
            p[0] = s;
            TSocketServer.MsgHandler methodToInvoke = OnMsgReceived;
            TMain.MainForm.Dispatcher.Invoke(methodToInvoke, DispatcherPriority.Normal, p);
        }

        private void OnMsgReceived(string s)
        {
            MsgReceived(this, s);
        }

        public void SendMsg(string s)
        {
            lock(ns)
            {
                //now UTF-8
                //new StreamWriter(ns, Encoding.UTF8)) //with BOM
                //new StreamWriter(ns)) //without BOM
                sw = new StreamWriter(ns);
                if (s.Length > 40 && s.IndexOf("utf-16", 30, 10) == 30)
                {
                    string s1 = s.Replace("utf-16", "utf-8");
                    sw.Write(StartString);
                    sw.Write(s1);
                    sw.Write(EndString);
                }
                else
                {
                    sw.Write(StartString + s + EndString);
                }
                sw.Flush();                
            }
        }

    }

    public class TSocketConnections : TCollection<TSocketConnections, TSocketConnection>
    {
        public TSocketServer Server;

        public TSocketConnections()
            : base()
        {
        }        

    }

}
