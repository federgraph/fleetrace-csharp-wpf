using System;
using System.Web;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace RiggVar.FR
{
    /// <summary>
    /// BridgeHandler handles incoming requests from FR and SKK BridgeClients.
    /// </summary>
    public class BridgeHandler : IHttpHandler
    {
        private HttpRequest Request;
        private HttpResponse Response;
        private BridgeService bs;
        private string Kat;
        private string token;

        const string error = "<html><body>error</body></html>";
        const string hello = "<html><body>Hello World ({0} BridgeHandler)<body></html>";

        public void ProcessRequest(HttpContext context)
        {
            Request = context.Request;
            Response = context.Response;

            Response.ContentType = "text/html";

            try
            {
                ParseToken(Request.Path);

                if (token == string.Empty)
                {
                    Response.StatusCode = 404;
                    Response.StatusDescription = "token not found";
                    Response.Write(error);
                    return;
                }

                if (Kat == "FR")
                {
                    bs = (BridgeService)context.Application["FRBridge"];
                }
                else if (Kat == "SKK")
                {
                    bs = (BridgeService)context.Application["SKKBridge"];
                }
                else
                {
                    Response.StatusCode = 401;
                    Response.StatusDescription = "invalid path";
                    Response.Write(error);
                    return;
                }

                if (bs == null)
                {
                    Response.StatusCode = 500;
                    Response.StatusDescription = "service not found";
                    Response.Write("error");
                    return;
                }

                switch (token)
                {
                    case "HelloWorld":
                        HelloWorld(); //none interface method
                        break;
                    case "CheckForBackup":
                        CheckForBackup();
                        break;
                    case "CheckForLog":
                        CheckForLog();
                        break;
                    case "GetBackup":
                        GetBackup();
                        break;
                    case "GetBackupSwitchID":
                        GetBackupSwitchID();
                        break;
                    case "GetLastBackupID":
                        GetLastBackupID();
                        break;
                    case "GetLastMsgID":
                        GetLastMsgID();
                        break;
                    case "GetNewMessages":
                        GetNewMessages();
                        break;
                    case "LogValid":
                        LogValid();
                        break;
                    case "Plugin":
                        Plugin();
                        break;
                    case "Plugout":
                        Plugout();
                        break;
                    case "SendBackupAndLog":
                        SendBackupAndLog();
                        break;
                    case "SendDiffLog":
                        SendDiffLog();
                        break;
                    case "SendMsg":
                        SendMsg();
                        break;
                    default:
                        Response.StatusCode = 404;
                        Response.StatusDescription = "unknown token";
                        Response.Write(error);
                        break;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        public bool IsReusable => false;

        private int ParamAsInt(string name, int def)
        {
            try
            {
                int temp = int.Parse(Request[name]);
                return temp;
            }
            catch
            {
                return def;
            }
        }

        private bool IsNullOrEmpty(string s)
        {
            return s == null || s == string.Empty;
        }

        private void ParseToken(string path)
        {
            Kat = string.Empty;
            token = string.Empty;
            Regex rx = new Regex(@"/(\S+)/(\S+)/(\S+)/(\S+).aspx$");
            Match m = rx.Match(path);
            string s;
            if (m.Success && m.Groups.Count == 5)
            {
                s = m.Groups[1].Value; //REST
                s = m.Groups[2].Value; //Bridge
                Kat = m.Groups[3].Value; //[FR|SKK]
                token = m.Groups[4].Value; //Token
            }
        }

        private void HelloWorld()
        {
            Response.Write(string.Format(hello, Kat));
        }

        protected void CheckForBackup()
        {
            int SwitchID = ParamAsInt("SwitchID", -1);
            int StartBackupID = ParamAsInt("StartBackupID", -1);

            bool paramsOK = true;
            if (SwitchID < 1)
            {
                paramsOK = false;
            }

            if (StartBackupID < 0)
            {
                paramsOK = false;
            }

            bool ret = false;
            if (paramsOK)
            {
                ret = bs.CheckForBackup(SwitchID, StartBackupID);
            }
            if (ret)
            {
                Response.Write("true");
            }
            else
            {
                Response.Write("false");
            }
        }

        protected void CheckForLog()
        {
            int SwitchID = ParamAsInt("SwitchID", -1);
            int StartMsgID = ParamAsInt("StartMsgID", -1);

            bool paramsOK = true;
            if (SwitchID < 1)
            {
                paramsOK = false;
            }

            if (StartMsgID < 0)
            {
                paramsOK = false;
            }

            bool ret = false; //id of last msg in difflog
            if (paramsOK)
            {
                ret = bs.CheckForLog(SwitchID, StartMsgID);
            }
            if (ret)
            {
                Response.Write("true");
            }
            else
            {
                Response.Write("false");
            }
        }

        protected void GetBackup()
        {
            string Backup = bs.GetBackup();
            Response.Write(Backup);
        }

        protected void GetBackupSwitchID()
        {
            int i = bs.GetBackupSwitchID();
            Response.Write(i.ToString());
        }

        protected void GetLastBackupID()
        {
            int i = bs.GetLastBackupID();
            Response.Write(i.ToString());
        }

        protected void GetLastMsgID()
        {
            int i = bs.GetLastMsgID();
            Response.Write(i.ToString());
        }

        protected void GetNewMessages()
        {
            int SwitchID = ParamAsInt("SwitchID", -1);
            int StartMsgID = ParamAsInt("StartMsgID", -1);

            bool paramsOK = true;
            if (SwitchID < 1)
            {
                paramsOK = false;
            }

            if (StartMsgID < 0)
            {
                paramsOK = false;
            }

            string ret; //list of messages in the form of a multiline text string
            if (paramsOK)
            {
                ret = bs.GetNewMessages(SwitchID, StartMsgID);
            }
            else
            {
                ret = "";
            }
            Response.Write(ret);
        }

        protected void LogValid()
        {
            if (bs.LogValid())
            {
                Response.Write("true");
            }
            else
            {
                Response.Write("false");
            }
        }

        protected void Plugin()
        {
            int i = bs.Plugin();
            Response.Write(i.ToString());
        }

        protected void Plugout()
        {
            int SwitchID = ParamAsInt("SwitchID", -1);
            if (SwitchID > 0)
            {
                bs.Plugout(SwitchID);
                Response.Write("ok");
            }
            else
            {
                Response.Write("nope");
            }
        }

        protected void SendBackupAndLog()
        {
            int SwitchID = ParamAsInt("SwitchID", -1);
            string Backup = Request["Backup"];
            string Log = Request["Log"];

            bool paramsOK = true;
            if (SwitchID < 1)
            {
                paramsOK = false;
            }

            if (IsNullOrEmpty(Backup))
            {
                paramsOK = false;
            }

            if (IsNullOrEmpty(Log))
            {
                Log = "";
            }

            int ret; //id of last msg in difflog
            if (paramsOK)
            {
                ret = bs.SendBackupAndLog(SwitchID, Backup, Log);
            }
            else
            {
                ret = -1;
            }
            Response.Write(ret.ToString());
        }

        protected void SendDiffLog()
        {
            int SwitchID = ParamAsInt("SwitchID", -1);
            string DiffLog = Request["DiffLog"];

            bool paramsOK = true;
            if (SwitchID <= 0)
            {
                paramsOK = false;
            }

            if (IsNullOrEmpty(DiffLog))
            {
                paramsOK = false;
            }

            if (paramsOK)
            {
                bs.SendDiffLog(SwitchID, DiffLog);
                Response.Write("ok");
            }
            else
            {
                Response.Write("invalid params");
            }
        }

        protected void SendMsg()
        {
            int SwitchID = ParamAsInt("SwitchID", -1);
            string msg = Request["msg"];

            bool paramsOK = true;
            if (SwitchID <= 0)
            {
                paramsOK = false;
            }

            if (IsNullOrEmpty(msg))
            {
                paramsOK = false;
            }

            if (paramsOK)
            {
                bs.SendMsg(SwitchID, msg);
                Response.Write("ok");
            }
            else
            {
                Response.Write("invalid params");
            }
        }

    }
}
