using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Diagnostics;

namespace RiggVar.FR
{
    /// <summary>
    /// is part of TMain.Controller,
    /// has methods Plugin, Plugout, Download, Upload, Synchronize.
    /// </summary>
    public class TSwitchController : TPeerController
    {
        public const string SwitchTokenConnect = "connect";
        public const string SwitchTokenDisconnect = "disconnect";
        public const string SwitchTokenUpload = "upload";
        public const string SwitchTokenDownload = "download";
        public const string SwitchTokenData = "data";
        public const string SwitchTokenSynchronize = "synchronize";

        public const int PluginTypeNone = 0; //-, --
        public const int PluginTypeServer = 1; //S, IO, Serve, InputOutput
        public const int PluginTypeInput = 2; //I, IC, Input, InputClient
        public const int PluginTypeOutput = 3; //O, OC, Output, OutputClient
        public const int PluginTypeCacheClient = 4; //C, CC, Cache, CacheClient
        public const int PluginTypeMultilineInput = 5; //M, MI, Muliline, MultilineInput
        public const int PluginTypeBatchRequest = 6; //B, BR, Batch, BatchRequest

        public const char MsgTypeUnspecified = '-';
        public const char MsgTypeInput = 'I';
        public const char MsgTypeOutput = 'O';
        public const char MsgTypeRegister = '+';
        public const char MsgTypeCommand = 'C';
        public const char MsgTypeRequest = 'R';

        public const char MsgType2Unspecified = '-';
        public const char MsgType2Undo = 'U';
        public const char MsgType2Redo = 'R';
        public const char MsgType2MultiLine = 'M';
        public const char MsgType2Request = 'Q';

        public const char MsgCreatorUnspecified = '-';
        public const char MsgCreatorLoad = 'L';
        public const char MsgCreatorRestore = 'R';
        public const char MsgCreatorMuliLineInput = 'M';
        public const char MsgCreatorAdapter = 'A';
        public const char MsgCreatorInput = 'I';
        public const char MsgCreatorDataSender = 'D';

        public const char MsgCategoryUnspecified = '-';
        public const char MsgCategoryParameter = 'P';
        public const char MsgCategoryProperty = 'E';
        public const char MsgCategoryCommand = 'C';
        public const char MsgCategoryTest = 'T';
        public const char MsgCategoryInput = 'I';
        public const char MsgCategoryRequest = 'R';

        //public bool PlugTouched;

        private TBaseIniImage IniImage;
        private TAdapterBO AdapterBO;
        public ISwitchBO SwitchBO;

