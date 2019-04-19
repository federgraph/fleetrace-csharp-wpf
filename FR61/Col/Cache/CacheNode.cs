using System;
using System.Collections.Generic;

namespace RiggVar.FR
{
    public class TCacheNode : TBaseNode<
        TCacheColGrid,
        TCacheBO,
        TCacheNode,
        TCacheRowCollection,
        TCacheRowCollectionItem,
        TCacheColProps,
        TCacheColProp
        >
    {

        public int RaceCount;
        public int ITCount;
        public int Age;
        public int Generation;

        public TCacheNode()
            : base()
        {
            RaceCount = 3;
            ITCount = 2;
            Age = 1;
        }

        public void Load()
        {
            Collection.Clear();
            Generation++;

            TCacheRowCollectionItem o;

            //GetParams (must be first line!)
            o = Collection.AddRow();
            o.Report = -1;
            o.Race = 0;
            o.IT = 0;
            o.Sort = 0;
            o.Mode = 0;
            o.TimeStamp = DateTime.Now;

            //xml-messages
            o = Collection.AddRow();
            o.Report = 1001;
            o.Request = "XML.Data.A";
            o.AddXmlHeader = true;
            o.IsXml = true;

            o = Collection.AddRow();
            o.Report = 1002;
            o.Request = "XML.Data.E";
            o.AddXmlHeader = true;
            o.IsXml = true;

            o = Collection.AddRow();
            o.Report = 1003;
            o.Request = "JavaScore.ProxyXmlInput";
            o.AddXmlHeader = true;
            o.IsXml = true;

            o = Collection.AddRow();
            o.Report = 1004;
            o.Request = "JavaScore.ProxyXmlOutput";
            o.AddXmlHeader = true;
            o.IsXml = true;

            o = Collection.AddRow();
            o.Report = 1005;
            o.Request = "JavaScore.XML";
            o.AddXmlHeader = false;
            o.IsXml = true;

            o = Collection.AddRow();
            o.Report = 1006;
            o.Request = "RiggVar.TXT";
            o.AddXmlHeader = true;
            o.IsXml = true;

            //EventReports
            for (int s = 0; s <= RaceCount + 8; s++) //SpaltenAnzahl
            {
                o = Collection.AddRow();
                o.Report = 11;
                o.Race = 0;
                o.IT = 0;
                o.Sort = s;
                o.Mode = 0;
                o.TimeStamp = DateTime.Now;

                o = Collection.AddRow();
                o.Report = 11;
                o.Race = 0;
                o.IT = 0;
                o.Sort = s;
                o.Mode = 1;
                o.TimeStamp = DateTime.Now;
            }

            //RaceReports
            for (int r = 1; r <= RaceCount; r++)
            {
                for (int i = 0; i <= ITCount; i++)
                {
                    o = Collection.AddRow();
                    o.Report = 105;
                    o.Race = r;
                    o.IT = i;
                    o.Sort = 0;
                    o.Mode = 0;
                    o.TimeStamp = DateTime.Now;
                }
            }
        }

        public List<string> RequestComboStrings
        {
            get
            {
                List<string> l = new List<string>();
                foreach (TCacheRowCollectionItem cr in Collection)
                {
                    l.Add(cr.Request);
                }
                return l;
            }
        }

    }

}
