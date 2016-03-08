namespace QponCollector
{
    partial class MainF
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
            this.components = new System.ComponentModel.Container();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.processSchdTAB = new System.Windows.Forms.TabControl();
            this.mainTab = new System.Windows.Forms.TabPage();
            this.adminInfoGB = new System.Windows.Forms.GroupBox();
            this.getAdminTokenCM = new System.Windows.Forms.Button();
            this.passwordED = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.emailProfileED = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.lastNameED = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.firstnameED = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.scheduleGB = new System.Windows.Forms.GroupBox();
            this.setScheduleCM = new System.Windows.Forms.Button();
            this.setTimeDownloadEdt = new System.Windows.Forms.DateTimePicker();
            this.downloadInfoCM = new System.Windows.Forms.Button();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.statusLB = new System.Windows.Forms.ToolStripStatusLabel();
            this.fmtcGB = new System.Windows.Forms.GroupBox();
            this.fmtcDwnMsg = new System.Windows.Forms.Label();
            this.useSourceFileOpt = new System.Windows.Forms.CheckBox();
            this.openFileCmd = new System.Windows.Forms.Button();
            this.sourcefmtcEdt = new System.Windows.Forms.TextBox();
            this.fmtcProductOpt = new System.Windows.Forms.CheckBox();
            this.fmtcMerchantOpt = new System.Windows.Forms.CheckBox();
            this.downloadFmtcOP = new System.Windows.Forms.CheckBox();
            this.fFilterED = new System.Windows.Forms.TextBox();
            this.fFilterLB = new System.Windows.Forms.Label();
            this.prosperentGB = new System.Windows.Forms.GroupBox();
            this.productOpt = new System.Windows.Forms.CheckBox();
            this.merchantOpt = new System.Windows.Forms.CheckBox();
            this.downloadProsperentOP = new System.Windows.Forms.CheckBox();
            this.pFilterED = new System.Windows.Forms.TextBox();
            this.pFilterLB = new System.Windows.Forms.Label();
            this.messagesTAB = new System.Windows.Forms.TabPage();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.syncModeCmd = new System.Windows.Forms.Button();
            this.asyncModeCmd = new System.Windows.Forms.Button();
            this.processSchdTAB.SuspendLayout();
            this.mainTab.SuspendLayout();
            this.adminInfoGB.SuspendLayout();
            this.scheduleGB.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.fmtcGB.SuspendLayout();
            this.prosperentGB.SuspendLayout();
            this.SuspendLayout();
            // 
            // timer1
            // 
            this.timer1.Interval = 3000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // processSchdTAB
            // 
            this.processSchdTAB.Controls.Add(this.mainTab);
            this.processSchdTAB.Controls.Add(this.messagesTAB);
            this.processSchdTAB.Dock = System.Windows.Forms.DockStyle.Fill;
            this.processSchdTAB.Location = new System.Drawing.Point(0, 0);
            this.processSchdTAB.Margin = new System.Windows.Forms.Padding(4);
            this.processSchdTAB.Name = "processSchdTAB";
            this.processSchdTAB.SelectedIndex = 0;
            this.processSchdTAB.Size = new System.Drawing.Size(950, 733);
            this.processSchdTAB.TabIndex = 0;
            // 
            // mainTab
            // 
            this.mainTab.Controls.Add(this.adminInfoGB);
            this.mainTab.Controls.Add(this.scheduleGB);
            this.mainTab.Controls.Add(this.fmtcGB);
            this.mainTab.Controls.Add(this.prosperentGB);
            this.mainTab.Location = new System.Drawing.Point(4, 25);
            this.mainTab.Margin = new System.Windows.Forms.Padding(4);
            this.mainTab.Name = "mainTab";
            this.mainTab.Padding = new System.Windows.Forms.Padding(4);
            this.mainTab.Size = new System.Drawing.Size(942, 704);
            this.mainTab.TabIndex = 0;
            this.mainTab.Text = "Process/Schedule";
            this.mainTab.UseVisualStyleBackColor = true;
            // 
            // adminInfoGB
            // 
            this.adminInfoGB.Controls.Add(this.getAdminTokenCM);
            this.adminInfoGB.Controls.Add(this.passwordED);
            this.adminInfoGB.Controls.Add(this.label4);
            this.adminInfoGB.Controls.Add(this.emailProfileED);
            this.adminInfoGB.Controls.Add(this.label3);
            this.adminInfoGB.Controls.Add(this.lastNameED);
            this.adminInfoGB.Controls.Add(this.label2);
            this.adminInfoGB.Controls.Add(this.firstnameED);
            this.adminInfoGB.Controls.Add(this.label1);
            this.adminInfoGB.Dock = System.Windows.Forms.DockStyle.Top;
            this.adminInfoGB.Location = new System.Drawing.Point(4, 371);
            this.adminInfoGB.Margin = new System.Windows.Forms.Padding(4);
            this.adminInfoGB.Name = "adminInfoGB";
            this.adminInfoGB.Padding = new System.Windows.Forms.Padding(4);
            this.adminInfoGB.Size = new System.Drawing.Size(934, 130);
            this.adminInfoGB.TabIndex = 6;
            this.adminInfoGB.TabStop = false;
            this.adminInfoGB.Text = "Admin Info";
            // 
            // getAdminTokenCM
            // 
            this.getAdminTokenCM.Location = new System.Drawing.Point(35, 90);
            this.getAdminTokenCM.Margin = new System.Windows.Forms.Padding(4);
            this.getAdminTokenCM.Name = "getAdminTokenCM";
            this.getAdminTokenCM.Size = new System.Drawing.Size(100, 28);
            this.getAdminTokenCM.TabIndex = 8;
            this.getAdminTokenCM.Text = "Admin Login";
            this.getAdminTokenCM.UseVisualStyleBackColor = true;
            this.getAdminTokenCM.Click += new System.EventHandler(this.getAdminTokenCM_Click);
            // 
            // passwordED
            // 
            this.passwordED.Location = new System.Drawing.Point(579, 23);
            this.passwordED.Margin = new System.Windows.Forms.Padding(4);
            this.passwordED.Name = "passwordED";
            this.passwordED.PasswordChar = '*';
            this.passwordED.Size = new System.Drawing.Size(132, 22);
            this.passwordED.TabIndex = 7;
            this.passwordED.Text = "Munish@";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(495, 27);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(69, 17);
            this.label4.TabIndex = 6;
            this.label4.Text = "Password";
            // 
            // emailProfileED
            // 
            this.emailProfileED.Location = new System.Drawing.Point(137, 55);
            this.emailProfileED.Margin = new System.Windows.Forms.Padding(4);
            this.emailProfileED.Name = "emailProfileED";
            this.emailProfileED.Size = new System.Drawing.Size(349, 22);
            this.emailProfileED.TabIndex = 5;
            this.emailProfileED.Text = "munish@socialqpon.com";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(25, 59);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(101, 17);
            this.label3.TabIndex = 4;
            this.label3.Text = "Email/Profile Id";
            // 
            // lastNameED
            // 
            this.lastNameED.Location = new System.Drawing.Point(340, 23);
            this.lastNameED.Margin = new System.Windows.Forms.Padding(4);
            this.lastNameED.Name = "lastNameED";
            this.lastNameED.Size = new System.Drawing.Size(132, 22);
            this.lastNameED.TabIndex = 3;
            this.lastNameED.Text = "Madan";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(256, 27);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(76, 17);
            this.label2.TabIndex = 2;
            this.label2.Text = "Last Name";
            // 
            // firstnameED
            // 
            this.firstnameED.Location = new System.Drawing.Point(109, 23);
            this.firstnameED.Margin = new System.Windows.Forms.Padding(4);
            this.firstnameED.Name = "firstnameED";
            this.firstnameED.Size = new System.Drawing.Size(132, 22);
            this.firstnameED.TabIndex = 1;
            this.firstnameED.Text = "Munish";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(25, 27);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(76, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "First Name";
            // 
            // scheduleGB
            // 
            this.scheduleGB.Controls.Add(this.asyncModeCmd);
            this.scheduleGB.Controls.Add(this.syncModeCmd);
            this.scheduleGB.Controls.Add(this.setScheduleCM);
            this.scheduleGB.Controls.Add(this.setTimeDownloadEdt);
            this.scheduleGB.Controls.Add(this.downloadInfoCM);
            this.scheduleGB.Controls.Add(this.statusStrip1);
            this.scheduleGB.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.scheduleGB.Location = new System.Drawing.Point(4, 509);
            this.scheduleGB.Margin = new System.Windows.Forms.Padding(4);
            this.scheduleGB.Name = "scheduleGB";
            this.scheduleGB.Padding = new System.Windows.Forms.Padding(4);
            this.scheduleGB.Size = new System.Drawing.Size(934, 191);
            this.scheduleGB.TabIndex = 5;
            this.scheduleGB.TabStop = false;
            this.scheduleGB.Text = "[ Schedule ]";
            // 
            // setScheduleCM
            // 
            this.setScheduleCM.Location = new System.Drawing.Point(269, 37);
            this.setScheduleCM.Margin = new System.Windows.Forms.Padding(4);
            this.setScheduleCM.Name = "setScheduleCM";
            this.setScheduleCM.Size = new System.Drawing.Size(172, 28);
            this.setScheduleCM.TabIndex = 3;
            this.setScheduleCM.Text = "Schedule";
            this.setScheduleCM.UseVisualStyleBackColor = true;
            this.setScheduleCM.Click += new System.EventHandler(this.setScheduleCM_Click);
            // 
            // setTimeDownloadEdt
            // 
            this.setTimeDownloadEdt.CustomFormat = "";
            this.setTimeDownloadEdt.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.setTimeDownloadEdt.Location = new System.Drawing.Point(449, 41);
            this.setTimeDownloadEdt.Margin = new System.Windows.Forms.Padding(4);
            this.setTimeDownloadEdt.Name = "setTimeDownloadEdt";
            this.setTimeDownloadEdt.ShowUpDown = true;
            this.setTimeDownloadEdt.Size = new System.Drawing.Size(165, 22);
            this.setTimeDownloadEdt.TabIndex = 2;
            // 
            // downloadInfoCM
            // 
            this.downloadInfoCM.Location = new System.Drawing.Point(29, 37);
            this.downloadInfoCM.Margin = new System.Windows.Forms.Padding(4);
            this.downloadInfoCM.Name = "downloadInfoCM";
            this.downloadInfoCM.Size = new System.Drawing.Size(172, 28);
            this.downloadInfoCM.TabIndex = 1;
            this.downloadInfoCM.Text = "Download Data Now!";
            this.downloadInfoCM.UseVisualStyleBackColor = true;
            this.downloadInfoCM.Click += new System.EventHandler(this.downloadInfoCM_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusLB});
            this.statusStrip1.Location = new System.Drawing.Point(4, 165);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Padding = new System.Windows.Forms.Padding(1, 0, 19, 0);
            this.statusStrip1.Size = new System.Drawing.Size(926, 22);
            this.statusStrip1.TabIndex = 0;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // statusLB
            // 
            this.statusLB.Name = "statusLB";
            this.statusLB.Size = new System.Drawing.Size(906, 17);
            this.statusLB.Spring = true;
            // 
            // fmtcGB
            // 
            this.fmtcGB.Controls.Add(this.fmtcDwnMsg);
            this.fmtcGB.Controls.Add(this.useSourceFileOpt);
            this.fmtcGB.Controls.Add(this.openFileCmd);
            this.fmtcGB.Controls.Add(this.sourcefmtcEdt);
            this.fmtcGB.Controls.Add(this.fmtcProductOpt);
            this.fmtcGB.Controls.Add(this.fmtcMerchantOpt);
            this.fmtcGB.Controls.Add(this.downloadFmtcOP);
            this.fmtcGB.Controls.Add(this.fFilterED);
            this.fmtcGB.Controls.Add(this.fFilterLB);
            this.fmtcGB.Dock = System.Windows.Forms.DockStyle.Top;
            this.fmtcGB.Location = new System.Drawing.Point(4, 121);
            this.fmtcGB.Margin = new System.Windows.Forms.Padding(4);
            this.fmtcGB.Name = "fmtcGB";
            this.fmtcGB.Padding = new System.Windows.Forms.Padding(4);
            this.fmtcGB.Size = new System.Drawing.Size(934, 250);
            this.fmtcGB.TabIndex = 3;
            this.fmtcGB.TabStop = false;
            this.fmtcGB.Text = "[ FMTC ]";
            // 
            // fmtcDwnMsg
            // 
            this.fmtcDwnMsg.AutoSize = true;
            this.fmtcDwnMsg.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fmtcDwnMsg.Location = new System.Drawing.Point(28, 180);
            this.fmtcDwnMsg.Name = "fmtcDwnMsg";
            this.fmtcDwnMsg.Size = new System.Drawing.Size(172, 17);
            this.fmtcDwnMsg.TabIndex = 11;
            this.fmtcDwnMsg.Text = "Fmtc Download Messages";
            // 
            // useSourceFileOpt
            // 
            this.useSourceFileOpt.AutoSize = true;
            this.useSourceFileOpt.Location = new System.Drawing.Point(28, 118);
            this.useSourceFileOpt.Name = "useSourceFileOpt";
            this.useSourceFileOpt.Size = new System.Drawing.Size(130, 21);
            this.useSourceFileOpt.TabIndex = 10;
            this.useSourceFileOpt.Text = "Use Source File";
            this.useSourceFileOpt.UseVisualStyleBackColor = true;
            // 
            // openFileCmd
            // 
            this.openFileCmd.Location = new System.Drawing.Point(662, 131);
            this.openFileCmd.Name = "openFileCmd";
            this.openFileCmd.Size = new System.Drawing.Size(135, 36);
            this.openFileCmd.TabIndex = 9;
            this.openFileCmd.Text = "Open File";
            this.openFileCmd.UseVisualStyleBackColor = true;
            this.openFileCmd.Click += new System.EventHandler(this.openFileCmd_Click);
            // 
            // sourcefmtcEdt
            // 
            this.sourcefmtcEdt.Location = new System.Drawing.Point(29, 145);
            this.sourcefmtcEdt.Name = "sourcefmtcEdt";
            this.sourcefmtcEdt.Size = new System.Drawing.Size(617, 22);
            this.sourcefmtcEdt.TabIndex = 7;
            // 
            // fmtcProductOpt
            // 
            this.fmtcProductOpt.AutoSize = true;
            this.fmtcProductOpt.Checked = true;
            this.fmtcProductOpt.CheckState = System.Windows.Forms.CheckState.Checked;
            this.fmtcProductOpt.Location = new System.Drawing.Point(374, 85);
            this.fmtcProductOpt.Margin = new System.Windows.Forms.Padding(4);
            this.fmtcProductOpt.Name = "fmtcProductOpt";
            this.fmtcProductOpt.Size = new System.Drawing.Size(106, 21);
            this.fmtcProductOpt.TabIndex = 6;
            this.fmtcProductOpt.Text = "Product Info";
            this.fmtcProductOpt.UseVisualStyleBackColor = true;
            // 
            // fmtcMerchantOpt
            // 
            this.fmtcMerchantOpt.AutoSize = true;
            this.fmtcMerchantOpt.Checked = true;
            this.fmtcMerchantOpt.CheckState = System.Windows.Forms.CheckState.Checked;
            this.fmtcMerchantOpt.Location = new System.Drawing.Point(250, 85);
            this.fmtcMerchantOpt.Margin = new System.Windows.Forms.Padding(4);
            this.fmtcMerchantOpt.Name = "fmtcMerchantOpt";
            this.fmtcMerchantOpt.Size = new System.Drawing.Size(116, 21);
            this.fmtcMerchantOpt.TabIndex = 5;
            this.fmtcMerchantOpt.Text = "Merchant Info";
            this.fmtcMerchantOpt.UseVisualStyleBackColor = true;
            // 
            // downloadFmtcOP
            // 
            this.downloadFmtcOP.AutoSize = true;
            this.downloadFmtcOP.Location = new System.Drawing.Point(29, 85);
            this.downloadFmtcOP.Margin = new System.Windows.Forms.Padding(4);
            this.downloadFmtcOP.Name = "downloadFmtcOP";
            this.downloadFmtcOP.Size = new System.Drawing.Size(167, 21);
            this.downloadFmtcOP.TabIndex = 4;
            this.downloadFmtcOP.Text = "Download FMTC Data";
            this.downloadFmtcOP.UseVisualStyleBackColor = true;
            // 
            // fFilterED
            // 
            this.fFilterED.Location = new System.Drawing.Point(29, 57);
            this.fFilterED.Margin = new System.Windows.Forms.Padding(4);
            this.fFilterED.Name = "fFilterED";
            this.fFilterED.Size = new System.Drawing.Size(789, 22);
            this.fFilterED.TabIndex = 3;
            // 
            // fFilterLB
            // 
            this.fFilterLB.AutoSize = true;
            this.fFilterLB.Location = new System.Drawing.Point(25, 32);
            this.fFilterLB.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.fFilterLB.Name = "fFilterLB";
            this.fFilterLB.Size = new System.Drawing.Size(167, 17);
            this.fFilterLB.TabIndex = 2;
            this.fFilterLB.Text = "Additional Filter for FMTC";
            // 
            // prosperentGB
            // 
            this.prosperentGB.Controls.Add(this.productOpt);
            this.prosperentGB.Controls.Add(this.merchantOpt);
            this.prosperentGB.Controls.Add(this.downloadProsperentOP);
            this.prosperentGB.Controls.Add(this.pFilterED);
            this.prosperentGB.Controls.Add(this.pFilterLB);
            this.prosperentGB.Dock = System.Windows.Forms.DockStyle.Top;
            this.prosperentGB.Location = new System.Drawing.Point(4, 4);
            this.prosperentGB.Margin = new System.Windows.Forms.Padding(4);
            this.prosperentGB.Name = "prosperentGB";
            this.prosperentGB.Padding = new System.Windows.Forms.Padding(4);
            this.prosperentGB.Size = new System.Drawing.Size(934, 117);
            this.prosperentGB.TabIndex = 2;
            this.prosperentGB.TabStop = false;
            this.prosperentGB.Text = "[ Prosperent ]";
            // 
            // productOpt
            // 
            this.productOpt.AutoSize = true;
            this.productOpt.Checked = true;
            this.productOpt.CheckState = System.Windows.Forms.CheckState.Checked;
            this.productOpt.Location = new System.Drawing.Point(374, 92);
            this.productOpt.Margin = new System.Windows.Forms.Padding(4);
            this.productOpt.Name = "productOpt";
            this.productOpt.Size = new System.Drawing.Size(106, 21);
            this.productOpt.TabIndex = 4;
            this.productOpt.Text = "Product Info";
            this.productOpt.UseVisualStyleBackColor = true;
            this.productOpt.CheckedChanged += new System.EventHandler(this.productOpt_CheckedChanged);
            // 
            // merchantOpt
            // 
            this.merchantOpt.AutoSize = true;
            this.merchantOpt.Checked = true;
            this.merchantOpt.CheckState = System.Windows.Forms.CheckState.Checked;
            this.merchantOpt.Location = new System.Drawing.Point(250, 92);
            this.merchantOpt.Margin = new System.Windows.Forms.Padding(4);
            this.merchantOpt.Name = "merchantOpt";
            this.merchantOpt.Size = new System.Drawing.Size(116, 21);
            this.merchantOpt.TabIndex = 3;
            this.merchantOpt.Text = "Merchant Info";
            this.merchantOpt.UseVisualStyleBackColor = true;
            // 
            // downloadProsperentOP
            // 
            this.downloadProsperentOP.AutoSize = true;
            this.downloadProsperentOP.Location = new System.Drawing.Point(32, 92);
            this.downloadProsperentOP.Margin = new System.Windows.Forms.Padding(4);
            this.downloadProsperentOP.Name = "downloadProsperentOP";
            this.downloadProsperentOP.Size = new System.Drawing.Size(200, 21);
            this.downloadProsperentOP.TabIndex = 2;
            this.downloadProsperentOP.Text = "Download Prosperent Data";
            this.downloadProsperentOP.UseVisualStyleBackColor = true;
            // 
            // pFilterED
            // 
            this.pFilterED.Location = new System.Drawing.Point(29, 63);
            this.pFilterED.Margin = new System.Windows.Forms.Padding(4);
            this.pFilterED.Name = "pFilterED";
            this.pFilterED.Size = new System.Drawing.Size(789, 22);
            this.pFilterED.TabIndex = 1;
            // 
            // pFilterLB
            // 
            this.pFilterLB.AutoSize = true;
            this.pFilterLB.Location = new System.Drawing.Point(25, 38);
            this.pFilterLB.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.pFilterLB.Name = "pFilterLB";
            this.pFilterLB.Size = new System.Drawing.Size(234, 17);
            this.pFilterLB.TabIndex = 0;
            this.pFilterLB.Text = "Additional Filter for Prosperent Data";
            // 
            // messagesTAB
            // 
            this.messagesTAB.Location = new System.Drawing.Point(4, 25);
            this.messagesTAB.Margin = new System.Windows.Forms.Padding(4);
            this.messagesTAB.Name = "messagesTAB";
            this.messagesTAB.Padding = new System.Windows.Forms.Padding(4);
            this.messagesTAB.Size = new System.Drawing.Size(942, 652);
            this.messagesTAB.TabIndex = 1;
            this.messagesTAB.Text = "Messages";
            this.messagesTAB.UseVisualStyleBackColor = true;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // syncModeCmd
            // 
            this.syncModeCmd.BackColor = System.Drawing.Color.BurlyWood;
            this.syncModeCmd.Location = new System.Drawing.Point(31, 101);
            this.syncModeCmd.Name = "syncModeCmd";
            this.syncModeCmd.Size = new System.Drawing.Size(169, 41);
            this.syncModeCmd.TabIndex = 4;
            this.syncModeCmd.Text = "Select Sync Options";
            this.syncModeCmd.UseVisualStyleBackColor = false;
            this.syncModeCmd.Click += new System.EventHandler(this.syncModeCmd_Click);
            // 
            // asyncModeCmd
            // 
            this.asyncModeCmd.BackColor = System.Drawing.Color.Orange;
            this.asyncModeCmd.Location = new System.Drawing.Point(224, 101);
            this.asyncModeCmd.Name = "asyncModeCmd";
            this.asyncModeCmd.Size = new System.Drawing.Size(169, 41);
            this.asyncModeCmd.TabIndex = 5;
            this.asyncModeCmd.Text = "Async Mode Only";
            this.asyncModeCmd.UseVisualStyleBackColor = false;
            this.asyncModeCmd.Click += new System.EventHandler(this.asyncModeCmd_Click);
            // 
            // MainF
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(950, 733);
            this.Controls.Add(this.processSchdTAB);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "MainF";
            this.Text = "Qpon Collector";
            this.processSchdTAB.ResumeLayout(false);
            this.mainTab.ResumeLayout(false);
            this.adminInfoGB.ResumeLayout(false);
            this.adminInfoGB.PerformLayout();
            this.scheduleGB.ResumeLayout(false);
            this.scheduleGB.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.fmtcGB.ResumeLayout(false);
            this.fmtcGB.PerformLayout();
            this.prosperentGB.ResumeLayout(false);
            this.prosperentGB.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.TabControl processSchdTAB;
        private System.Windows.Forms.TabPage mainTab;
        private System.Windows.Forms.GroupBox fmtcGB;
        private System.Windows.Forms.CheckBox downloadFmtcOP;
        private System.Windows.Forms.TextBox fFilterED;
        private System.Windows.Forms.Label fFilterLB;
        private System.Windows.Forms.GroupBox prosperentGB;
        private System.Windows.Forms.CheckBox downloadProsperentOP;
        private System.Windows.Forms.TextBox pFilterED;
        private System.Windows.Forms.Label pFilterLB;
        private System.Windows.Forms.TabPage messagesTAB;
        private System.Windows.Forms.GroupBox adminInfoGB;
        private System.Windows.Forms.TextBox passwordED;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox emailProfileED;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox lastNameED;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox firstnameED;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox scheduleGB;
        private System.Windows.Forms.Button setScheduleCM;
        private System.Windows.Forms.DateTimePicker setTimeDownloadEdt;
        private System.Windows.Forms.Button downloadInfoCM;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel statusLB;
        private System.Windows.Forms.Button getAdminTokenCM;
        private System.Windows.Forms.CheckBox productOpt;
        private System.Windows.Forms.CheckBox merchantOpt;
        private System.Windows.Forms.CheckBox fmtcProductOpt;
        private System.Windows.Forms.CheckBox fmtcMerchantOpt;
        private System.Windows.Forms.Button openFileCmd;
        private System.Windows.Forms.TextBox sourcefmtcEdt;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.CheckBox useSourceFileOpt;
        private System.Windows.Forms.Label fmtcDwnMsg;
        private System.Windows.Forms.Button asyncModeCmd;
        private System.Windows.Forms.Button syncModeCmd;
    }
}

