using System;
using System.Text;
using System.Net;
using System.IO;
using System.Diagnostics;
using System.Collections;
using System.Web;

namespace RiggVar.FR
{
    public class HttpBridgeClient
    {
        public bool debug;

        public HttpBridgeClient()
        {
        }

        public string Get(UriBuilder ub)
        {
            string result = "";
            try
            {
                WebRequest myRequest = WebRequest.Create(ub.Uri);        
                WebResponse myResponse = myRequest.GetResponse();
                try
                {
                    StreamReader sr = new StreamReader(myResponse.GetResponseStream());
                    result = sr.ReadToEnd();
                }
                finally
                {
                    myResponse.Close();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return result;
        }

        public string Post(UriBuilder ub, TStrings Params)
        {
            string result = "";
            try
            {
                WebRequest myRequest = WebRequest.Create(ub.Uri);
                
                HttpWebRequest wr = myRequest as HttpWebRequest;

                string postData = "";
                string key, value;
                for (int i = 0; i < Params.Count; i++)
                {
                    key = Params.Names(i);
                    value = Params.Values(key);

                    if (i == 0)
                    {
                        postData = HttpUtility.UrlEncode(key) + "=" + HttpUtility.UrlEncode(value);
                    }
                    else
                    {
                        postData += "&" + HttpUtility.UrlEncode(key) + "=" + HttpUtility.UrlEncode(value);
                    }
                }                

                ASCIIEncoding encoding=new ASCIIEncoding();
                byte[] buffer = encoding.GetBytes(postData);

                wr.Method = "POST";
                wr.ContentType="application/x-www-form-urlencoded";
                wr.ContentLength = postData.Length;

                Stream rs = wr.GetRequestStream();
                rs.Write(buffer, 0, buffer.Length);
                rs.Close();

                WebResponse response = myRequest.GetResponse();
                try
                {
                    StreamReader sr = new StreamReader(response.GetResponseStream());
                    result = sr.ReadToEnd();
                }
                finally
                {
                    response.Close();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return result;
        }

        public string PostMultiPart(UriBuilder ub, Hashtable Params)
        {
            string result = "";
            try
            {
                WebRequest myRequest = WebRequest.Create(ub.Uri);
                
                HttpWebRequest wr = myRequest as HttpWebRequest;

                string postData = "";
                string value;

                foreach (string key in Params.Keys)
                {
                    value = (string) Params[key];
                    postData +=
                        "--AAoBBoCC\r\n"
                        + "content-disposition: form-data; name=\"" + key + "\"\r\n"
                        + "content-type: text/plain\r\n\r\n"
                        + value + "\r\n";
                }
                postData += "--AAoBBoCC--\r\n";

                ASCIIEncoding encoding=new ASCIIEncoding();
                byte[] buffer = encoding.GetBytes(postData);

                wr.Headers.Clear();
                wr.Method = "POST";
                wr.Accept = "text/html";
                wr.UserAgent = "RiggVar FR (.Net)";
                wr.ContentType="multipart/form-data; boundary=AAoBBoCC";
                wr.ContentLength = buffer.Length;
                wr.KeepAlive = false;
                wr.AllowAutoRedirect = false;
                wr.Timeout = 5000;

                Stream rs = wr.GetRequestStream();                                
                rs.Write(buffer, 0, buffer.Length);
                rs.Close();

                WebResponse response = myRequest.GetResponse();
                try
                {
                    StreamReader sr = new StreamReader(response.GetResponseStream());
                    result = sr.ReadToEnd();
                }
                finally
                {
                    response.Close();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return result;
        }

    }
}
