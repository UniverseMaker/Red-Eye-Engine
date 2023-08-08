using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Win32;
using System.IO;
using System.Diagnostics;

using System.Net;
using System.Net.NetworkInformation;

using System.Windows.Forms;

namespace RedEyeEngine
{
    public class CoreInternetProtocols
    {
        string NICDevicesDefaultPath = @"SYSTEM\CurrentControlSet\Control\Class\{4D36E972-E325-11CE-BFC1-08002BE10318}";
        public CoreInternetProtocols()
        {

        }

        public void ChangeProxy(string Proxy)
        {
            RegistryKey registry = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings", true);
            registry.SetValue("ProxyServer", Proxy);
            registry.SetValue("ProxyEnable", 0);
        }

        public void ChangeMAC(string MAC)
        {

        }

        public void ChangeIP(string IP, string SUBNET, string GATEWAY, string DNS1, string DNS2)
        {

        }

        //Old Function
        public List<string> getNICDevices(string path)
        {
            List<string> result = new List<string>();

            try
            {
                RegistryKey reg = Registry.LocalMachine;
                reg = reg.OpenSubKey(path, true);


                if (reg != null)
                {
                    string[] temp = reg.GetSubKeyNames();
                    for (int i = 0; i < temp.Count(); i++)
                    {
                        try
                        {
                            RegistryKey regsub = Registry.LocalMachine;
                            regsub = regsub.OpenSubKey(path + "\\" + temp[i]);

                            if (regsub != null)
                            {
                                string temp2 = (string)regsub.GetValue("DriverDesc");
                                if (temp2 != "")
                                {
                                    result.Add(temp2 + "/_/" + temp[i] + "/_/");
                                }
                            }
                        }
                        catch
                        {

                        }
                    }
                }
            }
            catch
            {

            }

            return result;
        }

        public void changeMACAddr(string path, string deviceid)
        {
            try
            {
                Random rd = new Random();

                RegistryKey reg = Registry.LocalMachine;
                reg = reg.OpenSubKey(path + "\\" + deviceid, true);

                string newmac = "";
                for (int i = 0; i < 12; i++)
                    if (i != 1)
                        newmac += getMakeRandomMACAddr(rd.Next(0, 16), false);
                    else
                        newmac += getMakeRandomMACAddr(rd.Next(0, 8), true);

                reg.SetValue("NetworkAddress", newmac);
            }
            catch
            {

            }
        }

        public void changeMACAddr(string path, string deviceid, string newmac)
        {
            try
            {
                Random rd = new Random();

                RegistryKey reg = Registry.LocalMachine;
                reg = reg.OpenSubKey(path + "\\" + deviceid, true);

                reg.SetValue("NetworkAddress", newmac);
            }
            catch
            {

            }
        }

        public List<string> generateMACAddr(string pre, int length, int amount)
        {
            Random rd = new Random();
            List<string> result = new List<string>();

            for (int i = 0; i < amount; i++)
            {
                string caddr = "";

                for (int j = 0; j < length; j++)
                {
                    if((j == 1 && length == 12) || (j==0 && length == 11))
                        caddr += getMakeRandomMACAddr(rd.Next(0, 8), true);
                    else
                        caddr += getMakeRandomMACAddr(rd.Next(0, 16), false);
                        
                }

                result.Add(pre + caddr);
            }

            return result;
        }

        private string getMakeRandomMACAddr(int num, bool multi)
        {
            if (multi == false)
            {
                if (num == 10)
                    return "A";
                else if (num == 11)
                    return "B";
                else if (num == 12)
                    return "C";
                else if (num == 13)
                    return "D";
                else if (num == 14)
                    return "E";
                else if (num == 15)
                    return "F";
                else
                    return num.ToString();
            }
            else
            {
                if (num == 0)
                    return "0";
                else if (num == 1)
                    return "2";
                else if (num == 2)
                    return "4";
                else if (num == 3)
                    return "6";
                else if (num == 4)
                    return "8";
                else if (num == 5)
                    return "A";
                else if (num == 6)
                    return "C";
                else
                    return "E";
            }
        }

        public void changeNICDeviceStatus(string architecture, string path, string deviceid, string status)
        {
            RegistryKey reg = Registry.LocalMachine;
            reg = reg.OpenSubKey(path + "\\" + deviceid, true);

            string result = (string)reg.GetValue("ComponentId");

            string value = System.Text.RegularExpressions.Regex.Split(result, "&")[1];

            Process.Start(Application.StartupPath + "\\Bin\\devcon_" + architecture + ".exe", status + " *" + value + "*");

        }

        public string getMACAddress(string devicename)
        {
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();

            string paddr = "";
            foreach (NetworkInterface adapter in nics)
            {
                if (adapter.Description == devicename)
                {
                    paddr = adapter.GetPhysicalAddress().ToString();
                }
            }

            string paddr_build = "";

            try
            {
                for (int i = 0; i < 12; i = i + 2)
                {
                    if (i == 0)
                        paddr_build = paddr.Substring(i, 2);
                    else
                        paddr_build += "-" + paddr.Substring(i, 2);
                }

                return paddr_build;
            }
            catch
            {
                return paddr;
            }
        }
    }
}
