using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RedEyeEngine
{
    public partial class FormLogin : Form
    {
        CoreNaver CNAV = new CoreNaver();
        CoreDaum CDAU = new CoreDaum();
        CoreNate CNAT = new CoreNate();

        string Cookie = "";
        public FormLogin()
        {
            InitializeComponent();
        }

        private void typeAccount_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void idt_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                pwt.Focus();
            }
        }

        private void pwt_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                submit_Click(null, null);
            }
        }

        private void submit_Click(object sender, EventArgs e)
        {
            string[] lt = System.Text.RegularExpressions.Regex.Split(CDAU.Login("", "", ""), "/RESULT/");
            if (lt[0].IndexOf("document.location.replace(\"http://www.daum.net/\");") != -1)
            {
                Cookie = lt[1];
            }
            else
            {
                MessageBox.Show("로그인 실패! 아이디 혹은 암호를 확인해주세요");
            }
        }
    }
}
