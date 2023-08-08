using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

using System.Windows.Forms;

namespace RedEyeEngine
{
    public class CoreFramework
    {
        public CoreFramework()
        {

        }

        public List<string> ReadData(string src)
        {
            try
            {
                List<string> result = new List<string>();
                FileStream fs2 = new FileStream(src, FileMode.OpenOrCreate, FileAccess.Read);
                StreamReader st = new StreamReader(fs2, System.Text.Encoding.Default);
                st.BaseStream.Seek(0, SeekOrigin.Begin);

                while (st.Peek() > -1)
                {
                    string ImportTemp = st.ReadLine();
                    if (ImportTemp != "")
                    {
                        try
                        {
                            result.Add(ImportTemp);
                        }
                        catch
                        {

                        }
                    }
                }

                st.Close();
                fs2.Close();

                return result;
            }
            catch
            {
                return null;
            }
        }

        public int WriteData(string src, string data, bool append)
        {
            try
            {
                FileStream fs2 = new FileStream(src, FileMode.OpenOrCreate, FileAccess.Read);
                fs2.Close();

                StreamWriter sw = new StreamWriter(src, append, System.Text.Encoding.Default);
                sw.Write(data);

                sw.Close();
                return 0;
            }
            catch
            {
                return -1;
            }
        }

        public int WriteData(string src, List<string> data, bool append)
        {
            try
            {
                FileStream fs2 = new FileStream(src, FileMode.OpenOrCreate, FileAccess.Read);
                fs2.Close();

                StreamWriter sw = new StreamWriter(src, append, System.Text.Encoding.Default);
                for (int i = 0; i < data.Count(); i++)
                {
                    sw.WriteLine(data[i]);
                }

                sw.Close();
                return 0;
            }
            catch
            {
                return -1;
            }
        }

        public void TimerWait(int duration) //, bool CPUFullLoad
        {
            try
            {
                //long startTime = System.DateTime.Now.Ticks;

                //while (true)
                //{
                //    long nowTime = System.DateTime.Now.Ticks;
                //    double timeGap = (nowTime - startTime) / 10000.0f;

                //    if ((int)timeGap > duration)
                //        break;

                //    System.Threading.Thread.Sleep(0);
                //    Application.DoEvents();
                //}

                System.Threading.Thread.Sleep(duration);
            }
            catch
            {

            }
        }


