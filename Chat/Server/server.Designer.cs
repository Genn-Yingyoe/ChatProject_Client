using System.Windows.Forms;

namespace Server
{
    partial class server
    {
        private System.Windows.Forms.TextBox txtLog;
        private System.Windows.Forms.Button btnStart;

        private void InitializeComponent()
        {
            this.txtLog = new System.Windows.Forms.TextBox();
            this.btnStart = new System.Windows.Forms.Button();
            this.SuspendLayout();

            // txtLog
            this.txtLog.Multiline = true;
            this.txtLog.ScrollBars = ScrollBars.Vertical;
            this.txtLog.ReadOnly = true;
            this.txtLog.Dock = DockStyle.Fill;

            // btnStart
            this.btnStart.Text = "서버 시작";
            this.btnStart.Dock = DockStyle.Top;
            this.btnStart.Height = 40;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);

            // ServerForm
            this.Text = "Chat Server";
            this.ClientSize = new System.Drawing.Size(500, 400);
            this.Controls.Add(this.txtLog);
            this.Controls.Add(this.btnStart);
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}

