using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;

namespace RedEyeEngine
{
    public class CoreNaver
    {
        Engine Engine = new Engine();
        Random rd = new Random();

        public string Login(string ID, string PASSWORD, string PROXY)
        {
            string[] keys = System.Text.RegularExpressions.Regex.Split(GetENCPW(ID, PASSWORD), "///");

            List<string> Header = new List<string>();
            Header.Add("Referer: http://static.nid.naver.com/login.nhn?svc=wme&amp;url=http%3A%2F%2Fwww.naver.com&amp;t=2014080701");
            Header.Add("Content-Type: application/x-www-form-urlencoded");
            return Engine.HttpSend("ALL", "utf-8", "POST", "https://nid.naver.com/nidlogin.login", Header, new StringBuilder("enctp=&encpw=" + keys[0] + "&encnm=" + keys[1] + "&svctype=0&id=&pw=&x=" + rd.Next(10, 40).ToString() + "&y=" + rd.Next(10, 40).ToString()), PROXY, 0);

        }

        public string GetCurrentSession(string ID, string PASSWORD, string COOKIE, string PROXY)
        {
            List<string> Header = new List<string>();
            Header.Add("Accept: text/html, application/xhtml+xml, */*");
            Header.Add("Referer: https://nid.naver.com/user2/help/myInfo.nhn?menu=home");
            Header.Add("User-Agent: Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)");
            Header.Add("Cookie: " + COOKIE);
            string body = Engine.HttpSend("CONTENTS", "utf-8", "GET", "https://nid.naver.com/user2/help/myInfo.nhn?m=viewSecurity&menu=security", Header, null, PROXY, 0);
            string[] temp = System.Text.RegularExpressions.Regex.Split(body, "name=\"token_help\" value=\"");
            temp = System.Text.RegularExpressions.Regex.Split(temp[1], "\"");
            string token = temp[0];

            temp = System.Text.RegularExpressions.Regex.Split(GetENCPW(ID, PASSWORD), "///");

            Header.Clear();
            Header.Add("Accept: text/html, application/xhtml+xml, */*");
            Header.Add("Referer: https://nid.naver.com/user2/help/myInfoPasswd.nhn?m=viewInputPasswdForMyInfo&menu=security&token_help=" + token);
            Header.Add("User-Agent: Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)");
            Header.Add("Content-Type: application/x-www-form-urlencoded");
            Header.Add("Connection: Keep-Alive");
            Header.Add("Cookie: " + COOKIE);
            body = Engine.HttpSend("COOKIE", "utf-8", "POST", "https://nid.naver.com/user2/help/myInfoPasswd.nhn?m=actionInputPasswd", Header, new StringBuilder("token_help=" + token + "&encPasswd=" + temp[0] + "&encNm=" + temp[1] + "&upw="), PROXY, 0);
            COOKIE = Engine.CookieAssemble(COOKIE, body);
            System.Threading.Thread.Sleep(20480);

            Header.Clear();
            Header.Add("Accept: text/html, application/xhtml+xml, */*");
            Header.Add("Referer: https://nid.naver.com/user2/help/myInfoPasswd.nhn?m=actionInputPasswd");
            Header.Add("User-Agent: Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)");
            Header.Add("Cookie: " + COOKIE);
            //body = Engine.HttpSend("CONTENTS", "utf-8", "GET", "https://nid.naver.com/user2/help/myInfo.nhn?m=viewSecurity&menu=security", Header, null, PROXY);
            body = Engine.HttpSend("CONTENTS", "utf-8", "GET", "https://nid.naver.com/user2/help/userLoginLog.nhn?m=viewLoginStatus&menu=security&token_help=" + token, Header, null, PROXY, 0);
            return body;

            //return true;
        }

