using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using HtmlAgilityPack;
using System.IO.Compression;
using PigHelper.Pig;
using System.Threading;
namespace PigHelper
{
    /// <summary>
    /// 猪八戒网的
    /// </summary>
    public class PigHelper
    {
        /// <summary>
        /// 设置获取几个分页的数据
        /// </summary>
        public int PageCount { get; set; }

        private FrmMian _frm;

        private Thread _GetDataThread;

        public PigHelper(FrmMian frm)
        {
            PageCount = 10;
            _frm = frm;
        }

        public void StartGetThread()
        {
            _GetDataThread = new Thread(new ThreadStart(UpdatePages));
            _GetDataThread.IsBackground = true;
            _GetDataThread.Start();
        }

        private void UpdatePages()
        {
            while (true)
            {
                for (int i = 1; i <= PageCount; i++)
                {
                    UpdateOnePage(i);
                }
                _frm.UpdateEnd();
                Thread.Sleep(30000);
            }
        }

        private void UpdateOnePage(int index)
        {
            HttpWebRequest request = WebRequest.Create(@"http://task.zhubajie.com/t-rjkf/o7p"+index+".html") as HttpWebRequest;
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
                    int count = trs.Count > 40 ? 42 : trs.Count;
                    List <ProjectInfo > pros=new List<ProjectInfo> ();
                    for (int i = 0; i < count; i++)
                    {
                        if (trs[i].ChildNodes[3].InnerText == "已选标")
                            continue;
                        var td = trs[i].ChildNodes[0];
                        ProjectInfo projectInfo = new ProjectInfo();
                        if (td.ChildNodes.Count < 1)
                            continue;
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
                        else if (td.ChildNodes[0].ChildNodes.Count == 4)
                        {
                            projectInfo.Title = td.ChildNodes[0].ChildNodes[1].InnerText;
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

                        pros.Add(projectInfo);

                    }
                    ProjectService.AddRange(pros);
                    _frm.UpdateDataList(pros);
                }
                

            }
            response.Close();
            respStream.Close();
        }
    }
}
