using System;
using System.Windows.Forms;

namespace Shop
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            Text = "Головне меню";
            Size = new System.Drawing.Size(540, 720);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            BuildUI();
        }

        private void BuildUI()
        {
            var lblTitle = new Label
            {
                Text = "Система управління магазином",
                Font = new System.Drawing.Font("Segoe UI", 13, System.Drawing.FontStyle.Bold),
                Dock = DockStyle.Top,
                Height = 40,
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            };

            var panel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                Padding = new Padding(40, 10, 40, 10),
                AutoScroll = true
            };
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            panel.Controls.Add(MakeBtn("Магазини", () => new FormShops().ShowDialog()));
            panel.Controls.Add(MakeBtn("Товари", () => new FormProducts().ShowDialog()));
            panel.Controls.Add(MakeBtn("Одяг", () => new FormClothes().ShowDialog()));
            panel.Controls.Add(MakeBtn("Взуття", () => new FormShoes().ShowDialog()));
            panel.Controls.Add(MakeBtn("Прикраси", () => new FormJewelry().ShowDialog()));
            panel.Controls.Add(MakeBtn("Відділи", () => new FormDepartments().ShowDialog()));
            panel.Controls.Add(MakeBtn("Керівники магазинів", () => new FormHeads().ShowDialog()));
            panel.Controls.Add(MakeBtn("Працівники", () => new FormWorkers().ShowDialog()));
            panel.Controls.Add(MakeBtn("Постачальники", () => new FormProviders().ShowDialog()));
            panel.Controls.Add(MakeBtn("Постачання", () => new FormSupply().ShowDialog()));
            panel.Controls.Add(MakeBtn("Покупці", () => new FormCustomers().ShowDialog()));
            panel.Controls.Add(MakeBtn("Покупки", () => new FormPurchases().ShowDialog()));
            panel.Controls.Add(SectionLabel(" "));
            panel.Controls.Add(MakeBtn("Запити", () => new FormQueries().ShowDialog()));

            Controls.Add(panel);
            Controls.Add(lblTitle);
        }

        private Button MakeBtn(string text, Action action)
        {
            var btn = new Button
            {
                Text = text,
                Height = 34,
                Dock = DockStyle.Top,
                Margin = new Padding(0, 3, 0, 3)
            };
            btn.Click += (s, e) => action();
            return btn;
        }

        private Label SectionLabel(string text) =>
            new Label
            {
                Text = text,
                AutoSize = false,
                Height = 28,
                Dock = DockStyle.Top,
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
                Font = new System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Bold),
                Margin = new Padding(0, 8, 0, 2)
            };

        private void btnShops_Click(object sender, EventArgs e) => new FormShops().ShowDialog();
        private void btnProducts_Click(object sender, EventArgs e) => new FormProducts().ShowDialog();
        private void btnWorkers_Click(object sender, EventArgs e) => new FormWorkers().ShowDialog();
        private void btnCustomers_Click(object sender, EventArgs e) => new FormCustomers().ShowDialog();
        private void btnProviders_Click(object sender, EventArgs e) => new FormProviders().ShowDialog();
        private void btnSupply_Click(object sender, EventArgs e) => new FormSupply().ShowDialog();
        private void btnPurchases_Click(object sender, EventArgs e) => new FormPurchases().ShowDialog();
    }
}