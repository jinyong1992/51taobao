using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;

namespace WpfApplication1.Http_sock
{
    class GpHttpapp
    {
        public struct Method
        {
            public string Get { get { return "GET"; } }
            public string Post { get { return "POST"; } }
        }

        public string getcode { get { return getcodep; } }
        private string getcodep;
        public void GetHttp(string url) {
            try
            {
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
                req.Method = "GET";
                req.UserAgent = "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/34.0.1847.137 Safari/537.36 LBBROWSER";
                req.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
                HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
                Stream istrm = resp.GetResponseStream();
                StreamReader rdr = new StreamReader(istrm, UTF8Encoding.UTF8);
                string str = rdr.ReadToEnd();
                //System.Windows.Forms.MessageBox.Show(resp.Cookies.Count.ToString());
                    
                getcodep = str;
                resp.Close();
                rdr.Close();
                istrm.Close();
            }
            catch (Exception e)
            {

            }
        }
        public void Posthttp(string url,string Postdata)
        {
            HttpWebRequest req = null;
            HttpWebResponse res = null;
            try
            {
                CookieContainer cc = new CookieContainer();
                req = (HttpWebRequest)WebRequest.Create(url);
                req.Method = "POST";
               // req.ContentType=;
                byte[] Postdatabyte = Encoding.UTF8.GetBytes(Postdata);
                req.ContentLength = Postdatabyte.Length;
                req.AllowAutoRedirect = false;
                req.CookieContainer = cc;
                req.KeepAlive = true;

                //提交请求
                Stream stream;
                stream = req.GetRequestStream();
                stream.Write(Postdatabyte, 0, Postdatabyte.Length);
                stream.Close();

                res = (HttpWebResponse)req.GetResponse();
                res.Cookies = req.CookieContainer.GetCookies(req.RequestUri);
                CookieCollection cook = res.Cookies;
                string strcrook = req.CookieContainer.GetCookieHeader(req.RequestUri);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
