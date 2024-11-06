namespace BEM4
{
    partial class MenuConnectForm
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

        //
        #region Windows Form Designer generated code
        //

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MenuConnectForm));
            this.button1 = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.button2 = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.groupBox_ble = new System.Windows.Forms.GroupBox();
            this.groupBox_wifi = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.button4 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.tb_IP = new System.Windows.Forms.TextBox();
            this.timer2 = new System.Windows.Forms.Timer(this.components);
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.panel_wifi = new System.Windows.Forms.Panel();
            this.label_wifi = new System.Windows.Forms.Label();
            this.panel_ble = new System.Windows.Forms.Panel();
            this.label_ble = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.button_unbind = new System.Windows.Forms.Button();
            this.groupBox_ble.SuspendLayout();
            this.groupBox_wifi.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.panel_wifi.SuspendLayout();
            this.panel_ble.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            resources.ApplyResources(this.button1, "button1");
            this.button1.Name = "button1";
            this.button1.Tag = " ";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // textBox1
            // 
            resources.ApplyResources(this.textBox1, "textBox1");
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            // 
            // comboBox1
            // 
            this.comboBox1.BackColor = System.Drawing.SystemColors.Control;
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.comboBox1, "comboBox1");
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // button2
            // 
            resources.ApplyResources(this.button2, "button2");
            this.button2.Name = "button2";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // timer1
            // 
            this.timer1.Interval = 500;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // groupBox_ble
            // 
            this.groupBox_ble.Controls.Add(this.button_unbind);
            this.groupBox_ble.Controls.Add(this.comboBox1);
            this.groupBox_ble.Controls.Add(this.button1);
            this.groupBox_ble.Controls.Add(this.button2);
            resources.ApplyResources(this.groupBox_ble, "groupBox_ble");
            this.groupBox_ble.Name = "groupBox_ble";
            this.groupBox_ble.TabStop = false;
            // 
            // groupBox_wifi
            // 
            this.groupBox_wifi.Controls.Add(this.label1);
            this.groupBox_wifi.Controls.Add(this.button4);
            this.groupBox_wifi.Controls.Add(this.button3);
            this.groupBox_wifi.Controls.Add(this.tb_IP);
            resources.ApplyResources(this.groupBox_wifi, "groupBox_wifi");
            this.groupBox_wifi.Name = "groupBox_wifi";
            this.groupBox_wifi.TabStop = false;
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // button4
            // 
            resources.ApplyResources(this.button4, "button4");
            this.button4.Name = "button4";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // button3
            // 
            resources.ApplyResources(this.button3, "button3");
            this.button3.ForeColor = System.Drawing.SystemColors.WindowText;
            this.button3.Name = "button3";
            this.button3.Tag = " ";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // tb_IP
            // 
            resources.ApplyResources(this.tb_IP, "tb_IP");
            this.tb_IP.Name = "tb_IP";
            this.tb_IP.ReadOnly = true;
            // 
            // timer2
            // 
            this.timer2.Interval = 500;
            this.timer2.Tick += new System.EventHandler(this.timer2_Tick);
            // 
            // splitContainer1
            // 
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.splitContainer1, "splitContainer1");
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.panel_wifi);
            this.splitContainer1.Panel1.Controls.Add(this.panel_ble);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.panel3);
            // 
            // panel_wifi
            // 
            this.panel_wifi.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.panel_wifi.Controls.Add(this.label_wifi);
            resources.ApplyResources(this.panel_wifi, "panel_wifi");
            this.panel_wifi.Name = "panel_wifi";
            this.panel_wifi.Click += new System.EventHandler(this.panel_wifi_Click);
            // 
            // label_wifi
            // 
            resources.ApplyResources(this.label_wifi, "label_wifi");
            this.label_wifi.Name = "label_wifi";
            this.label_wifi.Click += new System.EventHandler(this.label_wifi_Click);
            // 
            // panel_ble
            // 
            this.panel_ble.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.panel_ble.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel_ble.Controls.Add(this.label_ble);
            resources.ApplyResources(this.panel_ble, "panel_ble");
            this.panel_ble.Name = "panel_ble";
            this.panel_ble.Click += new System.EventHandler(this.panel_ble_Click);
            // 
            // label_ble
            // 
            resources.ApplyResources(this.label_ble, "label_ble");
            this.label_ble.Name = "label_ble";
            this.label_ble.Click += new System.EventHandler(this.label_ble_Click);
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.groupBox_ble);
            this.panel3.Controls.Add(this.groupBox_wifi);
            this.panel3.Controls.Add(this.textBox1);
            resources.ApplyResources(this.panel3, "panel3");
            this.panel3.Name = "panel3";
            // 
            // button_unbind
            // 
            resources.ApplyResources(this.button_unbind, "button_unbind");
            this.button_unbind.ForeColor = System.Drawing.SystemColors.WindowText;
            this.button_unbind.Name = "button_unbind";
            this.button_unbind.Tag = " ";
            this.button_unbind.UseVisualStyleBackColor = true;
            this.button_unbind.Click += new System.EventHandler(this.button_unbind_Click);
            // 
            // MenuConnectForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MenuConnectForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MenuConnectForm_FormClosing);
            this.Load += new System.EventHandler(this.MenuConnectForm_Load);
            this.groupBox_ble.ResumeLayout(false);
            this.groupBox_wifi.ResumeLayout(false);
            this.groupBox_wifi.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.panel_wifi.ResumeLayout(false);
            this.panel_wifi.PerformLayout();
            this.panel_ble.ResumeLayout(false);
            this.panel_ble.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.ResumeLayout(false);

        }

        //
        #endregion
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.GroupBox groupBox_ble;
        private System.Windows.Forms.GroupBox groupBox_wifi;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.TextBox tb_IP;
        private System.Windows.Forms.Timer timer2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Panel panel_wifi;
        private System.Windows.Forms.Label label_wifi;
        private System.Windows.Forms.Panel panel_ble;
        private System.Windows.Forms.Label label_ble;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Button button_unbind;
    }
}

//end

