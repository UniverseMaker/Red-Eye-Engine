using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RedEyeEngine
{
    public class CoreDaum
    {
        Engine Engine = new Engine();
        Random rd = new Random();

        public string Login(string ID, string PASSWORD, string PROXY)
        {
            List<string> Header = new List<string>();
            Header.Add("Referer: https://logins.daum.net/accounts/srp.do?slogin=2&rid=23cb9696-2377-440e-878f-ec7fa76caa50&srplm1=3d9c14cfbc69b48d16f8c76029e29ba7cdfc9691fed777a6c4845661c4819a93");
            Header.Add("Content-Type: application/x-www-form-urlencoded");
            Header.Add("User-Agent: NetBrowser1.0 (compatible; Windows NT 6.2; WOW64; Trident/6.0)");
            return Engine.HttpSend("ALL", "utf-8", "POST", "https://logins.daum.net/accounts/login.do?slogin=2", Header, new StringBuilder("id=" + ID + "&pw=" + PASSWORD), PROXY, 0);
        }

        public bool GetCurrentSession(string ID, string PASSWORD, string COOKIE, string PROXY)
        {
            List<string> Header = new List<string>();
            Header.Add("Accept: */*");
            Header.Add("Referer: https://user.daum.net/modifyuser/loginhistoryinfo.daum?childPageCode=current");
            Header.Add("User-Agent: Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)");
            Header.Add("Cookie: " + COOKIE);
            string body = Engine.HttpSend("CONTENTS", "euc-kr", "GET", "https://user.daum.net/modifyuser/currentlogininfo.daum?dummy=1405795640232", Header, null, PROXY, 0);

            string[] temp = System.Text.RegularExpressions.Regex.Split(body, "<tbody>");
            temp = System.Text.RegularExpressions.Regex.Split(temp[1], "<tr");

            int pcc = 0;
            bool isCurrentSession = false;

            if (temp[temp.Count() - 1].IndexOf("<td>접속중</td>") != -1)
            {
                isCurrentSession = true;
            }

            //for (int i = 0; i < temp.Count(); i++)
            //{
            //    if (temp[i].IndexOf("<td>PC</td>") != -1)
            //    {
            //        if (temp[i].IndexOf("<td>접속중</td>") != -1 && pcc == 0)
            //        {
            //            isCurrentSession = false;
            //            break;
            //        }
            //    }
            //}

            return isCurrentSession;
        }
    }
}