        public string GetENCPW(string ID, string PASSWORD)
        {
            string[] keys = System.Text.RegularExpressions.Regex.Split(Engine.HttpSend("CONTENTS", "DEFAULT", "GET", "http://static.nid.naver.com/enclogin/keys.nhn", null, null, null, 0), ",");

            #region source
            string source = "// All Rights Reserved.\r\n// See \"LICENSE\" for details.\r\n\r\n// Basic JavaScript BN library - subset useful for RSA encryption.\r\n\r\n// Bits per digit\r\nvar dbits;\r\n\r\n// JavaScript engine analysis\r\nvar canary = 0xdeadbeefcafe;\r\nvar j_lm = ((canary&0xffffff)==0xefcafe);\r\n\r\n// (public) Constructor\r\nfunction BigInteger(a,b,c) {\r\n  if(a != null)\r\n    if(\"number\" == typeof a) this.fromNumber(a,b,c);\r\n    else if(b == null && \"string\" != typeof a) this.fromString(a,256);\r\n    else this.fromString(a,b);\r\n}\r\n\r\n// return new, unset BigInteger\r\nfunction nbi() { return new BigInteger(null); }\r\n\r\n// am: Compute w_j += (x*this_i), propagate carries,\r\n// c is initial carry, returns final carry.\r\n// c < 3*dvalue, x < 2*dvalue, this_i < dvalue\r\n// We need to select the fastest one that works in this environment.\r\n\r\n// am1: use a single mult and divide to get the high bits,\r\n// max digit bits should be 26 because\r\n// max internal value = 2*dvalue^2-2*dvalue (< 2^53)\r\nfunction am1(i,x,w,j,c,n) {\r\n  while(--n >= 0) {\r\n    var v = x*this[i++]+w[j]+c;\r\n    c = Math.floor(v/0x4000000);\r\n    w[j++] = v&0x3ffffff;\r\n  }\r\n  return c;\r\n}\r\n// am2 avoids a big mult-and-extract completely.\r\n// Max digit bits should be <= 30 because we do bitwise ops\r\n// on values up to 2*hdvalue^2-hdvalue-1 (< 2^31)\r\nfunction am2(i,x,w,j,c,n) {\r\n  var xl = x&0x7fff, xh = x>>15;\r\n  while(--n >= 0) {\r\n    var l = this[i]&0x7fff;\r\n    var h = this[i++]>>15;\r\n    var m = xh*l+h*xl;\r\n    l = xl*l+((m&0x7fff)<<15)+w[j]+(c&0x3fffffff);\r\n    c = (l>>>30)+(m>>>15)+xh*h+(c>>>30);\r\n    w[j++] = l&0x3fffffff;\r\n  }\r\n  return c;\r\n}\r\n// Alternately, set max digit bits to 28 since some\r\n// browsers slow down when dealing with 32-bit numbers.\r\nfunction am3(i,x,w,j,c,n) {\r\n  var xl = x&0x3fff, xh = x>>14;\r\n  while(--n >= 0) {\r\n    var l = this[i]&0x3fff;\r\n    var h = this[i++]>>14;\r\n    var m = xh*l+h*xl;\r\n    l = xl*l+((m&0x3fff)<<14)+w[j]+c;\r\n    c = (l>>28)+(m>>14)+xh*h;\r\n    w[j++] = l&0xfffffff;\r\n  }\r\n  return c;\r\n}\r\n\r\nBigInteger.prototype.am = am3;\r\ndbits = 28;\r\n\r\nBigInteger.prototype.DB = dbits;\r\nBigInteger.prototype.DM = ((1<<dbits)-1);\r\nBigInteger.prototype.DV = (1<<dbits);\r\n\r\nvar BI_FP = 52;\r\nBigInteger.prototype.FV = Math.pow(2,BI_FP);\r\nBigInteger.prototype.F1 = BI_FP-dbits;\r\nBigInteger.prototype.F2 = 2*dbits-BI_FP;\r\n\r\n// Digit conversions\r\nvar BI_RM = \"0123456789abcdefghijklmnopqrstuvwxyz\";\r\nvar BI_RC = new Array();\r\nvar rr,vv;\r\nrr = \"0\".charCodeAt(0);\r\nfor(vv = 0; vv <= 9; ++vv) BI_RC[rr++] = vv;\r\nrr = \"a\".charCodeAt(0);\r\nfor(vv = 10; vv < 36; ++vv) BI_RC[rr++] = vv;\r\nrr = \"A\".charCodeAt(0);\r\nfor(vv = 10; vv < 36; ++vv) BI_RC[rr++] = vv;\r\n\r\nfunction int2char(n) { return BI_RM.charAt(n); }\r\nfunction intAt(s,i) {\r\n  var c = BI_RC[s.charCodeAt(i)];\r\n  return (c==null)?-1:c;\r\n}\r\n\r\n// (protected) copy this to r\r\nfunction bnpCopyTo(r) {\r\n  for(var i = this.t-1; i >= 0; --i) r[i] = this[i];\r\n  r.t = this.t;\r\n  r.s = this.s;\r\n}\r\n\r\n// (protected) set from integer value x, -DV <= x < DV\r\nfunction bnpFromInt(x) {\r\n  this.t = 1;\r\n  this.s = (x<0)?-1:0;\r\n  if(x > 0) this[0] = x;\r\n  else if(x < -1) this[0] = x+DV;\r\n  else this.t = 0;\r\n}\r\n\r\n// return bigint initialized to value\r\nfunction nbv(i) { var r = nbi(); r.fromInt(i); return r; }\r\n\r\n// (protected) set from string and radix\r\nfunction bnpFromString(s,b) {\r\n  var k;\r\n  if(b == 16) k = 4;\r\n  else if(b == 8) k = 3;\r\n  else if(b == 256) k = 8; // byte array\r\n  else if(b == 2) k = 1;\r\n  else if(b == 32) k = 5;\r\n  else if(b == 4) k = 2;\r\n  else { this.fromRadix(s,b); return; }\r\n  this.t = 0;\r\n  this.s = 0;\r\n  var i = s.length, mi = false, sh = 0;\r\n  while(--i >= 0) {\r\n    var x = (k==8)?s[i]&0xff:intAt(s,i);\r\n    if(x < 0) {\r\n      if(s.charAt(i) == \"-\") mi = true;\r\n      continue;\r\n    }\r\n    mi = false;\r\n    if(sh == 0)\r\n      this[this.t++] = x;\r\n    else if(sh+k > this.DB) {\r\n      this[this.t-1] |= (x&((1<<(this.DB-sh))-1))<<sh;\r\n      this[this.t++] = (x>>(this.DB-sh));\r\n    }\r\n    else\r\n      this[this.t-1] |= x<<sh;\r\n    sh += k;\r\n    if(sh >= this.DB) sh -= this.DB;\r\n  }\r\n  if(k == 8 && (s[0]&0x80) != 0) {\r\n    this.s = -1;\r\n    if(sh > 0) this[this.t-1] |= ((1<<(this.DB-sh))-1)<<sh;\r\n  }\r\n  this.clamp();\r\n  if(mi) BigInteger.ZERO.subTo(this,this);\r\n}\r\n\r\n// (protected) clamp off excess high words\r\nfunction bnpClamp() {\r\n  var c = this.s&this.DM;\r\n  while(this.t > 0 && this[this.t-1] == c) --this.t;\r\n}\r\n\r\n// (public) return string representation in given radix\r\nfunction bnToString(b) {\r\n  if(this.s < 0) return \"-\"+this.negate().toString(b);\r\n  var k;\r\n  if(b == 16) k = 4;\r\n  else if(b == 8) k = 3;\r\n  else if(b == 2) k = 1;\r\n  else if(b == 32) k = 5;\r\n  else if(b == 4) k = 2;\r\n  else return this.toRadix(b);\r\n  var km = (1<<k)-1, d, m = false, r = \"\", i = this.t;\r\n  var p = this.DB-(i*this.DB)%k;\r\n  if(i-- > 0) {\r\n    if(p < this.DB && (d = this[i]>>p) > 0) { m = true; r = int2char(d); }\r\n    while(i >= 0) {\r\n      if(p < k) {\r\n        d = (this[i]&((1<<p)-1))<<(k-p);\r\n        d |= this[--i]>>(p+=this.DB-k);\r\n      }\r\n      else {\r\n        d = (this[i]>>(p-=k))&km;\r\n        if(p <= 0) { p += this.DB; --i; }\r\n      }\r\n      if(d > 0) m = true;\r\n      if(m) r += int2char(d);\r\n    }\r\n  }\r\n  return m?r:\"0\";\r\n}\r\n\r\n// (public) -this\r\nfunction bnNegate() { var r = nbi(); BigInteger.ZERO.subTo(this,r); return r; }\r\n\r\n// (public) |this|\r\nfunction bnAbs() { return (this.s<0)?this.negate():this; }\r\n\r\n// (public) return + if this > a, - if this < a, 0 if equal\r\nfunction bnCompareTo(a) {\r\n  var r = this.s-a.s;\r\n  if(r != 0) return r;\r\n  var i = this.t;\r\n  r = i-a.t;\r\n  if(r != 0) return r;\r\n  while(--i >= 0) if((r=this[i]-a[i]) != 0) return r;\r\n  return 0;\r\n}\r\n\r\n// returns bit length of the integer x\r\nfunction nbits(x) {\r\n  var r = 1, t;\r\n  if((t=x>>>16) != 0) { x = t; r += 16; }\r\n  if((t=x>>8) != 0) { x = t; r += 8; }\r\n  if((t=x>>4) != 0) { x = t; r += 4; }\r\n  if((t=x>>2) != 0) { x = t; r += 2; }\r\n  if((t=x>>1) != 0) { x = t; r += 1; }\r\n  return r;\r\n}\r\n\r\n// (public) return the number of bits in \"this\"\r\nfunction bnBitLength() {\r\n  if(this.t <= 0) return 0;\r\n  return this.DB*(this.t-1)+nbits(this[this.t-1]^(this.s&this.DM));\r\n}\r\n\r\n// (protected) r = this << n*DB\r\nfunction bnpDLShiftTo(n,r) {\r\n  var i;\r\n  for(i = this.t-1; i >= 0; --i) r[i+n] = this[i];\r\n  for(i = n-1; i >= 0; --i) r[i] = 0;\r\n  r.t = this.t+n;\r\n  r.s = this.s;\r\n}\r\n\r\n// (protected) r = this >> n*DB\r\nfunction bnpDRShiftTo(n,r) {\r\n  for(var i = n; i < this.t; ++i) r[i-n] = this[i];\r\n  r.t = Math.max(this.t-n,0);\r\n  r.s = this.s;\r\n}\r\n\r\n// (protected) r = this << n\r\nfunction bnpLShiftTo(n,r) {\r\n  var bs = n%this.DB;\r\n  var cbs = this.DB-bs;\r\n  var bm = (1<<cbs)-1;\r\n  var ds = Math.floor(n/this.DB), c = (this.s<<bs)&this.DM, i;\r\n  for(i = this.t-1; i >= 0; --i) {\r\n    r[i+ds+1] = (this[i]>>cbs)|c;\r\n    c = (this[i]&bm)<<bs;\r\n  }\r\n  for(i = ds-1; i >= 0; --i) r[i] = 0;\r\n  r[ds] = c;\r\n  r.t = this.t+ds+1;\r\n  r.s = this.s;\r\n  r.clamp();\r\n}\r\n\r\n// (protected) r = this >> n\r\nfunction bnpRShiftTo(n,r) {\r\n  r.s = this.s;\r\n  var ds = Math.floor(n/this.DB);\r\n  if(ds >= this.t) { r.t = 0; return; }\r\n  var bs = n%this.DB;\r\n  var cbs = this.DB-bs;\r\n  var bm = (1<<bs)-1;\r\n  r[0] = this[ds]>>bs;\r\n  for(var i = ds+1; i < this.t; ++i) {\r\n    r[i-ds-1] |= (this[i]&bm)<<cbs;\r\n    r[i-ds] = this[i]>>bs;\r\n  }\r\n  if(bs > 0) r[this.t-ds-1] |= (this.s&bm)<<cbs;\r\n  r.t = this.t-ds;\r\n  r.clamp();\r\n}\r\n\r\n// (protected) r = this - a\r\nfunction bnpSubTo(a,r) {\r\n  var i = 0, c = 0, m = Math.min(a.t,this.t);\r\n  while(i < m) {\r\n    c += this[i]-a[i];\r\n    r[i++] = c&this.DM;\r\n    c >>= this.DB;\r\n  }\r\n  if(a.t < this.t) {\r\n    c -= a.s;\r\n    while(i < this.t) {\r\n      c += this[i];\r\n      r[i++] = c&this.DM;\r\n      c >>= this.DB;\r\n    }\r\n    c += this.s;\r\n  }\r\n  else {\r\n    c += this.s;\r\n    while(i < a.t) {\r\n      c -= a[i];\r\n      r[i++] = c&this.DM;\r\n      c >>= this.DB;\r\n    }\r\n    c -= a.s;\r\n  }\r\n  r.s = (c<0)?-1:0;\r\n  if(c < -1) r[i++] = this.DV+c;\r\n  else if(c > 0) r[i++] = c;\r\n  r.t = i;\r\n  r.clamp();\r\n}\r\n\r\n// (protected) r = this * a, r != this,a (HAC 14.12)\r\n// \"this\" should be the larger one if appropriate.\r\nfunction bnpMultiplyTo(a,r) {\r\n  var x = this.abs(), y = a.abs();\r\n  var i = x.t;\r\n  r.t = i+y.t;\r\n  while(--i >= 0) r[i] = 0;\r\n  for(i = 0; i < y.t; ++i) r[i+x.t] = x.am(0,y[i],r,i,0,x.t);\r\n  r.s = 0;\r\n  r.clamp();\r\n  if(this.s != a.s) BigInteger.ZERO.subTo(r,r);\r\n}\r\n\r\n// (protected) r = this^2, r != this (HAC 14.16)\r\nfunction bnpSquareTo(r) {\r\n  var x = this.abs();\r\n  var i = r.t = 2*x.t;\r\n  while(--i >= 0) r[i] = 0;\r\n  for(i = 0; i < x.t-1; ++i) {\r\n    var c = x.am(i,x[i],r,2*i,0,1);\r\n    if((r[i+x.t]+=x.am(i+1,2*x[i],r,2*i+1,c,x.t-i-1)) >= x.DV) {\r\n      r[i+x.t] -= x.DV;\r\n      r[i+x.t+1] = 1;\r\n    }\r\n  }\r\n  if(r.t > 0) r[r.t-1] += x.am(i,x[i],r,2*i,0,1);\r\n  r.s = 0;\r\n  r.clamp();\r\n}\r\n\r\n// (protected) divide this by m, quotient and remainder to q, r (HAC 14.20)\r\n// r != q, this != m.  q or r may be null.\r\nfunction bnpDivRemTo(m,q,r) {\r\n  var pm = m.abs();\r\n  if(pm.t <= 0) return;\r\n  var pt = this.abs();\r\n  if(pt.t < pm.t) {\r\n    if(q != null) q.fromInt(0);\r\n    if(r != null) this.copyTo(r);\r\n    return;\r\n  }\r\n  if(r == null) r = nbi();\r\n  var y = nbi(), ts = this.s, ms = m.s;\r\n  var nsh = this.DB-nbits(pm[pm.t-1]);\t// normalize modulus\r\n  if(nsh > 0) { pm.lShiftTo(nsh,y); pt.lShiftTo(nsh,r); }\r\n  else { pm.copyTo(y); pt.copyTo(r); }\r\n  var ys = y.t;\r\n  var y0 = y[ys-1];\r\n  if(y0 == 0) return;\r\n  var yt = y0*(1<<this.F1)+((ys>1)?y[ys-2]>>this.F2:0);\r\n  var d1 = this.FV/yt, d2 = (1<<this.F1)/yt, e = 1<<this.F2;\r\n  var i = r.t, j = i-ys, t = (q==null)?nbi():q;\r\n  y.dlShiftTo(j,t);\r\n  if(r.compareTo(t) >= 0) {\r\n    r[r.t++] = 1;\r\n    r.subTo(t,r);\r\n  }\r\n  BigInteger.ONE.dlShiftTo(ys,t);\r\n  t.subTo(y,y);\t// \"negative\" y so we can replace sub with am later\r\n  while(y.t < ys) y[y.t++] = 0;\r\n  while(--j >= 0) {\r\n    // Estimate quotient digit\r\n    var qd = (r[--i]==y0)?this.DM:Math.floor(r[i]*d1+(r[i-1]+e)*d2);\r\n    if((r[i]+=y.am(0,qd,r,j,0,ys)) < qd) {\t// Try it out\r\n      y.dlShiftTo(j,t);\r\n      r.subTo(t,r);\r\n      while(r[i] < --qd) r.subTo(t,r);\r\n    }\r\n  }\r\n  if(q != null) {\r\n    r.drShiftTo(ys,q);\r\n    if(ts != ms) BigInteger.ZERO.subTo(q,q);\r\n  }\r\n  r.t = ys;\r\n  r.clamp();\r\n  if(nsh > 0) r.rShiftTo(nsh,r);\t// Denormalize remainder\r\n  if(ts < 0) BigInteger.ZERO.subTo(r,r);\r\n}\r\n\r\n// (public) this mod a\r\nfunction bnMod(a) {\r\n  var r = nbi();\r\n  this.abs().divRemTo(a,null,r);\r\n  if(this.s < 0 && r.compareTo(BigInteger.ZERO) > 0) a.subTo(r,r);\r\n  return r;\r\n}\r\n\r\n// Modular reduction using \"classic\" algorithm\r\nfunction Classic(m) { this.m = m; }\r\nfunction cConvert(x) {\r\n  if(x.s < 0 || x.compareTo(this.m) >= 0) return x.mod(this.m);\r\n  else return x;\r\n}\r\nfunction cRevert(x) { return x; }\r\nfunction cReduce(x) { x.divRemTo(this.m,null,x); }\r\nfunction cMulTo(x,y,r) { x.multiplyTo(y,r); this.reduce(r); }\r\nfunction cSqrTo(x,r) { x.squareTo(r); this.reduce(r); }\r\n\r\nClassic.prototype.convert = cConvert;\r\nClassic.prototype.revert = cRevert;\r\nClassic.prototype.reduce = cReduce;\r\nClassic.prototype.mulTo = cMulTo;\r\nClassic.prototype.sqrTo = cSqrTo;\r\n\r\n// (protected) return \"-1/this % 2^DB\"; useful for Mont. reduction\r\n// justification:\r\n//         xy == 1 (mod m)\r\n//         xy =  1+km\r\n//   xy(2-xy) = (1+km)(1-km)\r\n// x[y(2-xy)] = 1-k^2m^2\r\n// x[y(2-xy)] == 1 (mod m^2)\r\n// if y is 1/x mod m, then y(2-xy) is 1/x mod m^2\r\n// should reduce x and y(2-xy) by m^2 at each step to keep size bounded.\r\n// JS multiply \"overflows\" differently from C/C++, so care is needed here.\r\nfunction bnpInvDigit() {\r\n  if(this.t < 1) return 0;\r\n  var x = this[0];\r\n  if((x&1) == 0) return 0;\r\n  var y = x&3;\t\t// y == 1/x mod 2^2\r\n  y = (y*(2-(x&0xf)*y))&0xf;\t// y == 1/x mod 2^4\r\n  y = (y*(2-(x&0xff)*y))&0xff;\t// y == 1/x mod 2^8\r\n  y = (y*(2-(((x&0xffff)*y)&0xffff)))&0xffff;\t// y == 1/x mod 2^16\r\n  // last step - calculate inverse mod DV directly;\r\n  // assumes 16 < DB <= 32 and assumes ability to handle 48-bit ints\r\n  y = (y*(2-x*y%this.DV))%this.DV;\t\t// y == 1/x mod 2^dbits\r\n  // we really want the negative inverse, and -DV < y < DV\r\n  return (y>0)?this.DV-y:-y;\r\n}\r\n\r\n// Montgomery reduction\r\nfunction Montgomery(m) {\r\n  this.m = m;\r\n  this.mp = m.invDigit();\r\n  this.mpl = this.mp&0x7fff;\r\n  this.mph = this.mp>>15;\r\n  this.um = (1<<(m.DB-15))-1;\r\n  this.mt2 = 2*m.t;\r\n}\r\n\r\n// xR mod m\r\nfunction montConvert(x) {\r\n  var r = nbi();\r\n  x.abs().dlShiftTo(this.m.t,r);\r\n  r.divRemTo(this.m,null,r);\r\n  if(x.s < 0 && r.compareTo(BigInteger.ZERO) > 0) this.m.subTo(r,r);\r\n  return r;\r\n}\r\n\r\n// x/R mod m\r\nfunction montRevert(x) {\r\n  var r = nbi();\r\n  x.copyTo(r);\r\n  this.reduce(r);\r\n  return r;\r\n}\r\n\r\n// x = x/R mod m (HAC 14.32)\r\nfunction montReduce(x) {\r\n  while(x.t <= this.mt2)\t// pad x so am has enough room later\r\n    x[x.t++] = 0;\r\n  for(var i = 0; i < this.m.t; ++i) {\r\n    // faster way of calculating u0 = x[i]*mp mod DV\r\n    var j = x[i]&0x7fff;\r\n    var u0 = (j*this.mpl+(((j*this.mph+(x[i]>>15)*this.mpl)&this.um)<<15))&x.DM;\r\n    // use am to combine the multiply-shift-add into one call\r\n    j = i+this.m.t;\r\n    x[j] += this.m.am(0,u0,x,i,0,this.m.t);\r\n    // propagate carry\r\n    while(x[j] >= x.DV) { x[j] -= x.DV; x[++j]++; }\r\n  }\r\n  x.clamp();\r\n  x.drShiftTo(this.m.t,x);\r\n  if(x.compareTo(this.m) >= 0) x.subTo(this.m,x);\r\n}\r\n\r\n// r = \"x^2/R mod m\"; x != r\r\nfunction montSqrTo(x,r) { x.squareTo(r); this.reduce(r); }\r\n\r\n// r = \"xy/R mod m\"; x,y != r\r\nfunction montMulTo(x,y,r) { x.multiplyTo(y,r); this.reduce(r); }\r\n\r\nMontgomery.prototype.convert = montConvert;\r\nMontgomery.prototype.revert = montRevert;\r\nMontgomery.prototype.reduce = montReduce;\r\nMontgomery.prototype.mulTo = montMulTo;\r\nMontgomery.prototype.sqrTo = montSqrTo;\r\n\r\n// (protected) true iff this is even\r\nfunction bnpIsEven() { return ((this.t>0)?(this[0]&1):this.s) == 0; }\r\n\r\n// (protected) this^e, e < 2^32, doing sqr and mul with \"r\" (HAC 14.79)\r\nfunction bnpExp(e,z) {\r\n  if(e > 0xffffffff || e < 1) return BigInteger.ONE;\r\n  var r = nbi(), r2 = nbi(), g = z.convert(this), i = nbits(e)-1;\r\n  g.copyTo(r);\r\n  while(--i >= 0) {\r\n    z.sqrTo(r,r2);\r\n    if((e&(1<<i)) > 0) z.mulTo(r2,g,r);\r\n    else { var t = r; r = r2; r2 = t; }\r\n  }\r\n  return z.revert(r);\r\n}\r\n\r\n// (public) this^e % m, 0 <= e < 2^32\r\nfunction bnModPowInt(e,m) {\r\n  var z;\r\n  if(e < 256 || m.isEven()) z = new Classic(m); else z = new Montgomery(m);\r\n  return this.exp(e,z);\r\n}\r\n\r\n// protected\r\nBigInteger.prototype.copyTo = bnpCopyTo;\r\nBigInteger.prototype.fromInt = bnpFromInt;\r\nBigInteger.prototype.fromString = bnpFromString;\r\nBigInteger.prototype.clamp = bnpClamp;\r\nBigInteger.prototype.dlShiftTo = bnpDLShiftTo;\r\nBigInteger.prototype.drShiftTo = bnpDRShiftTo;\r\nBigInteger.prototype.lShiftTo = bnpLShiftTo;\r\nBigInteger.prototype.rShiftTo = bnpRShiftTo;\r\nBigInteger.prototype.subTo = bnpSubTo;\r\nBigInteger.prototype.multiplyTo = bnpMultiplyTo;\r\nBigInteger.prototype.squareTo = bnpSquareTo;\r\nBigInteger.prototype.divRemTo = bnpDivRemTo;\r\nBigInteger.prototype.invDigit = bnpInvDigit;\r\nBigInteger.prototype.isEven = bnpIsEven;\r\nBigInteger.prototype.exp = bnpExp;\r\n\r\n// public\r\nBigInteger.prototype.toString = bnToString;\r\nBigInteger.prototype.negate = bnNegate;\r\nBigInteger.prototype.abs = bnAbs;\r\nBigInteger.prototype.compareTo = bnCompareTo;\r\nBigInteger.prototype.bitLength = bnBitLength;\r\nBigInteger.prototype.mod = bnMod;\r\nBigInteger.prototype.modPowInt = bnModPowInt;\r\n\r\n// \"constants\"\r\nBigInteger.ZERO = nbv(0);\r\nBigInteger.ONE = nbv(1);\r\n// prng4.js - uses Arcfour as a PRNG\r\n\r\nfunction Arcfour() {\r\n  this.i = 0;\r\n  this.j = 0;\r\n  this.S = new Array();\r\n}\r\n\r\n// Initialize arcfour context from key, an array of ints, each from [0..255]\r\nfunction ARC4init(key) {\r\n  var i, j, t;\r\n  for(i = 0; i < 256; ++i)\r\n    this.S[i] = i;\r\n  j = 0;\r\n  for(i = 0; i < 256; ++i) {\r\n    j = (j + this.S[i] + key[i % key.length]) & 255;\r\n    t = this.S[i];\r\n    this.S[i] = this.S[j];\r\n    this.S[j] = t;\r\n  }\r\n  this.i = 0;\r\n  this.j = 0;\r\n}\r\n\r\nfunction ARC4next() {\r\n  var t;\r\n  this.i = (this.i + 1) & 255;\r\n  this.j = (this.j + this.S[this.i]) & 255;\r\n  t = this.S[this.i];\r\n  this.S[this.i] = this.S[this.j];\r\n  this.S[this.j] = t;\r\n  return this.S[(t + this.S[this.i]) & 255];\r\n}\r\n\r\nArcfour.prototype.init = ARC4init;\r\nArcfour.prototype.next = ARC4next;\r\n\r\n// Plug in your RNG constructor here\r\nfunction prng_newstate() {\r\n  return new Arcfour();\r\n}\r\n\r\n// Pool size must be a multiple of 4 and greater than 32.\r\n// An array of bytes the size of the pool will be passed to init()\r\nvar rng_psize = 256;\r\n// Random number generator - requires a PRNG backend, e.g. prng4.js\r\n\r\n// For best results, put code like\r\n// <body onClick='rng_seed_time();' onKeyPress='rng_seed_time();'>\r\n// in your main HTML document.\r\n\r\nvar rng_state;\r\nvar rng_pool;\r\nvar rng_pptr;\r\n\r\n// Mix in a 32-bit integer into the pool\r\nfunction rng_seed_int(x) {\r\n  rng_pool[rng_pptr++] ^= x & 255;\r\n  rng_pool[rng_pptr++] ^= (x >> 8) & 255;\r\n  rng_pool[rng_pptr++] ^= (x >> 16) & 255;\r\n  rng_pool[rng_pptr++] ^= (x >> 24) & 255;\r\n  if(rng_pptr >= rng_psize) rng_pptr -= rng_psize;\r\n}\r\n\r\n// Mix in the current time (w/milliseconds) into the pool\r\nfunction rng_seed_time() {\r\n  rng_seed_int(new Date().getTime());\r\n}\r\n\r\n// Initialize the pool with junk if needed.\r\nif(rng_pool == null) {\r\n  rng_pool = new Array();\r\n  rng_pptr = 0;\r\n  var t;\r\n  /*\r\n  if(navigator.appName == \"Netscape\" && navigator.appVersion < \"5\" && window.crypto) {\r\n    // Extract entropy (256 bits) from NS4 RNG if available\r\n    alert(z);\r\n    var z = window.crypto.random(32);\r\n    for(t = 0; t < z.length; ++t)\r\n      rng_pool[rng_pptr++] = z.charCodeAt(t) & 255;\r\n  } \r\n  */ \r\n  while(rng_pptr < rng_psize) {  // extract some randomness from Math.random()\r\n    t = Math.floor(65536 * Math.random());\r\n    rng_pool[rng_pptr++] = t >>> 8;\r\n    rng_pool[rng_pptr++] = t & 255;\r\n  }\r\n  rng_pptr = 0;\r\n  rng_seed_time();\r\n  //rng_seed_int(window.screenX);\r\n  //rng_seed_int(window.screenY);\r\n}\r\n\r\nfunction rng_get_byte() {\r\n  if(rng_state == null) {\r\n    rng_seed_time();\r\n    rng_state = prng_newstate();\r\n    rng_state.init(rng_pool);\r\n    for(rng_pptr = 0; rng_pptr < rng_pool.length; ++rng_pptr)\r\n      rng_pool[rng_pptr] = 0;\r\n    rng_pptr = 0;\r\n    //rng_pool = null;\r\n  }\r\n  // TODO: allow reseeding after first request\r\n  return rng_state.next();\r\n}\r\n\r\nfunction rng_get_bytes(ba) {\r\n  var i;\r\n  for(i = 0; i < ba.length; ++i) ba[i] = rng_get_byte();\r\n}\r\n\r\nfunction SecureRandom() {}\r\n\r\nSecureRandom.prototype.nextBytes = rng_get_bytes;\r\n// Depends on jsbn.js and rng.js\r\n\r\n// convert a (hex) string to a bignum object\r\nfunction parseBigInt(str,r) {\r\n  return new BigInteger(str,r);\r\n}\r\n\r\nfunction linebrk(s,n) {\r\n  var ret = \"\";\r\n  var i = 0;\r\n  while(i + n < s.length) {\r\n    ret += s.substring(i,i+n) + \"\\n\";\r\n    i += n;\r\n  }\r\n  return ret + s.substring(i,s.length);\r\n}\r\n\r\nfunction byte2Hex(b) {\r\n  if(b < 0x10)\r\n    return \"0\" + b.toString(16);\r\n  else\r\n    return b.toString(16);\r\n}\r\n\r\n// PKCS#1 (type 2, random) pad input string s to n bytes, and return a bigint\r\nfunction pkcs1pad2(s,n) {\r\n  if(n < s.length + 11) {\r\n    alert(\"Message too long for RSA\");\r\n    return null;\r\n  }\r\n  var ba = new Array();\r\n  var i = s.length - 1;\r\n  while(i >= 0 && n > 0) ba[--n] = s.charCodeAt(i--);\r\n  ba[--n] = 0;\r\n  var rng = new SecureRandom();\r\n  var x = new Array();\r\n  while(n > 2) { // random non-zero pad\r\n    x[0] = 0;\r\n    while(x[0] == 0) rng.nextBytes(x);\r\n    ba[--n] = x[0];\r\n  }\r\n  ba[--n] = 2;\r\n  ba[--n] = 0;\r\n  return new BigInteger(ba);\r\n}\r\n\r\n// \"empty\" RSA key constructor\r\nfunction RSAKey() {\r\n  this.n = null;\r\n  this.e = 0;\r\n  this.d = null;\r\n  this.p = null;\r\n  this.q = null;\r\n  this.dmp1 = null;\r\n  this.dmq1 = null;\r\n  this.coeff = null;\r\n}\r\n\r\n// Set the public key fields N and e from hex strings\r\nfunction RSASetPublic(N,E) {\r\n  if(N != null && E != null && N.length > 0 && E.length > 0) {\r\n    this.n = parseBigInt(N,16);\r\n    this.e = parseInt(E,16);\r\n  }\r\n  else\r\n    alert(\"Invalid RSA public key\");\r\n}\r\n\r\n// Perform raw public operation on \"x\": return x^e (mod n)\r\nfunction RSADoPublic(x) {\r\n  return x.modPowInt(this.e, this.n);\r\n}\r\n\r\n// Return the PKCS#1 RSA encryption of \"text\" as an even-length hex string\r\nfunction RSAEncrypt(text) {\r\n  var m = pkcs1pad2(text,(this.n.bitLength()+7)>>3);\r\n  if(m == null) return null;\r\n  var c = this.doPublic(m);\r\n  if(c == null) return null;\r\n  var h = c.toString(16);\r\n\tvar gap = (((this.n.bitLength()+7)>>3)<<1) - h.length;\r\n\twhile(gap-- > 0) h = \"0\" + h;\r\n//  if((h.length & 1) == 0) return h; else return \"0\" + h;\r\n\treturn h;\r\n}\r\n\r\n// Return the PKCS#1 RSA encryption of \"text\" as a Base64-encoded string\r\n//function RSAEncryptB64(text) {\r\n//  var h = this.encrypt(text);\r\n//  if(h) return hex2b64(h); else return null;\r\n//}\r\n\r\n// protected\r\nRSAKey.prototype.doPublic = RSADoPublic;\r\n\r\n// public\r\nRSAKey.prototype.setPublic = RSASetPublic;\r\nRSAKey.prototype.encrypt = RSAEncrypt;\r\n//RSAKey.prototype.encrypt_b64 = RSAEncryptB64;\r\nvar b64map=\"ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/\";\r\nvar b64pad=\"=\";\r\n\r\nfunction hex2b64(h) {\r\n  var i;\r\n  var c;\r\n  var ret = \"\";\r\n  for(i = 0; i+3 <= h.length; i+=3) {\r\n    c = parseInt(h.substring(i,i+3),16);\r\n    ret += b64map.charAt(c >> 6) + b64map.charAt(c & 63);\r\n  }\r\n  if(i+1 == h.length) {\r\n    c = parseInt(h.substring(i,i+1),16);\r\n    ret += b64map.charAt(c << 2);\r\n  }\r\n  else if(i+2 == h.length) {\r\n    c = parseInt(h.substring(i,i+2),16);\r\n    ret += b64map.charAt(c >> 2) + b64map.charAt((c & 3) << 4);\r\n  }\r\n  while((ret.length & 3) > 0) ret += b64pad;\r\n  return ret;\r\n}\r\n\r\n// convert a base64 string to hex\r\nfunction b64tohex(s) {\r\n  var ret = \"\"\r\n  var i;\r\n  var k = 0; // b64 state, 0-3\r\n  var slop;\r\n  for(i = 0; i < s.length; ++i) {\r\n    if(s.charAt(i) == b64pad) break;\r\n    v = b64map.indexOf(s.charAt(i));\r\n    if(v < 0) continue;\r\n    if(k == 0) {\r\n      ret += int2char(v >> 2);\r\n      slop = v & 3;\r\n      k = 1;\r\n    }\r\n    else if(k == 1) {\r\n      ret += int2char((slop << 2) | (v >> 4));\r\n      slop = v & 0xf;\r\n      k = 2;\r\n    }\r\n    else if(k == 2) {\r\n      ret += int2char(slop);\r\n      ret += int2char(v >> 2);\r\n      slop = v & 3;\r\n      k = 3;\r\n    }\r\n    else {\r\n      ret += int2char((slop << 2) | (v >> 4));\r\n      ret += int2char(v & 0xf);\r\n      k = 0;\r\n    }\r\n  }\r\n  if(k == 1)\r\n    ret += int2char(slop << 2);\r\n  return ret;\r\n}\r\n\r\n// convert a base64 string to a byte/number array\r\nfunction b64toBA(s) {\r\n  //piggyback on b64tohex for now, optimize later\r\n  var h = b64tohex(s);\r\n  var i;\r\n  var a = new Array();\r\n  for(i = 0; 2*i < h.length; ++i) {\r\n    a[i] = parseInt(h.substring(2*i,2*i+2),16);\r\n  }\r\n  return a;\r\n}\r\n\r\nfunction getLenChar(texts)\r\n{\r\n  texts = texts + \"\";\r\n  return String.fromCharCode(texts.length);\r\n}\r\n\r\nfunction createRsaKey(id, pw, sessionKey, keyName, eValue, nValue)\r\n{\r\n  var rsa = new RSAKey();\r\n  rsa.setPublic(eValue, nValue);\r\n    \r\n  var comVal = getLenChar(sessionKey) + sessionKey + getLenChar(id) + id;\r\n  return rsa.encrypt(comVal + getLenChar(pw) + pw);\r\n}";
            #endregion
            MSScriptControl.ScriptControl sc = new MSScriptControl.ScriptControl();
            sc.Language = "Javascript";
            sc.AddCode(source);

            Object[] oParams = new Object[6] { ID, PASSWORD, keys[0], keys[1], keys[2], keys[3] };
            string encpw = sc.Run("createRsaKey", ref oParams).ToString();

            return encpw + "///" + keys[1];
        }

