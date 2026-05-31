using System;
using System.Data;
using System.Windows.Forms;

namespace Shop
{
    public class FormWorkers : Form
    {
        private DataGridView dgv = new DataGridView { Dock = DockStyle.Fill };
        private TextBox txtId = new TextBox { Width = 50 };
        private TextBox txtName = new TextBox { Width = 100 };
        private TextBox txtSurname = new TextBox { Width = 100 };
        private TextBox txtMiddle = new TextBox { Width = 100 };
        private TextBox txtPosition = new TextBox { Width = 100 };
        private TextBox txtSalary = new TextBox { Width = 70 };
        private ComboBox cmbShop = new ComboBox { Width = 130, DropDownStyle = ComboBoxStyle.DropDownList };
        private ComboBox cmbDept = new ComboBox { Width = 130, DropDownStyle = ComboBoxStyle.DropDownList };
        private ComboBox cmbManager = new ComboBox { Width = 200, DropDownStyle = ComboBoxStyle.DropDownList };
        private Button btnSave = new Button { Text = "Зберегти", Width = 110 };
        private Button btnDelete = new Button { Text = "Видалити", Width = 90 };
        private Button btnRefresh = new Button { Text = "Оновити", Width = 90 };
        private Button btnClear = new Button { Text = "Очистити", Width = 90 };

        private int? _originalId = null;

        public FormWorkers()
        {
            Text = "Працівники"; Size = new System.Drawing.Size(1100, 520);
            StartPosition = FormStartPosition.CenterScreen;

            DbHelper.FillCombo(cmbShop, "SELECT name FROM public.shops ORDER BY name", "name");
            cmbShop.SelectedIndexChanged += (s, e) => FillDepts();
            FillDepts();
            FillManagers();

            var pnl = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 100,
                Padding = new Padding(6),
                WrapContents = true
            };
            pnl.Controls.Add(DbHelper.Lbl("ID:")); pnl.Controls.Add(txtId);
            pnl.Controls.Add(DbHelper.Lbl("Прізвище:")); pnl.Controls.Add(txtSurname);
            pnl.Controls.Add(DbHelper.Lbl("Ім'я:")); pnl.Controls.Add(txtName);
            pnl.Controls.Add(DbHelper.Lbl("По батькові:")); pnl.Controls.Add(txtMiddle);
            pnl.Controls.Add(DbHelper.Lbl("Посада:")); pnl.Controls.Add(txtPosition);
            pnl.Controls.Add(DbHelper.Lbl("Зарплата:")); pnl.Controls.Add(txtSalary);
            pnl.Controls.Add(DbHelper.Lbl("Магазин:")); pnl.Controls.Add(cmbShop);
            pnl.Controls.Add(DbHelper.Lbl("Відділ:")); pnl.Controls.Add(cmbDept);
            pnl.Controls.Add(DbHelper.Lbl("Менеджер (лише Manager):")); pnl.Controls.Add(cmbManager);
            pnl.Controls.Add(btnSave);
            pnl.Controls.Add(btnDelete);
            pnl.Controls.Add(btnRefresh);
            pnl.Controls.Add(btnClear);

            Controls.Add(dgv);
            Controls.Add(pnl);
            Controls.Add(DbHelper.MakeBottomPanel(() => Close(), this));

            btnSave.Click += BtnSave_Click;
            btnDelete.Click += BtnDelete_Click;
            btnRefresh.Click += (s, e) => { Load_(); FillManagers(); };
            btnClear.Click += (s, e) => ClearFields();

            dgv.CellClick += (s, e) =>
            {
                if (e.RowIndex < 0 || dgv.Rows[e.RowIndex].DataBoundItem == null) return;
                var row = ((DataRowView)dgv.Rows[e.RowIndex].DataBoundItem).Row;
                _originalId = Convert.ToInt32(row["ID"]);
                txtId.Text = _originalId.ToString();
                txtId.ReadOnly = true;
                txtSurname.Text = row["Прізвище"]?.ToString();
                txtName.Text = row["Ім'я"]?.ToString();
                txtMiddle.Text = row["По батькові"]?.ToString();
                txtPosition.Text = row["Посада"]?.ToString();
                txtSalary.Text = row["Зарплата"]?.ToString();
                string shop = row["Магазин"]?.ToString();
                if (shop != null && cmbShop.Items.Contains(shop)) cmbShop.SelectedItem = shop;
                string dept = row["Відділ"]?.ToString();
                if (dept != null && cmbDept.Items.Contains(dept)) cmbDept.SelectedItem = dept;
                btnSave.Text = "Зберегти зміни";
            };

            Load_();
        }

        void FillDepts()
        {
            if (cmbShop.SelectedItem == null) return;
            string shop = cmbShop.SelectedItem.ToString();
            cmbDept.Items.Clear();
            var dt = DbHelper.Query(
                "SELECT name FROM public.departments WHERE shop=@s ORDER BY name", ("s", shop));
            foreach (DataRow row in dt.Rows) cmbDept.Items.Add(row["name"].ToString());
            if (cmbDept.Items.Count > 0) cmbDept.SelectedIndex = 0;
        }

