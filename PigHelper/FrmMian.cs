using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PigHelper.Pig;
using System.Runtime.InteropServices;
namespace PigHelper
{
    public partial class FrmMian : Form
    {

        //注册热键的api
        [DllImport("user32")]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, uint control, Keys vk);

        //解除注册热键的api
        [DllImport("user32")]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        public FrmMian()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.button1.Enabled = false;
            PigHelper pig = new PigHelper(this);
            pig.StartGetThread ();

            DataAmount1 = 0;
            DataAmount2 = 0;
            DataAmount3 = 0;
            //foreach (var project in Pig.ProjectService .GetUnLookedProjects () )
            //{

            //    string[] item = new string[3];
            //    item[0] = project.Title;
            //    item[1] = project.Money;
            //    item[2] = project.Description;
            //    ListViewItem vItem = new ListViewItem(item);
            //    vItem.Tag = project;
            //    listView1.Items.Add(vItem);
            //}
        }

        public void UpdateEnd()
        {
            if (DataAmount3 > 0 || DataAmount1 > 0)
            {
                lbLog.Items.Add(DateTime.Now.ToShortTimeString() + "   推荐:" + DataAmount3 + "   正常：" + DataAmount1 + "   过滤:" + DataAmount2);
            } //  this.button1.Enabled = true;
            DataAmount1 = 0;
            DataAmount2 = 0;
            DataAmount3 = 0;
        }

        private int DataAmount1 = 0;
        private int DataAmount2 = 0;
        private int DataAmount3 = 0;
        private bool IsFirstBind = true;
        public void UpdateDataList(List<ProjectInfo> pros)
        {
            foreach (var project in pros)
            {
                if (IsFirstBind)
                {
                    Mark mark = new Mark();
                    List <string > urls=mark .LoadLocal ();
                    if (urls.Where(s => s == project.URL).FirstOrDefault() != null)
                    {
                        continue;
                    }
                }
                if (project.Title.Contains("系统") || project.Title.Contains("管理"))
                {
                    if (listView3.InvokeRequired)
                    {
                        Action<ProjectInfo> act = (x) =>
                        {
                            string[] item = new string[5];
                            item[0] = DataAmount3.ToString();
                            item[1] = x.Title;
                            item[2] = x.Money;
                            item[3] = x.Description;
                            ListViewItem vItem = new ListViewItem(item);
                            vItem.Tag = x;
                            listView3.Items.Add(vItem);
                            DataAmount3++;

                        };
                        listView2.Invoke(act, project);
                    }
                }
                else if (project.Title.Contains("破解") || project.Title.Contains("验证码") || project.Title.Contains("识别") || project.Title.Contains("外挂") || project.Title.Contains("淘宝") || project.Title.Contains("辅助"))
                {
                    if (listView2.InvokeRequired)
                    {
                        Action<ProjectInfo> act = (x) =>
                        {
                            string[] item = new string[5];
                            item[0] = DataAmount2.ToString();
                            item[1] = x.Title;
                            item[2] = x.Money;
                            item[3] = x.Description;
                            ListViewItem vItem = new ListViewItem(item);
                            vItem.Tag = x;
                            listView2.Items.Add(vItem);
                            DataAmount2++;

                        };
                        listView2.Invoke(act, project);
                    }
                }
                else
                {
                    if (listView1.InvokeRequired)
                    {
                        Action<ProjectInfo> act = (x) =>
                        {
                            string[] item = new string[5];
                            item[0] = DataAmount1.ToString();
                            item[1] = x.Title;
                            item[2] = x.Money;
                            item[3] = x.Description;
                            ListViewItem vItem = new ListViewItem(item);
                            vItem.Tag = x;
                            listView1.Items.Add(vItem);
                            DataAmount1++;

                        };
                        listView1.Invoke(act, project);
                    }
                }
            }
        }

        private void FrmMian_Load(object sender, EventArgs e)
        {
            ImageList imgList = new ImageList();
            imgList.ImageSize = new Size(1, 20);//分别是宽和高
            listView1.SmallImageList = imgList;   
            listView2.SmallImageList = imgList;
            listView3.SmallImageList = imgList;
            lvBid.SmallImageList = imgList;
            RegisterHotKey(this.Handle, 123, 2, Keys.Q);
            RegisterHotKey(this.Handle, 456, 2, Keys.W);
            Control.CheckForIllegalCrossThreadCalls = false;
        }

        ListView ContextMenuList = null;
        private void listView_MouseClick(object sender, MouseEventArgs e)
        {
            ListView lv = (ListView)sender;
            if (e.Button == MouseButtons.Right)
            {

                if (lv.SelectedItems.Count == 1)
                {
                    ContextMenuList = lv;
                    this.contextMenuStrip1.Show(this, new Point(e.Location.X + 15, e.Location.Y + 20));
                }
                else
                {
                    this.contextMenuStrip1.Hide();
                }
            }
            if (e.Button == MouseButtons.Left && lv.SelectedItems.Count>0) 
            {
                this.toolTip1.Hide(lv);
                this.toolTip1.Show(lv.SelectedItems [0].SubItems [3].Text, lv, new Point(e.X, e.Y+18));
            }
        }

        private void 标记ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var pro = (ProjectInfo)ContextMenuList.SelectedItems[0].Tag;
            pro.Looked = true;
            ContextMenuList.Items.Remove(ContextMenuList.SelectedItems[0]);
        }

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case 0x0312:  //这个是window消息定义的注册的热键消息  
                    if (m.WParam.ToString() == "123")   // 按下CTRL+Q隐藏  
                    {
                        this.Hide();
                    }
                    else if (m.WParam.ToString() == "456") // 按下CTRL+W显示
                    {
                        this.Visible = true;
                    }
                    break;
            }
            base.WndProc(ref m);
        }

        private void FrmMian_FormClosed(object sender, FormClosedEventArgs e)
        {
            UnregisterHotKey(this.Handle, 123);
            UnregisterHotKey(this.Handle, 456);
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }


        private void listView_DoubleClick(object sender, EventArgs e)
        {
            ListView lv = (ListView)sender;
            var pro = (ProjectInfo)lv.SelectedItems[0].Tag;
            System.Diagnostics.Process.Start(pro.URL); 
        }
        /// <summary>
        /// 关闭的时候将标记的 项目 保存至本地文件，用于下次打开程序载入
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosed(EventArgs e)
        {
            Mark mark = new Mark();
            mark.SaveLocal(ProjectService.Projects);
            base.OnClosed(e);
        }

        private void 已投ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var pro = (ProjectInfo)ContextMenuList.SelectedItems[0].Tag;
            ContextMenuList.Items.Remove(ContextMenuList.SelectedItems[0]);
            string[] item = new string[3];
            item[0] = pro.Title;
            item[1] = pro.Money;
            item[2] = pro.Description;
            ListViewItem vItem = new ListViewItem(item);
            vItem.Tag = pro;
            lvBid.Items.Add(vItem);
        }

    }
}
