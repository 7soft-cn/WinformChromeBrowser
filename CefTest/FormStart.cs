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
                var th = new Thread(new ThreadStart(StartMainForm));
                th.Start();
                this.Close();
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
