using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using HtmlAgilityPack;
using System.IO.Compression;
using PigHelper.Pig;
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

                    for (int i = 0; i < 42; i++)
                    {
                        if (trs[i].ChildNodes[3].InnerText == "已选标")
                            continue;
                        var td = trs[i].ChildNodes[0];
                        ProjectInfo projectInfo = new ProjectInfo();
                        if (td.ChildNodes[0].ChildNodes.Count == 2 || td.ChildNodes[0].ChildNodes.Count == 3)
                        {
                            projectInfo.Title = td.ChildNodes[0].ChildNodes[1].InnerText;
                            projectInfo.Money = td.ChildNodes[0].ChildNodes[0].InnerText;
      
                            projectInfo.URL = td.ChildNodes[0].ChildNodes[1].Attributes["href"].Value;
                        }
                        else if (td.ChildNodes[0].ChildNodes.Count == 5)
                        {
                            projectInfo.Title ="被屏蔽了";
                            projectInfo.URL = td.ChildNodes[0].ChildNodes[1].Attributes["href"].Value;
                            projectInfo.Money = td.ChildNodes[0].ChildNodes[0].InnerText;
                        }
                        else
                        {
                            projectInfo.Money = "";
                            projectInfo.Title = td.ChildNodes[0].ChildNodes[0].InnerText;
                            projectInfo.URL = td.ChildNodes[0].ChildNodes[0].Attributes["href"].Value;
                        }

                        if (projectInfo.Money.Length > 11)
                        {
                            projectInfo.Money = projectInfo.Money.Substring(11);
                        }
                        projectInfo.Description = td.ChildNodes[1].InnerText;


                        projectInfo.Looked = false;
                        projectInfo.GetTime = DateTime.Now;

                        ProjectService.Add(projectInfo);
                    }

                }
                

            }
            response.Close();
        }
    }
}
