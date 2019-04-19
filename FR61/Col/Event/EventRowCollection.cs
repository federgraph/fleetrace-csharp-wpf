using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace RiggVar.FR
{
    public class TEventRowCollection : TBaseRowCollection<
        TEventColGrid,
        TEventBO,
        TEventNode,
        TEventRowCollection,
        TEventRowCollectionItem,
        TEventColProps,
        TEventColProp
        >
    {
        public TEventRowCollection()
            : base()
        {
        }

        public string GetHashString()
        {
            StringBuilder sb = new StringBuilder();
            TEventRowCollectionItem cr;
            for (int i = 0; i < Count; i++)
            {
                cr = this[i];
                if (i == 0)
                {
                    sb.Append(cr.GRank);
                }
                else
                {
                    sb.Append('-');
                    sb.Append(cr.GRank);
                }
            }
            return sb.ToString();
        }

        public void WriteXml(XmlWriter xw)
        {
            xw.WriteStartElement("EventRowCollection");
            TEventRowCollectionItem cr;
            for (int i = 0; i < Count; i++)
            {
                cr = this[i];
                cr.WriteXml(xw);
            }
            xw.WriteEndElement();
        }

        public void GetXML(TStrings SL)
        {
            SL.Add("<e xmlns=\"http://riggvar.net/FR11.xsd\">");
            SL.Add("");
            GetXMLSchema(SL);
            SL.Add("");
            GetXMLResult(SL);
            SL.Add("");
            GetXMLBackup(SL);
            SL.Add("");
            SL.Add("</e>");
        }

        public void GetXMLSchema(TStrings SL)
        {
            SL.Add("<xs:schema id=\"e\" targetNamespace=\"http://riggvar.net/FR11.xsd\"");
            SL.Add("xmlns:mstns=\"http://riggvar.net/FR11.xsd\"");
            SL.Add("xmlns=\"http://riggvar.net/FR11.xsd\"");
            SL.Add("xmlns:xs=\"http://www.w3.org/2001/XMLSchema\"");
            SL.Add("xmlns:msdata=\"urn:schemas-microsoft-com:xml-msdata\"");
            SL.Add("attributeFormDefault=\"qualified\"");
            SL.Add("elementFormDefault=\"qualified\">");
            SL.Add("");
            SL.Add("<xs:element name=\"e\" msdata:IsDataSet=\"true\" msdata:Locale=\"de-DE\" msdata:EnforceConstraints=\"False\">");
            SL.Add("<xs:complexType>");
            SL.Add("<xs:choice maxOccurs=\"unbounded\">");
            SL.Add("");
            SL.Add("<xs:element name=\"r\">");
            SL.Add("<xs:complexType>");
            SL.Add("<xs:attribute name=\"ID\" form=\"unqualified\" type=\"xs:positiveInteger\" />");
            SL.Add("<xs:attribute name=\"Pos\" form=\"unqualified\" type=\"xs:positiveInteger\" />");
            SL.Add("<xs:attribute name=\"Bib\" form=\"unqualified\" type=\"xs:positiveInteger\" />");
            SL.Add("<xs:attribute name=\"SNR\" form=\"unqualified\" type=\"xs:positiveInteger\" />");
            SL.Add("<xs:attribute name=\"DN\" form=\"unqualified\" type=\"xs:string\" />");
            for (int r = 1; r < RCount; r++)
            {
                SL.Add("<xs:attribute name=\"W" + r.ToString() + "\" form=\"unqualified\" type=\"xs:string\" />");
            }
            SL.Add("<xs:attribute name=\"Pts\" form=\"unqualified\" type=\"xs:double\" />");
            SL.Add("</xs:complexType>");
            SL.Add("</xs:element>");
            SL.Add("");
            SL.Add("<xs:element name=\"b\">");
            SL.Add("<xs:complexType>");
            SL.Add("<xs:attribute name=\"I\" form=\"unqualified\" type=\"xs:positiveInteger\" />");
            SL.Add("<xs:attribute name=\"N\" form=\"unqualified\" type=\"xs:string\" />");
            SL.Add("<xs:attribute name=\"V\" form=\"unqualified\" type=\"xs:string\" />");
            SL.Add("</xs:complexType>");
            SL.Add("</xs:element>");
            SL.Add("");
            SL.Add("</xs:choice>");
            SL.Add("</xs:complexType>");
            SL.Add("</xs:element>");
            SL.Add("</xs:schema>");
        }

        public void GetXMLResult(TStrings SL)
        {
            string s;
            TEventRowCollectionItem cr;
            for (int i = 0; i < Count; i++)
            {
                cr = this[i];
                s = "<r ID=\"" + cr.BaseID.ToString() + "\" ";
                s = s + "Pos=\"" + cr.GRank.ToString() + "\" ";
                s = s + "Bib=\"" + cr.Bib.ToString() + "\" ";
                s = s + "SNR=\"" + cr.SNR.ToString() + "\" ";
                s = s + "DN=\"" + cr.DN + "\" ";

                for (int r = 1; r < RCount; r++)
                {
                    s = s + "W" + r.ToString() + "=\"" + cr[r] + "\" "; //RaceValue Indexer
                }
                s = s + "Pts=\"" + cr.GPoints + "\" ";
                s = s + "/>";
                SL.Add(s);
            }
        }

        public void GetXMLBackup(TStrings SL)
        {
            TStringList tempSL;
            TEventNode ru;
            string s;
            string sName, sValue;

            tempSL = new TStringList();
            ru = (TEventNode)BaseNode;

            TMain.BO.BackupToSL(tempSL, false);

            int j = 0;
            for (int i = 0; i < tempSL.Count; i++)
            {
                s = tempSL[i];

                if ((s == string.Empty) || (s[0] == '<'))
                {
                    sName = string.Empty;
                    sValue = string.Empty;
                }
                else if ((s[0] == '#') || (Utils.Copy(s, 1, 2) == "//"))
                {
                    sName = s;
                    sValue = string.Empty;
                }
                else
                {
                    sName = tempSL.Names(i).Trim();
                    sValue = tempSL.ValueFromIndex(i).Trim();
                }

                j++;
                s = "<b I=\"" + j.ToString() + "\" N=\"" + sName + "\" V=\"" + sValue + "\" />";
                tempSL[i] = s;
            }

            for (int i = 0; i < tempSL.Count; i++)
            {
                SL.Add(tempSL[i]);
            }
        }

        public void UpdateItem(TEventEntry e)
        {
            TEventRowCollectionItem o = FindKey(e.SNR);
            if (o == null)
            {
                o = AddRow();
            }

            if (o != null)
            {
                o.Assign(e);
            }
        }

        public TEventRowCollectionItem FindKey(int SNR)
        {
            for (int i = 0; i < Count; i++)
            {
                TEventRowCollectionItem o = this[i];
                if (o != null && o.SNR == SNR)
                {
                    return o;
                }
            }
            return null;
        }

        public int RCount
        {
            get
            {
                if (Count > 0)
                {
                    return this[0].RCount; //Items[0].RCount
                }
                else if (BaseNode is TEventNode)
                {
                    return ((TEventNode)BaseNode).RaceCount + 1;
                }
                else
                {
                    return -1;
                }
            }
        }

        public int FleetCount(int r)
        {
            int temp;
            int fc = 0;
            for (int i = 0; i < Count; i++)
            {
                temp = this[i].Race[r].Fleet;
                if (temp > fc)
                {
                    fc = temp;
                }
            }
            return fc;
        }

        public void FillFleetList(List<TEventRowCollectionItem> FL, int r, int f)
        {
            FL.Clear();
            if (r > 0 && r < RCount)
            {
                TEventRowCollectionItem cr;
                for (int i = 0; i < Count; i++)
                {
                    cr = this[i];
                    if (cr.Race[r].Fleet == f)
                    {
                        FL.Add(cr);
                    }
                }
            }
        }

    }

}
