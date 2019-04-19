
namespace RiggVar.FR
{

    public class StatusCache
    {
        public static bool PeerConnected;

        public static string CurrentIT;
        public static string CurrentRace;

        public static int GridUpdateCounter;
        public static int GraphUpdateCounter;

        public static string MsgText;

        public static string PeerConnectedText
        {
            get
            {
                if (PeerConnected)
                {
                    return "P";
                }
                else
                {
                    return "U";
                }
            }
        }

        public static void Reset()
        {
            GridUpdateCounter = 0;
            GraphUpdateCounter = 0;
        }

    }

}
