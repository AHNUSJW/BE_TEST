using System;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace BEM4
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            //判断之前语言选择
            if (MyDefine.myXET.userDAT == null)
            {
                Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("zh-CN");
                MyDefine.myXET.languageNum = 0;
            }
            else
            {
                string mePath = MyDefine.myXET.userDAT + @"\Language.txt";
                if (File.Exists(mePath))
                {
                    System.IO.File.SetAttributes(mePath, FileAttributes.Normal);
                    Int16 lang = Convert.ToInt16(File.ReadAllText(mePath));
                    MyDefine.myXET.languageNum = lang;
                    if(lang == 0)
                    {
                        Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("zh-CN");
                    }
                    else
                    {
                        Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("en");
                    }
                }
                else
                {
                    Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("zh-CN");
                    MyDefine.myXET.languageNum = 0;
                }
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Main());
        }
    }
}
