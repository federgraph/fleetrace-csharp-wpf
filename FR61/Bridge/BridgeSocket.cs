using System;
using System.Text;
using System.Net.Sockets;
using System.Windows.Threading;
using System.Diagnostics;
using System.IO;

namespace RiggVar.FR
{
    public class TBridgeConnection
    {
        MemoLogger Logger;
        public const char STX = (char)2;
        public const char ETX = (char)3;        

        public byte StartByte = 2; //wenn empfangen wird
        public byte EndByte = 3; //wenn empfangen wird

        public string StartString; //wenn gesendet wird
        public string EndString; //wenn gesendet wird

        protected TBridgeClient Controller;

        public static int NewID;
        public int ID;

        private TcpClient FClient;
        private NetworkStream ns;
        private byte[] buffer = new byte[1024];
        private StringBuilder sb = new StringBuilder();
        private bool isClosed;

        public TBridgeConnection(TBridgeClient controller)
        {
            Logger = TMain.PeerManager.Logger;

            StartString = Convert.ToString(STX);
            EndString = Convert.ToString(ETX);

            NewID++;
            ID = NewID;
            Controller = controller;
        }

        public void Close()
        {    
            if (!isClosed)
            {
                try
                {
                    if (ns != null)
                    {
                        //ms-help://MS.MSDNQTR.2005JAN.1033/enu_kbnetframeworkkb/netframeworkkb/821625.htm
                        ns.Close();
                    }
                    FClient.Close();            
                    OnDisconnected();
                    FClient = null;
                }
                catch (Exception)
                {
                    Logger.AppendLine("TBridgeConnection.Close(), Exception");
                }
                finally
                {
                    isClosed = true;
                }
            }
        }

        private void OnConnected()
        {
            Controller.HandleStatus(this, TBridgeClient.StatusConnected);
        }

        private void OnDisconnected()
        {
            Controller.Active = false;
            Controller.HandleStatus(this, TBridgeClient.StatusDisconnected);
        }

        private void InvokeOnMsgReceived(string s)
        {
            if (!isClosed)
            {
                object[] p = new object[1];
                p[0] = s;
                TSocketServer.MsgHandler methodToInvoke = OnMsgReceived;
                TMain.MainForm.Dispatcher.Invoke(methodToInvoke, DispatcherPriority.Normal, p);
            }
            else
            {
                Logger.AppendLine("TBridgeConnection.InvokeOnMsgReceived(), msg ignored because closed.");
            }
        }

        private void OnMsgReceived(string s)
        {
            if (!isClosed)
            {
                Controller.HandleMsg(this, s);
            }
            else
            {
                Logger.AppendLine("TBridgeConnection.OnMsgReceived(), msg ignored because closed.");
            }
        }

        public void Open(string host, int port)
        {
            try
            {
                FClient = new TcpClient();
                FClient.Connect(host, port);
                ns = FClient.GetStream();
                if (InitRead())
                {
                    OnConnected();
                }
                else
                {
                    OnDisconnected();
                }
            }
            catch (Exception ex)
            {
                Logger.AppendLine("BridgeConnection.Open(), Exception:");
                Logger.AppendLine(ex.Message);
                Logger.AppendEmptyLine();
                OnDisconnected();
            }
        }

        private bool InitRead()
        {
            bool isOK = false;
            lock(ns)
            {
                if (!isClosed && ns.CanRead)
                {            
                    ns.BeginRead(buffer, 0, buffer.Length, new AsyncCallback(DoReceive), null);            
                    isOK = true;
                }
            }
            return isOK;
        }

        /// <summary>
        /// Callback Routine, 
        /// wird vom .net Framework aufgerufen,
        /// wenn etwas empfangen wurde,
        /// im Context eines Threads aus dem Thread-Pool!
        /// </summary>
        /// <param name="ar"></param>
        private void DoReceive(IAsyncResult ar)
        {
            if (FClient == null)
            {
                return;
            }

            try
            {
                int c = 0;
                lock(ns)
                {
                    if (ns.CanRead)
                    {
                        c = ns.EndRead(ar);
                    }
                    else
                    {
                        Debug.WriteLine("~");
                    }
                }
                if (c > 0)
                {
                    BuildString(buffer, c); //übernehmen...
                    if (!InitRead()) //wieder anstoßen / im Hintergrund weiter empfangen...
                    {
                        OnDisconnected();
                    }
                }
            }
            catch (ObjectDisposedException)
            {
                Debug.WriteLine("#");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                OnDisconnected();
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
                    //Anfangszeichen der Msg wurde empfangen
                    sb = new StringBuilder();
                }
                else if (b == EndByte || b == ETX) // || b == Environment.NewLine[0])
                {
                    //Endezeichen der Msg wurde empfangen --> Msg verarbeiten
                    try
                    {
                        InvokeOnMsgReceived(sb.ToString());
                    }
                    catch (Exception ex)
                    {
                        Logger.AppendLine("BridgeConnection.BuildString(), Exception:");
                        if (ex.InnerException != null)
                        {
                            ex = ex.InnerException;
                        }

                        Logger.AppendLine(ex.Message);
                        Logger.AppendLine(ex.StackTrace);
                        Logger.AppendEmptyLine();
                    }
                    sb = new StringBuilder();                    
                }        
                else
                {
                    //ein Inhaltszeichen der Msg wurde empfangen
                    sb.Append(Convert.ToChar(b));
                }
            }
        }

        public bool SendMsg(string s)
        {
            //lock(ns)
            {
                try
                {
                    if (ns.CanWrite)
                    {
                        StreamWriter w = new StreamWriter(ns);
                        w.Write(StartString + s + EndString);
                        w.Flush();
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    if (ex.Message != "")
                    {
                        Logger.AppendLine("BridgeConnection.SendMsg(), Exception:");
                        Logger.AppendLine(ex.Message);
                    }
                    OnDisconnected();
                }
                return false;
            }
        }

    }

}
