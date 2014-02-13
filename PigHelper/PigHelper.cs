using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using HtmlAgilityPack;
using System.IO.Compression;

namespace PigHelper
{
    /// <summary>
    /// 猪八戒网的
    /// </summary>
    public class PigHelper
    {
        public void UpdateOrderList()
        {
            HttpWebRequest request = WebRequest.Create(@"http://task.zhubajie.com/t-rjkf/o7.html") as HttpWebRequest;
            request.CookieContainer = new CookieContainer();
            request.Method = "GET";
            request.Accept = @"text/html, application/xhtml+xml, */*";
            request.Headers.Add("Accept-Encoding", "gzip, deflate");
            request.Headers.Add("Accept-Language", "zh-CN");
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            Stream respStream = response.GetResponseStream();

            using (GZipStream reader = new GZipStream(respStream,CompressionMode.Decompress ))
            {
                using (StreamReader sr = new StreamReader(reader, Encoding.UTF8))
                {
                    HtmlDocument doc = new HtmlDocument();
                    doc.Load(sr);
                    HtmlNode mainTable = doc.DocumentNode.SelectNodes("//table[@class=\"list-task\"]")[0];
                    HtmlNodeCollection trs = mainTable.SelectNodes("//tr");
                }

            }
            response.Close();
        }
    }
}
