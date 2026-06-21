using System;
using System.Data;
using System.Windows.Forms;

namespace Shop
{
    public class FormShops : Form
    {
        private DataGridView dgv = new DataGridView { Dock = DockStyle.Fill };
        private TextBox txtName = new TextBox { Width = 150 };
        private TextBox txtAddr = new TextBox { Width = 200 };
        private TextBox txtCont = new TextBox { Width = 200 };
        private Button btnSave = new Button { Text = "Зберегти", Width = 110 };
        private Button btnDelete = new Button { Text = "Видалити", Width = 90 };
        private Button btnRefresh = new Button { Text = "Оновити", Width = 90 };
        private Button btnClear = new Button { Text = "Очистити", Width = 90 };

        private string _originalName = null;

        public FormShops()
        {
            Text = "Магазини"; Size = new System.Drawing.Size(820, 500);
            StartPosition = FormStartPosition.CenterScreen;

            var pnl = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 90,
                Padding = new Padding(6),
                WrapContents = true
            };
            pnl.Controls.Add(DbHelper.Lbl("Назва:")); pnl.Controls.Add(txtName);
            pnl.Controls.Add(DbHelper.Lbl("Адреса:")); pnl.Controls.Add(txtAddr);
            pnl.Controls.Add(DbHelper.Lbl("Контакти:")); pnl.Controls.Add(txtCont);
            pnl.Controls.Add(btnSave);
            pnl.Controls.Add(btnDelete);
            pnl.Controls.Add(btnRefresh);
            pnl.Controls.Add(btnClear);

            Controls.Add(dgv);
            Controls.Add(pnl);
            Controls.Add(DbHelper.MakeBottomPanel(() => Close(), this));

            btnSave.Click += BtnSave_Click;
            btnDelete.Click += BtnDelete_Click;
            btnRefresh.Click += (s, e) => Load_();
            btnClear.Click += (s, e) => ClearFields();

            dgv.CellClick += (s, e) =>
            {
                if (e.RowIndex < 0 || dgv.Rows[e.RowIndex].DataBoundItem == null) return;
                var row = ((DataRowView)dgv.Rows[e.RowIndex].DataBoundItem).Row;
                _originalName = row["Назва"]?.ToString();
                txtName.Text = _originalName;
                txtAddr.Text = row["Адреса"]?.ToString();
                txtCont.Text = row["Контакти"]?.ToString();
                btnSave.Text = "Зберегти зміни";
            };

            Load_();
        }

        void Load_() => DbHelper.Bind(dgv, DbHelper.Query(
            "SELECT name \"Назва\", address \"Адреса\", contacts \"Контакти\" FROM public.shops ORDER BY name"));

        void ClearFields()
        {
            _originalName = null;
            txtName.Clear(); txtAddr.Clear(); txtCont.Clear();
            btnSave.Text = "💾 Зберегти";
        }

        void BtnSave_Click(object s, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            { MessageBox.Show("Введіть назву магазину.", "Помилка"); return; }
            if (txtName.Text.Trim().Length > 20)
            { MessageBox.Show("Назва не може бути довшою за 20 символів.", "Помилка"); return; }
            if (txtAddr.Text.Trim().Length > 50)
            { MessageBox.Show("Адреса не може бути довшою за 50 символів.", "Помилка"); return; }
            if (txtCont.Text.Trim().Length > 50)
            { MessageBox.Show("Контакти не можуть бути довшими за 50 символів.", "Помилка"); return; }

            bool ok;
            if (_originalName != null)
            {
                string newName = txtName.Text.Trim();
                string newAddr = txtAddr.Text.Trim();
                string newCont = txtCont.Text.Trim();

                if (newName == _originalName)
                {
                    ok = DbHelper.Execute(
                        "UPDATE public.shops SET address=@a, contacts=@c WHERE name=@n",
                        ("a", newAddr), ("c", newCont), ("n", _originalName));
                }
                else
                {
                    ok = DbHelper.ExecuteTransaction(new[]
                    {
                        ("SET CONSTRAINTS ALL DEFERRED",
                         new (string,object)[0]),
                        ("INSERT INTO public.shops(name,address,contacts) VALUES(@newN,@a,@c)",
                         new[]{("newN",(object)newName),("a",(object)newAddr),("c",(object)newCont)}),
                        ("UPDATE public.departments SET shop=@newN WHERE shop=@origN",
                         new[]{("newN",(object)newName),("origN",(object)_originalName)}),
                        ("UPDATE public.workers    SET shop=@newN WHERE shop=@origN",
                         new[]{("newN",(object)newName),("origN",(object)_originalName)}),
                        ("UPDATE public.heads      SET shop=@newN WHERE shop=@origN",
                         new[]{("newN",(object)newName),("origN",(object)_originalName)}),
                        ("UPDATE public.product    SET owner=@newN WHERE owner=@origN",
                         new[]{("newN",(object)newName),("origN",(object)_originalName)}),
                        ("UPDATE public.supply     SET shop=@newN WHERE shop=@origN",
                         new[]{("newN",(object)newName),("origN",(object)_originalName)}),
                        ("DELETE FROM public.shops WHERE name=@origN",
                         new[]{("origN",(object)_originalName)}),
                    });
                }
            }
            else
            {
                ok = DbHelper.Execute(
                    "INSERT INTO public.shops(name,address,contacts) VALUES(@n,@a,@c)",
                    ("n", txtName.Text.Trim()), ("a", txtAddr.Text.Trim()), ("c", txtCont.Text.Trim()));
            }
            if (ok) { Load_(); ClearFields(); }
        }
     
        void BtnDelete_Click(object s, EventArgs e)
        {
            if (dgv.CurrentRow == null) return;
            string name = dgv.CurrentRow.Cells[0].Value?.ToString();
            if (MessageBox.Show($"Видалити магазин «{name}»?\nВсі пов'язані відділи, працівники та керівники теж будуть видалені!",
                "Підтвердження", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                if (DbHelper.Execute("DELETE FROM public.shops WHERE name=@n", ("n", name)))
                { Load_(); ClearFields(); }
        }
    }
}