using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace FR62.Tabs
{

    public class EventMenu02 : IEventMenu
    {
        private readonly string DefaultXml = @"
    <EventMenu>
      <ComboEntry Caption=""Athen 2004"">
        <DataFolder Url=""http://gsmac/CubeImages/Results05/"" >
          <ImgFolder Url=""http://gsmac/CubeImages/Images02/"">
            <Btn Data=""Event-01.xml"" Img=""Seite-1.png"" Text=""49er"" />
            <Btn Data=""Event-02.xml"" Img=""Seite-2.png"" Text=""470 W"" />
            <Btn Data=""Event-03.xml"" Img=""Seite-3.png"" Text=""470 M"" />
            <Btn Data=""Event-04.xml"" Img=""Seite-4.png"" Text=""Europe"" />
            <Btn Data=""Event-05.xml"" Img=""Seite-5.png"" Text=""Finn"" />
            <Btn Data=""Event-06.xml"" Img=""Seite-6.png"" Text=""Laser"" />
          </ImgFolder>
          <ImgFolder Url=""http://gsmac/CubeImages/Images03/"">
            <Btn Data=""Event-07.xml"" Img=""Seite-1.png"" Text=""Mistral M"" />
            <Btn Data=""Event-08.xml"" Img=""Seite-2.png"" Text=""Mistral W"" />
            <Btn Data=""Event-09.xml"" Img=""Seite-3.png"" Text=""Star"" />
            <Btn Data=""Event-10.xml"" Img=""Seite-4.png"" Text=""Tornado"" />
            <Btn Data=""Event-11.xml"" Img=""Seite-5.png"" Text=""Yngling"" />
          </ImgFolder>
        </DataFolder>
      </ComboEntry>
    </EventMenu>
        ";

        class BtnInfo
        {
            public string Data;
            public string Img;
            public string Text;
        }

        public string root;
        public string comboCaption;

        List<BtnInfo> Info;

        public EventMenu02()
        {
            Info = new List<BtnInfo>();
        }

        public void LoadTestData(Uri uri)
        {
            XElement xe = XElement.Parse(DefaultXml);
            string r = ParseRoot(xe);
            ParseComboEntry(xe.Element("ComboEntry"), r);
        }

        public static string ParseRoot(XElement xe)
        {
            if (xe != null)
            {
                if (xe.HasAttributes)
                {
                    if (xe.Attribute("Root") != null)
                    {
                        return xe.Attribute("Root").Value;
                    }
                }
            }

            return string.Empty;
        }

        public void ParseComboEntry(XElement xe, string r)
        {
            try
            {
                root = r;
                comboCaption = xe.Attribute("Caption").Value;
                ParseDataFolder(xe.Element("DataFolder"));
            }
            catch (Exception ex)
            {
                Info.Clear();
                System.Diagnostics.Debug.WriteLine(ex.Message);
            } 
        }

        /// <summary>
        /// Load Info from xml.
        /// </summary>
        /// <param name="xe">DataFolder element</param>
        public void ParseDataFolder(XElement xe)
        {
            BtnInfo bi;
            Uri DataUri;
            Uri ImgUri = null;

            DataUri = BuildUri(xe.Attribute("Url"));
            foreach(XElement xf in xe.Elements("ImgFolder"))
            {
                ImgUri = BuildUri(xf.Attribute("Url"));
                foreach(XElement xb in xf.Elements("Btn"))
                {
                    bi = new BtnInfo
                    {
                        Text = xb.Attribute("Text").Value,
                        Data = new Uri(DataUri, xb.Attribute("Data").Value).ToString()
                    };
                    if (ImgUri != null)
                    {
                        bi.Img = new Uri(ImgUri, xb.Attribute("Img").Value).ToString();
                    }

                    Info.Add(bi);
                }
            }
        }

        private Uri BuildUri(XAttribute a)
        {
            if (a != null)
            {
                string s = a.Value;
                if (!string.IsNullOrEmpty(s))
                {
                    if (string.IsNullOrEmpty(root) || s.StartsWith("http://"))
                    {
                        return new Uri(s);
                    }
                    else
                    {
                        return new Uri(root + s);
                    }
                }
            }
            return null;
        }

        public void Load(string data)
        {
            try
            {
                XElement xe = XElement.Parse(data);
                string r = ParseRoot(xe);
                ParseComboEntry(xe.Element("ComboEntry"), r);
            }
            catch (Exception ex)
            {
                Info.Clear();
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

        public string ComboCaption => comboCaption;

        public int Count => Info.Count;

        public string GetCaption(int i)
        {
            if (i < 0)
            {
                return "no selection";
            }
            else if (i > 0 && i <= Count)
            {
                return Info[i - 1].Text;
            }
            else
            {
                return "B" + i;
            }
        }

        public string GetImageUrl(int i)
        {
            return i > 0 && i <= Count ? Info[i - 1].Img : string.Empty;
        }

        public string GetDataUrl(int i)
        {
            return i > 0 && i <= Count ? Info[i - 1].Data : string.Empty;
        }

        public bool IsMock()
        {
            return false;
        }

    }

}
