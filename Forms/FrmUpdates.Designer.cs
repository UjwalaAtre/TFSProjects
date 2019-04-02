namespace gloUpdates
{
    partial class FrmUpdates
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmUpdates));
            this.pnlInstallProgress = new System.Windows.Forms.Panel();
            this.picUpdating = new System.Windows.Forms.PictureBox();
            this.lblProgress = new System.Windows.Forms.Label();
            this.lblExist = new System.Windows.Forms.Label();
            this.picExist = new System.Windows.Forms.Button();
            this.picSuccess = new System.Windows.Forms.Button();
            this.picFailed = new System.Windows.Forms.Button();
            this.lblSuccess = new System.Windows.Forms.Label();
            this.lblFailed = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label14 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.txtAUSID = new System.Windows.Forms.TextBox();
            this.pnlAUSID = new System.Windows.Forms.Panel();
            this.label6 = new System.Windows.Forms.Label();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnSave = new System.Windows.Forms.ToolStripButton();
            this.btnClose = new System.Windows.Forms.ToolStripButton();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.pnlInstallProgress.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picUpdating)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.pnlAUSID.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlInstallProgress
            // 
            this.pnlInstallProgress.BackColor = System.Drawing.Color.White;
            this.pnlInstallProgress.Controls.Add(this.picUpdating);
            this.pnlInstallProgress.Controls.Add(this.lblProgress);
            this.pnlInstallProgress.Controls.Add(this.lblExist);
            this.pnlInstallProgress.Controls.Add(this.picExist);
            this.pnlInstallProgress.Controls.Add(this.picSuccess);
            this.pnlInstallProgress.Controls.Add(this.picFailed);
            this.pnlInstallProgress.Controls.Add(this.lblSuccess);
            this.pnlInstallProgress.Controls.Add(this.lblFailed);
            this.pnlInstallProgress.Controls.Add(this.panel1);
            this.pnlInstallProgress.Controls.Add(this.label14);
            this.pnlInstallProgress.Controls.Add(this.label15);
            this.pnlInstallProgress.Controls.Add(this.label16);
            this.pnlInstallProgress.Controls.Add(this.label17);
            this.pnlInstallProgress.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.pnlInstallProgress.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlInstallProgress.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(73)))), ((int)(((byte)(125)))));
            this.pnlInstallProgress.Location = new System.Drawing.Point(0, 0);
            this.pnlInstallProgress.Name = "pnlInstallProgress";
            this.pnlInstallProgress.Size = new System.Drawing.Size(420, 241);
            this.pnlInstallProgress.TabIndex = 75;
            // 
            // picUpdating
            // 
            this.picUpdating.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.picUpdating.Image = ((System.Drawing.Image)(resources.GetObject("picUpdating.Image")));
            this.picUpdating.Location = new System.Drawing.Point(151, 12);
            this.picUpdating.Name = "picUpdating";
            this.picUpdating.Size = new System.Drawing.Size(100, 100);
            this.picUpdating.TabIndex = 69;
            this.picUpdating.TabStop = false;
            // 
            // lblProgress
            // 
            this.lblProgress.Font = new System.Drawing.Font("Cambria", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblProgress.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(1)))), ((int)(((byte)(126)))), ((int)(((byte)(194)))));
            this.lblProgress.Location = new System.Drawing.Point(68, 124);
            this.lblProgress.Name = "lblProgress";
            this.lblProgress.Size = new System.Drawing.Size(281, 40);
            this.lblProgress.TabIndex = 67;
            this.lblProgress.Text = "Updating gloLDSSniffer Service. This may take few minutes.";
            this.lblProgress.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblExist
            // 
            this.lblExist.AutoSize = true;
            this.lblExist.Font = new System.Drawing.Font("Cambria", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblExist.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(76)))), ((int)(((byte)(192)))));
            this.lblExist.Location = new System.Drawing.Point(106, 130);
            this.lblExist.Name = "lblExist";
            this.lblExist.Size = new System.Drawing.Size(190, 19);
            this.lblExist.TabIndex = 76;
            this.lblExist.Text = "No Updates Were Found.";
            this.lblExist.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblExist.Visible = false;
            // 
            // picExist
            // 
            this.picExist.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("picExist.BackgroundImage")));
            this.picExist.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.picExist.FlatAppearance.BorderSize = 0;
            this.picExist.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.picExist.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.picExist.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.picExist.Location = new System.Drawing.Point(151, 12);
            this.picExist.Name = "picExist";
            this.picExist.Size = new System.Drawing.Size(100, 100);
            this.picExist.TabIndex = 75;
            this.picExist.UseVisualStyleBackColor = true;
            this.picExist.Visible = false;
            // 
            // picSuccess
            // 
            this.picSuccess.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("picSuccess.BackgroundImage")));
            this.picSuccess.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.picSuccess.FlatAppearance.BorderSize = 0;
            this.picSuccess.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.picSuccess.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.picSuccess.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.picSuccess.Location = new System.Drawing.Point(151, 12);
            this.picSuccess.Name = "picSuccess";
            this.picSuccess.Size = new System.Drawing.Size(100, 100);
            this.picSuccess.TabIndex = 73;
            this.picSuccess.UseVisualStyleBackColor = true;
            this.picSuccess.Visible = false;
            // 
            // picFailed
            // 
            this.picFailed.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("picFailed.BackgroundImage")));
            this.picFailed.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.picFailed.FlatAppearance.BorderSize = 0;
            this.picFailed.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.picFailed.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.picFailed.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.picFailed.Location = new System.Drawing.Point(151, 12);
            this.picFailed.Name = "picFailed";
            this.picFailed.Size = new System.Drawing.Size(100, 100);
            this.picFailed.TabIndex = 74;
            this.picFailed.UseVisualStyleBackColor = true;
            this.picFailed.Visible = false;
            // 
            // lblSuccess
            // 
            this.lblSuccess.Font = new System.Drawing.Font("Cambria", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSuccess.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(57)))), ((int)(((byte)(151)))), ((int)(((byte)(5)))));
            this.lblSuccess.Location = new System.Drawing.Point(92, 120);
            this.lblSuccess.Name = "lblSuccess";
            this.lblSuccess.Size = new System.Drawing.Size(219, 44);
            this.lblSuccess.TabIndex = 71;
            this.lblSuccess.Text = "gloLDSSniffer Service Successfully Updated.";
            this.lblSuccess.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblSuccess.Visible = false;
            // 
            // lblFailed
            // 
            this.lblFailed.Font = new System.Drawing.Font("Cambria", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFailed.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(161)))), ((int)(((byte)(32)))), ((int)(((byte)(0)))));
            this.lblFailed.Location = new System.Drawing.Point(26, 119);
            this.lblFailed.Name = "lblFailed";
            this.lblFailed.Size = new System.Drawing.Size(365, 44);
            this.lblFailed.TabIndex = 72;
            this.lblFailed.Text = "gloLDSSniffer Service Failed to Update.\r\nTry Again or Contact System Administrato" +
    "r.";
            this.lblFailed.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblFailed.Visible = false;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(1)))), ((int)(((byte)(39)))), ((int)(((byte)(77)))));
            this.panel1.Controls.Add(this.pictureBox1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(1, 171);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(418, 69);
            this.panel1.TabIndex = 68;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pictureBox1.BackgroundImage")));
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox1.Location = new System.Drawing.Point(88, 5);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(224, 60);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // label14
            // 
            this.label14.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(73)))), ((int)(((byte)(125)))));
            this.label14.Dock = System.Windows.Forms.DockStyle.Right;
            this.label14.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(73)))), ((int)(((byte)(125)))));
            this.label14.Location = new System.Drawing.Point(419, 1);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(1, 239);
            this.label14.TabIndex = 66;
            this.label14.UseWaitCursor = true;
            // 
            // label15
            // 
            this.label15.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(73)))), ((int)(((byte)(125)))));
            this.label15.Dock = System.Windows.Forms.DockStyle.Left;
            this.label15.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(73)))), ((int)(((byte)(125)))));
            this.label15.Location = new System.Drawing.Point(0, 1);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(1, 239);
            this.label15.TabIndex = 65;
            this.label15.UseWaitCursor = true;
            // 
            // label16
            // 
            this.label16.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(73)))), ((int)(((byte)(125)))));
            this.label16.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.label16.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(73)))), ((int)(((byte)(125)))));
            this.label16.Location = new System.Drawing.Point(0, 240);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(420, 1);
            this.label16.TabIndex = 64;
            this.label16.UseWaitCursor = true;
            // 
            // label17
            // 
            this.label17.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(73)))), ((int)(((byte)(125)))));
            this.label17.Dock = System.Windows.Forms.DockStyle.Top;
            this.label17.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(73)))), ((int)(((byte)(125)))));
            this.label17.Location = new System.Drawing.Point(0, 0);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(420, 1);
            this.label17.TabIndex = 63;
            this.label17.UseWaitCursor = true;
            // 
            // txtAUSID
            // 
            this.txtAUSID.Location = new System.Drawing.Point(85, 105);
            this.txtAUSID.MaxLength = 100;
            this.txtAUSID.Name = "txtAUSID";
            this.txtAUSID.Size = new System.Drawing.Size(306, 22);
            this.txtAUSID.TabIndex = 0;
            // 
            // pnlAUSID
            // 
            this.pnlAUSID.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(207)))), ((int)(((byte)(224)))), ((int)(((byte)(248)))));
            this.pnlAUSID.Controls.Add(this.label6);
            this.pnlAUSID.Controls.Add(this.toolStrip1);
            this.pnlAUSID.Controls.Add(this.label5);
            this.pnlAUSID.Controls.Add(this.label4);
            this.pnlAUSID.Controls.Add(this.label3);
            this.pnlAUSID.Controls.Add(this.label2);
            this.pnlAUSID.Controls.Add(this.label1);
            this.pnlAUSID.Controls.Add(this.txtAUSID);
            this.pnlAUSID.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlAUSID.Location = new System.Drawing.Point(0, 0);
            this.pnlAUSID.Name = "pnlAUSID";
            this.pnlAUSID.Size = new System.Drawing.Size(420, 241);
            this.pnlAUSID.TabIndex = 1;
            // 
            // label6
            // 
            this.label6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(73)))), ((int)(((byte)(125)))));
            this.label6.Dock = System.Windows.Forms.DockStyle.Top;
            this.label6.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(73)))), ((int)(((byte)(125)))));
            this.label6.Location = new System.Drawing.Point(1, 54);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(418, 1);
            this.label6.TabIndex = 84;
            this.label6.UseWaitCursor = true;
            // 
            // toolStrip1
            // 
            this.toolStrip1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("toolStrip1.BackgroundImage")));
            this.toolStrip1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnSave,
            this.btnClose});
            this.toolStrip1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Flow;
            this.toolStrip1.Location = new System.Drawing.Point(1, 1);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(418, 53);
            this.toolStrip1.TabIndex = 2;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // btnSave
            // 
            this.btnSave.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSave.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(73)))), ((int)(((byte)(125)))));
            this.btnSave.Image = ((System.Drawing.Image)(resources.GetObject("btnSave.Image")));
            this.btnSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(87, 50);
            this.btnSave.Text = "Save&&Install";
            this.btnSave.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnSave.ToolTipText = "Save and Install";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnClose
            // 
            this.btnClose.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClose.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(73)))), ((int)(((byte)(125)))));
            this.btnClose.Image = ((System.Drawing.Image)(resources.GetObject("btnClose.Image")));
            this.btnClose.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(43, 50);
            this.btnClose.Text = "Close";
            this.btnClose.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(73)))), ((int)(((byte)(125)))));
            this.label5.Location = new System.Drawing.Point(27, 109);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(54, 14);
            this.label5.TabIndex = 83;
            this.label5.Text = "AUS ID :";
            // 
            // label4
            // 
            this.label4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(73)))), ((int)(((byte)(125)))));
            this.label4.Dock = System.Windows.Forms.DockStyle.Right;
            this.label4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(73)))), ((int)(((byte)(125)))));
            this.label4.Location = new System.Drawing.Point(419, 1);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(1, 239);
            this.label4.TabIndex = 82;
            this.label4.UseWaitCursor = true;
            // 
            // label3
            // 
            this.label3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(73)))), ((int)(((byte)(125)))));
            this.label3.Dock = System.Windows.Forms.DockStyle.Left;
            this.label3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(73)))), ((int)(((byte)(125)))));
            this.label3.Location = new System.Drawing.Point(0, 1);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(1, 239);
            this.label3.TabIndex = 81;
            this.label3.UseWaitCursor = true;
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(73)))), ((int)(((byte)(125)))));
            this.label2.Dock = System.Windows.Forms.DockStyle.Top;
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(73)))), ((int)(((byte)(125)))));
            this.label2.Location = new System.Drawing.Point(0, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(420, 1);
            this.label2.TabIndex = 80;
            this.label2.UseWaitCursor = true;
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(73)))), ((int)(((byte)(125)))));
            this.label1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(73)))), ((int)(((byte)(125)))));
            this.label1.Location = new System.Drawing.Point(0, 240);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(420, 1);
            this.label1.TabIndex = 79;
            this.label1.UseWaitCursor = true;
            // 
            // FrmUpdates
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(420, 241);
            this.Controls.Add(this.pnlAUSID);
            this.Controls.Add(this.pnlInstallProgress);
            this.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmUpdates";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Install Updates";
            this.Load += new System.EventHandler(this.FrmUpdates_Load);
            this.Shown += new System.EventHandler(this.FrmUpdates_Shown);
            this.pnlInstallProgress.ResumeLayout(false);
            this.pnlInstallProgress.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picUpdating)).EndInit();
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.pnlAUSID.ResumeLayout(false);
            this.pnlAUSID.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox pictureBox1;
        internal System.Windows.Forms.Panel pnlInstallProgress;
        internal System.Windows.Forms.Label lblProgress;
        internal System.Windows.Forms.PictureBox picUpdating;
        internal System.Windows.Forms.Label lblFailed;
        internal System.Windows.Forms.Label lblSuccess;
        private System.Windows.Forms.Button picFailed;
        private System.Windows.Forms.Button picSuccess;
        internal System.Windows.Forms.Label lblExist;
        private System.Windows.Forms.Button picExist;
        private System.Windows.Forms.TextBox txtAUSID;
        private System.Windows.Forms.Panel pnlAUSID;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btnSave;
        private System.Windows.Forms.ToolStripButton btnClose;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
    }
}

