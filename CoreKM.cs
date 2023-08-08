using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace RedEyeEngine
{
    public class CoreKM
    {
        [DllImport("user32.dll")]
        public static extern int SetCursorPos(int x, int y);

        CoreFramework CFRA = new CoreFramework();

        public CoreKM()
        {

        }

        public void randommouse()
        {
            try
            {
                Random rd = new Random();

                SetCursorPos(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
                CFRA.TimerWait(256);

                SetCursorPos(0, 0);
                CFRA.TimerWait(256);

                SetCursorPos(rd.Next(0, Screen.PrimaryScreen.Bounds.Width), rd.Next(0, Screen.PrimaryScreen.Bounds.Height));

                CFRA.TimerWait(1024);
            }
            catch
            {

            }
        }
    }
}