        public string GetRealtimeKeyword(string PROXY)
        {
            try
            {
                List<string> Header = new List<string>();
                Header.Add("Accept: */*");
                //Header.Add("Referer: http://www.naver.com");
                Header.Add("User-Agent: Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.2; WOW64; Trident/6.0; .NET4.0E; .NET4.0C; .NET CLR 3.5.30729; .NET CLR 2.0.50727; .NET CLR 3.0.30729)");
                string body = Engine.HttpSend("CONTENTS", "utf-8", "GET", "http://rank.search.naver.net/nexearch_utf8_v2.js?sm=tab_lve&callcntdummy=1" + rd.Next(1000, 9999).ToString() + "&dummy=945152" + rd.Next(1000, 9999).ToString(), Header, null, PROXY, 0);
                string[] tt = System.Text.RegularExpressions.Regex.Split(body, "K : \"");
                body = "";
                for (int i = 1; i < tt.Count(); i++)
                {
                    string[] tt2 = System.Text.RegularExpressions.Regex.Split(tt[i], "\"");
                    if (body == "")
                        body = tt2[0];
                    else
                        body += "///" + tt2[0];
                }

                return body;
            }
            catch
            {
                return "ERROR";
            }
        }

        public string GetRandomBlog(string PROXY)
        {
            List<string> Header = new List<string>();
            Header.Add("Accept: image/jpeg, application/x-ms-application, image/gif, application/xaml+xml, image/pjpeg, application/x-ms-xbap, application/vnd.ms-excel, application/vnd.ms-powerpoint, application/msword, */*");
            Header.Add("Referer: http://blog.naver.com/PostList.nhn?blogId=jjppy1&widgetTypeCall=true&topReferer=http%3A%2F%2Fsection.blog.naver.com%2F");
            string body = Engine.HttpSend("CONTENTS", "utf-8", "GET", "http://blog.naver.com/RandomBlog.nhn", Header, new StringBuilder(""), PROXY, 0);
            string[] bodys = System.Text.RegularExpressions.Regex.Split(body, "url=");
            bodys = System.Text.RegularExpressions.Regex.Split(bodys[1], "\"");
            bodys = System.Text.RegularExpressions.Regex.Split(bodys[0], "/");

            return bodys[bodys.Count() - 1];
        }

