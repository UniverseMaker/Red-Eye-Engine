using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RedEyeEngine
{
    public class CoreSettings
    {
        CoreFramework CFRA = new CoreFramework();
        string DefaultSrc = Application.StartupPath + "\\Lib\\settings.ini";

        public CoreSettings()
        {

        }

        public int SaveSettings(string Src, string Key, string Value)
        {
            if (Src == "")
                Src = DefaultSrc;

            List<string> data = CFRA.ReadData(Src);

            try
            {
                Dictionary<string, string> settings = new Dictionary<string, string>();
                for (int i = 0; i < data.Count(); i++)
                {
                    string[] st = System.Text.RegularExpressions.Regex.Split(data[i], "=");
                    settings.Add(st[0], data[i].Replace(st[0] + "=", ""));
                }

                if (settings.ContainsKey(Key) == true)
                    settings[Key] = Value;
                else
                    settings.Add(Key, Value);

                List<string> result = new List<string>();

                foreach (KeyValuePair<string, string> item in settings)
                {
                    result.Add(item.Key + "=" + item.Value);
                }

                CFRA.WriteData(Src, result, false);

                return 0;
            }
            catch
            {
                try
                {
                    CFRA.WriteData(Src, data, false);
                }
                catch
                {
                    CFRA.WriteData(Application.StartupPath + "\\Lib\\settings.ini.bak", data, false);
                }

                return -1;
            }
        }

        public string LoadSettings(string Src, string Key)
        {
            if (Src == "")
                Src = DefaultSrc;

            try
            {
                List<string> data = CFRA.ReadData(Src);
                Dictionary<string, string> settings = new Dictionary<string, string>();
                for (int i = 0; i < data.Count(); i++)
                {
                    string[] st = System.Text.RegularExpressions.Regex.Split(data[i], "=");
                    settings.Add(st[0], data[i].Replace(st[0] + "=", ""));
                }

                if (settings.ContainsKey(Key) == true)
                    return settings[Key];
                else
                    return null;
            }
            catch
            {
                return null;
            }
        }
    }
}
