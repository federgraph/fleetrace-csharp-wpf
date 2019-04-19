using System;
using RiggVar.FR;
using System.IO;
using System.Windows.Controls;

namespace FR62.Tabs
{
    internal class WorkspaceList : TWorkspaceListBase
    {
        private const string StrWorkspaceList = "fr-workspace-urls.txt";

        private bool FStop;
        private string FK;
        private TWorkspaceUrl FV;
        private string FError;
        private readonly int FCounter;
        private TStringList NL;

        public WorkspaceList() : base()
        {
            FCounter++;
            NL = new TStringList(); 
        }

        public bool WantNames { get; set; }

        private void InitLocal()
        {
            try
            {
                string s;
                string fn = GetWorkspaceListFileName();
                if (fn != "")
                {
                    TStringList WL = new TStringList();
                    WL.LoadFromFile(fn);
                    for (int i = 0; i < WL.Count; i++)
                    {
                        s = WL[i].Trim();
                        if (s == "stop")
                        {
                            FStop = true;
                            break;
                        }


                        if (s == "" || s.StartsWith("//") || s.StartsWith("#"))
                        {
                            continue;
                        }

                        ParseLine(s);

                        switch (FV.GetScheme())
                        {
                            case UrlScheme.Http:
                                AddEntry();
                                break;
                            case UrlScheme.File:
                                if (File.Exists(FV.Value))
                                {
                                    AddEntry();
                                }

                                break;
                            case UrlScheme.App:
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                FError = ex.Message;
            }
        }

        private string GetHomeDir()
        {
            string dn = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            return dn;
        }

        private string GetWorkspaceListFileName()
        {
            string dn;
            string fn;
            dn = GetHomeDir();
            if (dn != "")
            {
                //fn = IncludeTrailingPathDelimiter(dn) + StrWorkspaceList;
                fn = Path.Combine(dn, StrWorkspaceList);
                if (File.Exists(fn))
                {
                    return fn;
                }
            }
            return "";
        }

        private void ParseLine(string s)
        {
            //int i = s.IndexOf("=", StringComparison.OrdinalIgnoreCase);
            //if (i > 0)
            //{
            //    FK = s.Substring(0, i - 1).Trim(); //Trim(Copy(s, 1, i-1));
            //    FV.Value = s.Substring(i + 1).Trim(); //Trim(Copy(s, i+1, Length(s)));
            //}
            //else
            //{
            //    FK = "";
            //    FV.Value = s.Trim();
            //}

            //char [] sep = {'='};
            string [] sa = s.Split('=');
            if (sa.Length == 2)
            {
                FK = sa[0].Trim();
                FV.Value = sa[1].Trim();
            }
            else
            {
                FK = "";
                FV.Value = s.Trim();
            }
        }

        private void AddEntry()
        {
            NL.Add(FK);
            VL.Add(FV.Value);
        }

        protected override void AddUrl(string s)
        {
            ParseLine(s);
            AddEntry();
        }

        public override void Init()
        {
            InitLocal();
            if (!FStop)
            {
                InitDefault();
            }
        }

        public override void Load(ItemCollection Combo)
        {
            Combo.Clear();
            //Assert(NL.Count == VL.Count);
            //Assert(NL.Count > 0);
            string s;
            for (int i = 0; i < NL.Count; i++)
            {
                if (WantNames && NL[i] != "")
                {
                    s = NL[i];
                }
                else
                {
                    s = VL[i];
                }

                Combo.Add(s);
            }
        }

        public override string GetName(int i)
        {
            if (i >= 0 && i < NL.Count)
            {
                return NL[i];
            }

            return "";
        }

        public override string GetUrl(int i)
        {
            if (i >= 0 && i < VL.Count)
            {
                return VL[i];
            }

            return "";
        }

        public override bool IsWritable(int i)
        {
            return GetName(i).StartsWith("*");
        }

    }
}
