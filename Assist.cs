using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RedEyeEngine
{
    public class Assist
    {
        public string Version = "20210328_VS2008_3_0.3";
        public string Server = "superblaze.net";

        public Assist()
        {

        }

        public void formClosing()
        {
            System.Diagnostics.Process.GetCurrentProcess().Kill();
        }

        public string sendSMS(string to, string data)
        {
            string key = "NjI0RVlHQzBESTdXT1pKRkxITjVFQzQ3NTg4MjkyRTYmMGVlOGMyMDEzNDdmYTMyYjAwMjhmMDJiZWU4MTNiNzUxOWQ5MTkyOTYzNTNlYWIyYjNlYjA5ZTAyZjRhMDAxMA==";

            List<string> Header = new List<string>();
            RedEyeEngine.Engine Engine = new RedEyeEngine.Engine();
            string result = Engine.HttpSend("ALL", "utf-8", "GET", "https://" + Server + "/app/sms/send.php?from=A&to=" + to + "&key=" + key + "&data=" + data, Header, new StringBuilder(""), "", 0);

            return result;
        }

        public string saveLog(string source, string type, string authority, string data, bool b64)
        {
            string b64opt = "";
            if (b64 == true)
                b64opt = "&b64=y";
            if (type == null || type == "")
                type = "info";

            List<string> Header = new List<string>();
            RedEyeEngine.Engine Engine = new RedEyeEngine.Engine();
            string result = Engine.HttpSend("ALL", "utf-8", "GET", "http://" + Server + "/app/log/set.php?source=" + source + b64opt + "&authority" + authority + "&type=" + type + "&data=" + Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(data.Replace("\r", "").Replace("\n", ""))), Header, new StringBuilder(""), "", 0);

            return result;
        }

        public string setGlobalConfig(string key, string value)
        {
            List<string> Header = new List<string>();
            RedEyeEngine.Engine Engine = new RedEyeEngine.Engine();
            string result = Engine.HttpSend("ALL", "utf-8", "GET", "http://" + Server + "/app/globalconfig/set.php?key=" + key + "&value=" + DateTime.Now.ToString(), Header, new StringBuilder(""), "", 0);

            return result;
                
        }

        public string getGlobalConfig(string key)
        {
            List<string> Header = new List<string>();
            RedEyeEngine.Engine Engine = new RedEyeEngine.Engine();
            string result = Engine.HttpSend("CONTENTS", "utf-8", "GET", "http://" + Server + "/app/globalconfig/get.php?key=" + key, Header, new StringBuilder(""), "", 0);

            return result;
        }
    }
}
