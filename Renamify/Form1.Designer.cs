namespace Renamify
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.TextBox txtFolder;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.Button btnScan;
        private System.Windows.Forms.Button btnPreview;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.Button btnUndo;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.DataGridView grid;
        private System.Windows.Forms.TextBox txtLog;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.txtFolder = new System.Windows.Forms.TextBox();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.btnScan = new System.Windows.Forms.Button();
            this.btnPreview = new System.Windows.Forms.Button();
            this.btnApply = new System.Windows.Forms.Button();
            this.btnUndo = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.grid = new System.Windows.Forms.DataGridView();
            this.txtLog = new System.Windows.Forms.TextBox();

            ((System.ComponentModel.ISupportInitialize)(this.grid)).BeginInit();
            this.SuspendLayout();

            // Form
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Text = "Renamify";
            this.ClientSize = new System.Drawing.Size(1100, 700);
            this.MinimumSize = new System.Drawing.Size(900, 600);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;

            // txtFolder
            this.txtFolder.ReadOnly = true;
            this.txtFolder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFolder.Location = new System.Drawing.Point(12, 12);
            this.txtFolder.Size = new System.Drawing.Size(640, 27);

            // btnBrowse
            this.btnBrowse.Text = "Browse...";
            this.btnBrowse.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
            this.btnBrowse.Location = new System.Drawing.Point(658, 10);
            this.btnBrowse.Size = new System.Drawing.Size(120, 30);

            // btnScan
            this.btnScan.Text = "Scan";
            this.btnScan.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
            this.btnScan.Location = new System.Drawing.Point(784, 10);
            this.btnScan.Size = new System.Drawing.Size(90, 30);

            // btnPreview
            this.btnPreview.Text = "Preview";
            this.btnPreview.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
            this.btnPreview.Location = new System.Drawing.Point(880, 10);
            this.btnPreview.Size = new System.Drawing.Size(90, 30);

            // btnApply
            this.btnApply.Text = "Apply";
            this.btnApply.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
            this.btnApply.Location = new System.Drawing.Point(976, 10);
            this.btnApply.Size = new System.Drawing.Size(90, 30);

            // btnUndo
            this.btnUndo.Text = "Undo";
            this.btnUndo.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
            this.btnUndo.Location = new System.Drawing.Point(976, 46);
            this.btnUndo.Size = new System.Drawing.Size(90, 30);

            // lblStatus
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(12, 50);
            this.lblStatus.Size = new System.Drawing.Size(250, 20);
            this.lblStatus.Text = "Pick a folder, then click Scan.";

            // grid
            this.grid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.grid.Location = new System.Drawing.Point(12, 85);
            this.grid.Size = new System.Drawing.Size(1054, 450);

            // txtLog
            this.txtLog.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLog.Location = new System.Drawing.Point(12, 545);
            this.txtLog.Size = new System.Drawing.Size(1054, 140);
            this.txtLog.Multiline = true;
            this.txtLog.ReadOnly = true;
            this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;

            // Controls
            this.Controls.Add(this.txtFolder);
            this.Controls.Add(this.btnBrowse);
            this.Controls.Add(this.btnScan);
            this.Controls.Add(this.btnPreview);
            this.Controls.Add(this.btnApply);
            this.Controls.Add(this.btnUndo);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.grid);
            this.Controls.Add(this.txtLog);

            ((System.ComponentModel.ISupportInitialize)(this.grid)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
