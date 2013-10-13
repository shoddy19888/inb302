namespace INB302WPF
{
    partial class DepthSenor
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
            this.video = new System.Windows.Forms.PictureBox();
            this.rtbMessages = new System.Windows.Forms.TextBox();
            this.txtAngle = new System.Windows.Forms.TextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.video)).BeginInit();
            this.SuspendLayout();
            // 
            // video
            // 
            this.video.Location = new System.Drawing.Point(0, -1);
            this.video.Name = "video";
            this.video.Size = new System.Drawing.Size(434, 320);
            this.video.TabIndex = 0;
            this.video.TabStop = false;
            // 
            // rtbMessages
            // 
            this.rtbMessages.Location = new System.Drawing.Point(0, 325);
            this.rtbMessages.Name = "rtbMessages";
            this.rtbMessages.Size = new System.Drawing.Size(434, 20);
            this.rtbMessages.TabIndex = 1;
            // 
            // txtAngle
            // 
            this.txtAngle.Location = new System.Drawing.Point(469, 115);
            this.txtAngle.Name = "txtAngle";
            this.txtAngle.Size = new System.Drawing.Size(120, 20);
            this.txtAngle.TabIndex = 7;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(469, 64);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(121, 36);
            this.button2.TabIndex = 6;
            this.button2.Text = "down";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(469, 22);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(121, 36);
            this.button1.TabIndex = 5;
            this.button1.Text = "Up";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // DepthSenor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(667, 416);
            this.Controls.Add(this.txtAngle);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.rtbMessages);
            this.Controls.Add(this.video);
            this.Name = "DepthSenor";
            this.Text = "DepthSenor";
            this.Load += new System.EventHandler(this.DepthSenor_Load);
            ((System.ComponentModel.ISupportInitialize)(this.video)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox video;
        private System.Windows.Forms.TextBox rtbMessages;
        private System.Windows.Forms.TextBox txtAngle;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
    }
}