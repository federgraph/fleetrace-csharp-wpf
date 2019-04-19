using System;
using System.Collections;

namespace RiggVar.FR
{
    public class RESTConnection
    {
        protected int fEventType = 400;
        protected string fFileExtension = ".aspx";
        protected string fServerUrl;
        protected string fApplicationPath;

        //protected const string DefaultApplicationPath = "/FR88/";
        protected const string DefaultHostName = "localhost";
        protected const string DefaultControllerUrl = "http://localhost/FR88/";

        UriBuilder ub = null;

        internal TStringList SL = new TStringList();
        internal Hashtable HT = new Hashtable();
        //Hashtable <String, String> HT = new Hashtable<String, String>();

        HttpBridgeClient httpBridgeClient = new HttpBridgeClient();

        public RESTConnection(int eventType)
        {
            fEventType = eventType;
            SetServerUrl(DefaultControllerUrl);
        }

        public string GetServerUrl()
        {
            return ub != null ? fServerUrl : DefaultControllerUrl;
        }

        public void SetServerUrl(string value)
        {
            try
            {                
                ub = !value.EndsWith("/") ? new UriBuilder(value + "/") : new UriBuilder(value);
                ApplicationPath = ub.Path;
                fServerUrl = ub.ToString();
            }
            catch (Exception)
            {
            }
        }

        protected string GetPagePath(string pageName)
        {
            if (EventType == 400)
            {
                return ApplicationPath + "FR/" + pageName + FileExtension;
            }
            else if (EventType == 600)
            {
                return ApplicationPath + "SKK/" + pageName + FileExtension;
            }
            else
            {
                return ApplicationPath + pageName + FileExtension;
            }
        }

        public string ApplicationPath
        {
            get => fApplicationPath;
            set
            {
                if (value.EndsWith("/"))
                {
                    fApplicationPath = value;
                }
                else
                {
                    fApplicationPath = value + "/";
                }
            }
        }

        public string HostName => ub != null ? ub.Host : DefaultHostName;

        public int Port => ub != null && ub.Port != -1 ? ub.Port : 80;

        public string FileExtension
        {
            get => fFileExtension;
            set => fFileExtension = value;
        }

        public int EventType => fEventType;

        public string Get(string pageName)
        {
            string s = GetPagePath(pageName);
            ub.Path = s;
            s = ub.ToString();
            return httpBridgeClient.Get(ub);
        }

        public string Post(string pageName)
        {
            string s = GetPagePath(pageName);
            ub.Path = s;
            return httpBridgeClient.Post(ub, SL);
        }

        public string PostMultiPart(string pageName)
        {
            string s = GetPagePath(pageName);
            ub.Path = s;
            return httpBridgeClient.PostMultiPart(ub, HT);
        }

    }
}
