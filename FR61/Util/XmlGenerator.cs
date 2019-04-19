using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Data;
using System.IO;

namespace RiggVar.FR
{
    public class XmlGenerator
    {
        public XmlGenerator()
        {
        }
        public string ToXml(string data)
        {
            TStrings SL = new TStringList
            {
                Text = ToDataSet(data).GetXml()
            };
            SL.Insert(0, "<?xml version=\"1.0\" ?>");
            return SL.Text;
        }
        public string GetXml(string data)
        {
            if (data != "")
            {
                XmlDocument doc = ToXmlDocument(data);
                StringWriter sw = new StringWriter();
                XmlTextWriter xw = new XmlTextWriter(sw)
                {
                    Formatting = Formatting.Indented
                };
                XmlReader xr = new XmlNodeReader(doc);
                xw.WriteNode(xr, true);
                return sw.GetStringBuilder().ToString();
            }
            else
            {
                return "";
            }
        }
        public DataSet ToDataSet(string data)
        {
            return P3(ToXmlDocument(data));
        }
        public XmlDocument ToXmlDocument(string data)
        {
            return P2(P1(data));
        }
        public string P1(string data)
        {
            string crlf = Environment.NewLine;
            TStrings SL = new TStringList();
            StringBuilder sb = new StringBuilder();

            SL.Text = data;

            int propID = 0;
            foreach (string s in SL)
            {
                string xpath = "root";

                string[] p = s.Split('=');
                if (p.Length < 2)
                {
                    continue;
                }

                string[] a = p[0].Split('.');
                if (a.Length > 0)
                {
                    a[a.Length - 1] += "=" + p[1].Trim();
                }

                foreach (string b in a)
                {
                    if (b.Trim() == "")
                    {
                        continue;
                    }

                    if (b.StartsWith("//"))
                    {
                        continue;
                    }

                    if (b.StartsWith("#"))
                    {
                        continue;
                    }

                    if (b.IndexOf("=") > 0)
                    {
                        string[] c = b.Split('=');
                        if (c.Length == 2)
                        {
                            if (c[0].Trim() != "")
                            {
                                if (c[1].Trim() != "")
                                {
                                    if (c[0].StartsWith("Prop_"))
                                    {
                                        propID++;

                                        sb.Append(xpath + "/Prop/@oID=");
                                        sb.Append(propID);
                                        sb.Append("/@K=");
                                        sb.Append(c[0].Substring("Prop_".Length).Trim());
                                        sb.Append(crlf);

                                        sb.Append(xpath + "/Prop/@oID=");
                                        sb.Append(propID);
                                        sb.Append("/@V=");
                                        sb.Append(c[1].Trim());
                                        sb.Append(crlf);
                                    }
                                    else
                                    {
                                        sb.Append(xpath + "/@" + c[0].Trim());
                                        sb.Append("=");
                                        sb.Append(c[1].Trim());
                                        sb.Append(crlf);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        string pattern = @"[0-9]+\b";
                        if (Regex.IsMatch(b, pattern, RegexOptions.ExplicitCapture))
                        {
                            Match m = Regex.Match(b, pattern);
                            int index = m.Index;
                            if (index > 0)
                            {
                                string[] d = Regex.Split(b, pattern);
                                if (d.Length > 0)
                                {
                                    xpath += "/" + d[0];
                                    string e = b.Substring(index);
                                    xpath += "/@oID=" + e;
                                }
                            }
                            else
                            {
                                xpath += "/" + b;
                            }
                        }
                        else
                        {
                            xpath += "/" + b;
                        }
                    }
                }
            }

            return sb.ToString();
        }

        public XmlDocument P2(string data)
        {
            XmlDocument doc = new XmlDocument();

            System.Xml.XmlDeclaration dec = doc.CreateXmlDeclaration("1.0", null, null);
            doc.AppendChild(dec);

            XmlElement root = doc.CreateElement("root");
            doc.AppendChild(root);

            TStrings SL = new TStringList
            {
                Text = data
            };

            try
            {
                foreach (string s in SL)
                {
                    XmlNode n;
                    bool LastA = false;
                    string NodeWithIDName = "";
                    string xpath = "root";
                    string xpathP = "root";

                    string s1 = EscapeDelimiter(s);

                    //avoid problems with xslt/xpath:
                    //* is not allowed
                    //(e.g. anonymous division-name)
                    if (s1.IndexOf("*") > 0)
                    {
                        s1 = s1.Replace("*", "C"); //* is xpath expression
                    }

                    string[] a = s1.Split('/');

                    for (int i = 0; i < a.Length; i++)
                    {
                        string b = UnEscapeDelimiter(a[i]);

                        if (b.StartsWith("root"))
                        {
                            continue;
                        }

                        if (b.StartsWith("@"))
                        {
                            string[] c = b.Split('=');
                            string aName = c[0].Substring(1);
                            string aValue = c[1];
                            if (aName == "oID")
                            {
                                string xpathA = xpath + "[" + b + "]";
                                n = doc.SelectSingleNode(xpathA);
                                if (n == null)
                                {
                                    n = doc.SelectSingleNode(xpathP);
                                    if (n != null)
                                    {
                                        XmlElement e = doc.CreateElement(NodeWithIDName);
                                        n.AppendChild(e);
                                        NodeWithIDName = "";
                                        XmlNode attr = doc.CreateNode(XmlNodeType.Attribute, aName, "");
                                        attr.Value = aValue;
                                        e.Attributes.SetNamedItem(attr);
                                        LastA = true;
                                    }
                                }
                                xpath = xpathA;
                            }
                            else
                            {
                                n = doc.SelectSingleNode(xpath);
                                if (n != null)
                                {
                                    XmlNode attr = doc.CreateNode(XmlNodeType.Attribute, aName, "");
                                    attr.Value = aValue;
                                    n.Attributes.SetNamedItem(attr);
                                }
                            }
                        }
                        else
                        {
                            n = doc.SelectSingleNode(xpath);
                            if (n != null)
                            {
                                xpathP = xpath;
                                if (LastA)
                                {
                                    xpath = "(" + xpath + ")";
                                }

                                if (b[0] >= '0' && b[0] <= '9')
                                {
                                    b = "E" + b; //Element darf nicht mit Zahl beginnen
                                }

                                xpath += "/" + b;
                                XmlNode n1 = doc.SelectSingleNode(xpath);
                                if (n1 != null)
                                {
                                    if (i < a.Length - 1 && a[i + 1].StartsWith("@oID="))
                                    {
                                        NodeWithIDName = b; //existing Node ID
                                    }
                                }
                                if (n1 == null)
                                {
                                    if (i < a.Length - 1 && a[i + 1].StartsWith("@oID="))
                                    {
                                        NodeWithIDName = b; //new Node with ID
                                    }
                                    else
                                    {
                                        XmlElement e = doc.CreateElement(b);
                                        n.AppendChild(e);
                                    }
                                }
                            }
                            LastA = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Trace.WriteLine(ex.InnerException.Message);
                }

                System.Diagnostics.Trace.WriteLine(ex.Message);
            }

            return doc;
        }
        public DataSet P3(XmlDocument doc)
        {
            XmlReader xr = new XmlNodeReader(doc);
            DataSet ds = new DataSet();
            ds.ReadXml(xr);
            return ds;
        }
        protected virtual string EscapeDelimiter(string value)
        {
            //replace this method in subclass:
            //protected static new string EscapeDelimiter(string value)
            return value;
        }
        protected virtual string UnEscapeDelimiter(string value)
        {
            //replace this method in subclass:
            //protected static new string UnEscapeDelimiter(string value)
            return value;
        }

    }

    public class FRXmlGenerator : XmlGenerator
    {
        public FRXmlGenerator()
        {
        }
        protected override string EscapeDelimiter(string s)
        {
            //hack for javascore penalties
            if (s.IndexOf("RDG/") > 0)
            {
                s = s.Replace("RDG/", "RDG:");
            }
            else if (s.IndexOf("MAN/") > 0)
            {
                s = s.Replace("MAN/", "MAN:");
            }
            else if (s.IndexOf("TIM/") > 0)
            {
                s = s.Replace("TIM/", "TIM:");
            }
            else if (s.IndexOf("TMP/") > 0)
            {
                s = s.Replace("TMP/", "TMP:");
            }
            else if (s.IndexOf("DPI/") > 0)
            {
                s = s.Replace("DPI/", "DPI:");
            }

            return s;
        }
        protected override string UnEscapeDelimiter(string b)
        {
            //reverse hack, repair javascore penalties
            if (b.IndexOf("RDG:") > 0)
            {
                b = b.Replace("RDG:", "RDG/");
            }
            else if (b.IndexOf("MAN:") > 0)
            {
                b = b.Replace("MAN:", "MAN/");
            }
            else if (b.IndexOf("TIM:") > 0)
            {
                b = b.Replace("TIM:", "TIM/");
            }
            else if (b.IndexOf("TMP:") > 0)
            {
                b = b.Replace("TMP:", "TMP/");
            }
            else if (b.IndexOf("DPI:") > 0)
            {
                b = b.Replace("DPI:", "DPI/");
            }

            return b;
        }
    }

}
