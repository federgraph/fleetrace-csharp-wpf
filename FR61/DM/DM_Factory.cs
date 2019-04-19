
namespace RiggVar.FR
{

    public class DBEventFactory
    {
        public static IDBEvent GetDBEvent(string DBType)
        {
            switch (DBType)
            {
                case "TXT": return new DM.TDBEventTXT();
                case "MDB": return new DM.TDBEventMDB();
                case "WEB": return new DM.TDBEventWEB();
                case "SVC": return new DM.TRemoteWorkspace();
                case "REST": return new DM.TDBEventREST();
                default: return new TDBEvent();
            }
        }
    }

}