        public string GetBlogFromCategoryList(string PAGE, string SEQ, string PROXY)
        {
            string result = "";

            try
            {
                List<string> Header = new List<string>();
                Header.Add("Accept: text/html, application/xhtml+xml, image/jxr, */*");
                Header.Add("Referer: http://section.blog.naver.com/main/DirectoryPostList.nhn?option.page.currentPage=1&option.templateKind=0&option.directorySeq=0&option.viewType=default&option.orderBy=date");
                Header.Add("User-Agent: Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko");
                string body = Engine.HttpSend("CONTENTS", "utf-8", "GET", "http://section.blog.naver.com/main/DirectoryPostList.nhn?option.page.currentPage=" + PAGE + "&option.templateKind=0&option.directorySeq=" + SEQ + "&option.viewType=default&option.orderBy=date", Header, new StringBuilder(""), PROXY, 0);

                string[] tt = System.Text.RegularExpressions.Regex.Split(body, "class=\"add_img\"");
                for (int i = 1; i < tt.Count(); i++)
                {
                    string[] tt2 = System.Text.RegularExpressions.Regex.Split(tt[i], "href=\"");
                    tt2 = System.Text.RegularExpressions.Regex.Split(tt2[1], "\"");

                    if (result != "")
                        result += "///";

                    result += tt2[0];
                }
            }
            catch
            {

            }

            return result;
        }

