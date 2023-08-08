using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RedEyeEngine
{
    public class CoreCertification
    {
        Engine Engine = new Engine();
        Random rd = new Random();
        string BID = "cornie0988";

        public string GetProgramCertification(string PROGRAM, string ID, string PASSWORD, string PROXY)
        {
            try
            {
                string NID = Engine.GetMD5Hash("/" + ID + "/").ToLower();

                List<string> Header = new List<string>();
                Header.Add("Accept: text/html, application/xhtml+xml, */*");
                Header.Add("User-Agent: NetBrowser1.0 (compatible; Windows NT 6.2; WOW64; Trident/6.0)");
                string body = Engine.HttpSend("CONTENTS", "utf-8", "GET", "http://" + BID + ".wordpress.com/about/", Header, null, PROXY, 0);

                if (body.IndexOf(NID) != -1)
                {
                    string[] tt = System.Text.RegularExpressions.Regex.Split(body, "<div class=\"entry-content");
                    tt = System.Text.RegularExpressions.Regex.Split(tt[1], "__" + NID + "__");
                    //tt = System.Text.RegularExpressions.Regex.Split(tt[1], "///");

                    body = Engine.HttpSend("CONTENTS", "utf-8", "GET", tt[1], Header, null, PROXY, 0);
                    tt = System.Text.RegularExpressions.Regex.Split(body, "<div class=\"entry-content");
                    tt = System.Text.RegularExpressions.Regex.Split(tt[1], "_/_" + NID + "_/_");
                    return tt[1];
                }
                else
                {
                    return "Error";
                }
            }
            catch
            {
                return "Error";
            }
        }

        public void SetBID(string _BID)
        {
            BID = _BID;
        }
    }
}
