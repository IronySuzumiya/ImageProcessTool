namespace ImageProcessTool
{
    partial class MainForm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.pnlImage = new System.Windows.Forms.Panel();
            this.cmbBitmaps = new System.Windows.Forms.ComboBox();
            this.btnSaveImage = new System.Windows.Forms.Button();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.SuspendLayout();
            // 
            // pnlImage
            // 
            this.pnlImage.Location = new System.Drawing.Point(12, 45);
            this.pnlImage.Name = "pnlImage";
            this.pnlImage.Size = new System.Drawing.Size(428, 276);
            this.pnlImage.TabIndex = 0;
            this.pnlImage.Paint += new System.Windows.Forms.PaintEventHandler(this.pnlImage_Paint);
            // 
            // cmbBitmaps
            // 
            this.cmbBitmaps.FormattingEnabled = true;
            this.cmbBitmaps.Location = new System.Drawing.Point(12, 12);
            this.cmbBitmaps.Name = "cmbBitmaps";
            this.cmbBitmaps.Size = new System.Drawing.Size(136, 20);
            this.cmbBitmaps.TabIndex = 1;
            this.cmbBitmaps.SelectedIndexChanged += new System.EventHandler(this.cmbBitmaps_SelectedIndexChanged);
            // 
            // btnSaveImage
            // 
            this.btnSaveImage.Location = new System.Drawing.Point(365, 10);
            this.btnSaveImage.Name = "btnSaveImage";
            this.btnSaveImage.Size = new System.Drawing.Size(75, 23);
            this.btnSaveImage.TabIndex = 2;
            this.btnSaveImage.Text = "保存图片";
            this.btnSaveImage.UseVisualStyleBackColor = true;
            this.btnSaveImage.Click += new System.EventHandler(this.btnSaveImage_Click);
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.DefaultExt = "bmp";
            this.saveFileDialog.Filter = "位图文件|*.bmp";
            this.saveFileDialog.FileOk += new System.ComponentModel.CancelEventHandler(this.saveFileDialog_FileOk);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(452, 332);
            this.Controls.Add(this.btnSaveImage);
            this.Controls.Add(this.cmbBitmaps);
            this.Controls.Add(this.pnlImage);
            this.Name = "MainForm";
            this.Text = "Image Process Tool";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlImage;
        private System.Windows.Forms.ComboBox cmbBitmaps;
        private System.Windows.Forms.Button btnSaveImage;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
    }
}

