using System.Windows;
using System.IO;
using System.Collections.Specialized;
using System.Configuration;

namespace RiggVar.FR
{
    public class PlatformDiff
    {

        public static bool ShowMsgBoxYesNo(string text, string caption)
        {
            //return (MessageBox.Show(text, caption, MessageBoxButtons.YesNo) == DialogResult.Yes); //Windows.Forms
            return (MessageBox.Show(text, caption, MessageBoxButton.YesNo) == MessageBoxResult.Yes); //WPF
            //return false; //UWP
        }

        public static void ShowMsgBox(string text)
        {
            MessageBox.Show(text);
        }

        public static string GetAppName()
        {
            return TMain.AppName;
        }

        public static string GetAppDir()
        {
            return Path.GetDirectoryName(Application.Current.StartupUri.AbsolutePath);
        }

        public static string GetConfigFileName()
        {
            //return Application.ExecutablePath + ".config"; //Windows.Forms
            return Application.Current.StartupUri.AbsolutePath + ".config"; //WPF
        }

        public static bool CreateDirectory(string dn)
        {
            return Directory.CreateDirectory(dn) != null;
        }

        public static NameValueCollection GetAppSettings()
        {
            return ConfigurationManager.AppSettings;
        }

    }
}
