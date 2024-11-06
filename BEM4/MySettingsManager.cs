using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Newtonsoft.Json;

//Lumi 20240822

namespace BEM4W
{
    public class MySettingsManager
    {
        private const int GroupCount = 30; //组数
        private static string SettingsFilePath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings.json");
        public static List<AXMXSettingsGroup> SettingsGroups = new List<AXMXSettingsGroup>(); //组数据

        public static string CurrentGroupCode { get; set; } = "1";  //当前选择组的组号

        static MySettingsManager()
        {
        }

        //读取配置文件
        public static void LoadSettings()
        {
            try
            {
                if (File.Exists(SettingsFilePath))
                {
                    string serializedSettings = File.ReadAllText(SettingsFilePath);
                    var settingsData = JsonConvert.DeserializeObject<SettingsData>(serializedSettings);
                    CurrentGroupCode = settingsData.CurrentGroupCode;
                    SettingsGroups = settingsData.SettingsGroups;
                }
                else
                {
                    for (int i = 0; i < GroupCount; i++)
                    {
                        SettingsGroups.Add(new AXMXSettingsGroup($"{i + 1}"));
                    }
                    CurrentGroupCode = "1";
                    SaveSettings(); // 创建新的文件并保存
                }
            }
            catch
            {
                MessageBox.Show("编号代码名称读取错误");
            }
        }

        //保存配置文件
        public static bool SaveSettings()
        {
            try
            {
                var settingsData = new SettingsData
                {
                    CurrentGroupCode = CurrentGroupCode,

                    SettingsGroups = SettingsGroups
                };
                string serializedSettings = JsonConvert.SerializeObject(settingsData, Formatting.Indented);
                File.WriteAllText(SettingsFilePath, serializedSettings);

                return true;
            }
            catch
            {
                MessageBox.Show("编号代码名称保存出错");
                return false;
            }
        }

        // 获取当前组的设置值
        public static string GetAXMXSetting(string settingName)
        {
            var currentGroup = SettingsGroups.Find(group => group.GroupCode == CurrentGroupCode);
            if (currentGroup == null)
            {
                Console.WriteLine($"未找到组{CurrentGroupCode}");
                return "";
            }

            var property = typeof(AXMXSettingsGroup).GetProperty(settingName, BindingFlags.Public | BindingFlags.Instance);
            if (property != null)
            {
                return property.GetValue(currentGroup)?.ToString();
            }
            else
            {
                Console.WriteLine($"未找到属性{settingName}");
                return "";
            }
        }

        //依据组号获取设置值
        public static string GetAXMXSetting(string groupCode, string settingName)
        {
            var currentGroup = SettingsGroups.Find(group => group.GroupCode == groupCode);
            if (currentGroup == null)
            {
                Console.WriteLine($"未找到组{groupCode}");
                return "";
            }

            var property = typeof(AXMXSettingsGroup).GetProperty(settingName, BindingFlags.Public | BindingFlags.Instance);
            if (property != null)
            {
                return property.GetValue(currentGroup)?.ToString();
            }
            else
            {
                Console.WriteLine($"未找到属性{settingName}");
                return "";
            }
        }

        // 设置当前组的设置值
        public static void SetAXMXSetting(string settingName, string value)
        {
            var currentGroup = SettingsGroups.Find(group => group.GroupCode == CurrentGroupCode);
            if (currentGroup == null)
            {
                Console.WriteLine($"未找到组{CurrentGroupCode}");
                return;
            }

            var property = typeof(AXMXSettingsGroup).GetProperty(settingName, BindingFlags.Public | BindingFlags.Instance);
            if (property != null)
            {
                property.SetValue(currentGroup, value);
            }
            else
            {
                Console.WriteLine($"未找到属性{settingName}");
            }
        }

        // 依据组号获取组名称
        public static string GetGroupNameSetting(string groupCode)
        {
            var currentGroup = SettingsGroups.Find(group => group.GroupCode == groupCode);
            if (currentGroup == null)
            {
                Console.WriteLine($"未找到组{groupCode}");
                return "";
            }

            var property = typeof(AXMXSettingsGroup).GetProperty("GroupName", BindingFlags.Public | BindingFlags.Instance);
            if (property != null)
            {
                return property.GetValue(currentGroup)?.ToString();
            }
            else
            {
                Console.WriteLine($"未找到属性GroupName");
                return "";
            }
        }

        // 设置当前组的名称
        public static void SetGroupNameSetting(string value)
        {
            var currentGroup = SettingsGroups.Find(group => group.GroupCode == CurrentGroupCode);
            if (currentGroup == null)
            {
                Console.WriteLine($"未找到组{CurrentGroupCode}");
                return;
            }

            var property = typeof(AXMXSettingsGroup).GetProperty("GroupName", BindingFlags.Public | BindingFlags.Instance);
            if (property != null)
            {
                property.SetValue(currentGroup, value);
            }
            else
            {
                Console.WriteLine($"未找到属性GroupName");
            }
        }
    }

    //JSON配置
    public class SettingsData
    {
        public string CurrentGroupCode { get; set; }
        public List<AXMXSettingsGroup> SettingsGroups { get; set; }
    }

    //编号代码名称组
    public class AXMXSettingsGroup
    {
        public string GroupCode { get; set; }  //组号
        public string GroupName { get; set; }  //组名
        public string A1M0 { get; set; }
        public string A1M1 { get; set; }
        public string A1M2 { get; set; }
        public string A1M3 { get; set; }
        public string A1M4 { get; set; }
        public string A1M5 { get; set; }
        public string A1M6 { get; set; }
        public string A1M7 { get; set; }
        public string A1M8 { get; set; }
        public string A1M9 { get; set; }
        public string A2M0 { get; set; }
        public string A2M1 { get; set; }
        public string A2M2 { get; set; }
        public string A2M3 { get; set; }
        public string A2M4 { get; set; }
        public string A2M5 { get; set; }
        public string A2M6 { get; set; }
        public string A2M7 { get; set; }
        public string A2M8 { get; set; }
        public string A2M9 { get; set; }

        public AXMXSettingsGroup()
        {
            GroupCode = "1";
            GroupName = "未命名组";
            A1M0 = "";
            A1M1 = "";
            A1M2 = "";
            A1M3 = "";
            A1M4 = "";
            A1M5 = "";
            A1M6 = "";
            A1M7 = "";
            A1M8 = "";
            A1M9 = "";
            A2M0 = "";
            A2M1 = "";
            A2M2 = "";
            A2M3 = "";
            A2M4 = "";
            A2M5 = "";
            A2M6 = "";
            A2M7 = "";
            A2M8 = "";
            A2M9 = "";
        }

        public AXMXSettingsGroup(string groupCode)
        {
            GroupCode = groupCode;
            GroupName = "未命名组" + groupCode;
            A1M0 = "";
            A1M1 = "";
            A1M2 = "";
            A1M3 = "";
            A1M4 = "";
            A1M5 = "";
            A1M6 = "";
            A1M7 = "";
            A1M8 = "";
            A1M9 = "";
            A2M0 = "";
            A2M1 = "";
            A2M2 = "";
            A2M3 = "";
            A2M4 = "";
            A2M5 = "";
            A2M6 = "";
            A2M7 = "";
            A2M8 = "";
            A2M9 = "";
        }
    }
}
