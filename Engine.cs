using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Management;
using System.Security.Cryptography;
using System.IO;
using System.Diagnostics;

namespace RedEyeEngine
{
    public class Engine
    {
        #region HTTP SEND
        public string HttpSend(string ResultType, string ResultEncoding, string Type, string Url, List<string> Header, StringBuilder Data, string Proxy, int Timeout)
        {
            try
            {
                if (Data == null)
                    Data = new StringBuilder("");
                if (Proxy == null)
                    Proxy = "";

                //if (Data.ToString() == "")
                //    Type = "GET";
                //else
                //    Type = "POST";

                // 요청 String -> 요청 Byte 변환
                //byte[] byteDataParams = UTF8Encoding.UTF8.GetBytes(Data.ToString());
                byte[] byteDataParams = System.Text.Encoding.GetEncoding(ResultEncoding).GetBytes(Data.ToString());
                
                /////////////////////////////////////////////////////////////////////////////////////
                /* POST */
                // HttpWebRequest 객체 생성, 설정
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
                if (Proxy != "")
                {
                    string[] proxys = System.Text.RegularExpressions.Regex.Split(Proxy, ":");
                    request.Proxy = new WebProxy(proxys[0], Convert.ToInt32(proxys[1]));
                }

                if (Timeout != 0)
                {
                    request.Timeout = Timeout;
                }

                request.Method = Type;    // 기본값 "GET"
                request.UserAgent = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)";

                bool clp = false;
                if (Header != null)
                {
                    for (int i = 0; i < Header.Count(); i++)
                    {
                        string[] t1 = System.Text.RegularExpressions.Regex.Split(Header[i], ": ");
                        
                        if (t1[0] == "User-Agent")
                        {
                            request.UserAgent = t1[1];
                        }
                        else if (t1[0] == "Content-Type")
                        {
                            if (t1[1].ToLower() == "default")
                                request.ContentType = "application/x-www-form-urlencoded";
                            else
                                request.ContentType = t1[1];
                        }
                        else if (t1[0] == "Content-Length")
                        {
                            clp = true;
                            request.ContentLength = Convert.ToInt64(t1[1]);
                        }
                        else if (t1[0] == "Accept")
                        {
                            request.Accept = t1[1];
                        }
                        else if (t1[0] == "Accept-Encoding")
                        {
                            //request.Headers.Add(Header[i]);
                            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                            //request.TransferEncoding = t1[1];
                        }
                        else if (t1[0] == "Referer")
                        {
                            request.Referer = t1[1];
                        }
                        else if (t1[0] == "Connection")
                        {
                            request.KeepAlive = true;
                        }
                        else
                        {
                            request.Headers.Add(Header[i]);
                        }
                    }
                }
                if(clp == false)
                    request.ContentLength = byteDataParams.Length;
                //request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

                /* GET */
                // GET 방식은 Uri 뒤에 보낼 데이터를 입력하시면 됩니다.
                /*
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(strUri + "?" + dataParams);
                request.Method = "GET";
                */
                //////////////////////////////////////////////////////////////////////////////////////


                // 요청 Byte -> 요청 Stream 변환
                if (Type == "POST")
                {
                    using (Stream stDataParams = request.GetRequestStream())
                        stDataParams.Write(byteDataParams, 0, byteDataParams.Length);
                    //stDataParams.Close();
                }

                // 요청, 응답 받기
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                string charSet = response.CharacterSet;
                //if (charSet == "ISO-8859-1")
                //    charSet = "";
                //else 
                if (charSet.ToUpper().IndexOf("ISO") != -1 || charSet.ToUpper().IndexOf("MS949") != -1)
                    charSet = ResultEncoding;

                if (charSet == "" && ResultEncoding != "default")
                {
                    charSet = ResultEncoding;
                }

                System.Text.Encoding enc;
                if (String.IsNullOrEmpty(charSet))
                    enc = System.Text.Encoding.Default;
                else
                    enc = System.Text.Encoding.GetEncoding(charSet);

                // 응답 Stream -> 응답 String 변환
                string preResult = "";
                string strResult = "";

                // 응답 Stream 읽기
                using (Stream stReadData = response.GetResponseStream())
                {
                    StreamReader srReadData = new StreamReader(stReadData, enc);

                    if (ResultType.ToUpper() == "ALL")
                    {
                        preResult = srReadData.ReadToEnd();
                    }

                    if (ResultType.ToUpper() == "COOKIE" || ResultType.ToUpper() == "ALL")
                    {
                        strResult = response.Headers.ToString();
                        strResult = strResult.Replace("HttpOnly,", "");
                        strResult = strResult.Replace("Path=/,", "");

                        Dictionary<string, string> cdata = new Dictionary<string, string>();

                        try
                        {
                            StringBuilder newheader = new StringBuilder();
                            string[] rheaders = System.Text.RegularExpressions.Regex.Split(strResult, "Set-Cookie: ");

                            for (int ri = 1; ri < rheaders.Count(); ri++)
                            {
                                try
                                {
                                    string[] rheaders2 = System.Text.RegularExpressions.Regex.Split(rheaders[ri], "\r\n");
                                    rheaders2[0] = rheaders2[0].Replace(" HttpOnly", "");
                                    rheaders2[0] = rheaders2[0].Replace("HttpOnly", "");

                                    rheaders2[0] = rheaders2[0].Replace("HTTPOnly,", "");
                                    rheaders2[0] = rheaders2[0].Replace("Secure;", "");
                                    rheaders2[0] = rheaders2[0].Replace("HTTPOnly;", "");
                                    rheaders2[0] = rheaders2[0].Replace("Secure", "");
                                    rheaders2[0] = rheaders2[0].Replace("HTTPOnly", "");

                                    rheaders2 = System.Text.RegularExpressions.Regex.Split(rheaders2[0], ";");

                                    for (int i = 0; i < rheaders2.Count(); i++)
                                    {
                                        try
                                        {
                                            if (rheaders2[i] != "" && rheaders2[i].IndexOf("=") != -1)
                                            {
                                                if (rheaders2[i].Substring(0, 1) == " ")
                                                    rheaders2[i] = rheaders2[i].Substring(1);

                                                if (rheaders2[i].Substring(0, 1) == ",")
                                                    rheaders2[i] = rheaders2[i].Substring(1);

                                                string[] rh2 = System.Text.RegularExpressions.Regex.Split(rheaders2[i], "=");
                                                if (rh2.Count() > 2)
                                                {
                                                    string k = rh2[1];
                                                    string v = rh2[2];
                                                    if (k.IndexOf(",") != -1)
                                                    {
                                                        string[] ms = System.Text.RegularExpressions.Regex.Split(rh2[1], ",");
                                                        k = ms[ms.Count() - 1];
                                                    }

                                                    if (cdata.ContainsKey(k) == false)
                                                        cdata.Add(k, v);
                                                    else
                                                        cdata[k] = v;
                                                }
                                                else if (rh2[0].ToUpper() != "PATH" && rh2[0].ToUpper() != "DOMAIN" && rh2[0].ToUpper() != "EXPIRES")
                                                {
                                                    //newheader.Append(rheaders2[i] + "; ");
                                                    string[] rh3 = System.Text.RegularExpressions.Regex.Split(rheaders2[i], "=");
                                                    if (cdata.ContainsKey(rh3[0]) == false)
                                                        cdata.Add(rh3[0], rh3[1]);
                                                    else
                                                        cdata[rh3[0]] = rh3[1];
                                                }
                                                else if (rh2[1].IndexOf(",") != -1 && rh2[1].IndexOf("GMT") == -1)
                                                {
                                                    rh2 = System.Text.RegularExpressions.Regex.Split(rheaders2[i], ",");
                                                    //newheader.Append(rh2[1] + "; ");

                                                    string[] rh3 = System.Text.RegularExpressions.Regex.Split(rh2[1], "=");
                                                    if (cdata.ContainsKey(rh3[0]) == false)
                                                        cdata.Add(rh3[0], rh3[1]);
                                                    else
                                                        cdata[rh3[0]] = rh3[1];
                                                }
                                            }
                                        }
                                        catch
                                        {

                                        }
                                    }


                                }
                                catch
                                {

                                }
                            }

                            foreach (KeyValuePair<string, string> item in cdata)
                            {
                                if (newheader.ToString() != "")
                                    newheader.Append(" ");

                                newheader.Append(item.Key + "=" + item.Value + ";");
                            }
                            string nhprocess = newheader.ToString();
                            while (nhprocess.IndexOf("  ") != -1)
                                nhprocess = nhprocess.Replace("  ", " ");

                            strResult = nhprocess;

                        }
                        catch
                        {

                        }
                    }
                    else if (ResultType.ToUpper() == "DEBUG")
                    {
                        strResult = request.Headers.ToString() + "\r\n\r\n" + Data.ToString() + "\r\n\r\n" + response.StatusCode.ToString() + " " + response.Headers.ToString() + "\r\n\r\n" + srReadData.ReadToEnd();
                    }
                    else
                    {
                        strResult = srReadData.ReadToEnd();
                    }
                }

                if (preResult == "")
                    return strResult;
                else
                    return preResult + "/RESULT/" + strResult;
            }
            catch(Exception ee)
            {
                return "Error: " + ee.Message;
            }
        }
        #endregion

