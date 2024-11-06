using NetFwTypeLib;
using System;
using System.Drawing;
using System.IO;
using System.Security.Principal;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace BEM4
{
    public partial class MenuAccountForm : Form
    {
        //
        private Boolean isSave = false;
        private Boolean isNew = false;
        private String myUser = "admin";
        private String myPSW = "";
        private String myDatPath = Application.StartupPath + @"\dat";
        private String myCfgPath = Application.StartupPath + @"\cfg";
        private String myLogPath = Application.StartupPath + @"\log";
        private String myOutPath = Application.StartupPath + @"\out";

        //
        #region set and get
        //

        public Boolean IsSave
        {
            get
            {
                return isSave;
            }
        }
        public Boolean IsNew
        {
            get
            {
                return isNew;
            }
        }
        public String MyUser
        {
            set
            {
                myUser = value;
            }
            get
            {
                return myUser;
            }
        }
        public String MyPSW
        {
            set
            {
                myPSW = value;
            }
            get
            {
                return myPSW;
            }
        }
        public String MyDatPath
        {
            set
            {
                myDatPath = value;
            }
            get
            {
                return myDatPath;
            }
        }
        public String MyCfgPath
        {
            set
            {
                myCfgPath = value;
            }
            get
            {
                return myCfgPath;
            }
        }
        public String MyLogPath
        {
            set
            {
                myLogPath = value;
            }
            get
            {
                return myLogPath;
            }
        }
        public String MyOutPath
        {
            set
            {
                myOutPath = value;
            }
            get
            {
                return myOutPath;
            }
        }

        //
        #endregion
        //

        //
        public MenuAccountForm()
        {
            InitializeComponent();
        }

        //报警提示
        private void warning_NI(string meErr)
        {
            timer1.Enabled = true;
            timer1.Interval = 3000;
            notifyIcon1.Visible = true;
            notifyIcon1.ShowBalloonTip(3000, notifyIcon1.Text, meErr, ToolTipIcon.Info);
            if (button3.Visible == true)
            {
                label4.Location = new Point(91, 105);
            }
            else
            {
                label4.Location = new Point(91, 144);
            }
            label4.Text = meErr;
            label4.Visible = true;
        }

        //账号密码规则
        private void psw_KeyPress(object sender, KeyPressEventArgs e)
        {
            //不可以有以下特殊字符
            // \/:*?"<>|
            // \\
            // \|
            // ""
            Regex meRgx = new Regex(@"[\\/:*?""<>\|]");
            if (meRgx.IsMatch(e.KeyChar.ToString()))
            {
                warning_NI(MyDefine.myXET.languageNum == 0 ? "不能使用\\/:*?\"<>|" : "Cannot use \\/:*?\"<>|");
                e.Handled = true;
            }
        }

        //登录创建保存按钮- 登录
        private void login_button1_Click()
        {
            //工厂使用超级账号密码
            if ((comboBox1.Text == "zhoup") && (textBox1.Text == "BEM4"))
            {
                myUser = "zhoup";
                myPSW = "BEM4";
                myDatPath = Application.StartupPath + @"\dat";
                myCfgPath = Application.StartupPath + @"\cfg";
                myLogPath = Application.StartupPath + @"\log";
                isSave = true;
                isNew = false;
                this.Hide();
            }
            //客户使用dat账号密码
            else
            {
                //用户文件
                String meString = myDatPath + @"\user." + comboBox1.Text + ".dat";

                //验证用户
                if (File.Exists(meString))
                {
                    //读取用户信息
                    FileStream meFS = new FileStream(meString, FileMode.Open, FileAccess.Read);
                    BinaryReader meRead = new BinaryReader(meFS);
                    if (meFS.Length > 0)
                    {
                        //有内容文件
                        myUser = meRead.ReadString();
                        myPSW = meRead.ReadString();
                        myDatPath = meRead.ReadString();
                        myDatPath = Application.StartupPath + @"\dat";
                        myCfgPath = meRead.ReadString();
                        myCfgPath = Application.StartupPath + @"\cfg";
                        myLogPath = meRead.ReadString();
                        myLogPath = Application.StartupPath + @"\log";
                        isNew = false;
                    }
                    else
                    {
                        //空文件
                        myUser = comboBox1.Text;
                        myPSW = "";
                        myDatPath = Application.StartupPath + @"\dat";
                        myCfgPath = Application.StartupPath + @"\cfg";
                        myLogPath = Application.StartupPath + @"\log";
                        isNew = true;
                    }
                    meRead.Close();
                    meFS.Close();

                    //验证密码
                    if (myPSW == textBox1.Text)
                    {
                        isSave = true;
                        this.Hide();
                    }
                    else
                    {
                        warning_NI(MyDefine.myXET.languageNum == 0 ? "密码错误！" : "Password incorrect.");
                    }
                }
                else
                {
                    //不存在admin用户
                    if ((comboBox1.Text == "admin") && (textBox1.Text == ""))
                    {
                        myUser = "admin";
                        myPSW = "";
                        myDatPath = Application.StartupPath + @"\dat";
                        myCfgPath = Application.StartupPath + @"\cfg";
                        myLogPath = Application.StartupPath + @"\log";
                        isSave = true;
                        isNew = true;
                        this.Hide();
                    }
                    //不存在用户提示
                    else
                    {
                        warning_NI(MyDefine.myXET.languageNum == 0 ? "不存在用户！" : "No user exists!");
                    }
                }
            }
        }

        //登录创建保存按钮- 创建
        private void create_button1_Click()
        {
            if (comboBox1.SelectedIndex < 0)//帐号验证
            {
                if (textBox1.Text == textBox2.Text)//密码验证
                {
                    myUser = comboBox1.Text;
                    myPSW = textBox1.Text;
                    myDatPath = Application.StartupPath + @"\dat";
                    myCfgPath = Application.StartupPath + @"\cfg";
                    myLogPath = Application.StartupPath + @"\log";
                    isSave = true;
                    isNew = true;
                    this.Hide();
                }
                else
                {
                    warning_NI(MyDefine.myXET.languageNum == 0 ? "密码错误！" : "The password is wrong!");
                }
            }
            else
            {
                warning_NI(MyDefine.myXET.languageNum == 0 ? "已存在账号！" : "The account already exists!");
            }
        }

        //登录创建保存按钮- 保存
        private void save_button1_Click()
        {
            if (comboBox1.Text == myUser)//帐号验证
            {
                if (textBox1.Text == myPSW)//密码验证
                {
                    myUser = comboBox1.Text;
                    myPSW = textBox2.Text;
                    myDatPath = Application.StartupPath + @"\dat";
                    myCfgPath = Application.StartupPath + @"\cfg";
                    myLogPath = Application.StartupPath + @"\log";
                    isSave = true;
                    isNew = true;
                    this.Hide();
                }
                else
                {
                    warning_NI(MyDefine.myXET.languageNum == 0 ? "密码验证错误！" : "Password verification error!");
                }
            }
            else
            {
                warning_NI(MyDefine.myXET.languageNum == 0 ? "账号错误！" : "Account error!");
            }
        }
        //登录创建保存按钮
        private void button1_Click(object sender, EventArgs e)
        {
            if (button1.Text == "登 录" || button1.Text == "Login")
            {
                if (IsAdministrator())
                {
                    FirewallOperateByObject();
                }
                login_button1_Click();
            }
            else if (button1.Text == "创 建" || button1.Text == "Create")
            {
                create_button1_Click();
            }
            else if (button1.Text == "保 存" || button1.Text == "Save")
            {
                save_button1_Click();
            }
        }

        //新建
        private void button2_Click(object sender, EventArgs e)
        {
            button2.Visible = false;
            button3.Visible = false;
            label3.Visible = true;
            textBox2.Visible = true;
            button1.Text = MyDefine.myXET.languageNum == 0 ? "创 建" : "Create";
        }

        //改密码
        private void button3_Click(object sender, EventArgs e)
        {
            button2.Visible = false;
            button3.Visible = false;
            label3.Visible = true;
            textBox2.Visible = true;
            label2.Text = MyDefine.myXET.languageNum == 0 ? "新密码：" : "New password";
            button1.Text = MyDefine.myXET.languageNum == 0 ? "保 存" : "Save";
        }

        //取消按钮
        private void button4_Click(object sender, EventArgs e)
        {
            if (this.Text == "欢迎使用！" || this.Text == "Welcome！")
            {
                System.Environment.Exit(0);
            }
            else
            {
                isSave = false;
                isNew = false;
                this.Hide();
            }
        }

        //时间控制
        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            notifyIcon1.Visible = false;
        }

        //帐号登录加载
        private void MenuAccountForm_Load(object sender, EventArgs e)
        {
            //窗口元素调整
            if (this.Text == "欢迎使用！" || this.Text == "Welcome！")
            {
                button2.Visible = true;
                button3.Visible = false;
                label3.Visible = false;
                textBox2.Visible = false;
            }
            else
            {
                button2.Visible = false;
                button3.Visible = true;
                label3.Visible = false;
                textBox2.Visible = false;
            }

            //所有用户加载
            if (Directory.Exists(myDatPath))
            {
                //存在
                DirectoryInfo meDirectory = new DirectoryInfo(myDatPath);
                String meString = null;
                foreach (FileInfo meFiles in meDirectory.GetFiles("user.*.dat"))
                {
                    meString = meFiles.Name;
                    meString = meString.Replace("user.", "");
                    meString = meString.Replace(".dat", "");
                    comboBox1.Items.Add(meString);
                }
            }
            else
            {
                //不存在则创建文件夹
                Directory.CreateDirectory(myDatPath);
                //不存在则创建文件
                myUser = "admin";
                myPSW = "";
                myDatPath = Application.StartupPath + @"\dat";
                myCfgPath = Application.StartupPath + @"\cfg";
                myLogPath = Application.StartupPath + @"\log";
                isSave = true;
                isNew = true;
                //增加初始用户
                comboBox1.Items.Add("admin");
            }

            //用户名加载
            comboBox1.Text = myUser;
            textBox1.Text = "";
        }

        //退出帐号登录
        private void MenuAccountForm_Closed(object sender, FormClosedEventArgs e)
        {
            //关闭退出
            if (this.Text == "欢迎使用！" || this.Text == "Welcome！")
            {
                System.Environment.Exit(0);
            }
            else
            {
                isSave = false;
                isNew = false;
            }
        }

        /// <summary>
        /// 判断程序是否拥有管理员权限
        /// </summary>
        /// <returns>true:是管理员；false:不是管理员</returns>
        public static bool IsAdministrator()
        {
            WindowsIdentity current = WindowsIdentity.GetCurrent();
            WindowsPrincipal windowsPrincipal = new WindowsPrincipal(current);
            return windowsPrincipal.IsInRole(WindowsBuiltInRole.Administrator);
        }
        /// <summary>
        /// 通过对象防火墙操作
        /// </summary>
        /// <param name="isOpenDomain">域网络防火墙（禁用：false；启用（默认）：true）</param>
        /// <param name="isOpenPublicState">公共网络防火墙（禁用：false；启用（默认）：true）</param>
        /// <param name="isOpenStandard">专用网络防火墙（禁用: false；启用（默认）：true）</param>
        /// <returns></returns>
        public static bool FirewallOperateByObject(bool isOpenDomain = false, bool isOpenPublicState = false, bool isOpenStandard = false)
        {
            try
            {
                INetFwPolicy2 firewallPolicy = (INetFwPolicy2)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FwPolicy2"));
                // 启用<高级安全Windows防火墙> - 专有配置文件的防火墙
                firewallPolicy.set_FirewallEnabled(NET_FW_PROFILE_TYPE2_.NET_FW_PROFILE2_PRIVATE, isOpenStandard);
                // 启用<高级安全Windows防火墙> - 公用配置文件的防火墙
                firewallPolicy.set_FirewallEnabled(NET_FW_PROFILE_TYPE2_.NET_FW_PROFILE2_PUBLIC, isOpenPublicState);
                // 启用<高级安全Windows防火墙> - 域配置文件的防火墙
                firewallPolicy.set_FirewallEnabled(NET_FW_PROFILE_TYPE2_.NET_FW_PROFILE2_DOMAIN, isOpenDomain);
            }
            catch (Exception e)
            {
                string msg = MyDefine.myXET.languageNum == 0 ? "防火墙修改出错：" : "An error occurred in the firewall modification:";
                string error = msg + $"{e.Message}";
                throw new Exception(error);
            }
            return true;
        }
    }
}

//end

