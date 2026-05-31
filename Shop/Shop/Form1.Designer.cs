namespace Shop
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblForms = new System.Windows.Forms.Label();
            this.lblQueryLbl = new System.Windows.Forms.Label();
            this.btnShops = new System.Windows.Forms.Button();
            this.btnProducts = new System.Windows.Forms.Button();
            this.btnWorkers = new System.Windows.Forms.Button();
            this.btnCustomers = new System.Windows.Forms.Button();
            this.btnProviders = new System.Windows.Forms.Button();
            this.btnSupply = new System.Windows.Forms.Button();
            this.btnPurchases = new System.Windows.Forms.Button();
            this.btnQueries = new System.Windows.Forms.Button();
            this.btnReports = new System.Windows.Forms.Button();
            this.SuspendLayout();

            this.lblTitle.Text = "Система управління магазином";
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Location = new System.Drawing.Point(20, 20);
            this.lblTitle.Size = new System.Drawing.Size(560, 36);
            this.lblTitle.AutoSize = false;

            this.lblForms.Text = "Довідники";
            this.lblForms.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Italic);
            this.lblForms.Location = new System.Drawing.Point(20, 70);
            this.lblForms.Size = new System.Drawing.Size(560, 20);
            this.lblForms.ForeColor = System.Drawing.Color.Gray;

            int bW = 250, bH = 38, col1 = 30, col2 = 310, row = 100, gap = 48;
            System.Drawing.Font bf = new System.Drawing.Font("Segoe UI", 10F);

            SetBtn(this.btnShops, "Магазини", col1, row, bW, bH, bf);
            SetBtn(this.btnProducts, "Товари", col2, row, bW, bH, bf);
            row += gap;
            SetBtn(this.btnWorkers, "Працівники", col1, row, bW, bH, bf);
            SetBtn(this.btnCustomers, "Покупці", col2, row, bW, bH, bf);
            row += gap;
            SetBtn(this.btnProviders, "Постачальники", col1, row, bW, bH, bf);
            SetBtn(this.btnSupply, "Постачання", col2, row, bW, bH, bf);
            row += gap;
            SetBtn(this.btnPurchases, "Покупки", col1, row, bW, bH, bf);

            row += gap + 10;
            this.lblQueryLbl.Text = "Аналітика";
            this.lblQueryLbl.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Italic);
            this.lblQueryLbl.Location = new System.Drawing.Point(20, row);
            this.lblQueryLbl.Size = new System.Drawing.Size(560, 20);
            this.lblQueryLbl.ForeColor = System.Drawing.Color.Gray;
            row += 28;

            SetBtn(this.btnQueries, "Запити", col1, row, bW, bH, bf);

            this.btnShops.Click += btnShops_Click;
            this.btnProducts.Click += btnProducts_Click;
            this.btnWorkers.Click += btnWorkers_Click;
            this.btnCustomers.Click += btnCustomers_Click;
            this.btnProviders.Click += btnProviders_Click;
            this.btnSupply.Click += btnSupply_Click;
            this.btnPurchases.Click += btnPurchases_Click;

            this.ClientSize = new System.Drawing.Size(610, row + bH + 30);
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                lblTitle, lblForms, btnShops, btnProducts, btnWorkers,
                btnCustomers, btnProviders, btnSupply, btnPurchases,
                lblQueryLbl, btnQueries, btnReports
            });
            this.Text = "Головне меню";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.ResumeLayout(false);
        }

        private void SetBtn(System.Windows.Forms.Button b, string text, int x, int y, int w, int h, System.Drawing.Font f)
        {
            b.Text = text;
            b.Location = new System.Drawing.Point(x, y);
            b.Size = new System.Drawing.Size(w, h);
            b.Font = f;
            b.FlatStyle = System.Windows.Forms.FlatStyle.System;
        }

        private System.Windows.Forms.Label lblTitle, lblForms, lblQueryLbl;
        private System.Windows.Forms.Button btnShops, btnProducts, btnWorkers,
                                            btnCustomers, btnProviders, btnSupply,
                                            btnPurchases, btnQueries, btnReports;
    }
}