        #region HTTP SEND BYTES
        public string HttpSendBytes(string ResultType, string ResultEncoding, string Type, string Url, List<string> Header, byte[] Data, string Proxy, int Timeout)
        {
            try
            {
                if (Data == null)
                    Data = System.Text.Encoding.Default.GetBytes("");
                if (Proxy == null)
                    Proxy = "";

                // 요청 String -> 요청 Byte 변환
                byte[] byteDataParams = Data;

                /////////////////////////////////////////////////////////////////////////////////////
                /* POST */
                // HttpWebRequest 객체 생성, 설정
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
                if (Proxy != "")
                {
                    string[] proxys = System.Text.RegularExpressions.Regex.Split(Proxy, ":");
                    request.Proxy = new WebProxy(proxys[0], Convert.ToInt32(proxys[1]));
                }

                if (Timeout != 0)
                {
                    request.Timeout = Timeout;
                }

                request.Method = Type;    // 기본값 "GET"
                request.UserAgent = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)";

                if (Header != null)
                {
                    for (int i = 0; i < Header.Count(); i++)
                    {
                        string[] t1 = System.Text.RegularExpressions.Regex.Split(Header[i], ": ");
                        if (t1[0] == "User-Agent")
                        {
                            request.UserAgent = t1[1];
                        }
                        else if (t1[0] == "Content-Type")
                        {
                            if (t1[1].ToLower() == "default")
                                request.ContentType = "application/x-www-form-urlencoded";
                            else
                                request.ContentType = t1[1];
                        }
                        else if (t1[0] == "Accept")
                        {
                            request.Accept = t1[1];
                        }
                        else if (t1[0] == "Accept-Encoding")
                        {
                            //request.Headers.Add(Header[i]);
                            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                            //request.TransferEncoding = t1[1];
                        }
                        else if (t1[0] == "Referer")
                        {
                            request.Referer = t1[1];
                        }
                        else if (t1[0] == "Connection")
                        {
                            request.Connection = t1[1];
                        }
                        else
                        {
                            request.Headers.Add(Header[i]);
                        }
                    }
                }
                request.ContentLength = byteDataParams.Length;
                //request.ContentLength = System.Text.Encoding.Default.GetString(Data).Length;
                request.SendChunked = false;

                /* GET */
                // GET 방식은 Uri 뒤에 보낼 데이터를 입력하시면 됩니다.
                /*
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(strUri + "?" + dataParams);
                request.Method = "GET";
                */
                //////////////////////////////////////////////////////////////////////////////////////


                // 요청 Byte -> 요청 Stream 변환
                if (Type == "POST")
                {
                    Stream stDataParams = request.GetRequestStream();
                    stDataParams.Write(byteDataParams, 0, byteDataParams.Length);
                    stDataParams.Close();
                }

                // 요청, 응답 받기
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                System.Text.Encoding enc = null;
                if (ResultEncoding.ToLower() == "default")
                    enc = System.Text.Encoding.Default;
                else
                    enc = System.Text.Encoding.GetEncoding(ResultEncoding);

                // 응답 Stream 읽기
                Stream stReadData = response.GetResponseStream();
                StreamReader srReadData = new StreamReader(stReadData, enc);

                // 응답 Stream -> 응답 String 변환
                string preResult = "";
                string strResult = "";

                if (ResultType.ToUpper() == "ALL")
                {
                    preResult = srReadData.ReadToEnd();
                }

                if (ResultType.ToUpper() == "COOKIE" || ResultType.ToUpper() == "ALL")
                {
                    strResult = response.Headers.ToString();
                    strResult = strResult.Replace("HttpOnly,", "");
                    strResult = strResult.Replace("Path=/,", "");

                    try
                    {
                        StringBuilder newheader = new StringBuilder();
                        string[] rheaders = System.Text.RegularExpressions.Regex.Split(strResult, "Set-Cookie: ");

                        for (int ri = 1; ri < rheaders.Count(); ri++)
                        {
                            string[] rheaders2 = System.Text.RegularExpressions.Regex.Split(rheaders[ri], "\r\n");
                            rheaders2 = System.Text.RegularExpressions.Regex.Split(rheaders2[0], ";");

                            for (int i = 0; i < rheaders2.Count(); i++)
                            {
                                if (rheaders2[i] != "")
                                {
                                    if (rheaders2[i].Substring(0, 1) == " ")
                                        rheaders2[i] = rheaders2[i].Substring(1);

                                    if (rheaders2[i].Substring(0, 1) == ",")
                                        rheaders2[i] = rheaders2[i].Substring(1);

                                    string[] rh2 = System.Text.RegularExpressions.Regex.Split(rheaders2[i], "=");
                                    if (rh2[0].ToUpper() != "PATH" && rh2[0].ToUpper() != "DOMAIN" && rh2[0].ToUpper() != "EXPIRES")
                                    {
                                        newheader.Append(rheaders2[i] + "; ");
                                    }
                                    else if (rh2[1].IndexOf(",") != -1 && rh2[1].IndexOf("GMT") == -1)
                                    {
                                        rh2 = System.Text.RegularExpressions.Regex.Split(rheaders2[i], ",");
                                        newheader.Append(rh2[1] + "; ");
                                    }
                                }
                            }
                        }
                        strResult = newheader.ToString();
                    }
                    catch
                    {

                    }
                }
                else
                {
                    strResult = srReadData.ReadToEnd();
                }

                if (preResult == "")
                    return strResult;
                else
                    return preResult + "/RESULT/" + strResult;
            }
            catch (Exception ee)
            {
                return "Error: " + ee.Message;
            }
        }
        #endregion

        #region HTTP SEND SECURE COOKIE
        public CookieCollection HttpSendSecureCookie(string ResultType, string ResultEncoding, string Type, string Url, List<string> Header, StringBuilder Data, string Proxy, int Timeout)
        {
            try
            {
                if (Data == null)
                    Data = new StringBuilder("");
                if (Proxy == null)
                    Proxy = "";

                // 요청 String -> 요청 Byte 변환
                byte[] byteDataParams = UTF8Encoding.UTF8.GetBytes(Data.ToString());

                /////////////////////////////////////////////////////////////////////////////////////
                /* POST */
                // HttpWebRequest 객체 생성, 설정
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
                if (Proxy != "")
                {
                    string[] proxys = System.Text.RegularExpressions.Regex.Split(Proxy, ":");
                    request.Proxy = new WebProxy(proxys[0], Convert.ToInt32(proxys[1]));
                }

                if (Timeout != 0)
                {
                    request.Timeout = Timeout;
                }

                request.Method = Type;    // 기본값 "GET"
                request.UserAgent = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)";

                if (Header != null)
                {
                    for (int i = 0; i < Header.Count(); i++)
                    {
                        string[] t1 = System.Text.RegularExpressions.Regex.Split(Header[i], ": ");
                        if (t1[0] == "User-Agent")
                        {
                            request.UserAgent = t1[1];
                        }
                        else if (t1[0] == "Content-Type")
                        {
                            if (t1[1].ToLower() == "default")
                                request.ContentType = "application/x-www-form-urlencoded";
                            else
                                request.ContentType = t1[1];
                        }
                        else if (t1[0] == "Accept")
                        {
                            request.Accept = t1[1];
                        }
                        else if (t1[0] == "Accept-Encoding")
                        {
                            //request.Headers.Add(Header[i]);
                            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                            //request.TransferEncoding = t1[1];
                        }
                        else if (t1[0] == "Referer")
                        {
                            request.Referer = t1[1];
                        }
                        else if (t1[0] == "Connection")
                        {
                            request.Connection = t1[1];
                        }
                        else
                        {
                            request.Headers.Add(Header[i]);
                        }
                    }
                }
                request.ContentLength = byteDataParams.Length;
                //request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

                CookieContainer container = new CookieContainer();
                request.CookieContainer = container;


                /* GET */
                // GET 방식은 Uri 뒤에 보낼 데이터를 입력하시면 됩니다.
                /*
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(strUri + "?" + dataParams);
                request.Method = "GET";
                */
                //////////////////////////////////////////////////////////////////////////////////////


                // 요청 Byte -> 요청 Stream 변환
                if (Type == "POST")
                {
                    Stream stDataParams = request.GetRequestStream();
                    stDataParams.Write(byteDataParams, 0, byteDataParams.Length);
                    stDataParams.Close();
                }

                // 요청, 응답 받기
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                //response.Cookies = request.CookieContainer.GetCookies(request.RequestUri);

                string charSet = response.CharacterSet;
                //if (charSet == "ISO-8859-1")
                //    charSet = "";
                //else 
                if (charSet.IndexOf("ISO") != -1)
                    charSet = "utf-8";

                if (charSet == "" && ResultEncoding != "default")
                {
                    charSet = ResultEncoding;
                }

                System.Text.Encoding enc;
                if (String.IsNullOrEmpty(charSet))
                    enc = System.Text.Encoding.Default;
                else
                    enc = System.Text.Encoding.GetEncoding(charSet);

                // 응답 Stream 읽기
                Stream stReadData = response.GetResponseStream();
                StreamReader srReadData = new StreamReader(stReadData, enc);

                // 응답 Stream -> 응답 String 변환
                string preResult = "";
                string strResult = "";

                //return container;
                return response.Cookies;
            }
            catch (Exception ee)
            {
                return null;
            }
        }
        #endregion

        public string HttpSendSocket(string ResultType, string ResultEncoding, string Type, string Url, string Header, string Data, string Proxy, int Timeout)
        {
            Uri pUrl = new Uri(Url);

            string request = Type + " " + pUrl.PathAndQuery + " HTTP/1.1\r\n" +
            Header.Replace("Content-Length: ", "Content-Length: " + Data.Length.ToString()) + "\r\n\r\n" + Data;
            
            if (Url.IndexOf("https") != -1)
            {
                // Create a TCP/IP client socket.
                // machineName is the host running the server application.
                TcpClient client = new TcpClient(Dns.GetHostAddresses(pUrl.Authority)[0].ToString(), pUrl.Port);
                //export("Client connected.");
                // Create an SSL stream that will close the client's stream.
                SslStream sslStream = new SslStream(
                    client.GetStream(),
                    false,
                    new RemoteCertificateValidationCallback(ValidateServerCertificate),
                    null
                    );
                // The server name must match the name on the server certificate.
                try
                {
                    string authurl = "";
                    string[] aut = pUrl.Authority.Split(Convert.ToChar("."));
                    for (int i = 1; i < aut.Count(); i++)
                    {
                        if (authurl != "")
                            authurl += ".";

                        authurl += aut[i];
                    }

                    sslStream.AuthenticateAsClient(authurl);
                }
                catch (AuthenticationException e)
                {
                    //export("Exception: " + e.Message);
                    if (e.InnerException != null)
                    {
                        //export("Inner exception: " + e.InnerException.Message);
                    }
                    //export("Authentication failed - closing the connection.");
                    client.Close();
                    return "Error: SSL Fail";
                }
                // Encode a test message into a byte array.
                // Signal the end of the message using the "<EOF>".


                byte[] messsage = Encoding.UTF8.GetBytes(request);
                // Send hello message to the server. 
                sslStream.Write(messsage);
                sslStream.Flush();
                // Read message from the server.
                string serverMessage = ReadMessage(sslStream);
                //export("Server says: " + serverMessage);
                // Close the client connection.
                client.Close();

                return serverMessage;
            }
            else
            {

                // TCP/IP Socket 객체 생성
                Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                // 호스트를 IP로 변경
                IPHostEntry hostEntry = Dns.GetHostEntry(pUrl.Host);
                IPAddress ip = hostEntry.AddressList[0];

                // HTTP 서버 접속
                var httpEndPoint = new IPEndPoint(ip, pUrl.Port);
                sock.Connect(httpEndPoint);

                // Send
                var sendBuff = Encoding.UTF8.GetBytes(request);
                sock.Send(sendBuff, SocketFlags.None);

                // Receive
                byte[] recvBuff = new byte[sock.ReceiveBufferSize];
                int nCount = sock.Receive(recvBuff);

                // 파일 저장
                string result = Encoding.UTF8.GetString(recvBuff, 0, nCount);
                //File.WriteAllText("test.out", result);

                sock.Close();

                return result;
            }
        }

        // The following method is invoked by the RemoteCertificateValidationDelegate.
        public static bool ValidateServerCertificate(
              object sender,
              X509Certificate certificate,
              X509Chain chain,
              SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None)
                return true;

            //export("Certificate error: " + sslPolicyErrors);

            // Do not allow this client to communicate with unauthenticated servers.
            return false;
        }

        static string ReadMessage(SslStream sslStream)
        {
            // Read the  message sent by the server.
            // The end of the message is signaled using the
            // "<EOF>" marker.
            byte[] buffer = new byte[2048];
            StringBuilder messageData = new StringBuilder();
            int bytes = -1;
            int offset = 0;
            try
            {
                do
                {
                    bytes = sslStream.Read(buffer, offset, buffer.Length);
                    offset += bytes;

                    // Use Decoder class to convert from bytes to UTF8
                    // in case a character spans two buffers.
                    Decoder decoder = Encoding.UTF8.GetDecoder();
                    char[] chars = new char[decoder.GetCharCount(buffer, 0, bytes)];
                    decoder.GetChars(buffer, 0, bytes, chars, 0);
                    messageData.Append(chars);
                    // Check for EOF.
                    if (bytes < buffer.Length)
                    {
                        break;
                    }
                } while (bytes != 0);
            }
            catch
            {

            }

            return messageData.ToString();
        }

        #region 인코딩
        public string GetURLEncode(string data, string type)
        {
            try
            {
                return System.Web.HttpUtility.UrlEncode(data, Encoding.GetEncoding(type));
            }
            catch(Exception ee)
            {
                return ee.Message;
            }
        }

        public string GetURLDecode(string data, string type)
        {
            try
            {
                return System.Web.HttpUtility.UrlDecode(data, Encoding.GetEncoding(type));
            }
            catch (Exception ee)
            {
                return ee.Message;
            }
        }
        #endregion

        #region 문자열 MD5 해시
        public string GetMD5Hash(string input)
        {
            MD5 md5Hash = MD5.Create();
            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hash.ComputeHash(System.Text.Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            System.Text.StringBuilder sBuilder = new System.Text.StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }
        #endregion

        #region 컴퓨터 고유번호 제어
        public string GetSYSInfo()
        {
            string body = "";
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_DiskDrive");

            foreach (ManagementObject wmi_HD in searcher.Get())
            {
                body += wmi_HD["Model"].ToString() + wmi_HD["InterfaceType"].ToString();
            }

            searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PhysicalMedia");

            int i = 0;
            foreach (ManagementObject wmi_HD in searcher.Get())
            {
                // get the hardware serial no.
                if (wmi_HD["SerialNumber"] == null)
                    body += "None";
                else
                    body += wmi_HD["SerialNumber"].ToString();

                ++i;
            }

            return body;
        }
        #endregion

        #region 네이버 시간 제어
        public string GetNaverServerDateTime()
        {
            try
            {
                Engine Engine = new Engine();
                string body = Engine.HttpSend("CONTENTS", "DEFAULT", "GET", "http://blog.naver.com/NWatch4Ajax.nhn?blogId=good&skinType=&skinId=0", null, null, null, 0);

                string[] str = System.Text.RegularExpressions.Regex.Split(body, "<starttime>");
                str = System.Text.RegularExpressions.Regex.Split(str[1], "</starttime>");

                return str[0].Substring(0, 4) + "-" + str[0].Substring(4, 2) + "-" + str[0].Substring(6, 2) + "-" + str[0].Substring(8, 2) + "-" + str[0].Substring(10, 2) + "-" + str[0].Substring(12, 2);
            }
            catch
            {
                return "Error";
            }
        }
        #endregion

        #region 일반 시간 제어
        public string GetMinuteSubtract(string time1, string time2)
        {
            //시간 형식 시-분-초
            string[] t1 = System.Text.RegularExpressions.Regex.Split(time1, "-");
            string[] t2 = System.Text.RegularExpressions.Regex.Split(time2, "-");

            int sec = Convert.ToInt32(t1[2]) - Convert.ToInt32(t2[2]);
            if (sec < 0)
            {
                sec = (60 + Convert.ToInt32(t1[2])) - Convert.ToInt32(t2[2]);
                t1[1] = Convert.ToString(Convert.ToInt32(t1[1]) - 1);
            }

            int minute = Convert.ToInt32(t1[2]) - Convert.ToInt32(t2[2]);
            if (minute < 0)
            {
                minute = (60 + Convert.ToInt32(t1[2])) - Convert.ToInt32(t2[2]);
                t1[0] = Convert.ToString(Convert.ToInt32(t1[0]) - 1);
            }

            return minute + "-" + sec;
        }

        public void SetSystemTime(string time)
        {
            //시간 형식 년-월-일-시-분-초

        }
        #endregion

        #region 쿠키 제어
        public string CookieProcess(string OriginalCookie, string RawCookie)
        {
            string[] rct = System.Text.RegularExpressions.Regex.Split(RawCookie, "set-cookie: ");
            if(rct.Count() <= 1)
                rct = System.Text.RegularExpressions.Regex.Split(RawCookie, "Set-Cookie: ");

            for (int i = 1; i < rct.Count(); i++)
            {
                string[] rct2 = System.Text.RegularExpressions.Regex.Split(rct[i], "\r\n");
                OriginalCookie = CookieAssemble(OriginalCookie, rct2[0]);
            }

            return OriginalCookie;
        }

        public string CookieAssemble(string OriginalCookie, string RawData)
        {
            try
            {
                string[] temp = System.Text.RegularExpressions.Regex.Split(RawData, " ");

                for (int i = 0; i < temp.Count(); i++)
                {
                    try
                    {
                        if (temp[i] != "" && temp[i].IndexOf("=") != -1)
                        {
                            string[] temp2 = System.Text.RegularExpressions.Regex.Split(temp[i], "=");
                            string name = temp2[0];

                            if (name.ToLower() != "expires" && name.ToLower() != "path" && name.ToLower() != "domain")
                            {
                                string n2 = name + "=";
                                if (OriginalCookie.IndexOf(" " + name + "=") != -1 || OriginalCookie.Substring(0, n2.Length) == n2)
                                {
                                    temp2 = System.Text.RegularExpressions.Regex.Split(temp2[1], ";");
                                    string value = temp2[0];

                                    temp2 = System.Text.RegularExpressions.Regex.Split(OriginalCookie, name + "=");
                                    temp2 = System.Text.RegularExpressions.Regex.Split(temp2[1], ";");

                                    OriginalCookie = OriginalCookie.Replace(temp2[0], value);
                                }
                                else
                                {
                                    OriginalCookie += temp[i] + " ";
                                }
                            }
                        }
                    }
                    catch
                    {

                    }
                }

                return OriginalCookie;
            }
            catch
            {
                return "Error";
            }
        }
        #endregion

        #region 한글 자모음 분리, 영타 한글변환, 글자 타입 확인
        public string KoreanSeperate(string data)
        {
            int a, b, c;//자소버퍼 초성중성종성순
            string result = "";//분리결과가 저장되는 문자열
            string nresult = "";
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

                    nresult += result;
                    result = "";
                }
                else
                {
                    //result += string.Format("{0}", (char)x);
                    nresult += data[cnt];
                }
            }
            return nresult;
        }

        public string KoreanSeperateProcessView(string data)
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
                        nresult += lastcomp + KoreanAssemble(Convert.ToString((char)ChoSung[a]), Convert.ToString((char)JwungSung[b]), null) + "/";
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

        public string KoreanAssemble(string first, string middle, string last)
        {
            // 초성, 중성, 종성 테이블.
            string m초성Tbl = "ㄱㄲㄴㄷㄸㄹㅁㅂㅃㅅㅆㅇㅈㅉㅊㅋㅌㅍㅎ";
            string m중성Tbl = "ㅏㅐㅑㅒㅓㅔㅕㅖㅗㅘㅙㅚㅛㅜㅝㅞㅟㅠㅡㅢㅣ";
            string m종성Tbl = " ㄱㄲㄳㄴㄵㄶㄷㄹㄺㄻㄼㄽㄾㄿㅀㅁㅂㅄㅅㅆㅇㅈㅊㅋㅌㅍㅎ";
            ushort mUniCode한글Base = 0xAC00;
            ushort mUniCode한글Last = 0xD79F;
            int i초성위치, i중성위치, i종성위치 = 0;
            int iUniCode;
            i초성위치 = m초성Tbl.IndexOf(first);   // 초성 위치
            i중성위치 = m중성Tbl.IndexOf(middle);   // 중성 위치

            if (last != null && last != "")
                i종성위치 = m종성Tbl.IndexOf(last);   // 종성 위치

            // 앞서 만들어 낸 계산식
            if (last != null && last != "")
                iUniCode = mUniCode한글Base + (i초성위치 * 21 + i중성위치) * 28 + i종성위치;
            else
                iUniCode = mUniCode한글Base + (i초성위치 * 21 + i중성위치) * 28;

            // 코드값을 문자로 변환
            char temp = Convert.ToChar(iUniCode);
            return temp.ToString();
        }

        public string KoreanToEnglishTyping(string data)
        {
            try
            {
                string result = "";
                Dictionary<string, string> di = new Dictionary<string, string>();
                di.Add("ㅁ", "a");
                di.Add("ㅠ", "b");
                di.Add("ㅊ", "c");
                di.Add("ㅇ", "d");
                di.Add("ㄷ", "e");
                di.Add("ㄹ", "f");
                di.Add("ㅎ", "g");
                di.Add("ㅗ", "h");
                di.Add("ㅑ", "i");
                di.Add("ㅓ", "j");
                di.Add("ㅏ", "k");
                di.Add("ㅣ", "l");
                di.Add("ㅡ", "m");
                di.Add("ㅜ", "n");
                di.Add("ㅐ", "o");
                di.Add("ㅔ", "p");
                di.Add("ㅂ", "q");
                di.Add("ㄱ", "r");
                di.Add("ㄴ", "s");
                di.Add("ㅅ", "t");
                di.Add("ㅕ", "u");
                di.Add("ㅍ", "v");
                di.Add("ㅈ", "w");
                di.Add("ㅌ", "x");
                di.Add("ㅛ", "y");
                di.Add("ㅋ", "z");

                di.Add("ㅃ", "Q");
                di.Add("ㅉ", "W");
                di.Add("ㄸ", "E");
                di.Add("ㄲ", "R");
                di.Add("ㅆ", "T");
                di.Add("ㅒ", "O");
                di.Add("ㅖ", "P");

                di.Add("ㅘ", "hk");
                di.Add("ㅙ", "ho");
                di.Add("ㅚ", "hl");
                di.Add("ㅝ", "nj");
                di.Add("ㅞ", "np");
                di.Add("ㅟ", "nl");
                di.Add("ㅢ", "ml");
                di.Add("ㄳ", "rt");
                di.Add("ㄵ", "sw");
                di.Add("ㄶ", "sg");
                di.Add("ㄺ", "fr");
                di.Add("ㄻ", "fa");
                di.Add("ㄼ", "fq");
                di.Add("ㄽ", "ft");
                di.Add("ㄾ", "fx");
                di.Add("ㄿ", "fv");
                di.Add("ㅀ", "fg");
                di.Add("ㅄ", "qt");
                
                for (int i = 0; i < data.Length; i++)
                {
                    if (di.ContainsKey(data.Substring(i, 1)))
                    {
                        result += di[data.Substring(i, 1)];
                    }
                    else
                    {
                        result += data.Substring(i, 1);
                    }
                }

                return result;
            }
            catch
            {
                return "ERROR";
            }
        }

        public bool IsKorean(char ch)
        {
            //( 한글자 || 자음 , 모음 )
            if ((0xAC00 <= ch && ch <= 0xD7A3) || (0x3131 <= ch && ch <= 0x318E))
                return true;
            else
                return false;
        }

        public bool IsEnglish(char ch)
        {
            if ((0x61 <= ch && ch <= 0x7A) || (0x41 <= ch && ch <= 0x5A))
                return true;
            else
                return false;
        }

        public bool IsNumeric(char ch)
        {
            if (0x30 <= ch && ch <= 0x39)
                return true;
            else
                return false;
        }

        public bool IsAllowedCharacter(char ch, string allowedCharacters)
        {
            return allowedCharacters.Contains<char>(ch);
        }
        #endregion

        #region 현재 상태
        public string getNowIP(string PROXY)
        {
            try
            {
                List<string> Header = new List<string>();
                Header.Add("Accept: */*");
                Header.Add("User-Agent: Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)");
                string body = HttpSend("CONTENTS", "default", "GET", "http://www.findip.kr/", new List<string>(), new StringBuilder(""), PROXY, 5000);

                string[] tt = System.Text.RegularExpressions.Regex.Split(body, "My IP Address");
                tt = System.Text.RegularExpressions.Regex.Split(tt[1], " : ");
                tt = System.Text.RegularExpressions.Regex.Split(tt[1], "</h1>");

                return tt[0];
            }
            catch (Exception ee)
            {
                return "Error: " + ee.Message;
            }
        }
        #endregion
    }
}
