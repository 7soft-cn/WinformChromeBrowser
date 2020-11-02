using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Kanbanview
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
            timerRetry.Stop();
            try
            {
                var url = ConfigurationManager.AppSettings["url"];
                //如果可以访问服务器，则打开另一窗口
                if (CheckNetPing(url) || UrlCheck(url))
                {

                    var path = GetPath(ConfigurationManager.AppSettings["BrowserName"]);
                    if (string.IsNullOrEmpty(path))
                    {
                        MessageBox.Show($"找不到[{path}]程序路径");
                    }
                    var Arguments = ConfigurationManager.AppSettings["Arguments"];
                    if (Arguments.Contains("{url}"))
                    {
                        Arguments = Arguments.Replace("{url}", url);
                    }
                    System.Diagnostics.Process p = new System.Diagnostics.Process();
                    p.StartInfo.FileName = path;
                    p.StartInfo.Arguments = Arguments;
                    p.StartInfo.UseShellExecute = false;    //是否使用操作系统shell启动
                    p.StartInfo.RedirectStandardInput = true;//接受来自调用程序的输入信息
                    p.StartInfo.RedirectStandardOutput = true;//由调用程序获取输出信息
                    p.StartInfo.RedirectStandardError = true;//重定向标准错误输出
                    p.StartInfo.CreateNoWindow = true;//不显示程序窗口
                    p.Start();//启动程序
                              //var th = new Thread(new ThreadStart(StartMainForm));
                              //th.Start();
                    this.Close();
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                timerRetry.Start();
#if DEBUG
                timerRetry.Enabled = false;
#endif
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
                    return "";
                }
            }
            catch
            {
                if (softName.Contains("chrome"))//如果配置chorme内核的浏览器，但电脑实际上没有安装，那么原生chrome也可以兼容
                {
                    return GetPath("chrome");
                }
                else
                {
                    return "";
                }
            }
        }

        /// <summary>
        /// 启动主界面
        /// </summary>
        [STAThread]
        private static void StartMainForm()
        {
            FormView form2 = new FormView();
            Application.Run(form2);
        }

        private bool CheckNetPing(string url)
        {
//#if DEBUG
//            return true;
//#endif
            Ping pingSender = new Ping();
            try
            {
                var host = url.Split('/')[2];
                if (host.Contains(":"))
                {
                    host = host.Split(':')[0];
                }
                PingReply reply = pingSender.Send(host, 1000);
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

        private bool UrlCheck(string strUrl)
        {
            if (!strUrl.Contains("http://") && !strUrl.Contains("https://"))
            {
                strUrl = "http://" + strUrl;
            }
            try
            {
                HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(strUrl);
                myRequest.Method = "GET";
                myRequest.Timeout = 2000;  //超时时间2秒
                HttpWebResponse res = (HttpWebResponse)myRequest.GetResponse();
                return (res.StatusCode == HttpStatusCode.OK);
            }
            catch(Exception ex)
            {
                return false;
            }
        }
    }
}
