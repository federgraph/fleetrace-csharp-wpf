namespace RiggVar.FR
{
    public class TStammdatenRowCollection : TBaseRowCollection<
        TStammdatenColGrid,
        TStammdatenBO,
        TStammdatenNode,
        TStammdatenRowCollection,
        TStammdatenRowCollectionItem,
        TStammdatenColProps,
        TStammdatenColProp
        >
    {
        private TStringList MapList = new TStringList();
        private TStringList FieldCaptionList = new TStringList();

        public const int FixFieldCount = 6;
        private int fieldCount = FixFieldCount;

        public TStammdatenRowCollection()
            : base()
        {
            FieldMap = "SN";
        }

        public void UpdateItem(TStammdatenEntry e)
        {
            TStammdatenRowCollectionItem o = FindKey(e.SNR);
            if (o == null)
            {
                o = AddRow();
            }

            o.AssignEntry(e);
        }

        public TStammdatenRowCollectionItem FindKey(int SNR)
        {
            for (int i = 0; i < Count; i++)
            {
                TStammdatenRowCollectionItem o = this[i];
                if (o != null && o.SNR == SNR)
                {
                    return o;
                }
            }
            return null;
        }

        public void CalcDisplayName(TStammdatenRowCollectionItem cr)
        {
            if (MapList.Count < 1)
            {
                return;
            }

            string s, t;

            t = "";
            for (int i = 0; i < MapList.Count; i++)
            {
                s = MapList[i];
                if (s == FieldNames.FN || s == "FN")
                {
                    t += cr.FN;
                }
                else if (s == FieldNames.LN || s == "LN")
                {
                    t += cr.LN;
                }
                else if (s == FieldNames.SN || s == "SN")
                {
                    t += cr.SN;
                }
                else if (s == FieldNames.NC || s == "NC")
                {
                    t += cr.NC;
                }
                else if (s == FieldNames.GR || s == "GR")
                {
                    t += cr.GR;
                }
                else if (s == FieldNames.PB || s == "PB")
                {
                    t += cr.PB;
                }
                else if (s == "_" || s == "Space")
                {
                    t += " ";
                }
                else if (s == "-")
                {
                    t += " - ";
                }
                else if (s == "Slash")
                {
                    t += " / ";
                }
                else if (s == "Komma")
                {
                    t += ", ";
                }
                else
                {
                    t += s;
                }
            }
            cr.DN = t;
        }

        public string FieldMap
        {
            get
            {
                string s = MapList.CommaText;
                if (s == "")
                {
                    s = "SN";
                }

                return s;

            }
            set
            {
                if (value != null && value != "")
                {
                    MapList.CommaText = value;
                }
                else
                {
                    MapList.CommaText = "SN";
                }

                foreach (TStammdatenRowCollectionItem cr in this)
                {
                    CalcDisplayName(cr);
                }
            }
        }

        public string GetFieldCaption(int index)
        {
            //'FieldCaptions' EventProp excluding SNR, starting with N1,N2,N3,...
            //e.g 'EP.FieldCaptions=FN1' maps 'FN1' as Caption of col_FN at index 1)
            if (index > 0 && index <= FieldCaptionList.Count)
            {
                string s = FieldCaptionList.Strings(index-1);
                if (s != null && s != "")
                {
                    return s;
                }
            }
            switch (index)
            {
                case 0: return "SNR";
                case 1: return FieldNames.FN;
                case 2: return FieldNames.LN;
                case 3: return FieldNames.SN;
                case 4: return FieldNames.NC;
                case 5: return FieldNames.GR;
                case 6: return FieldNames.PB;
            }
            return "N" + index.ToString();
        }

        public string FieldCaptions
        {
            get => FieldCaptionList.CommaText;
            set => FieldCaptionList.CommaText = value;
        }

        public int FieldCount
        {
            get => fieldCount;
            set
            {
                fieldCount = value;
                TMain.BO.AdapterParams.FieldCount = value;
            }
        }

        public int SchemaCode
        {
            get => FieldNames.SchemaCode;
            set => FieldNames.SchemaCode = value;
        }

        public void Swap(int f1, int f2)
        {
            TStammdatenRowCollectionItem cr;
            string s;
            if (f1 != f2
                && f1 > 0
                && f2 > 0
                && f1 <= FieldCount
                && f2 <= FieldCount)
            {
                for (int i = 0; i < Count; i++)
                {
                    cr = this[i];
                    s = cr[f1];
                    cr[f1] = cr[f2];
                    cr[f2] = s;
                }
            }
        }

        public void GetXML(TStrings SL)
        {
            SL.Add("<e xmlns=\"http://riggvar.net/Entries.xsd\">");
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
            SL.Add("<xs:schema id=\"e\" targetNamespace=\"http://riggvar.net/Entries.xsd\"");
            SL.Add("xmlns:mstns=\"http://riggvar.net/Entries.xsd\"");
            SL.Add("xmlns=\"http://riggvar.net/Entries.xsd\"");
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
            SL.Add("<xs:attribute name=\"Id\" form=\"unqualified\" type=\"xs:positiveInteger\" />");
            SL.Add("<xs:attribute name=\"SNR\" form=\"unqualified\" type=\"xs:positiveInteger\" />");
            for (int r = 1; r <= FieldCount; r++)
            {
                SL.Add("<xs:attribute name=\"N" + r.ToString() + "\" form=\"unqualified\" type=\"xs:string\" />");
            }
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
            TStammdatenRowCollectionItem cr;
            for (int i = 0; i < Count; i++)
            {
                cr = this[i];
                s = "<r Id=\"" + cr.BaseID.ToString() + "\" ";
                s = s + "SNR=\"" + cr.SNR.ToString() + "\" ";
                for (int r = 1; r < FieldCount; r++)
                {
                    s = s + "N" + r.ToString() + "=\"" + cr[r] + "\" ";
                }
                s = s + "/>";
                SL.Add(s);
            }
        }

        public void GetXMLBackup(TStrings SL)
        {
            TStringList tempSL;
            TStammdatenNode ru;
            string s;
            string sName, sValue;

            tempSL = new TStringList();
            ru = (TStammdatenNode)BaseNode;

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

    }

}