        void FillManagers()
        {
            cmbManager.Items.Clear();
            cmbManager.Items.Add("(без менеджера)");
            var dt = DbHelper.Query(
                @"SELECT id, surname || ' ' || name AS fn, position
                  FROM public.workers
                  WHERE LOWER(position) = 'manager'
                  ORDER BY surname");
            foreach (DataRow row in dt.Rows)
                cmbManager.Items.Add($"{row["id"]} — {row["fn"]}");
            cmbManager.SelectedIndex = 0;
        }

        void Load_()
        {
            DbHelper.Bind(dgv, DbHelper.Query(
                @"SELECT w.id ""ID"", w.surname ""Прізвище"", w.name ""Ім'я"",
                         w.middle_name ""По батькові"",
                         w.position ""Посада"", w.salary ""Зарплата"",
                         w.department ""Відділ"", w.shop ""Магазин"",
                         m.surname ""Менеджер""
                  FROM public.workers w
                  LEFT JOIN public.workers m ON m.id = w.manager_id
                  ORDER BY w.id"));
        }

        void ClearFields()
        {
            _originalId = null;
            txtId.ReadOnly = false;
            txtId.Clear(); txtName.Clear(); txtSurname.Clear();
            txtMiddle.Clear(); txtPosition.Clear(); txtSalary.Clear();
            btnSave.Text = "Зберегти";
        }

        void BtnSave_Click(object s, EventArgs e)
        {
            if (!int.TryParse(txtId.Text, out int id))
            { MessageBox.Show("ID повинно бути цілим числом.", "Помилка"); return; }
            if (id <= 0)
            { MessageBox.Show("ID повинно бути більшим за 0.", "Помилка"); return; }
            if (string.IsNullOrWhiteSpace(txtSurname.Text))
            { MessageBox.Show("Введіть прізвище.", "Помилка"); return; }
            if (string.IsNullOrWhiteSpace(txtName.Text))
            { MessageBox.Show("Введіть ім'я.", "Помилка"); return; }
            if (string.IsNullOrWhiteSpace(txtPosition.Text))
            { MessageBox.Show("Введіть посаду.", "Помилка"); return; }
            if (cmbShop.SelectedItem == null || cmbDept.SelectedItem == null)
            { MessageBox.Show("Оберіть магазин і відділ.", "Помилка"); return; }

            decimal salary = 0;
            if (!string.IsNullOrWhiteSpace(txtSalary.Text))
            {
                if (!decimal.TryParse(txtSalary.Text.Replace(',', '.'),
                    System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture, out salary) || salary < 0)
                { MessageBox.Show("Зарплата повинна бути невід'ємним числом.", "Помилка"); return; }
            }

            int? managerId = null;
            if (cmbManager.SelectedIndex > 0)
                managerId = int.Parse(cmbManager.SelectedItem.ToString().Split('—')[0].Trim());

            bool ok;
            if (_originalId.HasValue)
            {
                ok = DbHelper.Execute(
                    @"UPDATE public.workers SET name=@nm,surname=@sn,middle_name=@mn,
                        position=@pos,salary=@sal,department=@dep,shop=@shop,manager_id=@mgr
                      WHERE id=@origId",
                    ("nm", txtName.Text.Trim()), ("sn", txtSurname.Text.Trim()),
                    ("mn", txtMiddle.Text.Trim()), ("pos", txtPosition.Text.Trim()),
                    ("sal", salary), ("dep", cmbDept.SelectedItem.ToString()),
                    ("shop", cmbShop.SelectedItem.ToString()),
                    ("mgr", managerId.HasValue ? (object)managerId.Value : DBNull.Value),
                    ("origId", _originalId.Value));
            }
            else
            {
                ok = DbHelper.Execute(
                    @"INSERT INTO public.workers(id,name,surname,middle_name,position,salary,department,shop,manager_id)
                      VALUES(@id,@nm,@sn,@mn,@pos,@sal,@dep,@shop,@mgr)",
                    ("id", id), ("nm", txtName.Text.Trim()), ("sn", txtSurname.Text.Trim()),
                    ("mn", txtMiddle.Text.Trim()), ("pos", txtPosition.Text.Trim()),
                    ("sal", salary), ("dep", cmbDept.SelectedItem.ToString()),
                    ("shop", cmbShop.SelectedItem.ToString()),
                    ("mgr", managerId.HasValue ? (object)managerId.Value : DBNull.Value));
            }
            if (ok) { Load_(); FillManagers(); ClearFields(); }
        }

        void BtnDelete_Click(object s, EventArgs e)
        {
            if (dgv.CurrentRow == null) return;
            int id = Convert.ToInt32(dgv.CurrentRow.Cells[0].Value);
            if (MessageBox.Show($"Видалити працівника #{id}?", "Підтвердження",
                MessageBoxButtons.YesNo) == DialogResult.Yes)
                if (DbHelper.Execute("DELETE FROM public.workers WHERE id=@i", ("i", id)))
                { Load_(); FillManagers(); ClearFields(); }
        }
    }
}