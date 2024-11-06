using System;
using System.Windows.Forms;

//Lumi 20240717

namespace BEM4W
{
    public partial class MenuDataSettingForm : Form
    {
        public MenuDataSettingForm()
        {
            InitializeComponent();
        }

        //页面加载
        private void MenuDataSettingForm_Load(object sender, EventArgs e)
        {
            this.MaximizeBox = false;

            textBox1.Text = Properties.Settings.Default.WrenchName;

            //DataExcelType 1:导出完整 2:导出结果 0:不导出
            switch (Properties.Settings.Default.DataExcelType)
            {
                default:
                case 1:
                    radioButton1.Checked = true;
                    break;
                case 2:
                    radioButton2.Checked = true;
                    break;
                case 0:
                    radioButton3.Checked = true;
                    break;
            }
        }

        //保存
        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length > 10)
            {
                MessageBox.Show("保存失败，扳手名称不能超过10个字符！");
                return;
            }

            Properties.Settings.Default.WrenchName = textBox1.Text;
            Properties.Settings.Default.DataExcelType = radioButton1.Checked ? 1 : (radioButton2.Checked ? 2 : 0);
            Properties.Settings.Default.Save();

            this.Close();
        }
    }
}
