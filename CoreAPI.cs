using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace RedEyeEngine
{
    public class CoreAPI
    {
        public CoreAPI()
        {

        }

        public int MAKELPARAM(int p, int p_2)
        {
            return ((p_2 << 16) | (p & 0xFFFF));
        }
    }
}
