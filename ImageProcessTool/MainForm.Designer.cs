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
            this.btnSave = new System.Windows.Forms.Button();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.btnSaveAll = new System.Windows.Forms.Button();
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
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
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(284, 10);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 2;
            this.btnSave.Text = "保存";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.DefaultExt = "bmp";
            this.saveFileDialog.Filter = "位图文件|*.bmp";
            this.saveFileDialog.FileOk += new System.ComponentModel.CancelEventHandler(this.saveFileDialog_FileOk);
            // 
            // btnSaveAll
            // 
            this.btnSaveAll.Location = new System.Drawing.Point(365, 10);
            this.btnSaveAll.Name = "btnSaveAll";
            this.btnSaveAll.Size = new System.Drawing.Size(75, 23);
            this.btnSaveAll.TabIndex = 3;
            this.btnSaveAll.Text = "保存全部";
            this.btnSaveAll.UseVisualStyleBackColor = true;
            this.btnSaveAll.Click += new System.EventHandler(this.btnSaveAll_Click);
            // 
            // folderBrowserDialog
            // 
            this.folderBrowserDialog.Description = "选择图片存放位置";
            this.folderBrowserDialog.RootFolder = System.Environment.SpecialFolder.MyComputer;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(452, 332);
            this.Controls.Add(this.btnSaveAll);
            this.Controls.Add(this.btnSave);
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
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.Button btnSaveAll;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
    }
}