        public string GetCategoryFromCategoryList(string PROXY)
        {
            string result = "";

            try
            {
                List<string> Header = new List<string>();
                Header.Add("Accept: */*");
                Header.Add("Content-Type: application/x-www-form-urlencoded; charset=utf-8");
                Header.Add("Referer: http://section.blog.naver.com/main/DirectoryPostList.nhn?option.page.currentPage=1&option.templateKind=0&option.directorySeq=0&option.viewType=default&option.orderBy=date");
                Header.Add("User-Agent: Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko");
                string body = Engine.HttpSend("CONTENTS", "utf-8", "GET", "http://section.blog.naver.com/main/DirectoryGroupListAsync.nhn", Header, new StringBuilder(""), PROXY, 0);

                string[] tt = System.Text.RegularExpressions.Regex.Split(body, "name\":\"");

                for (int i = 1; i < tt.Count(); i++)
                {
                    if (tt[i].IndexOf("seq\":") != -1)
                    {
                        string[] tt2 = System.Text.RegularExpressions.Regex.Split(tt[i], "\"");
                        string name = tt2[0];

                        tt2 = System.Text.RegularExpressions.Regex.Split(tt[i], "seq\":");
                        tt2 = System.Text.RegularExpressions.Regex.Split(tt2[1], ",");
                        string seq = tt2[0];

                        if (result != "")
                            result += "///";

                        result += name + "/" + seq;
                    }
                }
            }
            catch
            {

            }

            return result;
        }

