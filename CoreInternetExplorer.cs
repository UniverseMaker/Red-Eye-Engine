using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MSHTML;
using SHDocVw;
using System.Runtime.InteropServices;

using System.Drawing;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;

namespace RedEyeEngine
{
    public class CoreInternetExplorer
    {
        [System.Runtime.InteropServices.DllImport("wininet.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto, SetLastError = true)]
        public static extern bool InternetSetOption(int hInternet, int dwOption, IntPtr lpBuffer, int dwBufferLength);

        [DllImport("user32.dll")]
        public static extern int PostMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);

        [DllImport("user32.dll")]
        public static extern int FindWindowEx(IntPtr hWnd1, IntPtr hWnd2, string lpsz1, IntPtr lpsz2);

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

        /// <summary>
        /// filter function
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        public delegate bool EnumDelegate(IntPtr hWnd, int lParam);

        /// <summary>
        /// check if windows visible
        /// </summary>
        /// <param name="hWnd"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsWindowVisible(IntPtr hWnd);

        /// <summary>
        /// return windows text
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="lpWindowText"></param>
        /// <param name="nMaxCount"></param>
        /// <returns></returns>
        [DllImport("user32.dll", EntryPoint = "GetWindowText",
        ExactSpelling = false, CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpWindowText, int nMaxCount);

        /// <summary>
        /// enumarator on all desktop windows
        /// </summary>
        /// <param name="hDesktop"></param>
        /// <param name="lpEnumCallbackFunction"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        [DllImport("user32.dll", EntryPoint = "EnumDesktopWindows",
        ExactSpelling = false, CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool EnumDesktopWindows(IntPtr hDesktop, EnumDelegate lpEnumCallbackFunction, IntPtr lParam);

        public const int WM_MOUSEMOVE3 = 0x200;
        public const int WM_LBUTTONDOWN3 = 0x201;
        public const int WM_LBUTTONUP3 = 0x202;

        public const int WM_LBUTTONDOWN = 0x0202;
        public const int WM_LBUTTONUP = 0x0201;
        public const int WM_RBUTTONDOWN = 0x0204;
        public const int WM_RBUTTONUP = 0x0205;

        public const uint WM_LBUTTONDOWN2 = 0x0002;
        public const uint WM_LBUTTONUP2 = 0x0004;
        public const uint WM_RBUTTONDOWN2 = 0x0008;
        public const uint WM_RBUTTONUP2 = 0x00010;

        public const int WM_MOUSEMOVE = 0x0000;
        public const uint WM_MOUSEMOVE2 = 0x0200;

        public const int WM_USER_VALUE = 0x000C;

        public const int KEYEVENTF_KEYDOWN = 0x00;
        public const int KEYEVENTF_EXTENDEDKEY = 0x1;
        public const int KEYEVENTF_KEYUP = 0x02;

        const int WM_KEYDOWN = 0x0100;
        const int WM_KEYUP = 0x0101;
        const int WM_CHAR = 0x0102;
        const int VK_TAB = 0x09;
        const int VK_ENTER = 0x0D;
        const int VK_SPACE = 0x20;
        const int VK_UP = 0x26;
        const int VK_DOWN = 0x28;
        const int VK_RIGHT = 0x27;

        const int WM_CLOSE = 0x0010;

        CoreFramework CFRA = new CoreFramework();
        CoreAPI CAPI = new CoreAPI();

        public CoreInternetExplorer()
        {

        }

        public void IEKill()
        {
            bool noie = true;

            do
            {
                noie = true;

                ProcessStartInfo cmd = new ProcessStartInfo();
                Process process = new Process();
                cmd.FileName = @"cmd";
                cmd.WindowStyle = ProcessWindowStyle.Normal;
                cmd.CreateNoWindow = true;
                cmd.UseShellExecute = false;
                cmd.RedirectStandardError = true;
                cmd.RedirectStandardInput = true;
                cmd.RedirectStandardOutput = true;
                process.EnableRaisingEvents = false;
                process.StartInfo = cmd;
                process.Start();
                process.StandardInput.WriteLine("taskkill /T /F /IM iexplore.exe\r\n");
                process.StandardInput.Close();
                StreamReader reader = process.StandardOutput;

                noie = true;
                foreach (Process processa in Process.GetProcesses())
                {
                    try
                    {
                        if (processa.MainModule.ModuleName.ToString().IndexOf("iexplore") != -1)
                        {
                            processa.Kill();
                            noie = false;
                        }
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }
            } while (noie == false);
        }

        
        public void IEKillPopup(InternetExplorer IE, bool Windows81)
        {
            try
            {
                if (Windows81 == true)
                {
                    int iecount = 0;
                    do
                    {
                        iecount = 0;
                        foreach (InternetExplorer _ie in new ShellWindows())
                        {
                            iecount++;
                            if (IE.HWND != _ie.HWND)
                            {
                                _ie.Quit();
                            }
                        }
                    } while (iecount > 1);
                }
                else
                {
                    int iecount = 0;
                    int trycount = 0;

                    do
                    {
                        iecount = 0;
                        trycount++;
                        Dictionary<string, List<int>> hwndlist = new Dictionary<string, List<int>>();

                        EnumDelegate filter = delegate(IntPtr hWnd, int lParam)
                        {
                            StringBuilder strbTitle = new StringBuilder(255);
                            int nLength = GetWindowText(hWnd, strbTitle, strbTitle.Capacity + 1);
                            string strTitle = strbTitle.ToString();

                            if (IsWindowVisible(hWnd) && string.IsNullOrEmpty(strTitle) == false)
                            {
                                if (hwndlist.ContainsKey(strTitle) == false)
                                    hwndlist.Add(strTitle, new List<int>());

                                hwndlist[strTitle].Add(hWnd.ToInt32());
                            }
                            return true;
                        };

                        if (EnumDesktopWindows(IntPtr.Zero, filter, IntPtr.Zero))
                        {
                            foreach (KeyValuePair<string, List<int>> item in hwndlist)
                            {
                                if (item.Key.IndexOf("Internet Explorer") != -1 && item.Key.IndexOf("Universal Internet Explorer") == -1)
                                {
                                    for (int i = 0; i < item.Value.Count(); i++)
                                    {
                                        if (IE == null)
                                        {
                                            SendMessage((IntPtr)item.Value[i], WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
                                        }
                                        else
                                        {
                                            if (item.Value[i] != IE.HWND)
                                                SendMessage((IntPtr)item.Value[i], WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
                                        }
                                    }
                                }
                            }
                        }

                        CFRA.TimerWait(1000);
                    } while (iecount != 1 && trycount < 10);
                }
            }
            catch
            {

            }
        }

        public void IEClean()
        {
            delcookie(false, System.Security.Principal.WindowsIdentity.GetCurrent().Name.Split(Convert.ToChar("\\"))[1]);

            try
            {
                int rd32c = 0;
                foreach (Process po in Process.GetProcesses())
                {
                    if (po.ProcessName.ToString().IndexOf("rundll32") != -1)
                    {
                        rd32c++;
                    }
                }

                System.Diagnostics.Process.Start("rundll32.exe", "InetCpl.cpl,ClearMyTracksByProcess 255");
                System.Threading.Thread.Sleep(256);

                bool waitrundll32 = true;
                while (waitrundll32 == true)
                {
                    waitrundll32 = false;
                    int tc = 0;
                    foreach (Process po in Process.GetProcesses())
                    {
                        if (po.ProcessName.ToString().IndexOf("rundll32") != -1)
                        {
                            waitrundll32 = true;
                            tc++;
                        }
                    }

                    if (rd32c >= tc)
                        waitrundll32 = false;

                    System.Threading.Thread.Sleep(256);
                    Application.DoEvents();
                }
            }
            catch
            {

            }

            try
            {
                string path = Environment.GetFolderPath(Environment.SpecialFolder.InternetCache);
                //for deleting files
                System.IO.DirectoryInfo di = new DirectoryInfo(path);
                foreach (FileInfo file in di.GetFiles())
                {
                    try
                    {
                        file.Delete();
                    }
                    catch
                    {

                    }
                }
                foreach (DirectoryInfo dir in di.GetDirectories())
                {
                    try
                    {
                        dir.Delete(true); //delete subdirectories and files
                    }
                    catch
                    {

                    }
                }

            }
            catch
            {

            }

            try
            {
                unsafe
                {
                    int option = (int)3/* INTERNET_SUPPRESS_COOKIE_PERSIST*/;
                    int* optionPtr = &option;

                    InternetSetOption(0, 81/*INTERNET_OPTION_SUPPRESS_BEHAVIOR*/, new IntPtr(optionPtr), sizeof(int));
                    InternetSetOption(0, 42/*INTERNET_OPTION_SUPPRESS_BEHAVIOR*/, IntPtr.Zero, 0);
                }
            }
            catch
            {

            }
        }

        private string delcookie(bool wxp, string wname)
        {
            //
            //xp일경우 cmd결과값 반환지점이 동적변수 갯수 - 4지점
            //7일경우 cmd결과값 반환지점이 동적변수 갯수 - 3지점(검증안됨)
            //
            //
            if (wxp == true)
            {
                ProcessStartInfo cmd = new ProcessStartInfo();
                Process process = new Process();
                cmd.FileName = @"cmd";
                cmd.WindowStyle = ProcessWindowStyle.Normal;
                cmd.CreateNoWindow = true;
                cmd.UseShellExecute = false;
                cmd.RedirectStandardError = true;
                cmd.RedirectStandardInput = true;
                cmd.RedirectStandardOutput = true;
                process.EnableRaisingEvents = false;
                process.StartInfo = cmd;
                process.Start();
                process.StandardInput.WriteLine("del C:\\Documents and Settings\\" + wname + "\\Cookies /S /F /Q \r\n");
                process.StandardInput.Close();
                StreamReader reader = process.StandardOutput;

                string tempdata = reader.ReadToEnd();
                cmd = null;
                process = null;
                reader = null;
                System.GC.Collect();
                return tempdata;
            }
            else
            {
                ProcessStartInfo cmd = new ProcessStartInfo();
                Process process = new Process();
                cmd.FileName = @"cmd";
                cmd.WindowStyle = ProcessWindowStyle.Normal;
                cmd.CreateNoWindow = true;
                cmd.UseShellExecute = false;
                cmd.RedirectStandardError = true;
                cmd.RedirectStandardInput = true;
                cmd.RedirectStandardOutput = true;
                process.EnableRaisingEvents = false;
                process.StartInfo = cmd;
                process.Start();
                process.StandardInput.WriteLine("del C:\\Users\\" + wname + "\\AppData\\Roaming\\Microsoft\\Windows\\Cookies /S /F /Q\r\n");
                process.StandardInput.Close();
                StreamReader reader = process.StandardOutput;

                string tempdata = reader.ReadToEnd();
                cmd = null;
                process = null;
                reader = null;
                System.GC.Collect();
                return tempdata;
            }
        }

        public void ChangeUserAgent(string UserAgent)
        {
            Microsoft.Win32.RegistryKey rk = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Internet Settings\5.0\User Agent", true);
            rk.SetValue(null, UserAgent);
        }

        public IHTMLElement FindElementEqual(SHDocVw.InternetExplorer IE, string Tag, string Key, string Value)
        {
            MSHTML.HTMLDocument html = new MSHTML.HTMLDocument();
            html = IE.Document as MSHTML.HTMLDocument;

            foreach (IHTMLElement ia in html.getElementsByTagName(Tag))
            {
                try
                {
                    if (ia.getAttribute(Key, 0).ToString().ToLower() == Value.ToLower())
                    {
                        return ia;
                    }
                }
                catch
                {
                    
                }
            }

            return null;
        }

        public IHTMLElement FindElementEqual(SHDocVw.InternetExplorer IE, string Tag, string Key, string Value, int SEQ)
        {
            MSHTML.HTMLDocument html = new MSHTML.HTMLDocument();
            html = IE.Document as MSHTML.HTMLDocument;
            int NSEQ = 0;

            foreach (IHTMLElement ia in html.getElementsByTagName(Tag))
            {
                try
                {
                    if (ia.getAttribute(Key, 0).ToString().ToLower() == Value.ToLower())
                    {
                        NSEQ++;
                        if(NSEQ >= SEQ)
                            return ia;
                    }
                }
                catch
                {

                }
            }

            return null;
        }

        public string FindElementEqual_OuterHTML(SHDocVw.InternetExplorer IE, string Tag, string Key, string Value)
        {
            MSHTML.HTMLDocument html = new MSHTML.HTMLDocument();
            html = IE.Document as MSHTML.HTMLDocument;

            foreach (IHTMLElement ia in html.getElementsByTagName(Tag))
            {
                try
                {
                    if (ia.getAttribute(Key, 0).ToString().ToLower() == Value.ToLower())
                    {
                        return ia.outerHTML;
                    }
                }
                catch
                {

                }
            }

            return null;
        }

        public string FindRank(SHDocVw.InternetExplorer IE, string Value)
        {
            MSHTML.HTMLDocument html = new MSHTML.HTMLDocument();
            html = IE.Document as MSHTML.HTMLDocument;

            string rank = "0";
            foreach (IHTMLElement ia in html.getElementsByTagName("li"))
            {
                try
                {
                    if (ia.getAttribute("id", 0).ToString().ToLower().IndexOf("fusion_") != -1 && ia.innerHTML.ToString().ToLower().IndexOf(Value) != -1)
                    {
                        string[] rt = System.Text.RegularExpressions.Regex.Split(ia.outerHTML.ToString(), "id=\"fusion_");
                        rt = System.Text.RegularExpressions.Regex.Split(rt[1], "\"");
                        return rt[0];
                    }
                }
                catch
                {

                }
            }

            return rank;
        }

        public IHTMLElement FindElementIndexOf(SHDocVw.InternetExplorer IE, string Tag, string Key, string Value)
        {
            MSHTML.HTMLDocument html = new MSHTML.HTMLDocument();
            html = IE.Document as MSHTML.HTMLDocument;

            foreach (IHTMLElement ia in html.getElementsByTagName(Tag))
            {
                try
                {
                    if (ia.getAttribute(Key, 0).ToString().ToLower().IndexOf(Value.ToLower()) != -1)
                    {
                        return ia;
                    }
                }
                catch
                {
                    
                }
            }

            return null;
        }

        public IHTMLElement FindElementInnerHtmlIndexOf(SHDocVw.InternetExplorer IE, string Tag, string Value)
        {
            MSHTML.HTMLDocument html = new MSHTML.HTMLDocument();
            html = IE.Document as MSHTML.HTMLDocument;

            foreach (IHTMLElement ia in html.getElementsByTagName(Tag))
            {
                try
                {
                    if (ia.innerHTML.ToString().ToLower().IndexOf(Value.ToLower()) != -1)
                    {
                        return ia;
                    }
                }
                catch
                {
                    
                }
            }

            return null;
        }

        public IHTMLElement FindElementID(SHDocVw.InternetExplorer IE, string ID)
        {
            try
            {
                MSHTML.HTMLDocument html = new MSHTML.HTMLDocument();
                html = IE.Document as MSHTML.HTMLDocument;

                return html.getElementById("ID");
            }
            catch
            {
                return null;
            }
        }

        public List<string> FindTargets(SHDocVw.InternetExplorer IE)
        {
            try
            {
                MSHTML.HTMLDocument html = new MSHTML.HTMLDocument();
                html = IE.Document as MSHTML.HTMLDocument;
                Random rd = new Random();
                List<string> target = new List<string>();
                List<string> result = new List<string>();

                foreach (IHTMLElement ia in html.getElementsByTagName("a"))
                {
                    try
                    {
                        if (ia.getAttribute("className", 0).ToString().ToLower().IndexOf("sh_blog_title") != -1 || ia.getAttribute("className", 0).ToString().ToLower().IndexOf("sh_cafe_title") != -1 || ia.getAttribute("className", 0).ToString().ToLower().IndexOf("_sp_each_title") != -1 || ia.getAttribute("href", 0).ToString().ToLower().IndexOf("http://kin.naver.com/qna/detail.nhn") != -1)
                        {
                            bool mtdup = false;
                            for (int ja = 0; ja < target.Count(); ja++)
                            {
                                if (target[ja] == ia.getAttribute("href", 0).ToString())
                                    mtdup = true;
                            }

                            if (ia.getAttribute("href", 0).ToString().IndexOf("naver.com") == -1)
                                mtdup = true;

                            if (mtdup == false)
                                target.Add(ia.getAttribute("href", 0).ToString());
                        }

                        if (ia.getAttribute("className", 0).ToString().ToLower().IndexOf("total_wrap") != -1 || ia.getAttribute("className", 0).ToString().ToLower().IndexOf("news_wrap") != -1)
                        {
                            bool mtdup = false;
                            for (int ja = 0; ja < target.Count(); ja++)
                            {
                                if (target[ja] == ia.getAttribute("href", 0).ToString())
                                    mtdup = true;
                            }

                            if (ia.getAttribute("href", 0).ToString().IndexOf("naver.com") == -1)
                                mtdup = true;

                            if (mtdup == false)
                                target.Add(ia.getAttribute("href", 0).ToString());
                        }
                    }
                    catch
                    {

                    }
                }

                while (target.Count() != 0)
                {
                    int select = rd.Next(0, target.Count());
                    result.Add(target[select]);
                    target.RemoveAt(select);
                }

                return result;
            }
            catch
            {
                return null;
            }
        }

        public IHTMLElement FindMore(SHDocVw.InternetExplorer IE)
        {
            MSHTML.HTMLDocument html = new MSHTML.HTMLDocument();
            html = IE.Document as MSHTML.HTMLDocument;

            foreach (IHTMLElement ia in html.getElementsByTagName("a"))
            {
                try
                {
                    if (ia.getAttribute("href",0).ToString().ToLower().IndexOf("?where=post&") != -1 && ia.getAttribute("className",0).ToString().ToLower() == "go_more")
                    {
                        return ia;
                    }

                    if (ia.getAttribute("href",0).ToString().ToLower().IndexOf("?where=m&") != -1 && ia.getAttribute("className",0).ToString().ToLower() == "more")
                    {
                        return ia;
                    }
                }
                catch
                {
                    
                }
            }

            return null;
        }

        public IHTMLElement FindNext(SHDocVw.InternetExplorer IE)
        {
            MSHTML.HTMLDocument html = new MSHTML.HTMLDocument();
            html = IE.Document as MSHTML.HTMLDocument;

            foreach (IHTMLElement ia in html.getElementsByTagName("a"))
            {
                try
                {
                    if (ia.getAttribute("className", 0).ToString().ToLower() == "next")
                    {
                        return ia;
                    }
                }
                catch
                {
                    
                }
            }

            foreach (IHTMLElement ia in html.getElementsByTagName("button"))
            {
                try
                {
                    if (ia.getAttribute("className", 0).ToString().ToLower() == "pg2b_btn" && ia.innerHTML.ToString().IndexOf("spcm pg2b_next") != -1)
                    {
                        return ia;
                    }
                }
                catch
                {

                }
            }

            return null;
        }

        public int BrowserBack(SHDocVw.InternetExplorer IE, string TargetURL)
        {
            try
            {
                string NowURL = IE.LocationURL.ToString();
                int fail = 0;

                do
                {
                    fail++;

                    IE.GoBack();
                    while (IE.Busy == true)
                    {
                        System.Threading.Thread.Sleep(0);
                        Application.DoEvents();
                    }
                    CFRA.TimerWait(3000);
                } while (IE.LocationURL.ToString().IndexOf(NowURL) != -1 && fail < 3);

                return 0;
            }
            catch
            {
                return -1;
            }
        }

        public int BrowserClick(SHDocVw.InternetExplorer IE, IHTMLElement HTMLElement, bool Retry, bool IE11)
        {
            try
            {
                MSHTML.HTMLDocument html = new MSHTML.HTMLDocument();
                html = IE.Document as MSHTML.HTMLDocument;
                MSHTML.HTMLHtmlElement htmldocumentelement = html.documentElement as HTMLHtmlElement;
                string beforeurl = IE.LocationURL.ToString();
                string nowurl = "";
                int fail = 0;

                Random rd = new Random();

                HTMLElement.scrollIntoView(true);
                CFRA.TimerWait(3000);

                Point pos = GetPos(HTMLElement);

                pos.X += rd.Next(0, HTMLElement.offsetWidth);
                pos.Y += rd.Next(0, HTMLElement.offsetHeight);

                pos.X -= htmldocumentelement.scrollLeft;
                pos.Y -= htmldocumentelement.scrollTop;

                IntPtr pControl = (IntPtr)FindWindowEx(new IntPtr(IE.HWND), IntPtr.Zero, "Frame Tab", IntPtr.Zero);
                pControl = (IntPtr)FindWindowEx(pControl, IntPtr.Zero, "TabWindowClass", IntPtr.Zero);
                pControl = (IntPtr)FindWindowEx(pControl, IntPtr.Zero, "Shell DocObject View", IntPtr.Zero);
                pControl = (IntPtr)FindWindowEx(pControl, IntPtr.Zero, "Internet Explorer_Server", IntPtr.Zero);

                do
                {
                    fail++;

                    PostMessage(pControl, WM_MOUSEMOVE, 0, CAPI.MAKELPARAM(pos.X, pos.Y));
                    PostMessage(pControl, WM_LBUTTONDOWN3, 0, CAPI.MAKELPARAM(pos.X, pos.Y));
                    CFRA.TimerWait(100);
                    PostMessage(pControl, WM_LBUTTONUP3, 0, CAPI.MAKELPARAM(pos.X, pos.Y));
                    CFRA.TimerWait(3000);

                    if (IE11 == true)
                        if(FindIE() != null)
                            IE = FindIE();

                    nowurl = IE.LocationURL.ToString();

                } while (beforeurl == nowurl && Retry == true && fail < 3);

                return 0;
            }
            catch
            {
                return -1;
            }
        }

        public int BrowserDoubleClick(SHDocVw.InternetExplorer IE, IHTMLElement HTMLElement, bool Retry, bool IE11)
        {
            try
            {
                MSHTML.HTMLDocument html = new MSHTML.HTMLDocument();
                html = IE.Document as MSHTML.HTMLDocument;
                MSHTML.HTMLHtmlElement htmldocumentelement = html.documentElement as HTMLHtmlElement;
                string beforeurl = IE.LocationURL.ToString();
                string nowurl = "";
                int fail = 0;

                Random rd = new Random();

                HTMLElement.scrollIntoView(true);
                CFRA.TimerWait(3000);

                Point pos = GetPos(HTMLElement);

                pos.X += rd.Next(0, HTMLElement.offsetWidth);
                pos.Y += rd.Next(0, HTMLElement.offsetHeight);

                pos.X -= htmldocumentelement.scrollLeft;
                pos.Y -= htmldocumentelement.scrollTop;

                IntPtr pControl = (IntPtr)FindWindowEx(new IntPtr(IE.HWND), IntPtr.Zero, "Frame Tab", IntPtr.Zero);
                pControl = (IntPtr)FindWindowEx(pControl, IntPtr.Zero, "TabWindowClass", IntPtr.Zero);
                pControl = (IntPtr)FindWindowEx(pControl, IntPtr.Zero, "Shell DocObject View", IntPtr.Zero);
                pControl = (IntPtr)FindWindowEx(pControl, IntPtr.Zero, "Internet Explorer_Server", IntPtr.Zero);

                do
                {
                    fail++;

                    PostMessage(pControl, WM_MOUSEMOVE, 0, CAPI.MAKELPARAM(pos.X, pos.Y));
                    PostMessage(pControl, WM_LBUTTONDOWN3, 0, CAPI.MAKELPARAM(pos.X, pos.Y));
                    CFRA.TimerWait(100);
                    PostMessage(pControl, WM_LBUTTONUP3, 0, CAPI.MAKELPARAM(pos.X, pos.Y));

                    PostMessage(pControl, WM_MOUSEMOVE, 0, CAPI.MAKELPARAM(pos.X, pos.Y));
                    PostMessage(pControl, WM_LBUTTONDOWN3, 0, CAPI.MAKELPARAM(pos.X, pos.Y));
                    CFRA.TimerWait(100);
                    PostMessage(pControl, WM_LBUTTONUP3, 0, CAPI.MAKELPARAM(pos.X, pos.Y));
                    CFRA.TimerWait(3000);

                    if (IE11 == true)
                        if (FindIE() != null)
                            IE = FindIE();

                    nowurl = IE.LocationURL.ToString();

                } while (beforeurl == nowurl && Retry == true && fail < 3);

                return 0;
            }
            catch
            {
                return -1;
            }
        }

        public int BrowserSendKey(SHDocVw.InternetExplorer IE)
        {
            try
            {
                IntPtr pControl = (IntPtr)FindWindowEx(new IntPtr(IE.HWND), IntPtr.Zero, "Frame Tab", IntPtr.Zero);
                pControl = (IntPtr)FindWindowEx(pControl, IntPtr.Zero, "TabWindowClass", IntPtr.Zero);
                pControl = (IntPtr)FindWindowEx(pControl, IntPtr.Zero, "Shell DocObject View", IntPtr.Zero);
                pControl = (IntPtr)FindWindowEx(pControl, IntPtr.Zero, "Internet Explorer_Server", IntPtr.Zero);

                PostMessage(pControl, WM_KEYDOWN, VK_ENTER, 1);

                return 0;
            }
            catch
            {
                return -1;
            }
        }

        public int BrowserSendKeyS(SHDocVw.InternetExplorer IE)
        {
            try
            {
                IntPtr pControl = (IntPtr)FindWindowEx(new IntPtr(IE.HWND), IntPtr.Zero, "Frame Tab", IntPtr.Zero);
                pControl = (IntPtr)FindWindowEx(pControl, IntPtr.Zero, "TabWindowClass", IntPtr.Zero);
                pControl = (IntPtr)FindWindowEx(pControl, IntPtr.Zero, "Shell DocObject View", IntPtr.Zero);
                pControl = (IntPtr)FindWindowEx(pControl, IntPtr.Zero, "Internet Explorer_Server", IntPtr.Zero);

                PostMessage(pControl, WM_KEYDOWN, VK_SPACE, 1);

                return 0;
            }
            catch
            {
                return -1;
            }
        }

        public static Point GetPos(MSHTML.IHTMLElement obj)
        {
            int top = 0;
            int left = 0;

            if (obj.offsetParent != null)
            {
                while (obj.offsetParent != null)
                {
                    top += obj.offsetTop;
                    left += obj.offsetLeft;
                    
                    obj = obj.offsetParent;
                }
            }

            return new Point(left, top);
        }

        public InternetExplorer FindIE()
        {
            try
            {
                InternetExplorer ier = null;

                foreach (InternetExplorer ie in new ShellWindows())
                {
                    if (ie.LocationURL.IndexOf("file://") == -1)
                    {
                        ier = ie;
                    }
                }

                return ier;
            }
            catch
            {
                return null;
            }
        }

        public bool IEMessageBox()
        {
            try
            {
                Dictionary<string, List<int>> hwndlist = new Dictionary<string, List<int>>();

                EnumDelegate filter = delegate(IntPtr hWnd, int lParam)
                {
                    StringBuilder strbTitle = new StringBuilder(255);
                    int nLength = GetWindowText(hWnd, strbTitle, strbTitle.Capacity + 1);
                    string strTitle = strbTitle.ToString();

                    if (IsWindowVisible(hWnd) && string.IsNullOrEmpty(strTitle) == false)
                    {
                        if (hwndlist.ContainsKey(strTitle) == false)
                            hwndlist.Add(strTitle, new List<int>());

                        hwndlist[strTitle].Add(hWnd.ToInt32());
                    }
                    return true;
                };

                if (EnumDesktopWindows(IntPtr.Zero, filter, IntPtr.Zero))
                {
                    foreach (KeyValuePair<string, List<int>> item in hwndlist)
                    {
                        if (item.Key.IndexOf("웹 페이지 메시지") != -1)
                        {
                            return true;
                        }
                    }
                }
            }
            catch
            {

            }

            return false;
        }

        public void IEMessageBoxKill()
        {
            try
            {
                Dictionary<string, List<int>> hwndlist = new Dictionary<string, List<int>>();

                EnumDelegate filter = delegate(IntPtr hWnd, int lParam)
                {
                    StringBuilder strbTitle = new StringBuilder(255);
                    int nLength = GetWindowText(hWnd, strbTitle, strbTitle.Capacity + 1);
                    string strTitle = strbTitle.ToString();

                    if (IsWindowVisible(hWnd) && string.IsNullOrEmpty(strTitle) == false)
                    {
                        if (hwndlist.ContainsKey(strTitle) == false)
                            hwndlist.Add(strTitle, new List<int>());

                        hwndlist[strTitle].Add(hWnd.ToInt32());
                    }
                    return true;
                };

                if (EnumDesktopWindows(IntPtr.Zero, filter, IntPtr.Zero))
                {
                    foreach (KeyValuePair<string, List<int>> item in hwndlist)
                    {
                        if (item.Key.IndexOf("웹 페이지 메시지") != -1)
                        {
                            for (int i = 0; i < item.Value.Count(); i++)
                            {
                                IntPtr pControl = (IntPtr)FindWindowEx((IntPtr)item.Value[i], IntPtr.Zero, "Button", IntPtr.Zero);
                                PostMessage(pControl, WM_MOUSEMOVE, 0, CAPI.MAKELPARAM(1,1));
                                PostMessage(pControl, WM_LBUTTONDOWN3, 0, CAPI.MAKELPARAM(1, 1));
                                CFRA.TimerWait(100);
                                PostMessage(pControl, WM_LBUTTONUP3, 0, CAPI.MAKELPARAM(1, 1));
                                CFRA.TimerWait(3000);

                                //SendMessage((IntPtr)item.Value[i], WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
                            }
                        }
                    }
                }
            }
            catch
            {

            }   
        }

        public void IEMessageBoxKill(string hwnd)
        {
            try
            {
                SendMessage((IntPtr)Convert.ToInt32(hwnd), WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
            }
            catch
            {

            }

        }

        public void IEKill2()
        {
            try
            {
                int iecount = 0;
                int trycount = 0;

                //do
                //{
                    iecount = 0;
                    trycount++;
                    Dictionary<string, List<int>> hwndlist = new Dictionary<string, List<int>>();

                    EnumDelegate filter = delegate(IntPtr hWnd, int lParam)
                    {
                        StringBuilder strbTitle = new StringBuilder(255);
                        int nLength = GetWindowText(hWnd, strbTitle, strbTitle.Capacity + 1);
                        string strTitle = strbTitle.ToString();

                        if (IsWindowVisible(hWnd) && string.IsNullOrEmpty(strTitle) == false)
                        {
                            if (hwndlist.ContainsKey(strTitle) == false)
                                hwndlist.Add(strTitle, new List<int>());

                            hwndlist[strTitle].Add(hWnd.ToInt32());
                        }
                        return true;
                    };

                    if (EnumDesktopWindows(IntPtr.Zero, filter, IntPtr.Zero))
                    {
                        foreach (KeyValuePair<string, List<int>> item in hwndlist)
                        {
                            if (item.Key.IndexOf("Internet Explorer") != -1 && item.Key.IndexOf("Universal Internet Explorer") == -1)
                            {
                                for (int i = 0; i < item.Value.Count(); i++)
                                {
                                        SendMessage((IntPtr)item.Value[i], WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
                                }
                            }
                        }
                    }

                   // CFRA.TimerWait(1000);
                //} while (iecount != 0 && trycount < 10);
            }
            catch
            {

            }
        }
    }
}