        public TSwitchController(ISwitchBO aSwitchBO) : base(aSwitchBO.GetBaseIniImage())
        {
            SwitchBO = aSwitchBO;
            IniImage = SwitchBO.GetBaseIniImage();
            AdapterBO = SwitchBO.GetAdapterBO();
        }
        public override void Close()
        {
            if (PlugTouched)
            {
                Plug(SwitchTokenDisconnect);
            }
        }
        private void Plug(string mOperation)
        {
            string mHost;
            int mPortIn;
            int mPortOut;
            int mEventType;
            string m;

            if (AdapterBO != null)
            {
                mHost = GetLocalHostName();
                mPortIn = AdapterBO.InputServer.Server.Port();
                mPortOut = AdapterBO.OutputServer.Server.Port();
                mEventType = IniImage.EventType;

                string c = ",";
                StringBuilder sb = new StringBuilder(mHost);
                sb.Append(c);
                sb.Append(mPortIn);
                sb.Append(c);
                sb.Append(mPortOut);
                sb.Append(c);
                sb.Append(PluginTypeServer);
                sb.Append(c);
                sb.Append(mOperation);
                sb.Append(c);
                sb.Append(mEventType);

                m = sb.ToString();

                SendMsg(IniImage.SwitchHost, IniImage.SwitchPort, m, false, 2000);
            }        
        }
        public override bool IsMaster
        {
            get { return IniImage.IsMaster; }
        }
        public override bool Connected        
        {
            get { return PlugTouched && (MsgContext.SwitchSender != null); }
        }
        public override void Plugin()
        {
            PlugTouched = true;
            Plug(SwitchTokenConnect);
        }
        public override void Plugout()
        {
            PlugTouched = false;
            Plug(SwitchTokenDisconnect);
        }
        public override void Synchronize()
        {
            if (Connected)
            {
                AdapterBO.InputServer.Server.Reply(MsgContext.SwitchSender, SwitchTokenSynchronize);
            }
        }
        public override void Upload(string s)
        {
            if (Connected)
            {
                AdapterBO.InputServer.Server.Reply(MsgContext.SwitchSender, SwitchTokenUpload);
                AdapterBO.InputServer.Server.Reply(MsgContext.SwitchSender, s);
            }
        }
        /// <summary>
        /// Variante mit System.IO.WebRequest
        /// </summary>
        /// <returns>backup data</returns>
        public override string Download()
        {
            string result = "";
            try
            {
                UriBuilder ub = new UriBuilder("http", IniImage.SwitchHost, IniImage.SwitchPortHTTP, "data");
                ub.Query = "EventType=" + IniImage.EventType.ToString(); 
                WebRequest myRequest = WebRequest.Create(ub.Uri);        
                WebResponse myResponse = myRequest.GetResponse();
                try
                {
                    StreamReader sr = new StreamReader(myResponse.GetResponseStream());
                    result = sr.ReadToEnd();
                }
                finally
                {
                    myResponse.Close();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
            return result;
        }

        private string SendMsg(string Host, int Port, string Msg, bool WaitForResult, int Timeout)
        {    
            string result = "";

            if (Host == "")
            {
                return "";
            }

            if (Port <= 0)
            {
                return "";
            }

            if (Timeout <= 10)
            {
                Timeout = 1000;
            }

            try
            {
                TcpClient client = new TcpClient();
                client.SendTimeout = Timeout;
                client.Connect(Host, Port);
                byte[] data = System.Text.Encoding.ASCII.GetBytes((char)2 + Msg + (char)3);
                NetworkStream stream = client.GetStream();
                stream.Write(data, 0, data.Length);


                        if (WaitForResult)
                        {
                    //not implemented yet

                    //// Buffer to store the response bytes.
                    //data = new Byte[256];
                    //// String to store the response ASCII representation.
                    //String responseData = String.Empty;
                    //// Read the first batch of the TcpServer response bytes.
                    //Int32 bytes = stream.Read(data, 0, data.Length);
                    //responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
                    //Debug.WriteLine("Received: {0}", responseData);
                }


                // Close everything.
                stream.Close();
                client.Close();
            }
            catch (Exception e)
            {    
                Debug.WriteLine(e.Message);
            }                    
            return result;
        }

        private string GetLocalHostName()
        {
            if (IniImage.UseRouterHost)
            {
                return IniImage.RouterHost;
            }

            return Environment.MachineName;
        }

        public override void EditProps()
        {
            TMain.FormAdapter.EditSwitchProps(TMain.BaseIniImage);
            //TFormSwitchProps.EditSwitchProps(this, IniImage);
        }

        public override bool IsEnabled(SwitchOp Op)
        {
            switch (Op)
            {
                case SwitchOp.Plugin: return true;
                case SwitchOp.Plugout: return true;
                case SwitchOp.Synchronize: 
                    return IsMaster;
                case SwitchOp.Upload: 
                    return Connected && IsMaster;
                case SwitchOp.Download: return true;
                default: return false;
            }
        }

        public override void GetStatusReport(StringBuilder sb)
        {
            sb.Append("ClassType: TSwitchController" + Environment.NewLine);
            TSwitchItem cr = new TSwitchItem();
            cr.InitFromCurrent();
            cr.Show(sb);
        }

    }
}