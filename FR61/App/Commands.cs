using System.Windows.Input;

namespace RiggVar.FR
{

    public static class Commands
    {
        static Commands()
        {
            Plugin = new RoutedUICommand("Plugin", "Plugin", typeof(Commands));
            Plugout = new RoutedUICommand("Plugout", "Plugout", typeof(Commands));
            Synchronize = new RoutedUICommand("Synchronize", "Synchronize", typeof(Commands));
            Upload = new RoutedUICommand("Upload", "Upload", typeof(Commands));
            Download = new RoutedUICommand("Download", "Download", typeof(Commands));
        }


        public static RoutedUICommand Plugin
        {
            get; 
            private set;
        }

        public static RoutedUICommand Plugout
        {
            get;
            private set;
        }

        public static RoutedUICommand Synchronize
        {
            get;
            private set;
        }

        public static RoutedUICommand Upload
        {
            get;
            private set;
        }

        public static RoutedUICommand Download
        {
            get;
            private set;
        }

    }

}
