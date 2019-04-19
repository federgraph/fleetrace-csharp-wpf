using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace FR62
{
    public class InfoMemo
    {
        private string ApplicationName;
        private string ApplicationDir;
        private string FileVersion;
        private string ProductName;
        private string Description;

        public InfoMemo()
        {
        }

        public void WriteLogo(StringBuilder ML)
        {
            ML.AppendLine("");
            ML.AppendLine("-       F");
            ML.AppendLine("-      * * *");
            ML.AppendLine("-     *   *   G");
            ML.AppendLine("-    *     * *   *");
            ML.AppendLine("-   E - - - H - - - I");
            ML.AppendLine("-    *     * *         *");
            ML.AppendLine("-     *   *   *           *");
            ML.AppendLine("-      * *     *             *");
            ML.AppendLine("-       D-------A---------------B");
            ML.AppendLine("-                *");
            ML.AppendLine("-                (C) 2010-2019 federgraph.de");
            ML.AppendLine("");
        }

        public string Fill()
        {
            StringBuilder ML = new StringBuilder();

            ML.AppendLine("product name  : " + ProductName);
            ML.AppendLine("product title : " + Description);
            ML.AppendLine("---");
            ML.AppendLine("app location  : " + ApplicationDir);
            ML.AppendLine("app name      : " + ApplicationName);
            ML.AppendLine("app version   : " + FileVersion);
            ML.AppendLine("---");
            WriteLogo(ML);

            return ML.ToString();
        }

        public void Init()
        {
            string fn;
            fn = Application.ExecutablePath;

            ProductName = "RiggVar FleetRace FR";
            Description = "FR Application (Event only)";
            ApplicationName = Path.GetFileName(fn);  //Path.GetFileNameWithoutExtension(fn);
            ApplicationDir = Path.GetDirectoryName(fn);
            FileVersion =  GetFileVersion(fn);
        }

        private string GetFileVersion(string fn)
        {
            string s = FileVersionInfo.GetVersionInfo(fn).ProductVersion;
            if (! string.IsNullOrEmpty(s))
            {
                return s;
            }
            else
            {
                return "unknown";
            }
        }

    }
}
