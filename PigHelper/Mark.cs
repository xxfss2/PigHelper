using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PigHelper.Pig;
using System.IO;

namespace PigHelper
{
    /// <summary>
    /// 处理 标记 功能（标记已经看过，或投过的标，下次查询出来的时候不需要显示）
    /// </summary>
    public  class Mark
    {
        public void SaveLocal(List<ProjectInfo> pros)
        {
            FileStream fs = new FileStream("mark.txt", FileMode.Append, FileAccess.Write);
            foreach (var item in pros)
            {
                if (item.Looked == false)
                    continue;
                string line = string.Concat(item.URL, Environment.NewLine);
                byte[] bytes = System.Text.Encoding.Default.GetBytes(line);
                fs.Write(bytes, 0, bytes.Length);
            }
            fs.Close();
        }

        public List <string > LoadLocal()
        {
            
            List<string> markedUrl = new List<string>();
            if (File.Exists("mark.txt"))
            {
                StreamReader reader = new StreamReader("mark.txt");
                string line;
                while (!string.IsNullOrEmpty(line = reader.ReadLine()))
                {
                    markedUrl.Add(line);
                }
                reader.Close();
            }
            return markedUrl;
        }
    }
}