        public string Seperate(string data)
        {
            int a, b, c;//자소버퍼 초성중성종성순
            string result = "";//분리결과가 저장되는 문자열
            string nresult = "";
            string lastcomp = "";
            int cnt;

            //한글의 유니코드

            // ㄱ ㄲ ㄴ ㄷ ㄸ ㄹ ㅁ ㅂ ㅃ ㅅ ㅆ ㅇ ㅈ ㅉ ㅊ ㅋ ㅌ ㅍ ㅎ
            int[] ChoSung ={ 0x3131, 0x3132, 0x3134, 0x3137, 0x3138, 0x3139, 0x3141
            , 0x3142, 0x3143, 0x3145, 0x3146, 0x3147, 0x3148, 0x3149, 0x314a
            , 0x314b, 0x314c, 0x314d, 0x314e };

            // ㅏ ㅐ ㅑ ㅒ ㅓ ㅔ ㅕ ㅖ ㅗ ㅘ ㅙ ㅚ ㅛ ㅜ ㅝ ㅞ ㅟ ㅠ ㅡ ㅢ ㅣ
            int[] JwungSung = {   0x314f, 0x3150, 0x3151, 0x3152, 0x3153, 0x3154, 0x3155
            , 0x3156, 0x3157, 0x3158, 0x3159, 0x315a, 0x315b, 0x315c, 0x315d, 0x315e
            , 0x315f, 0x3160, 0x3161, 0x3162, 0x3163 };

            // ㄱ ㄲ ㄳ ㄴ ㄵ ㄶ ㄷ ㄹ ㄺ ㄻ ㄼ ㄽ ㄾ ㄿ ㅀ ㅁ ㅂ ㅄ ㅅ ㅆ ㅇ ㅈ ㅊ ㅋ ㅌ ㅍ ㅎ
            int[] JongSung = { 0, 0x3131, 0x3132, 0x3133, 0x3134, 0x3135, 0x3136
            , 0x3137, 0x3139, 0x313a, 0x313b, 0x313c, 0x313d, 0x313e, 0x313f
            , 0x3140, 0x3141, 0x3142, 0x3144, 0x3145, 0x3146, 0x3147, 0x3148
            , 0x314a, 0x314b, 0x314c, 0x314d, 0x314e };


            int x;
            for (cnt = 0; cnt < data.Length; cnt++)
            {
                x = (int)data[cnt];
                //한글일 경우만 분리 시행
                if (x >= 0xAC00 && x <= 0xD7A3)
                {
                    c = x - 0xAC00;
                    a = c / (21 * 28);
                    c = c % (21 * 28);
                    b = c / 28;
                    c = c % 28;

                    /*
                    a = (int)a;
                    b = (int)b;
                    c = (int)c;
                    */
                    result += string.Format("{0}{1}", (char)ChoSung[a], (char)JwungSung[b]);

                    // $c가 0이면, 즉 받침이 있을경우
                    if (c != 0)
                    {
                        result += string.Format("{0}", (char)JongSung[c]);
                    }

                    if (result.Length == 2)
                    {
                        nresult += lastcomp + (char)ChoSung[a] + "/";
                        nresult += lastcomp + data[cnt] + "/";
                        lastcomp += data[cnt];
                    }
                    else if (result.Length == 3)
                    {
                        //nresult += (char)ChoSung[a];
                        //nresult += (char)JwungSung[b];
                        nresult += lastcomp + (char)ChoSung[a] + "/";
                        nresult += lastcomp + 자소합치기(Convert.ToString((char)ChoSung[a]), Convert.ToString((char)JwungSung[b]), null) + "/";
                        nresult += lastcomp + data[cnt] + "/";
                        lastcomp += data[cnt];
                    }

                    result = "";
                }
                else
                {
                    //result += string.Format("{0}", (char)x);
                    nresult += lastcomp + data[cnt] + "/";
                    lastcomp += data[cnt];
                }
            }
            return nresult.Substring(0, nresult.Length - 1);
        }

        public static string 자소합치기(string s초성, string s중성, string s종성)
        {
            // 초성, 중성, 종성 테이블.
            string m초성Tbl = "ㄱㄲㄴㄷㄸㄹㅁㅂㅃㅅㅆㅇㅈㅉㅊㅋㅌㅍㅎ";
            string m중성Tbl = "ㅏㅐㅑㅒㅓㅔㅕㅖㅗㅘㅙㅚㅛㅜㅝㅞㅟㅠㅡㅢㅣ";
            string m종성Tbl = " ㄱㄲㄳㄴㄵㄶㄷㄹㄺㄻㄼㄽㄾㄿㅀㅁㅂㅄㅅㅆㅇㅈㅊㅋㅌㅍㅎ";
            ushort mUniCode한글Base = 0xAC00;
            ushort mUniCode한글Last = 0xD79F;
            int i초성위치, i중성위치, i종성위치 = 0;
            int iUniCode;
            i초성위치 = m초성Tbl.IndexOf(s초성);   // 초성 위치
            i중성위치 = m중성Tbl.IndexOf(s중성);   // 중성 위치

            if (s종성 != null && s종성 != "")
                i종성위치 = m종성Tbl.IndexOf(s종성);   // 종성 위치

            // 앞서 만들어 낸 계산식
            if (s종성 != null && s종성 != "")
                iUniCode = mUniCode한글Base + (i초성위치 * 21 + i중성위치) * 28 + i종성위치;
            else
                iUniCode = mUniCode한글Base + (i초성위치 * 21 + i중성위치) * 28;

            // 코드값을 문자로 변환
            char temp = Convert.ToChar(iUniCode);
            return temp.ToString();
        }

        public bool Is64BitOperatingSystem()
        {
            if (IntPtr.Size == 8)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
