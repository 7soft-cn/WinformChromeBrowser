using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace CefTest
{
    public partial class FormStart : Form
    {
        public FormStart()
        {
            InitializeComponent();
        }

        private void FormStart_Load(object sender, EventArgs e)
        {
            timerRetry.Interval = 5000;//五秒钟检测一次
            timerRetry.Start();

        }

        private void timerRetry_Tick(object sender, EventArgs e)
        {
            //如果可以访问服务器，则打开另一窗口
            if (CheckNet())
            {

                var path = GetPath(ConfigurationManager.AppSettings["BrowserName"]);
                if (string.IsNullOrEmpty(path))
                {
                    MessageBox.Show($"找不到[{path}]程序路径");
                }
                //System.Diagnostics.Process p = new System.Diagnostics.Process();
                //p.StartInfo.FileName = path;
                //p.StartInfo.Arguments = ConfigurationManager.AppSettings["Arguments"] ;
                //p.StartInfo.UseShellExecute = false;    //是否使用操作系统shell启动
                //p.StartInfo.RedirectStandardInput = true;//接受来自调用程序的输入信息
                //p.StartInfo.RedirectStandardOutput = true;//由调用程序获取输出信息
                //p.StartInfo.RedirectStandardError = true;//重定向标准错误输出
                //p.StartInfo.CreateNoWindow = true;//不显示程序窗口
                //p.Start();//启动程序
                var th = new Thread(new ThreadStart(StartMainForm));
                th.Start();
                this.Close();
            }
        }

        /// <summary>
        /// 根据程序名从注册表获取程序路径
        /// </summary>
        /// <param name="softName"></param>
        /// <returns></returns>
        public string GetPath(string softName)
        {
            try
            {
                string strKeyName = string.Empty;
                string softPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\";
                RegistryKey regKey = Registry.LocalMachine;
                RegistryKey regSubKey = regKey.OpenSubKey(softPath + softName + ".exe", false);

                object objResult = regSubKey.GetValue(strKeyName);
                RegistryValueKind regValueKind = regSubKey.GetValueKind(strKeyName);
                if (regValueKind == RegistryValueKind.String)
                {
                    return objResult.ToString();
                }
                else
                {
                    if (softName == "360se")
                    {
                        return "C:\\Users\\Administrator\\AppData\\Roaming\\360se6\\Application\\360se.exe";
                    }
                    else
                    {
                        return "";
                    }
                }
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// 启动主界面
        /// </summary>
        [STAThread]
        private static void StartMainForm()
        {
            Form1 form2 = new Form1();
            Application.Run(form2);
        }

        private bool CheckNet()
        {
#if DEBUG
            return true;
#endif
            Ping pingSender = new Ping();
            PingReply reply = null;
            try
            {
                //http://192.168.1.68/Emes-CQRT-ESOPCLIENT/#/
                var url = ConfigurationManager.AppSettings["url"];
                var host = url.Split('/')[2];
                if (host.Contains(":"))
                {
                    host = host.Split(':')[0];
                }
                reply = pingSender.Send(host, 3000);
                if (reply.Status == IPStatus.Success)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
