using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;

namespace RedEyeEngine
{
    public class CoreADB
    {
        public CoreADB()
        {

        }

        public string adb(string command)
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
            process.StandardInput.WriteLine("\"" + Application.StartupPath.ToString() + "\\platform-tools\\adb.exe\" " + command + "\r\n");
            process.StandardInput.Close();
            StreamReader reader = process.StandardOutput;

            string tempdata = reader.ReadToEnd();
            cmd = null;
            process = null;
            reader = null;
            System.GC.Collect();
            return tempdata;
        }

        public string androidipchange(string x, string y)
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
            process.StandardInput.WriteLine("\"" + Application.StartupPath.ToString() + "\\platform-tools\\adb.exe\" shell input tap " + x + " " + y + "\r\n");
            process.StandardInput.Close();
            StreamReader reader = process.StandardOutput;

            string tempdata = reader.ReadToEnd();
            cmd = null;
            process = null;
            reader = null;
            System.GC.Collect();
            return tempdata;
        }

        public bool androidscreenstate()
        {
            try
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
                process.StandardInput.WriteLine("\"" + Application.StartupPath.ToString() + "\\platform-tools\\adb.exe\" shell dumpsys input_method\r\n");
                //process.StandardInput.WriteLine("\"" + Application.StartupPath.ToString() + "\\platform-tools\\adb.exe\" shell dumpsys power\r\n");
                process.StandardInput.Close();
                StreamReader reader = process.StandardOutput;

                string tempdata = reader.ReadToEnd();

                string spliter = "mScreenOn=";
                if (tempdata.IndexOf("mScreenOn=") != -1)
                    spliter = "mScreenOn=";
                else if (tempdata.IndexOf("Display Power: state=") != -1)
                    spliter = "Display Power: state=";
                else if (tempdata.IndexOf("mInteractive=") != -1)
                    spliter = "mInteractive=";

                string[] tt = System.Text.RegularExpressions.Regex.Split(tempdata, spliter);
                //string[] tt = System.Text.RegularExpressions.Regex.Split(tempdata, "");

                cmd = null;
                process = null;
                reader = null;
                System.GC.Collect();

                if (tt[1].Substring(0, 4).ToLower() == "true" || tt[1].Substring(0, 2).ToLower() == "on")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        public string androidscreen(bool onistrue)
        {
            if (onistrue != androidscreenstate())
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
                process.StandardInput.WriteLine("\"" + Application.StartupPath.ToString() + "\\platform-tools\\adb.exe\" shell input keyevent 26\r\n");
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
                return "NULL";
            }
        }
    }
}
