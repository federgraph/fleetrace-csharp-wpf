namespace RiggVar.FR
{
    public interface IBOFactory
    {
        void CreateBO(TAdapterParams BOParams);
        void DeleteBO();
    }

    public interface IBORecreator
    {
        void CreateNew(TStrings ml); //use params from ml, do not load data in ml
        void LoadNew(string data); //use params in data, then load data in data
        void RecreateBOFromBackup(); //use params and data in backup, from default location
        void RecreateBO(TAdapterParams p); //use params given, load old data
        string GetTestData(); //get built in test data
    }

    public interface IBOConnector
    {
        void ConnectBO();
        void DisconnectBO();
        bool Connected { get; }
        bool ConnectedBO { get; }
    }

    public interface IBOComputer
    {
        string Test(string EventData, bool IsWebService);
        string CalcStatefull(string EventData, bool IsWebService);
        string CalcStateless(string EventData, bool IsWebService);
    }

    public interface IBridgeBO
    {
        TBaseIniImage GetBaseIniImage();
        TConnection GetInputConnection();
        TConnection GetOutputConnection();
        void Broadcast(string s);
        string GetReport(string sRequest);
        void InjectClientBridgeMsg(string s);
    }

    public interface ISwitchBO
    {
        TBaseIniImage GetBaseIniImage();
        TAdapterBO GetAdapterBO();
    }

}
