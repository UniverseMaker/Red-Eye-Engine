namespace RedEyeEngine
{
    partial class FormLogin
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.submit = new System.Windows.Forms.Button();
            this.pwt = new System.Windows.Forms.TextBox();
            this.idt = new System.Windows.Forms.TextBox();
            this.typeAccount = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // submit
            // 
            this.submit.Location = new System.Drawing.Point(127, 4);
            this.submit.Name = "submit";
            this.submit.Size = new System.Drawing.Size(75, 62);
            this.submit.TabIndex = 11;
            this.submit.Text = "로그인";
            this.submit.UseVisualStyleBackColor = true;
            this.submit.Click += new System.EventHandler(this.submit_Click);
            // 
            // pwt
            // 
            this.pwt.Location = new System.Drawing.Point(3, 45);
            this.pwt.Name = "pwt";
            this.pwt.Size = new System.Drawing.Size(121, 21);
            this.pwt.TabIndex = 10;
            this.pwt.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.pwt_KeyPress);
            // 
            // idt
            // 
            this.idt.Location = new System.Drawing.Point(3, 24);
            this.idt.Name = "idt";
            this.idt.Size = new System.Drawing.Size(121, 21);
            this.idt.TabIndex = 9;
            this.idt.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.idt_KeyPress);
            // 
            // typeAccount
            // 
            this.typeAccount.FormattingEnabled = true;
            this.typeAccount.Items.AddRange(new object[] {
            "다음",
            "네이트(준비중)"});
            this.typeAccount.Location = new System.Drawing.Point(3, 4);
            this.typeAccount.Name = "typeAccount";
            this.typeAccount.Size = new System.Drawing.Size(121, 20);
            this.typeAccount.TabIndex = 8;
            this.typeAccount.Text = "연계계정 종류선택";
            this.typeAccount.SelectedIndexChanged += new System.EventHandler(this.typeAccount_SelectedIndexChanged);
            // 
            // FormLogin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(204, 71);
            this.Controls.Add(this.submit);
            this.Controls.Add(this.pwt);
            this.Controls.Add(this.idt);
            this.Controls.Add(this.typeAccount);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "FormLogin";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FormLogin";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button submit;
        private System.Windows.Forms.TextBox pwt;
        private System.Windows.Forms.TextBox idt;
        private System.Windows.Forms.ComboBox typeAccount;
    }
}