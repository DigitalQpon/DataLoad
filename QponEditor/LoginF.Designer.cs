namespace QponEditor
{
    partial class LoginF
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
            this.label1 = new System.Windows.Forms.Label();
            this.loginIdEdt = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.passwordEdt = new System.Windows.Forms.TextBox();
            this.loginCmd = new System.Windows.Forms.Button();
            this.cancelCmd = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(34, 36);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "Login Id";
            // 
            // loginIdEdt
            // 
            this.loginIdEdt.FormattingEnabled = true;
            this.loginIdEdt.Location = new System.Drawing.Point(118, 33);
            this.loginIdEdt.Name = "loginIdEdt";
            this.loginIdEdt.Size = new System.Drawing.Size(222, 24);
            this.loginIdEdt.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(34, 80);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(69, 17);
            this.label2.TabIndex = 2;
            this.label2.Text = "Password";
            // 
            // passwordEdt
            // 
            this.passwordEdt.Location = new System.Drawing.Point(118, 76);
            this.passwordEdt.Name = "passwordEdt";
            this.passwordEdt.PasswordChar = '*';
            this.passwordEdt.Size = new System.Drawing.Size(149, 22);
            this.passwordEdt.TabIndex = 3;
            // 
            // loginCmd
            // 
            this.loginCmd.Location = new System.Drawing.Point(93, 124);
            this.loginCmd.Name = "loginCmd";
            this.loginCmd.Size = new System.Drawing.Size(93, 34);
            this.loginCmd.TabIndex = 4;
            this.loginCmd.Text = "Login";
            this.loginCmd.UseVisualStyleBackColor = true;
            this.loginCmd.Click += new System.EventHandler(this.loginCmd_Click);
            // 
            // cancelCmd
            // 
            this.cancelCmd.Location = new System.Drawing.Point(217, 124);
            this.cancelCmd.Name = "cancelCmd";
            this.cancelCmd.Size = new System.Drawing.Size(93, 34);
            this.cancelCmd.TabIndex = 5;
            this.cancelCmd.Text = "Cancel";
            this.cancelCmd.UseVisualStyleBackColor = true;
            this.cancelCmd.Click += new System.EventHandler(this.cancelCmd_Click);
            // 
            // LoginF
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(379, 186);
            this.Controls.Add(this.cancelCmd);
            this.Controls.Add(this.loginCmd);
            this.Controls.Add(this.passwordEdt);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.loginIdEdt);
            this.Controls.Add(this.label1);
            this.Name = "LoginF";
            this.Text = "Qpon Login";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button loginCmd;
        private System.Windows.Forms.Button cancelCmd;
        public System.Windows.Forms.ComboBox loginIdEdt;
        public System.Windows.Forms.TextBox passwordEdt;
    }
}