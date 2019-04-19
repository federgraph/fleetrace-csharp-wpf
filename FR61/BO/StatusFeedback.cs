using System;
using System.Text;
using System.Net.Sockets;
using System.Diagnostics;

namespace RiggVar.FR
{
    public class TStatusFeedback : TLineParser
    {
        private UdpClient udp;
        private TBO BO;

        public bool Enabled = false;
        public string Host = "localhost";
        public int Port = 6001;

        public TStatusFeedback(TBO abo)
        {
            BO = abo;
        }

        public void Dispose()
        {
            if (udp != null)
            {
                try
                {
                    udp.Close();
                    udp = null;
                }
                catch
                {
                }
            }
        }

        protected override bool ParseKeyValue(string Key, string Value)
        {

            if (Utils.Pos("Manage.", Key) == 1)
            {
                Key = Utils.Copy(Key, ("Manage.").Length + 1, Key.Length);
            }

            if (Key == "StatusTrigger")
            {
                BO.OnIdle();
                SendStatus(BO.GetHashString());
            }
            else if (Key == "Clear")
            {
                BO.ClearCommand();
            }
            else if (Key == "Feedback.Host")
            {
                Host = Value;
            }
            else if (Key == "Feedback.Port")
            {
                Port = Utils.StrToIntDef(Value, Port);
            }
            else
            {
                return false;
            }

            return true;
        }

        public void SendStatus(string msg)
        {
            if (Enabled)
            {
                try
                {
                    if (udp == null)
                    {
                        udp = new UdpClient();
                    }
                    byte[] sendBytes = Encoding.ASCII.GetBytes(msg);
                    udp.Send(sendBytes, sendBytes.Length, Host, Port);
                }
                catch (Exception e)
                {
                    Enabled = false;
                    if (udp != null)
                    {
                        udp.Close();
                        udp = null;
                    }
                    Debug.WriteLine(e.ToString());
                }
            }

        }

    }

}