        public List<string> GetBlogCategory(string BlogID, string PROXY)
        {
            List<string> Header = new List<string>();
            Header.Add("Accept: */*");
            Header.Add("Content-Type: application/x-www-form-urlencoded; charset=utf-8");
            Header.Add("User-Agent: Mozilla/5.0 (Windows NT 6.3; WOW64; Trident/7.0; rv:11.0) like Gecko");
            Header.Add("Referer: http://blog.naver.com/PostList.nhn?blogId=" + BlogID + "&widgetTypeCall=true");
            string body = Engine.HttpSend("CONTENTS", "DEFAULT", "POST", "http://blog.naver.com/WidgetListAsync.nhn", Header, new StringBuilder("blogId=" + BlogID + "&listNumVisitor=1&isVisitorOpen=false&isBuddyOpen=true&selectCategoryNo=&skinId=0&skinType=&isCategoryOpen=true&isEnglish=true&listNumComment=5&areaCode=11B10101&weatherType=0&currencySign=ALL&enableWidgetKeys=content%2Cprofile%2Ccategory%2Cmapview%2Csearch%2Clibrary%2Ctag%2Ccalendar%2Ccomment%2Cvisitor%2Cbuddy%2Cstat%2Ccounter%2Crss%2Cpowered%2Cmenu%2Cgnb%2Cexternalwidget%2Cmusic&writingMaterialListType=1"), PROXY, 0);
            
            List<string> categoryList = new List<string>();
            string[] bodys = System.Text.RegularExpressions.Regex.Split(body, "/RESULT/");
            bodys = System.Text.RegularExpressions.Regex.Split(bodys[0], "widget_category");
            for (int i = 1; i < bodys.Count(); i++)
            {
                string[] body2 = System.Text.RegularExpressions.Regex.Split(bodys[i], "</a>");
                body2 = System.Text.RegularExpressions.Regex.Split(body2[0], "\">");
                categoryList.Add(body2[0].Replace(")", "").Replace("|", "").Trim() + "///" + body2[1].Trim());
            }
            
            return categoryList;
            //return body;
        }

