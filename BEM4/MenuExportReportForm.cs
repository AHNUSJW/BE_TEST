using System;
using System.IO;
using System.Windows.Forms;

namespace BEM4
{
    public partial class MenuExportReportForm : Form
    {
        public string reportFileName;
        public string reportCompany;
        public string reportLoad;
        public string reportCommodity;
        public string reportStandard;
        public string reportOpsn;
        public string reportDate;

        public string reportPeak;
        public string reportAngle;
        public string reportUnit;
        public string reportMode;
        public string reportAngleSpeed;
        public string reportStamp;
        public bool report = false;

        public bool bt_click = false;
        public MenuExportReportForm()
        {
            InitializeComponent();
        }
        //保存记录
        private void SaveUserInfo()
        {
            String mePath = MyDefine.myXET.userCFG + @"\user." + MyDefine.myXET.userName + ".ifo";
            if (File.Exists(mePath))
            {
                System.IO.File.SetAttributes(mePath, FileAttributes.Normal);
            }
            FileStream meFS = new FileStream(mePath, FileMode.Create, FileAccess.Write);
            TextWriter meWrite = new StreamWriter(meFS);
            if (tb_fileName.TextLength > 0)
            {
                meWrite.WriteLine("reportFileName=" + tb_fileName.Text);
            }
            if (tb_company.TextLength > 0)
            {
                meWrite.WriteLine("reportCompany=" + tb_company.Text);
            }
            if (tb_load.TextLength > 0)
            {
                meWrite.WriteLine("reportLoad=" + tb_load.Text);
            }
            if (tb_commodity.TextLength > 0)
            {
                meWrite.WriteLine("reportCommodity=" + tb_commodity.Text);
            }
            if (tb_standard.TextLength > 0)
            {
                meWrite.WriteLine("reportStandard=" + tb_standard.Text);
            }
            meWrite.Close();
            meFS.Close();
            System.IO.File.SetAttributes(mePath, FileAttributes.ReadOnly);
        }

        //读取记录
        private void GetUserInfo()
        {
            String mePath = MyDefine.myXET.userCFG + @"\user." + MyDefine.myXET.userName + ".ifo";

            if (File.Exists(mePath))
            {
                String[] meLines = File.ReadAllLines(mePath);

                foreach (String line in meLines)
                {
                    switch (line.Substring(0, line.IndexOf('=')))
                    {
                        case "reportFileName": tb_fileName.Text = line.Substring(line.IndexOf('=') + 1); break;
                        case "reportCompany": tb_company.Text = line.Substring(line.IndexOf('=') + 1); break;
                        case "reportLoad": tb_load.Text = line.Substring(line.IndexOf('=') + 1); break;
                        case "reportCommodity": tb_commodity.Text = line.Substring(line.IndexOf('=') + 1); break;
                        case "reportStandard": tb_standard.Text = line.Substring(line.IndexOf('=') + 1); break;
                        case "reportOpsn": tb_opsn.Text = bt_click ? "" : line.Substring(line.IndexOf('=') + 1); break;
                        default: break;
                    }
                }
            }

            //初始化
            string msg = MyDefine.myXET.languageNum == 0 ? "扭力测试曲线报告" : "TorqueTestCurveReport";
            tb_fileName.Text = tb_fileName.Text == "" ? msg + DateTime.Now.ToString("yyMMddHHmmssfff") : tb_fileName.Text;
            tb_company.Text = tb_company.Text == "" ? "芜湖艾瑞特机电设备有限公司" : tb_company.Text;
            tb_load.Text = tb_load.Text == "" ? MyDefine.myXET.userOut : tb_load.Text;
            tb_commodity.Text = tb_commodity.Text == "" ? "智能扭力扳手" : tb_commodity.Text;
            tb_standard.Text = tb_standard.Text == "" ? "YS//T276-2011" : tb_standard.Text;
            tb_time.Text = System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
        }

        //生成报告
        private void bt_report_Click(object sender, EventArgs e)
        {
            reportFileName = tb_fileName.Text;
            reportCompany = tb_company.Text;
            reportLoad = tb_load.Text;
            reportCommodity = tb_commodity.Text;
            reportStandard = tb_standard.Text;
            reportOpsn = tb_opsn.Text;
            reportDate = tb_time.Text;

            if (reportFileName != "")
            {
                report = true;
            }
            else
            {
                if (MyDefine.myXET.languageNum == 0)//语言选择为中文
                {
                    MessageBox.Show("请先输入文件名称");
                }
                else
                {
                    MessageBox.Show("Please enter the file name first.");
                }
                return;
            }
            SaveUserInfo();
            this.Hide();
        }

        private void MenuExportReportForm_Load(object sender, EventArgs e)
        {
            GetUserInfo();
        }

        private void MenuExportReportForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            //取消关闭事件
            this.FormClosing -= new System.Windows.Forms.FormClosingEventHandler(this.MenuExportReportForm_FormClosing);
        }

        //设置保存路径
        private void bt_load_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            folderBrowserDialog.Description = MyDefine.myXET.languageNum == 0 ? "请选择文件保存路径" : "Please select a path to save the file.";
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                reportLoad = folderBrowserDialog.SelectedPath;
            }
            tb_load.Text = reportLoad;
        }
    }
}
