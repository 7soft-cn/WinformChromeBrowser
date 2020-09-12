using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;


namespace Kanbanview
{
    public partial class FormView : Form
    {
        ChromiumWebBrowser browser = null;
        StringBuilder cookies = new StringBuilder();

        string url;//URL
        string user;//账号
        string password;//密码

        public FormView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 初始化浏览器
        /// </summary>
        public void InitBrowser()
        {

            var browerSetting = new CefSettings();

            //指定cookie的存储地方
            //browerSetting.CachePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)+@"\mycef";

            //在Win7或更高版本上使用DPI高分辨率
            Cef.EnableHighDPISupport();

            Cef.Initialize(browerSetting);

            browser = new ChromiumWebBrowser(url)
            {
                LifeSpanHandler = new TestLifeSpanHandler(),

                //在页面右键菜单中启用或关闭开发者工具，仅限于调试目的
                MenuHandler = new MenuHandler()
            };


            //var eventObject = new AmazonEventBindingObject();
            //eventObject.ABName = user;
            //eventObject.ABPassword =password;
            //eventObject.EventArrived += OnJavascriptEventArrived;
            //browser.RegisterJsObject("el-form login", eventObject);
            //browser.FrameLoadEnd += Browser_FrameLoadEnd;
            setCookies("http://192.168.102.20:8002", "loginFlag","1",DateTime.Now.AddMonths(1));
            setCookies("http://192.168.102.20:8002", "loginName", "sys", DateTime.Now.AddMonths(1));
            setCookies("http://192.168.102.20:8002", "password", "123456", DateTime.Now.AddMonths(1));
            setCookies("http://192.168.102.20:8002", "companyAndUserName", "{%22OpenId%22:%22sys%22%2C%22Token%22:%2210025f30-ab79-4a8c-b2cc-3016b847ae62%22%2C%22TokenExpires%22:172800%2C%22TokenCreateTime%22:%222020-09-12T12:30:06.3095468+08:00%22%2C%22USER_ID%22:%220f8f176a-753d-4812-8b98-d75826dedda2%22%2C%22USER_NAME%22:%22%E7%AE%A1%E7%90%86%E5%91%98%22%2C%22USER_NUM%22:%22%22%2C%22USER_PWD%22:%22E10ADC3949BA59ABBE56E057F20F883E%22%2C%22SEX%22:%22%E7%94%B7%22%2C%22MOBILE%22:%2213815653259%22%2C%22ENABLE%22:%221%22%2C%22IP%22:%22192.168.102.20%22%2C%22PROJECT_TYPE%22:%22SINGLE%22}", DateTime.Now.AddMonths(1));

            this.Controls.Add(browser);

            browser.Dock = DockStyle.Fill;
            
        }

        public bool setCookies(string host, string name, string value, DateTime ExpiresTime)
        {

            //            value = @"configIp=192.168.102.20; IP=192.168.102.20; 
            //loginFlag=1; 
            //companyAndUserName={%22OpenId%22:%22sys%22%2C%22Token%22:%2210025f30-ab79-4a8c-b2cc-3016b847ae62%22%2C%22TokenExpires%22:172800%2C%22TokenCreateTime%22:%222020-09-12T12:30:06.3095468+08:00%22%2C%22USER_ID%22:%220f8f176a-753d-4812-8b98-d75826dedda2%22%2C%22USER_NAME%22:%22%E7%AE%A1%E7%90%86%E5%91%98%22%2C%22USER_NUM%22:%22%22%2C%22USER_PWD%22:%22E10ADC3949BA59ABBE56E057F20F883E%22%2C%22SEX%22:%22%E7%94%B7%22%2C%22MOBILE%22:%2213815653259%22%2C%22ENABLE%22:%221%22%2C%22IP%22:%22192.168.102.20%22%2C%22PROJECT_TYPE%22:%22SINGLE%22};
            //loginName=sys; password=123456";
            var cookieManager = Cef.GetGlobalCookieManager();

            var setTask = cookieManager.SetCookieAsync(host, new CefSharp.Cookie()
            {
                Domain = host,
                Name = name,
                Value = value,
                Expires = ExpiresTime
            });

            setTask.Wait();


            if (setTask.IsCompleted)
            {

            }

            return setTask.Result;


            //存储cookie
            //var dbPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Google\Chrome\User Data\Default\Cookies";
            //bool flag = cookieManager.SetStoragePath(AppDomain.CurrentDomain.BaseDirectory, true);
        }


        /// <summary>
        /// 页面加载完整后的事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Browser_FrameLoadEnd(object sender, FrameLoadEndEventArgs e)
        {
            var cookieManager = Cef.GetGlobalCookieManager();

            //读取Cookie
            CookieVisitor visitor = new CookieVisitor();
            visitor.SendCookie += visitor_SendCookie;
            cookieManager.VisitAllCookies(visitor);

            #region
            //读取
            //var visitor = new CookieVisitor(all_cookies => {
            //    var sb = new StringBuilder();
            //    foreach (var nameValue in all_cookies)
            //        sb.AppendLine(nameValue.Item1 + " = " + nameValue.Item2);

            //    BeginInvoke(new MethodInvoker(() => {
            //        MessageBox.Show(sb.ToString());
            //    }));
            //});
            //cookieManager.VisitAllCookies(visitor);
            #endregion


            //执行Javascript代码
            if (e.Frame.IsMain)
            {
                browser.ExecuteScriptAsync(@"

 (function () {

    if (window.amazonBoundEvent) {
        var unameElem = document.getElementById('ap_email');
        var passwordElem = document.getElementById('ap_password');

        
        unameElem.value = window.amazonBoundEvent.aBName;
        passwordElem.value = window.amazonBoundEvent.aBPassword;
    }
    


    var elem = document.getElementById('signInSubmit');

    if (elem) {
        elem.addEventListener('click', function (e) {
            if (!window.amazonBoundEvent) {
                console.log('window.amazonBoundEvent does not exist.');
                return;
            }

            var uname = document.getElementById('ap_email').value;
            var password = document.getElementById('ap_password').value;

            if (uname == "" && password == "")
                return;

            if (uname == "" || password == "")
                return;

            window.amazonBoundEvent.raiseEvent('click', { unameField: uname, upasswordField: password });
        });
        //console.log(`Added click listener to ${elem.id}.`);
    }
})();
        ");


            }


        }

        private void visitor_SendCookie(CefSharp.Cookie obj)
        {
            //此处处理cookie
            cookies.Append(obj.Domain.TrimStart('.') + "^" + obj.Name + "^" + obj.Value + "$");
        }

        /// <summary>
        /// 当页面上的Javascript事件触发时，执行
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="eventData"></param>
        private static void OnJavascriptEventArrived(string eventName, object eventData)
        {
            switch (eventName)
            {
                case "click":
                    {
                        var message = eventData.ToString();
                        var dataDictionary = eventData as Dictionary<string, object>;
                        if (dataDictionary != null)
                        {
                            var result = string.Join(", ", dataDictionary.Select(pair => pair.Key + "=" + pair.Value));
                            message = "event data: " + result;
                        }
                        MessageBox.Show(message, "Javascript event arrived", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        break;
                    }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            url = ConfigurationManager.AppSettings["url"];
            user = ConfigurationManager.AppSettings["usr"];
            password = ConfigurationManager.AppSettings["pwd"];
            this.FormBorderStyle = FormBorderStyle.None;     //设置窗体为无边框样式
            this.WindowState = FormWindowState.Maximized;    //最大化窗体 
            InitBrowser();
        }
    }
}
