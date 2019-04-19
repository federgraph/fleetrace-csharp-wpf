using System.Collections.Specialized;

namespace RiggVar.FR
{
    /// <summary>
    /// the compatible method...
    /// </summary>
    public class IniLoaderDefault : IIniLoader
    {

        protected string DataProviderInfo = @"
[DataProviderInfo]
TXT=TXT-Files
MDB=Local-DB
WEB=WEB-Service
";
        protected string ScoringProviderInfo= @"
[ScoringProviderInfo]
0=Default
1=SimpleTest
2=Inline
3=ProxyDLL
4=ProxyXML
";
        protected string BridgeProviderInfo= @"
[BridgeProviderInfo]
0=Mock
1=Switch
2=Bridge
";
        protected string BridgeProxyInfo = @"
[BridgeProxyInfo]
0=MockBridge
1=ASP.NET Web Service
2=PHP Web Service
3=ClientBridge
4=ServerBridge
5=SynchronBridge
6=RESTBridge
7=ProxyBridge
8=CombiBridge
";

        private NameValueCollection Options = new NameValueCollection();
        private NameValueCollection Bridge = new NameValueCollection();
        private NameValueCollection Connections = new NameValueCollection();
        private NameValueCollection Switch = new NameValueCollection();

        private void InitNameValueCollections()
        {

        }

        #region IniLoader Member

        public void LoadSettings(TBaseIniImage ini)
        {
        }

        public void SaveSettings(TBaseIniImage ini)
        {
        }

        #endregion

    }

}
