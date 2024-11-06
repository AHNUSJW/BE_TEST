using BEM4W;
using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace BEM4
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }

        //账号
        private void AccountToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Login_Account(MyDefine.myXET.languageNum == 0 ? "切换用户！" : "Transter User");
            this.BringToFront();
        }

        //退出
        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //退出所有窗口
            System.Environment.Exit(0);
        }

        //设备连接
        private void ConnectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MenuConnectForm myConnectForm = new MenuConnectForm();
            myConnectForm.StartPosition = FormStartPosition.CenterParent;
            myConnectForm.ShowDialog();
            this.BringToFront();
            myConnectForm.Dispose();
            myConnectForm = null;
        }

        //设备配网
        private void ConfigureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MenuConfigureForm myConfigureForm = new MenuConfigureForm();
            myConfigureForm.StartPosition = FormStartPosition.CenterParent;
            myConfigureForm.ShowDialog();
            this.BringToFront();
            myConfigureForm.Dispose();
            myConfigureForm = null;
        }

        //设备参数
        private void setToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MenuCalForm myCalForm = new MenuCalForm();
            myCalForm.StartPosition = FormStartPosition.CenterParent;
            myCalForm.ShowDialog();
            this.BringToFront();
            myCalForm.Dispose();
            myCalForm = null;
        }

        //实时监控
        private void RunToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (Form form in this.MdiChildren)
            {
                if (form.GetType().Name == "MenuRunForm")
                {
                    form.BringToFront();
                    return;
                }
            }

            MenuRunForm myRunForm = new MenuRunForm();
            myRunForm.MdiParent = this;
            myRunForm.Show();
            myRunForm.WindowState = FormWindowState.Maximized;
        }

        //数据处理(导入数据，生成pdf）
        private void DataProcess_Click(object sender, EventArgs e)
        {
            ImportData();
            foreach (Form form in this.MdiChildren)
            {
                if (form.GetType().Name == "MenuDataProcessForm")
                {
                    form.BringToFront();
                    return;
                }
            }
            MenuDataProcessForm dataProcessForm = new MenuDataProcessForm();
            dataProcessForm.MdiParent = this;
            dataProcessForm.Show();
            dataProcessForm.WindowState = FormWindowState.Maximized;
        }

        //数据载入
        private void ImportData()
        {
            if (!Directory.Exists(MyDefine.myXET.userLOG))
            {
                Directory.CreateDirectory(MyDefine.myXET.userLOG);
            }
            OpenFileDialog fileDialog = new OpenFileDialog();
            if (MyDefine.myXET.languageNum == 0)//语言选择为中文
            {
                fileDialog.Title = "请选择数据";
                fileDialog.Filter = "数据(*.csv)|*.csv";
            }
            else
            {
                fileDialog.Title = "Please select data";
                fileDialog.Filter = "data(*.csv)|*.csv";
            }
            fileDialog.RestoreDirectory = true;
            fileDialog.InitialDirectory = MyDefine.myXET.userLOG;

            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                MyDefine.myXET.DPT.Clear();
                string[] aryLine = null;
                string[] bem_line = File.ReadAllLines(fileDialog.FileName);
                foreach (string line in bem_line)
                {
                    aryLine = line.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    MyDefine.myXET.DPT.Add(new DataProcessTable(
                        aryLine.Length,
                        aryLine
                        ));
                }
                if (MyDefine.myXET.languageNum == 0)
                {
                    MessageBox.Show("数据加载完毕。");
                }
                else
                {
                    MessageBox.Show("Data loading complete.");
                }
            }
            //委托
            MyDefine.myXET.form_Update();
        }
        //工厂设置
        private void SetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MenuFactForm myFactForm = new MenuFactForm();
            myFactForm.StartPosition = FormStartPosition.CenterParent;
            myFactForm.ShowDialog();
            this.BringToFront();
            myFactForm.Dispose();
            myFactForm = null;
        }

        //操作手册
        private void ReferToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (File.Exists(MyDefine.myXET.userPIC + @"\Process.pdf"))
            {
                System.Diagnostics.Process.Start(MyDefine.myXET.userPIC + @"\Process.pdf");
                this.BringToFront();
            }
        }

        //软件版本
        private void AboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MenuAboutBox myAboutBox = new MenuAboutBox();
            myAboutBox.ShowDialog();
            this.BringToFront();
            myAboutBox.Dispose();
            myAboutBox = null;
        }

        //注册
        private void InitializationRegister()
        {
            //验证MAC地址
            Int64 net_Mac = 0;
            Int64 net_Var = 0;
            //验证regedit
            Int64 reg_Mac = 0;
            Int64 reg_Var = 0;
            //验证C盘文件
            Int64 sys_Mac = 0;
            Int64 sys_Var = 0;
            Int32 sys_num = 0;
            //验证本地文件
            Int64 use_Mac = 0;
            Int64 use_Var = 0;
            Int32 use_num = 0;

            //验证MAC地址
            string macAddress = "";
            Process myProcess = null;
            StreamReader reader = null;
            try
            {
                ProcessStartInfo start = new ProcessStartInfo("cmd.exe");

                start.FileName = "ipconfig";
                start.Arguments = "/all";
                start.CreateNoWindow = true;
                start.RedirectStandardOutput = true;
                start.RedirectStandardInput = true;
                start.UseShellExecute = false;
                myProcess = Process.Start(start);
                reader = myProcess.StandardOutput;
                string line = reader.ReadLine();
                while (!reader.EndOfStream)
                {
                    if (line.ToLower().IndexOf("physical address") > 0 || line.ToLower().IndexOf("物理地址") > 0)
                    {
                        int index = line.IndexOf(":");
                        index += 2;
                        macAddress = line.Substring(index);
                        macAddress = macAddress.Replace('-', ':');
                        break;
                    }
                    line = reader.ReadLine();
                }
            }
            catch
            {

            }
            finally
            {
                if (myProcess != null)
                {
                    reader.ReadToEnd();
                    myProcess.WaitForExit();
                    myProcess.Close();
                }
                if (reader != null)
                {
                    reader.Close();
                }
            }

            if (macAddress.Length == 17)
            {
                macAddress = macAddress.Replace(":", "");
                net_Mac = Convert.ToInt64(macAddress, 16);
                net_Var = net_Mac;
                while ((net_Var % 2) == 0)
                {
                    net_Var = net_Var / 2;
                }
                while ((net_Var % 3) == 0)
                {
                    net_Var = net_Var / 3;
                }
                while ((net_Var % 5) == 0)
                {
                    net_Var = net_Var / 5;
                }
                while ((net_Var % 7) == 0)
                {
                    net_Var = net_Var / 7;
                }
            }

            //验证regedit
            RegistryKey myKey = Registry.LocalMachine.OpenSubKey("software");
            string[] names = myKey.GetSubKeyNames();
            foreach (string keyName in names)
            {
                if (keyName == "WinES")
                {
                    myKey = Registry.LocalMachine.OpenSubKey("software\\WinES");
                    reg_Mac = Convert.ToInt64(myKey.GetValue("input").ToString());
                    reg_Var = Convert.ToInt64(myKey.GetValue("ouput").ToString());
                }
            }
            myKey.Close();

            //验证C盘文件
            if (!File.Exists("C:\\Windows\\user.dat"))
            {
                if (File.Exists(Application.StartupPath + @"\dat" + @"\user.num"))
                {
                    File.Copy((Application.StartupPath + @"\dat" + @"\user.num"), ("C:\\Windows\\user.dat"), true);
                }
            }
            if (File.Exists("C:\\Windows\\user.dat"))
            {
                //读取用户信息
                FileStream meFS = new FileStream("C:\\Windows\\user.dat", FileMode.Open, FileAccess.Read);
                BinaryReader meRead = new BinaryReader(meFS);
                if (meFS.Length > 0)
                {
                    //有内容文件
                    sys_Mac = meRead.ReadInt64();
                    sys_Var = meRead.ReadInt64();
                    sys_num = meRead.ReadInt32();
                }
                meRead.Close();
                meFS.Close();
            }

            //验证本地文件
            if (!File.Exists(Application.StartupPath + @"\dat" + @"\user.num"))
            {
                if (File.Exists("C:\\Windows\\user.dat"))
                {
                    if (!Directory.Exists(Application.StartupPath + @"\dat"))
                    {
                        Directory.CreateDirectory(Application.StartupPath + @"\dat");
                    }
                    File.Copy(("C:\\Windows\\user.dat"), (Application.StartupPath + @"\dat" + @"\user.num"), true);
                }
            }
            if (File.Exists(Application.StartupPath + @"\dat" + @"\user.num"))
            {
                //读取用户信息
                FileStream meFS = new FileStream((Application.StartupPath + @"\dat" + @"\user.num"), FileMode.Open, FileAccess.Read);
                BinaryReader meRead = new BinaryReader(meFS);
                if (meFS.Length > 0)
                {
                    //有内容文件
                    use_Mac = meRead.ReadInt64();
                    use_Var = meRead.ReadInt64();
                    use_num = meRead.ReadInt32();
                }
                meRead.Close();
                meFS.Close();
            }

            //注册分析
            if ((net_Mac == reg_Mac) && (net_Var == reg_Var) && (sys_Mac == use_Mac) && (sys_Var == use_Var) && (net_Mac == use_Mac) && (net_Var == use_Var))
            {
                MyDefine.myXET.myPC = 1;
                MyDefine.myXET.myMac = sys_Mac;
                MyDefine.myXET.myVar = sys_Var;
            }
            else
            {
                this.Text += " - no signed";
            }
        }

        //登录处理
        private void Login_Account(string meTitle)
        {
            //
            MenuAccountForm myAccountForm = new MenuAccountForm();
            //
            myAccountForm.MyUser = MyDefine.myXET.userName;
            myAccountForm.MyPSW = MyDefine.myXET.userPassword;
            myAccountForm.MyDatPath = MyDefine.myXET.userDAT;
            myAccountForm.MyCfgPath = MyDefine.myXET.userCFG;
            myAccountForm.MyLogPath = MyDefine.myXET.userLOG;
            myAccountForm.MyOutPath = MyDefine.myXET.userOut;
            //
            myAccountForm.Text = meTitle;
            myAccountForm.StartPosition = FormStartPosition.CenterScreen;
            myAccountForm.ShowDialog();

            //加载
            if (myAccountForm.IsSave)
            {
                MyDefine.myXET.userName = myAccountForm.MyUser;
                MyDefine.myXET.userPassword = myAccountForm.MyPSW;
                MyDefine.myXET.userDAT = myAccountForm.MyDatPath;
                MyDefine.myXET.userCFG = myAccountForm.MyCfgPath;
                MyDefine.myXET.userLOG = myAccountForm.MyLogPath;
                MyDefine.myXET.userOut = myAccountForm.MyOutPath;
            }

            //创建
            if (myAccountForm.IsNew)
            {
                MyDefine.myXET.SaveToDat();
            }

            //超级账户
            if ((MyDefine.myXET.userName == "zhoup") && (MyDefine.myXET.userPassword == "BEM4") && (MyDefine.myXET.myPC == 1))
            {
                GxSetToolStripMenuItem.Visible = true;
            }
            else
            {
                GxSetToolStripMenuItem.Visible = false;
            }

            //读取流水号
            String meString = MyDefine.myXET.userCFG + @"\bat." + MyDefine.myXET.userName + ".cfg";

            //验证用户
            if (File.Exists(meString))
            {
                //读取信息
                FileStream meFS = new FileStream(meString, FileMode.Open, FileAccess.Read);
                BinaryReader meRead = new BinaryReader(meFS);
                if (meFS.Length > 0)
                {
                    //有内容文件
                    MyDefine.myXET.snDate = meRead.ReadString();
                    MyDefine.myXET.snBat = meRead.ReadUInt32();
                }
                meRead.Close();
                meFS.Close();
                //
                if (System.DateTime.Now.ToString("yyMMdd").Contains(MyDefine.myXET.snDate) == false)
                {
                    MyDefine.myXET.snBat = 1;
                }
            }
            else
            {
                MyDefine.myXET.SaveToBat();
            }
        }

        //退出
        private void Main_FormClosing(object sender, EventArgs e)
        {
            //
            MyDefine.myXET.SaveToBat();

            //退出所有窗口
            System.Environment.Exit(0);
        }

        //启动
        private void Main_Load(object sender, EventArgs e)
        {
            this.Hide();
            InitializationRegister();
            Login_Account(MyDefine.myXET.languageNum == 0 ? "欢迎使用！" : "Welcome！");

            MySettingsManager.LoadSettings();

            this.Show();
            this.BringToFront();
        }

        //语言切换（中文）
        private void 中文ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //zh-CN 为中文，更多的关于 Culture 的字符串请查 MSDN
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("zh-CN");
            //语言设置为中文
            MyDefine.myXET.languageNum = 0;
            //提示
            MessageBox.Show("请重新启动软件");
            //MessageBox.Show("请重新启动软件", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
            //对当前窗体应用更改后的资源
            ApplyResource();
            //保存选择的语言
            RecordLanguage(0);
        }

        //语言切换（英文）
        private void 英文ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //en 为英文，更多的关于 Culture 的字符串请查 MSDN
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("en");
            //语言设置为英文
            MyDefine.myXET.languageNum = 1;
            //提示
            MessageBox.Show("Please restart the software.");
            //MessageBox.Show("Please restart the software.","Hint", MessageBoxButtons.OK,MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
            //对当前窗体应用更改后的资源
            ApplyResource();
            //保存选择的语言
            RecordLanguage(1);
        }

        /// <summary>
        ///  应用资源
        /// ApplyResources 的第一个参数为要设置的控件
        ///                  第二个参数为在资源文件中的ID，默认为控件的名称
        /// </summary>
        private void ApplyResource()
        {
            SuspendLayout();// SuspendLayout()是临时挂起控件的布局逻辑（msdn）
            ComponentResourceManager res = new ComponentResourceManager(typeof(Main));
            foreach (Control ctl in Controls)
            {
                if (ctl == menuStrip1)
                {
                    foreach (ToolStripMenuItem ctl2 in menuStrip1.Items)
                    {
                        res.ApplyResources(ctl2, ctl2.Name);
                        foreach (ToolStripMenuItem ctl3 in ctl2.DropDownItems)
                        {
                            res.ApplyResources(ctl3, ctl3.Name);
                        }

                    }
                }
                else
                {
                    res.ApplyResources(ctl, ctl.Name);
                }
            }
            res.ApplyResources(this.ChineseToolStripMenuItem, "ChineseToolStripMenuItem");
            res.ApplyResources(this.EnglishToolStripMenuItem, "EnglishToolStripMenuItem");
            this.ResumeLayout(false);
            this.PerformLayout();
            res.ApplyResources(this, "$this");
            SuspendLayout();
        }

        //保存选择的语言
        public void RecordLanguage(int language)
        {
            //空
            if (MyDefine.myXET.userDAT == null)
            {
                return;
            }
            //创建新路径
            else if (!Directory.Exists(MyDefine.myXET.userDAT))
            {
                Directory.CreateDirectory(MyDefine.myXET.userDAT);
            }

            //写入
            try
            {
                string mePath = MyDefine.myXET.userDAT + @"\Language.txt";//设置文件路径
                if (File.Exists(mePath))
                {
                    System.IO.File.SetAttributes(mePath, FileAttributes.Normal);
                }
                File.WriteAllText(mePath, language.ToString());
                System.IO.File.SetAttributes(mePath, FileAttributes.ReadOnly);
            }
            catch
            {
            }
        }

    }
}
