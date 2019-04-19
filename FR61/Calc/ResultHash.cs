using System;
using System.Text;
using System.IO;
using System.Security.Cryptography;

namespace RiggVar.FR
{
    public class ResultHash
    {
        public static string TestMsg = "abc";

        public static string Test
        {
            get
            {
                ResultHash o = new ResultHash();
                return o.TestMD5();
            }
        }

        public static string Value
        {
            get
            {
                ResultHash o = new ResultHash();
                return o.GetMD5();
            }
        }

        public static string MemoString
        {
            get
            {
                ResultHash o = new ResultHash();
                return o.GetMemoString();
            }
        }


        int DebugLevel = 1;
        Encoding encoding = Encoding.Default;
        SHA1 sha1 = SHA1.Create();
        MD5 md5 = MD5.Create();

        int[,] ResultArray;
        byte[] ByteArray;

        private ResultHash()
        {
            Init();
        }

        private void Init()
        {
            InitResultArray();
            InitByteArray();
        }

        private int Count
        {
            get 
            {
                return TMain.BO.EventNode.Collection.Count;
            }
        }

        private void InitResultArray()
        {
            TEventRowCollection cl = TMain.BO.EventNode.Collection;
            ResultArray = new int[cl.Count,2];
            TEventRowCollectionItem cr;
            int v0, v1;
            for (int i = 0; i < cl.Count; i++)
            {
                cr = cl[i];
                cr = cl[cr.PLZ];
                v0 = cr.ID+1;
                v1 = cr.Race[0].CTime;
                ResultArray[i,0] = v0;
                ResultArray[i,1] = v1;
            }
        }

        private void InitByteArray()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                byte[] ba;
                int c = Count;
                for (int i = 0; i < c; i++)
                {
                    ba = EntryBytes(i); ;
                    ms.Write(EntryBytes(i), 0, ba.Length);
                }
                ByteArray = ms.ToArray();
            }
        }

        private string GetMemoString()
        {
            StringBuilder sb = new StringBuilder();

            string m0 = TestMD5();
            string m1 = GetMD5();

            string s0 = TestSHA1();
            string s1 = GetSHA1();

            string t1 = string.Format("<ResultHash algo=\"md5\" value=\"{0}\" />", m0);

            string x1 = string.Format("Test-MD5: {0}", m0);
            string x2 = string.Format("Real-MD5: {0}", m1);

            string x3 = string.Format("Test-SHA1: {0}", s0);
            string x4 = string.Format("Real-SHA1: {0}", s1);

            sb.AppendLine(t1);
            if (DebugLevel > 0)
            {
                sb.AppendLine();
                int c = Count;
                for (int i = 0; i < c; i++)
                {
                    sb.AppendLine(EntryString(i));
                }
            }
            sb.AppendLine();
            sb.AppendLine("Test-Msg: " + ResultHash.TestMsg);
            sb.AppendLine("Real-Msg: MsgList, w/o line-breaks");
            sb.AppendLine();
            sb.AppendLine(x1);
            sb.AppendLine(x2);
            sb.AppendLine();
            sb.AppendLine(x3);
            sb.AppendLine(x4);

            return sb.ToString();
        }

        private string TestMD5()
        {
            byte[] ba = Encoding.UTF8.GetBytes(ResultHash.TestMsg);
            byte[] h = md5.ComputeHash(ba);
            return BitConverter.ToString(h);
        }

        private string TestSHA1()
        {
            byte[] ba = Encoding.UTF8.GetBytes(ResultHash.TestMsg);
            byte[] h = sha1.ComputeHash(ba);
            return BitConverter.ToString(h);
        }

        private string GetMD5()
        {
            byte[] h = md5.ComputeHash(ByteArray);
            return BitConverter.ToString(h);
        }

        private string GetSHA1()
        {
            byte[] h = sha1.ComputeHash(ByteArray);
            return BitConverter.ToString(h);
        }

        private string EntryString(int i)
        {
            return string.Format("{0:000}:{1:00000};", ResultArray[i, 0], ResultArray[i, 1]);
        }

        private byte[] EntryBytes(int i)
        {
            return encoding.GetBytes(EntryString(i));
        }

    }

}
