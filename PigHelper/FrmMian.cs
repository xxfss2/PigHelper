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
            PigHelper pig = new PigHelper(this);
            pig.StartGetThread ();

            DataAmount1 = 0;
            DataAmount2 = 0;
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

        private int DataAmount1 = 0;
        private int DataAmount2 = 0;
        public void UpdateDataList(List<ProjectInfo> pros)
        {
            foreach (var project in pros)
            {
                if (project.Title.Contains("破解") || project.Title.Contains("验证码") || project.Title.Contains("识别") || project.Title.Contains("外挂") || project.Title.Contains("淘宝") || project.Title.Contains("辅助"))
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
            RegisterHotKey(this.Handle, 123, 2, Keys.Q);
            RegisterHotKey(this.Handle, 456, 2, Keys.W);

        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
          var pro =(ProjectInfo)listView1.SelectedItems[0].Tag;
          System.Diagnostics.Process.Start(pro.URL ); 
        }

        private void listView1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (this.listView1.SelectedItems.Count == 1)
                {
                    this.contextMenuStrip1.Show(this, new Point(e.Location.X + 15, e.Location.Y + 20));
                }
                else
                {
                    this.contextMenuStrip1.Hide();
                }
            }
        }

        private void 标记ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var pro = (ProjectInfo)listView1.SelectedItems[0].Tag;
            pro.Looked = true;
            listView1.Items.Remove(listView1.SelectedItems[0]);
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
    }
}
