
namespace SecurityApplication
{
    partial class FormSecurityApp
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
            this.textBoxIPAddress = new System.Windows.Forms.TextBox();
            this.labelIPAddress = new System.Windows.Forms.Label();
            this.buttonConnect = new System.Windows.Forms.Button();
            this.labelConnectionStatus = new System.Windows.Forms.Label();
            this.checkBoxActive = new System.Windows.Forms.CheckBox();
            this.labelInUse = new System.Windows.Forms.Label();
            this.buttonDisarm = new System.Windows.Forms.Button();
            this.labelAlarmStatus = new System.Windows.Forms.Label();
            this.trackBarVolume = new System.Windows.Forms.TrackBar();
            this.labelVolumeControl = new System.Windows.Forms.Label();
            this.labelBrightness = new System.Windows.Forms.Label();
            this.trackBarBrightness = new System.Windows.Forms.TrackBar();
            this.labelErrorMessage = new System.Windows.Forms.Label();
            this.textBoxAlarmStatus = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarVolume)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarBrightness)).BeginInit();
            this.SuspendLayout();
            // 
            // textBoxIPAddress
            // 
            this.textBoxIPAddress.Location = new System.Drawing.Point(136, 30);
            this.textBoxIPAddress.Name = "textBoxIPAddress";
            this.textBoxIPAddress.Size = new System.Drawing.Size(157, 22);
            this.textBoxIPAddress.TabIndex = 0;
            // 
            // labelIPAddress
            // 
            this.labelIPAddress.AutoSize = true;
            this.labelIPAddress.Location = new System.Drawing.Point(50, 33);
            this.labelIPAddress.Name = "labelIPAddress";
            this.labelIPAddress.Size = new System.Drawing.Size(80, 17);
            this.labelIPAddress.TabIndex = 1;
            this.labelIPAddress.Text = "IP Address:";
            // 
            // buttonConnect
            // 
            this.buttonConnect.Location = new System.Drawing.Point(53, 64);
            this.buttonConnect.Name = "buttonConnect";
            this.buttonConnect.Size = new System.Drawing.Size(75, 23);
            this.buttonConnect.TabIndex = 2;
            this.buttonConnect.Text = "Connect";
            this.buttonConnect.UseVisualStyleBackColor = true;
            this.buttonConnect.Click += new System.EventHandler(this.buttonConnect_Click);
            // 
            // labelConnectionStatus
            // 
            this.labelConnectionStatus.AutoSize = true;
            this.labelConnectionStatus.Location = new System.Drawing.Point(395, 33);
            this.labelConnectionStatus.Name = "labelConnectionStatus";
            this.labelConnectionStatus.Size = new System.Drawing.Size(102, 17);
            this.labelConnectionStatus.TabIndex = 3;
            this.labelConnectionStatus.Text = "Not Connected";
            // 
            // checkBoxActive
            // 
            this.checkBoxActive.AutoSize = true;
            this.checkBoxActive.Location = new System.Drawing.Point(41, 347);
            this.checkBoxActive.Name = "checkBoxActive";
            this.checkBoxActive.Size = new System.Drawing.Size(68, 21);
            this.checkBoxActive.TabIndex = 4;
            this.checkBoxActive.Text = "Active";
            this.checkBoxActive.UseVisualStyleBackColor = true;
            this.checkBoxActive.CheckedChanged += new System.EventHandler(this.checkBoxActive_CheckedChanged);
            // 
            // labelInUse
            // 
            this.labelInUse.AutoSize = true;
            this.labelInUse.Location = new System.Drawing.Point(50, 206);
            this.labelInUse.Name = "labelInUse";
            this.labelInUse.Size = new System.Drawing.Size(52, 17);
            this.labelInUse.TabIndex = 5;
            this.labelInUse.Text = "In Use:";
            // 
            // buttonDisarm
            // 
            this.buttonDisarm.Location = new System.Drawing.Point(41, 374);
            this.buttonDisarm.Name = "buttonDisarm";
            this.buttonDisarm.Size = new System.Drawing.Size(75, 23);
            this.buttonDisarm.TabIndex = 6;
            this.buttonDisarm.Text = "Disarm";
            this.buttonDisarm.UseVisualStyleBackColor = true;
            this.buttonDisarm.Click += new System.EventHandler(this.buttonDisarm_Click);
            // 
            // labelAlarmStatus
            // 
            this.labelAlarmStatus.AutoSize = true;
            this.labelAlarmStatus.Location = new System.Drawing.Point(504, 206);
            this.labelAlarmStatus.Name = "labelAlarmStatus";
            this.labelAlarmStatus.Size = new System.Drawing.Size(52, 17);
            this.labelAlarmStatus.TabIndex = 7;
            this.labelAlarmStatus.Text = "Status:";
            // 
            // trackBarVolume
            // 
            this.trackBarVolume.Location = new System.Drawing.Point(136, 232);
            this.trackBarVolume.Maximum = 9;
            this.trackBarVolume.Name = "trackBarVolume";
            this.trackBarVolume.Size = new System.Drawing.Size(278, 56);
            this.trackBarVolume.TabIndex = 8;
            this.trackBarVolume.Scroll += new System.EventHandler(this.trackBarVolume_Scroll);
            this.trackBarVolume.MouseUp += new System.Windows.Forms.MouseEventHandler(this.trackBarVolume_MouseUp);
            // 
            // labelVolumeControl
            // 
            this.labelVolumeControl.AutoSize = true;
            this.labelVolumeControl.Location = new System.Drawing.Point(50, 244);
            this.labelVolumeControl.Name = "labelVolumeControl";
            this.labelVolumeControl.Size = new System.Drawing.Size(59, 17);
            this.labelVolumeControl.TabIndex = 9;
            this.labelVolumeControl.Text = "Volume:";
            // 
            // labelBrightness
            // 
            this.labelBrightness.AutoSize = true;
            this.labelBrightness.Location = new System.Drawing.Point(50, 298);
            this.labelBrightness.Name = "labelBrightness";
            this.labelBrightness.Size = new System.Drawing.Size(79, 17);
            this.labelBrightness.TabIndex = 11;
            this.labelBrightness.Text = "Brightness:";
            // 
            // trackBarBrightness
            // 
            this.trackBarBrightness.Location = new System.Drawing.Point(136, 283);
            this.trackBarBrightness.Maximum = 9;
            this.trackBarBrightness.Name = "trackBarBrightness";
            this.trackBarBrightness.Size = new System.Drawing.Size(278, 56);
            this.trackBarBrightness.TabIndex = 12;
            this.trackBarBrightness.Scroll += new System.EventHandler(this.trackBarBrightness_Scroll);
            this.trackBarBrightness.MouseUp += new System.Windows.Forms.MouseEventHandler(this.trackBarBrightness_MouseUp);
            // 
            // labelErrorMessage
            // 
            this.labelErrorMessage.AutoSize = true;
            this.labelErrorMessage.Location = new System.Drawing.Point(395, 64);
            this.labelErrorMessage.MaximumSize = new System.Drawing.Size(300, 100);
            this.labelErrorMessage.Name = "labelErrorMessage";
            this.labelErrorMessage.Size = new System.Drawing.Size(298, 34);
            this.labelErrorMessage.TabIndex = 13;
            this.labelErrorMessage.Text = "Really long error message for example to see what\'s going on hmmmmmm";
            // 
            // textBoxAlarmStatus
            // 
            this.textBoxAlarmStatus.Location = new System.Drawing.Point(507, 226);
            this.textBoxAlarmStatus.Name = "textBoxAlarmStatus";
            this.textBoxAlarmStatus.ReadOnly = true;
            this.textBoxAlarmStatus.Size = new System.Drawing.Size(116, 22);
            this.textBoxAlarmStatus.TabIndex = 14;
            this.textBoxAlarmStatus.Text = "Disarmed";
            this.textBoxAlarmStatus.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // FormSecurityApp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 549);
            this.Controls.Add(this.textBoxAlarmStatus);
            this.Controls.Add(this.labelErrorMessage);
            this.Controls.Add(this.trackBarBrightness);
            this.Controls.Add(this.labelBrightness);
            this.Controls.Add(this.labelVolumeControl);
            this.Controls.Add(this.trackBarVolume);
            this.Controls.Add(this.labelAlarmStatus);
            this.Controls.Add(this.buttonDisarm);
            this.Controls.Add(this.labelInUse);
            this.Controls.Add(this.checkBoxActive);
            this.Controls.Add(this.labelConnectionStatus);
            this.Controls.Add(this.buttonConnect);
            this.Controls.Add(this.labelIPAddress);
            this.Controls.Add(this.textBoxIPAddress);
            this.Name = "FormSecurityApp";
            this.Text = "Room Security Application";
            ((System.ComponentModel.ISupportInitialize)(this.trackBarVolume)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarBrightness)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxIPAddress;
        private System.Windows.Forms.Label labelIPAddress;
        private System.Windows.Forms.Button buttonConnect;
        private System.Windows.Forms.Label labelConnectionStatus;
        private System.Windows.Forms.CheckBox checkBoxActive;
        private System.Windows.Forms.Label labelInUse;
        private System.Windows.Forms.Button buttonDisarm;
        private System.Windows.Forms.Label labelAlarmStatus;
        private System.Windows.Forms.TrackBar trackBarVolume;
        private System.Windows.Forms.Label labelVolumeControl;
        private System.Windows.Forms.Label labelBrightness;
        private System.Windows.Forms.TrackBar trackBarBrightness;
        private System.Windows.Forms.Label labelErrorMessage;
        private System.Windows.Forms.TextBox textBoxAlarmStatus;
    }
}