        public bool SetBlogBuddyAdd(string Cookie, string TargetID, string Relation, string Message, string PROXY)
        {
            List<string> Header = new List<string>();
            Header.Add("Accept: image/jpeg, application/x-ms-application, image/gif, application/xaml+xml, image/pjpeg, application/x-ms-xbap, application/vnd.ms-excel, application/vnd.ms-powerpoint, application/msword, */*");
            Header.Add("Referer: http://blog.naver.com/BuddyAdd.nhn");
            Header.Add("User-Agent: Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.2; WOW64; Trident/6.0; .NET4.0E; .NET4.0C; .NET CLR 3.5.30729; .NET CLR 2.0.50727; .NET CLR 3.0.30729)");
            Header.Add("Content-Type: application/x-www-form-urlencoded");
            Header.Add("Cookie: " + Cookie);
            string body = Engine.HttpSend("CONTENTS", "euc-kr", "POST", "http://blog.naver.com/BuddyAdd.nhn", Header, new StringBuilder("blogId=" + TargetID + "&relation=" + Relation + "&groupId=1&groupIdSelector=1&groupName=%BB%F5+%B1%D7%B7%EC&groupOpen=true&message=" + Engine.GetURLEncode(Message, "euc-kr") + "&x=" + rd.Next(10, 35).ToString() + "&y=" + rd.Next(10, 35).ToString()), PROXY, 0);

            if (body.IndexOf("신청되었습니다") != -1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public string Post(string Cookie, string NowID, string Title, string Contents, string Tag, string Category, string Options, string PROXY)
        {
            string[] otmp;
            string imgwidth = "";
            string imgheight = "";
            string imgurl = "";
            string imgpath = "";
            string attach = "";

            if (Options != "")
            {
                string mainthum = "true";

                if (Options.IndexOf("////") != -1)
                {
                    string[] ot = System.Text.RegularExpressions.Regex.Split(Options, "////");

                    for (int i = 0; i < ot.Count(); i++)
                    {
                        otmp = System.Text.RegularExpressions.Regex.Split(ot[i], "///");
                        imgwidth = otmp[1];
                        imgheight = otmp[2];
                        imgurl = otmp[0];
                        string[] itmp = System.Text.RegularExpressions.Regex.Split(imgurl, "/");

                        imgpath = imgurl.Replace("http://blogfiles.naver.net", "");
                        imgpath = imgpath.Replace("/" + itmp[itmp.Count() - 1], "");

                        attach += "&attachFiles=" + System.Web.HttpUtility.UrlEncode("{\"directory\":\"" + imgpath + "\",\"height\":" + imgheight + ",\"id\":-" + Convert.ToString(i + 1) + ",\"isMainThumbnail\":" + mainthum + ",\"key\":\"\",\"name\":\"" + itmp[itmp.Count() - 1] + "\",\"path\":\"\",\"size\":40654,\"status\":\"add\",\"type\":\"I\",\"version\":1,\"width\":" + imgwidth + "}", Encoding.GetEncoding("utf-8"));
                        mainthum = "false";
                    }
                }
                else
                {
                    otmp = System.Text.RegularExpressions.Regex.Split(Options, "///");
                    imgwidth = otmp[1];
                    imgheight = otmp[2];
                    imgurl = otmp[0];
                    string[] itmp = System.Text.RegularExpressions.Regex.Split(imgurl, "/");

                    imgpath = imgurl.Replace("http://blogfiles.naver.net", "");
                    imgpath = imgpath.Replace("/" + itmp[itmp.Count() - 1], "");

                    attach += "&attachFiles=" + System.Web.HttpUtility.UrlEncode("{\"directory\":\"" + imgpath + "\",\"height\":" + imgheight + ",\"id\":-1,\"isMainThumbnail\":" + mainthum + ",\"key\":\"\",\"name\":\"" + itmp[itmp.Count() - 1] + "\",\"path\":\"\",\"size\":40654,\"status\":\"add\",\"type\":\"I\",\"version\":1,\"width\":" + imgwidth + "}", Encoding.GetEncoding("utf-8"));
                    mainthum = "false";
                }
            }
            

            string pbody = "captchaKey=&captchaValue=&appId=&tempLogNo=&blogId=" + NowID + "&post.logNo=&post.sourceCode=0&post.contents.contentsValue=" + System.Web.HttpUtility.UrlEncode(System.Web.HttpUtility.UrlEncode(Contents, Encoding.GetEncoding("utf-8")), Encoding.GetEncoding("utf-8")) + "&post.prePostRegistDirectly=false";
            pbody += "&post.lastRelayTime=&smartEditorVersion=2&post.book.ratingScore=0&post.music.ratingScore=0&post.movie.ratingScore=0&post.scrapedYn=false&post.clientType=&post.contents.summaryYn=false&post.contents.summaryToggleText=";
            pbody += "&post.contents.summaryTogglePosition=&post.templatePhoto.width=0&post.templatePhoto.height=0&post.addedInfoSet.addedInfoStruct=&post.mapAttachmentSet.mapAttachStruct=&post.calendarAttachmentSet.calendarAttachmentStruct=";
            pbody += "&post.musicPlayerAttachmentSet.musicPlayerAttachmentStruct=&post.eventWriteInfo.eventCode=&post.eventWriteInfo.writeCode=&post.eventWriteInfo.eventType=&post.eventWriteInfo.eventLink=&post.postOptions.openType=2";
            pbody += "&post.postOptions.commentYn=true&post.postOptions.isRelayOpen=true&post.postOptions.sympathyYn=true&post.postOptions.outSideAllowYn=true&post.me2dayPostingYn=&post.facebookPostingYn=false&post.twitterPostingYn=false";
            pbody += "&post.postOptions.searchYn=true&post.postOptions.rssOpenYn=true&post.postOptions.scrapType=2&post.postOptions.ccl.commercialUsesYn=false&post.postOptions.ccl.contentsModification=&post.postOptions.attachedVideoScrapYn=";
            pbody += "&directorySeq=14&directoryDetail=&post.bookTheme.infoPk=&post.movieTheme.infoPk=&post.musicTheme.infoPk=&post.kitchenTheme.recipeName=&post.postOptions.directoryOptions.directoryChangeYn=false&post.postOptions.directoryOptions.tagAutoChangedYn=false";
            pbody += "&post.postOptions.isAutoTaggingEnabled=true&post.postOptions.greenReviewBannerYn=false&previewGreenReviewBannerAsInteger=0&post.leverageOptions.themeSourceCode=&post.music.subType=&post.postOptions.isContinueSaved=false&post.mrBlogTalk.talkCode=";
            pbody += "&happyBeanGiveDayReqparam=&post.postOptions.isExifEnabled=false&editorSource=qcO3MZKpRHhM%2FEqi0leaWg%3D%3D&post.category.categoryNo=" + Category + "&post.title=" + System.Web.HttpUtility.UrlEncode(System.Web.HttpUtility.UrlEncode(Title, Encoding.GetEncoding("utf-8")), Encoding.GetEncoding("utf-8")) + "&post.trackback.trackbackUrl=&ir1=%0D%0A%0D%0A";
            pbody += "&query=%EC%A7%80%EC%97%AD%EB%AA%85%EC%9D%84%20%EC%9E%85%EB%A0%A5%ED%95%B4%20%EC%A3%BC%EC%84%B8%EC%9A%94&char_preview=%3F%C2%BA%E2%8A%86%E2%97%8F%E2%97%8B&se2_tbp=on&se2_tbp3=on&=on&post.directorySeq=14";

            if(Tag == "")
                pbody += "&post.tag.names=%ED%83%9C%EA%B7%B8%EC%99%80%20%ED%83%9C%EA%B7%B8%EB%8A%94%20%EC%89%BC%ED%91%9C%EB%A1%9C%20%EA%B5%AC%EB%B6%84%ED%95%98%EB%A9%B0%2C%2010%EA%B0%9C%EA%B9%8C%EC%A7%80%20%EC%9E%85%EB%A0%A5%ED%95%98%EC%8B%A4%20%EC%88%98%20%EC%9E%88%EC%8A%B5%EB%8B%88%EB%8B%A4.";
            else
                pbody += "&post.tag.names=" + System.Web.HttpUtility.UrlEncode(Tag, Encoding.GetEncoding("utf-8"));

            pbody += "&openType=2&post.postWriteTimeType=now&prePostDay=" + System.Web.HttpUtility.UrlEncode(DateTime.Now.Year.ToString() + "년 " + DateTime.Now.Month.ToString() + "월 " + DateTime.Now.Day.ToString() + "일", Encoding.GetEncoding("utf-8")) + "&prePostDateType.hour=" + DateTime.Now.Hour.ToString() + "&prePostDateType.minute=30&prePostDateType.year=&prePostDateType.month=&prePostDateType.date=&commercialUses=false&contentsModification=0&writingMaterialInfos=%5B%5D" + attach;

            

            List<string> Header = new List<string>();
            Header.Add("Referer: http://blog.naver.com/" + NowID + "/postwrite");
            Header.Add("Content-Type: application/x-www-form-urlencoded");
            Header.Add("Accept: */*");
            Header.Add("User-Agent: Mozilla/5.0 (Windows NT 6.3; WOW64; Trident/7.0; rv:11.0) like Gecko");
            Header.Add("Cookie: " + Cookie);
            string body = System.Text.RegularExpressions.Regex.Split(Engine.HttpSend("ALL", "utf-8", "POST", "http://blog.naver.com/TempPostWriteAsync.nhn", Header, new StringBuilder(pbody), PROXY, 0), "/RESULT/")[0];

            string pbody2 = "captchaKey=&captchaValue=&appId=&tempLogNo=&blogId=" + NowID + "&post.logNo=" + body + "&post.sourceCode=0&post.contents.contentsValue=" + System.Web.HttpUtility.UrlEncode(Contents, Encoding.GetEncoding("euc-kr")) + "&post.prePostRegistDirectly=false&post.lastRelayTime=&smartEditorVersion=2&post.book.ratingScore=0";
            pbody2 += "&post.music.ratingScore=0&post.movie.ratingScore=0&post.scrapedYn=false&post.clientType=&post.contents.summaryYn=false&post.contents.summaryToggleText=&post.contents.summaryTogglePosition=&post.templatePhoto.width=0&post.templatePhoto.height=0&post.addedInfoSet.addedInfoStruct=";
            pbody2 += "&post.mapAttachmentSet.mapAttachStruct=&post.calendarAttachmentSet.calendarAttachmentStruct=&post.musicPlayerAttachmentSet.musicPlayerAttachmentStruct=&post.eventWriteInfo.eventCode=&post.eventWriteInfo.writeCode=&post.eventWriteInfo.eventType=&post.eventWriteInfo.eventLink=";
            pbody2 += "&post.postOptions.openType=2&post.postOptions.commentYn=true&post.postOptions.isRelayOpen=true&post.postOptions.sympathyYn=true&post.postOptions.outSideAllowYn=true&post.me2dayPostingYn=&post.facebookPostingYn=false&post.twitterPostingYn=false&post.postOptions.searchYn=true";
            pbody2 += "&post.postOptions.rssOpenYn=true&post.postOptions.scrapType=2&post.postOptions.ccl.commercialUsesYn=false&post.postOptions.ccl.contentsModification=&post.postOptions.attachedVideoScrapYn=&directorySeq=14&directoryDetail=&post.bookTheme.infoPk=&post.movieTheme.infoPk=";
            pbody2 += "&post.musicTheme.infoPk=&post.kitchenTheme.recipeName=&post.postOptions.directoryOptions.directoryChangeYn=false&post.postOptions.directoryOptions.tagAutoChangedYn=false&post.postOptions.isAutoTaggingEnabled=true&post.postOptions.greenReviewBannerYn=false&previewGreenReviewBannerAsInteger=0";
            pbody2 += "&post.leverageOptions.themeSourceCode=&post.music.subType=&post.postOptions.isContinueSaved=false&post.mrBlogTalk.talkCode=&happyBeanGiveDayReqparam=&post.postOptions.isExifEnabled=false&editorSource=qcO3MZKpRHhM%2FEqi0leaWg%3D%3D&post.category.categoryNo=" + Category + "&post.title=" + System.Web.HttpUtility.UrlEncode(Title, Encoding.GetEncoding("euc-kr")) + "&post.trackback.trackbackUrl=";
            pbody2 += "&ir1=" + System.Web.HttpUtility.UrlEncode(Contents, Encoding.GetEncoding("euc-kr")) + "&query=%C1%F6%BF%AA%B8%ED%C0%BB+%C0%D4%B7%C2%C7%D8+%C1%D6%BC%BC%BF%E4&=%23e97d81&char_preview=%3F%A8%AC%A1%F6%A1%DC%A1%DB&=4&=4&se2_tbp=on&=1&=%23cccccc&=%23ffffff&=+&=+&=%C1%F7%C1%A2+%C0%D4%B7%C2&se2_tbp3=on&=1024&=768&=on&=0";
            pbody2 += "&=%C3%A5&=%BF%E4%B8%AE%B8%ED%C0%BB+%B3%D6%BE%EE%C1%D6%BC%BC%BF%E4&post.directorySeq=14&post.tag.names=" + System.Web.HttpUtility.UrlEncode(Tag, Encoding.GetEncoding("euc-kr")) + "&openType=2&post.postWriteTimeType=now&prePostDay=" + System.Web.HttpUtility.UrlEncode(DateTime.Now.Year.ToString() + "년 " + DateTime.Now.Month.ToString() + "월 " + DateTime.Now.Day.ToString() + "일", Encoding.GetEncoding("euc-kr")) + "&prePostDateType.hour=" + DateTime.Now.Hour.ToString() + "&prePostDateType.minute=30&prePostDateType.year=" + DateTime.Now.Year.ToString() + "&prePostDateType.month=10";
            pbody2 += "&prePostDateType.date=30&commercialUses=false&contentsModification=0&writingMaterialInfos=%5B%5D" + attach;

            Header = new List<string>();
            Header.Add("Referer: http://blog.naver.com/" + NowID + "/postwrite");
            Header.Add("Content-Type: application/x-www-form-urlencoded");
            Header.Add("Accept: text/html, application/xhtml+xml, */*");
            Header.Add("User-Agent: Mozilla/5.0 (Windows NT 6.3; WOW64; Trident/7.0; rv:11.0) like Gecko");
            Header.Add("Cookie: " + Cookie);
            body = Engine.HttpSend("ALL", "euc-kr", "POST", "http://blog.naver.com/PostWrite.nhn", Header, new StringBuilder(pbody2), PROXY, 0);

            return body;
        }

        public string BlogImageUpload(string Cookie, string NowID, string Filename, Image Image, string PROXY)
        {
            Random rd = new Random();
            
            List<string> Header = new List<string>();
            Header.Add("Content-Type: application/x-www-form-urlencoded");
            Header.Add("Accept: */*");
            Header.Add("Accept-Language: ko-KR");
            Header.Add("Cookie: " + Cookie);
            string body = Engine.HttpSend("ALL", "utf-8", "POST", "http://blog.naver.com/AttachPhotoWebtopForm.nhn?mode=base&blogId=" + NowID, Header, new StringBuilder(""), PROXY, 0);

            string[] temp = System.Text.RegularExpressions.Regex.Split(body, "sessionKey=");
            temp = System.Text.RegularExpressions.Regex.Split(temp[1], "&");
            string keysession = temp[0];

            string[] fenc = Filename.Split('.');
            string FilenameENC = System.Web.HttpUtility.UrlEncode(fenc[0], Encoding.GetEncoding("utf-8")) + "." + fenc[1];

            string pbody = "------------cH2ei4Ij5ae0ae0ei4GI3cH2Ij5Ef1\r\n";
            pbody += "Content-Disposition: form-data; name=\"Filename\"\r\n\r\n";

            pbody += Filename + "\r\n";
            pbody += "------------cH2ei4Ij5ae0ae0ei4GI3cH2Ij5Ef1\r\n";
            pbody += "Content-Disposition: form-data; name=\"userId\"\r\n\r\n";

            pbody += NowID + "\r\n";
            pbody += "------------cH2ei4Ij5ae0ae0ei4GI3cH2Ij5Ef1\r\n";
            pbody += "Content-Disposition: form-data; name=\"fileName\"\r\n\r\n";

            pbody += FilenameENC + "\r\n";
            pbody += "------------cH2ei4Ij5ae0ae0ei4GI3cH2Ij5Ef1\r\n";
            pbody += "Content-Disposition: form-data; name=\"imgIndex\"\r\n\r\n";

            pbody += "0\r\n";
            pbody += "------------cH2ei4Ij5ae0ae0ei4GI3cH2Ij5Ef1\r\n";
            pbody += "Content-Disposition: form-data; name=\"image\"; filename=\"" + Filename + "\"\r\n";
            pbody += "Content-Type: application/octet-stream\r\n\r\n";

            string sizelistw = Image.Width.ToString();
            string sizelisth = Image.Height.ToString();

            byte[] dd2 = imageToByteArray(Image);
            //string pbody1 = System.Text.Encoding.Default.GetString(dd2);

            string pbody2 = "------------cH2ei4Ij5ae0ae0ei4GI3cH2Ij5Ef1\r\n";
            pbody2 += "Content-Disposition: form-data; name=\"Upload\"\r\n\r\n";

            pbody2 += "Submit Query\r\n";
            pbody2 += "------------cH2ei4Ij5ae0ae0ei4GI3cH2Ij5Ef1--\r\n";

            byte[] dd1 = System.Text.Encoding.UTF8.GetBytes(pbody);
            byte[] dd3 = System.Text.Encoding.UTF8.GetBytes(pbody2);

            byte[] dd = new byte[dd1.Length + dd2.Length + dd3.Length];
            for (int i = 0; i < dd.Length; i++)
            {
                if (i < dd1.Length)
                {
                    dd[i] = dd1[i];
                }
                else if (i < dd2.Length + dd1.Length)
                {
                    dd[i] = dd2[i - dd1.Length];
                }
                else
                {
                    dd[i] = dd3[i - dd1.Length - dd2.Length];
                }
            }

            Header = new List<string>();
            Header.Add("Accept: */*");
            Header.Add("Content-Type: multipart/form-data; boundary=----------cH2ei4Ij5ae0ae0ei4GI3cH2Ij5Ef1");
            Header.Add("User-Agent: Shockwave Flash");
            Header.Add("Cookie: " + Cookie);
            body = Engine.HttpSendBytes("ALL", "utf-8", "POST", "http://blog.upphoto.naver.com/" + keysession + "/upload/0", Header, dd, PROXY, 10000);

           
            //Header = new List<string>();
            //Header.Add("Referer: http://blog.upphoto.naver.com/flash/20141002/PhotoWebtop.swf?$Rev: 20162 $");
            //Header.Add("Accept: */*");
            //Header.Add("Content-Type: application/x-www-form-urlencoded");
            //Header.Add("x-flash-version: 11,3,372,94");
            //Header.Add("User-Agent: Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.2; WOW64; Trident/6.0; .NET4.0E; .NET4.0C; .NET CLR 3.5.30729; .NET CLR 2.0.50727; .NET CLR 3.0.30729)");
            //Header.Add("Cookie: " + Cookie);
            //body = Engine.HttpSend("ALL", "utf-8", "POST", "http://blog.upphoto.naver.com/" + keysession + "/modify", Header, new StringBuilder("command=%3Citems%3E%0A%20%20%3Citem%3E%0A%20%20%20%20%3Cindex%3E0%3C%2Findex%3E%0A%20%20%20%20%3Ccommand%3E%0A%20%20%20%20%20%20%3Cwidth%20force%3D%22false%22%3E" + sizelistw + "%3C%2Fwidth%3E%0A%20%20%20%20%3C%2Fcommand%3E%0A%20%20%3C%2Fitem%3E%0A%3C%2Fitems%3E"), PROXY, 10000);
           

            Header = new List<string>();
            Header.Add("Referer: http://blog.upphoto.naver.com/flash/20141002/PhotoWebtop.swf?$Rev: 20162 $");
            Header.Add("Accept: */*");
            Header.Add("Content-Type: application/x-www-form-urlencoded");
            Header.Add("x-flash-version: 11,3,372,94");
            Header.Add("User-Agent: Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.2; WOW64; Trident/6.0; .NET4.0E; .NET4.0C; .NET CLR 3.5.30729; .NET CLR 2.0.50727; .NET CLR 3.0.30729)");
            Header.Add("Cookie: " + Cookie);
            body = Engine.HttpSend("ALL", "utf-8", "POST", "http://blog.upphoto.naver.com/" + keysession + "/save/0", Header, new StringBuilder("order=0"), PROXY, 10000);
            temp = System.Text.RegularExpressions.Regex.Split(body, "<url>");
            temp = System.Text.RegularExpressions.Regex.Split(temp[1], "</url>");

            Header = new List<string>();
            Header.Add("Referer: http://blog.upphoto.naver.com/flash/20141002/PhotoWebtop.swf?$Rev: 20162 $");
            Header.Add("Accept: */*");
            Header.Add("Content-Type: application/x-www-form-urlencoded");
            Header.Add("x-flash-version: 11,3,372,94");
            Header.Add("User-Agent: Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.2; WOW64; Trident/6.0; .NET4.0E; .NET4.0C; .NET CLR 3.5.30729; .NET CLR 2.0.50727; .NET CLR 3.0.30729)");
            Header.Add("Cookie: " + Cookie);
            body = Engine.HttpSend("ALL", "utf-8", "GET", "http://blog.upphoto.naver.com/" + keysession + "/finish", Header, new StringBuilder(""), PROXY, 10000);

            return "http://blogfiles.naver.net" + temp[0] + "///" + sizelistw + "///" + sizelisth;
        }

        public byte[] imageToByteArray(System.Drawing.Image imageIn)
        {
            MemoryStream ms = new MemoryStream();
            imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            return ms.ToArray();
        }

        public Image byteArrayToImage(byte[] byteArrayIn)
        {
            MemoryStream ms = new MemoryStream(byteArrayIn);
            Image returnImage = Image.FromStream(ms);
            return returnImage;
        }
    }
}
