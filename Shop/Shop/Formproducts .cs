using System;
using System.Data;
using System.Windows.Forms;

namespace Shop
{
    public class FormProducts : Form
    {
        private DataGridView dgv = new DataGridView { Dock = DockStyle.Fill };
        private TextBox txtId = new TextBox { Width = 50 };
        private TextBox txtName = new TextBox { Width = 120 };
        private TextBox txtBrand = new TextBox { Width = 100 };
        private TextBox txtSize = new TextBox { Width = 60 };
        private TextBox txtColour = new TextBox { Width = 80 };
        private TextBox txtPrice = new TextBox { Width = 70 };
        private ComboBox cmbOwner = new ComboBox { Width = 140, DropDownStyle = ComboBoxStyle.DropDownList };
        private Button btnSave = new Button { Text = "Зберегти", Width = 110 };
        private Button btnDelete = new Button { Text = "Видалити", Width = 90 };
        private Button btnRefresh = new Button { Text = "Оновити", Width = 90 };
        private Button btnClear = new Button { Text = "Очистити", Width = 90 };

        private int? _originalId = null;

        public FormProducts()
        {
            Text = "Товари"; Size = new System.Drawing.Size(1000, 500);
            StartPosition = FormStartPosition.CenterScreen;

            DbHelper.FillCombo(cmbOwner, "SELECT name FROM public.shops ORDER BY name", "name");

            var pnl = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 90,
                Padding = new Padding(6),
                WrapContents = true
            };
            pnl.Controls.Add(DbHelper.Lbl("ID:")); pnl.Controls.Add(txtId);
            pnl.Controls.Add(DbHelper.Lbl("Назва:")); pnl.Controls.Add(txtName);
            pnl.Controls.Add(DbHelper.Lbl("Бренд:")); pnl.Controls.Add(txtBrand);
            pnl.Controls.Add(DbHelper.Lbl("Розмір:")); pnl.Controls.Add(txtSize);
            pnl.Controls.Add(DbHelper.Lbl("Колір:")); pnl.Controls.Add(txtColour);
            pnl.Controls.Add(DbHelper.Lbl("Ціна:")); pnl.Controls.Add(txtPrice);
            pnl.Controls.Add(DbHelper.Lbl("Магазин:")); pnl.Controls.Add(cmbOwner);
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
                _originalId = Convert.ToInt32(row["ID"]);
                txtId.Text = _originalId.ToString();
                txtId.ReadOnly = true;
                txtName.Text = row["Назва"]?.ToString();
                txtBrand.Text = row["Бренд"]?.ToString();
                txtSize.Text = row["Розмір"]?.ToString();
                txtColour.Text = row["Колір"]?.ToString();
                txtPrice.Text = row["Ціна"]?.ToString();
                string shop = row["Магазин"]?.ToString();
                if (shop != null) cmbOwner.SelectedItem = shop;
                btnSave.Text = "Зберегти зміни";
            };

            Load_();
        }

        void Load_() => DbHelper.Bind(dgv, DbHelper.Query(
            @"SELECT p.id ""ID"", p.name ""Назва"", p.brand ""Бренд"",
                     p.size ""Розмір"", p.colour ""Колір"",
                     p.price ""Ціна"", p.owner ""Магазин""
              FROM public.product p ORDER BY p.id"));

        void ClearFields()
        {
            _originalId = null;
            txtId.ReadOnly = false;
            txtId.Clear(); txtName.Clear(); txtBrand.Clear();
            txtSize.Clear(); txtColour.Clear(); txtPrice.Clear();
            btnSave.Text = "Зберегти";
        }

        void BtnSave_Click(object s, EventArgs e)
        {
            if (!int.TryParse(txtId.Text, out int id) || id <= 0)
            { MessageBox.Show("ID повинно бути цілим числом більшим за 0.", "Помилка"); return; }
            if (string.IsNullOrWhiteSpace(txtName.Text))
            { MessageBox.Show("Введіть назву товару.", "Помилка"); return; }
            if (cmbOwner.SelectedItem == null)
            { MessageBox.Show("Оберіть магазин-власник.", "Помилка"); return; }

            decimal price = 0;
            if (!string.IsNullOrWhiteSpace(txtPrice.Text))
            {
                if (!decimal.TryParse(txtPrice.Text.Replace(',', '.'),
                    System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture, out price) || price < 0)
                { MessageBox.Show("Ціна повинна бути невід'ємним числом.", "Помилка"); return; }
            }

            bool ok;
            if (_originalId.HasValue)
            {
                ok = DbHelper.Execute(
                    @"UPDATE public.product SET name=@nm,brand=@br,size=@sz,colour=@cl,price=@pr,owner=@ow
                      WHERE id=@origId",
                    ("nm", txtName.Text.Trim()), ("br", txtBrand.Text.Trim()),
                    ("sz", txtSize.Text.Trim()), ("cl", txtColour.Text.Trim()),
                    ("pr", price), ("ow", cmbOwner.SelectedItem.ToString()),
                    ("origId", _originalId.Value));
            }
            else
            {
                ok = DbHelper.Execute(
                    @"INSERT INTO public.product(id,name,brand,size,colour,price,owner)
                      VALUES(@id,@nm,@br,@sz,@cl,@pr,@ow)",
                    ("id", id), ("nm", txtName.Text.Trim()), ("br", txtBrand.Text.Trim()),
                    ("sz", txtSize.Text.Trim()), ("cl", txtColour.Text.Trim()),
                    ("pr", price), ("ow", cmbOwner.SelectedItem.ToString()));
            }
            if (ok) { Load_(); ClearFields(); }
        }

        void BtnDelete_Click(object s, EventArgs e)
        {
            if (dgv.CurrentRow == null) return;
            int id = Convert.ToInt32(dgv.CurrentRow.Cells[0].Value);
            if (MessageBox.Show($"Видалити товар #{id}?", "Підтвердження",
                MessageBoxButtons.YesNo) == DialogResult.Yes)
                if (DbHelper.Execute("DELETE FROM public.product WHERE id=@i", ("i", id)))
                { Load_(); ClearFields(); }
        }
    }
}