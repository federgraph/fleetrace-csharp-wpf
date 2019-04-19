using System;
using System.Text;
using System.Xml;
using System.IO;
using System.Collections.Generic;

namespace RiggVar.FR
{
    class StringStack
    {
        private Stack<string> stack = new Stack<string>();
        public void PushElement(string s)
        {
            stack.Push(Text + s);
        }
        public string Pop()
        {            
            if (stack.Count > 0)
            {
                return stack.Pop() as string;
            }
            else
            {
                return "";
            }
        }
        public string Text
        {
            get
            {
                if (stack.Count > 0)
                {
                    return stack.Peek() as string;
                }
                else
                {
                    return "";
                }
            }
        }
    }
    /// <summary>
    /// Creates Txt Representation from Xml.
    /// </summary>
    public class XmlConverter
    {
        internal StringStack es;
        internal StringBuilder sb;
        internal string crlf = Environment.NewLine;

        internal string currentElement = "";
        internal string currentProp = "";
        internal string currentLeaf = "";
        string currentPath;

        public XmlConverter()
        {
        }
        public string Convert(string xmlText)
        {

            if (! xmlText.StartsWith("<?xml"))
            {
                return "xml string expected";
            }

            es = new StringStack();
            sb = new StringBuilder();

            StringReader strReader = new StringReader(xmlText);
            XmlTextReader reader = new XmlTextReader(strReader);
            reader.WhitespaceHandling = WhitespaceHandling.None;

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {    
                    if (reader.Name == "root")
                    {
                        continue;
                    }

                    currentElement = reader.Name;
                    currentLeaf = reader.Name;

                    if (reader.HasAttributes)
                    {    
                        currentPath = es.Text;

                        string a;

                        if (currentElement == "Prop")
                        {
                            //must read K-Attribute before V-Attribute
                            a = reader.GetAttribute("K");
                            if (a != null && a.Length > 0)
                            {
                                currentProp = a;
                            
                                //read V next
                                a = reader.GetAttribute("V");
                                if (a != null && a.Length > 0)
                                {
                                    //sb.Append(currentPath);
                                    sb.Append("EP.");
                                    sb.Append(currentProp);
                                    sb.Append("=");
                                    sb.Append(a); //reader.Value);
                                    sb.Append(crlf);
                                }
                                continue; //nothing else to read in Prop-Element
                            }
                        }

                        //must read oID first
                        a = reader.GetAttribute("oID");
                        if (a != null && a.Length > 0)
                        {
                            currentLeaf = currentElement + a;
                            //must read Attribute before pushing to stack
                            //do not push if Element is Table row (oID + at least one more attribute)
                            if (reader.AttributeCount == 1)
                            {
                                es.PushElement(currentLeaf + '.');
                                //sb.Append(crlf);
                                continue;
                            }
                        }

                        //normal Attributes
                        reader.MoveToFirstAttribute();
                        ProcessAttribute(reader);
                        while (reader.MoveToNextAttribute())
                        {
                            ProcessAttribute(reader);
                        }
                    }
                    else //if (! reader.HasAttributes)
                    {
                        es.PushElement(currentElement + '.');
                    }
                }    
                if (reader.NodeType == XmlNodeType.EndElement)
                {    
                    es.Pop();
                }
            }
            reader.Close();
            return sb.ToString();
        }
        private void ProcessAttribute(XmlTextReader reader)
        {
            if (reader.Name == "oID") //is in currentLeaf (TableName + RowID)
            {
            }
            else
            {
                sb.Append(currentPath);//does not include current table
                sb.Append(currentLeaf); //TableName + oID
                sb.Append('.');
                sb.Append(reader.Name); //Command
                sb.Append("=");
                sb.Append(reader.Value); //Attribute Value / Field
                sb.Append(crlf);
            }
        }
    }
}
