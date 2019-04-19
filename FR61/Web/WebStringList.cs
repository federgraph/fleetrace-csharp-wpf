using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RiggVar.FR
{
    public class TWebStringList
    {

        public const string crlf = "\r\n";
        public static char[] NameValueSep = { '='};
        public static string[] LineSep = { crlf };

        public LinkedList<string> SL;

        public TWebStringList()
        {
            SL = new LinkedList<string>();
        }

        public void Clear()
        {
            SL.Clear();
        }

        public int Count
        {
            get
            {
                return SL.Count;
            }
        }

        public void Add(string s)
        {
            SL.AddLast(s);
        }

        public void AddFirst(string s)
        {
            SL.AddFirst(s);
        }

        public void RemoveFirst()
        {
            SL.RemoveFirst();
        }

        public string this [int index]
        {
            get
            {
                return SL.ElementAt(index);
            }
        }

        public string GetName(int index)
        {
            string s = this[index];
            return s.Split(NameValueSep)[0];
        }

        public string GetValue(int index)
        {
            string s = this[index];
            return s.Split(NameValueSep)[1];
        }

        public string Text
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                foreach (string s in SL)
                {
                    sb.Append(s);
                    sb.Append(crlf);
                }
                return sb.ToString();
            }

            set
            {
                SL.Clear();
                foreach (string s in value.Split(LineSep, StringSplitOptions.RemoveEmptyEntries))
                {
                    if (!s.Equals(""))
                    {
                        SL.AddLast(s);
                    }
                }
            }
        }

    }

}
