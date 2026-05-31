using System;
using System.Data;
using System.Windows.Forms;

namespace Shop
{
    public class FormDepartments : Form
    {
        private DataGridView dgvDept = new DataGridView();
        private DataGridView dgvWorkers = new DataGridView();

        private TextBox txtName = new TextBox { Width = 130 };
        private TextBox txtWorkerNum = new TextBox { Width = 60 };
        private ComboBox cmbShop = new ComboBox { Width = 140, DropDownStyle = ComboBoxStyle.DropDownList };
        private Button btnSave = new Button { Text = "Зберегти", Width = 110 };
        private Button btnDelete = new Button { Text = "Видалити", Width = 90 };
        private Button btnRefresh = new Button { Text = "Оновити", Width = 90 };
        private Button btnClear = new Button { Text = "Очистити", Width = 90 };

        private string _originalName = null;
        private string _originalShop = null;

        private string _selDept = null;
        private string _selShop = null;

        public FormDepartments()
        {
            Text = "Відділи (з працівниками)";
            Size = new System.Drawing.Size(980, 660);
            StartPosition = FormStartPosition.CenterScreen;

            DbHelper.FillCombo(cmbShop, "SELECT name FROM public.shops ORDER BY name", "name");

            var pnlInput = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 60,
                Padding = new Padding(6),
                WrapContents = true
            };
            pnlInput.Controls.Add(DbHelper.Lbl("Назва відділу:")); pnlInput.Controls.Add(txtName);
            pnlInput.Controls.Add(DbHelper.Lbl("К-сть прац.:")); pnlInput.Controls.Add(txtWorkerNum);
            pnlInput.Controls.Add(DbHelper.Lbl("Магазин:")); pnlInput.Controls.Add(cmbShop);
            pnlInput.Controls.Add(btnSave);
            pnlInput.Controls.Add(btnDelete);
            pnlInput.Controls.Add(btnRefresh);
            pnlInput.Controls.Add(btnClear);

            dgvDept.Dock = DockStyle.None;
            dgvDept.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            dgvDept.Location = new System.Drawing.Point(0, 0);
            dgvDept.Height = 200;

            var lblSub = new Label
            {
                Text = "Працівники обраного відділу",
                Dock = DockStyle.None,
                Font = new System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Bold),
                ForeColor = System.Drawing.Color.DarkSlateBlue,
                Height = 22,
                TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            };

            dgvWorkers.Dock = DockStyle.None;
            dgvWorkers.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            dgvWorkers.Height = 200;

            var split = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Horizontal,
                SplitterDistance = 230
            };

            split.Panel1.Controls.Add(dgvDept);
            dgvDept.Dock = DockStyle.Fill;

            split.Panel2.Controls.Add(dgvWorkers);
            split.Panel2.Controls.Add(lblSub);
            dgvWorkers.Dock = DockStyle.Fill;
            lblSub.Dock = DockStyle.Top;

            Controls.Add(split);
            Controls.Add(pnlInput);
            Controls.Add(DbHelper.MakeBottomPanel(() => Close(), this));

            btnSave.Click += BtnSave_Click;
            btnDelete.Click += BtnDelete_Click;
            btnRefresh.Click += (s, e) => { LoadDepts(); ClearWorkers(); };
            btnClear.Click += (s, e) => ClearFields();

            dgvDept.CellClick += (s, e) =>
            {
                if (e.RowIndex < 0 || dgvDept.Rows[e.RowIndex].DataBoundItem == null) return;
                var row = ((DataRowView)dgvDept.Rows[e.RowIndex].DataBoundItem).Row;
                _originalName = row["Відділ"]?.ToString();
                _originalShop = row["Магазин"]?.ToString();
                _selDept = _originalName;
                _selShop = _originalShop;
                txtName.Text = _originalName;
                txtWorkerNum.Text = row["К-сть прац."]?.ToString();
                string shop = _originalShop;
                if (shop != null && cmbShop.Items.Contains(shop)) cmbShop.SelectedItem = shop;
                btnSave.Text = "Зберегти зміни";

                LoadWorkers(_selDept, _selShop);
            };

            SetupDgv(dgvDept);
            SetupDgv(dgvWorkers);
            LoadDepts();
            ClearWorkers();
        }

        static void SetupDgv(DataGridView dgv)
        {
            dgv.ReadOnly = true;
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
        }

        void LoadDepts() => DbHelper.Bind(dgvDept, DbHelper.Query(
            @"SELECT d.name ""Відділ"", d.workers_num ""К-сть прац."", d.shop ""Магазин""
              FROM public.departments d ORDER BY d.shop, d.name"));

        void LoadWorkers(string dept, string shop)
        {
            if (dept == null || shop == null) { ClearWorkers(); return; }
            DbHelper.Bind(dgvWorkers, DbHelper.Query(
                @"SELECT w.id ""ID"", w.surname ""Прізвище"", w.name ""Ім'я"",
                         w.position ""Посада"", w.salary ""Зарплата"",
                         m.surname ""Менеджер""
                  FROM public.workers w
                  LEFT JOIN public.workers m ON m.id = w.manager_id
                  WHERE w.department = @d AND w.shop = @s
                  ORDER BY w.surname",
                ("d", dept), ("s", shop)));
        }

        void ClearWorkers() => DbHelper.Bind(dgvWorkers, new DataTable());

        void ClearFields()
        {
            _originalName = null; _originalShop = null;
            txtName.Clear(); txtWorkerNum.Clear();
            btnSave.Text = "Зберегти";
        }

        void BtnSave_Click(object s, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            { MessageBox.Show("Введіть назву відділу.", "Помилка"); return; }
            if (cmbShop.SelectedItem == null)
            { MessageBox.Show("Оберіть магазин.", "Помилка"); return; }
            int wn = 0;
            if (!string.IsNullOrWhiteSpace(txtWorkerNum.Text) && (!int.TryParse(txtWorkerNum.Text, out wn) || wn < 0))
            { MessageBox.Show("К-сть працівників повинна бути невід'ємним числом.", "Помилка"); return; }

            bool ok;
            if (_originalName != null && _originalShop != null)
                ok = DbHelper.Execute(
                    "UPDATE public.departments SET name=@newN, workers_num=@w WHERE name=@origN AND shop=@origS",
                    ("newN", txtName.Text.Trim()), ("w", wn),
                    ("origN", _originalName), ("origS", _originalShop));
            else
                ok = DbHelper.Execute(
                    "INSERT INTO public.departments(name,workers_num,shop) VALUES(@n,@w,@s) ON CONFLICT(name,shop) DO UPDATE SET workers_num=@w",
                    ("n", txtName.Text.Trim()), ("w", wn), ("s", cmbShop.SelectedItem.ToString()));

            if (ok) { LoadDepts(); ClearFields(); ClearWorkers(); }
        }

        void BtnDelete_Click(object s, EventArgs e)
        {
            if (dgvDept.CurrentRow == null) return;
            string name = dgvDept.CurrentRow.Cells[0].Value?.ToString();
            string shop = dgvDept.CurrentRow.Cells[2].Value?.ToString();
            if (MessageBox.Show($"Видалити відділ «{name}» (магазин {shop})?", "Підтвердження",
                MessageBoxButtons.YesNo) == DialogResult.Yes)
                if (DbHelper.Execute("DELETE FROM public.departments WHERE name=@n AND shop=@s",
                    ("n", name), ("s", shop)))
                { LoadDepts(); ClearFields(); ClearWorkers(); }
        }
    }
    public class FormHeads : Form
    {
        private DataGridView dgv = new DataGridView { Dock = DockStyle.Fill };
        private TextBox txtId = new TextBox { Width = 50 };
        private TextBox txtName = new TextBox { Width = 100 };
        private TextBox txtSurname = new TextBox { Width = 100 };
        private TextBox txtMiddle = new TextBox { Width = 100 };
        private ComboBox cmbShop = new ComboBox { Width = 140, DropDownStyle = ComboBoxStyle.DropDownList };
        private Button btnSave = new Button { Text = "Зберегти", Width = 110 };
        private Button btnDelete = new Button { Text = "Видалити", Width = 90 };
        private Button btnRefresh = new Button { Text = "Оновити", Width = 90 };
        private Button btnClear = new Button { Text = "Очистити", Width = 90 };

        private int? _originalId = null;

        public FormHeads()
        {
            Text = "Керівники магазинів"; Size = new System.Drawing.Size(900, 460);
            StartPosition = FormStartPosition.CenterScreen;

            DbHelper.FillCombo(cmbShop, "SELECT name FROM public.shops ORDER BY name", "name");

            var pnl = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 90,
                Padding = new Padding(6),
                WrapContents = true
            };
            pnl.Controls.Add(DbHelper.Lbl("ID:")); pnl.Controls.Add(txtId);
            pnl.Controls.Add(DbHelper.Lbl("Прізвище:")); pnl.Controls.Add(txtSurname);
            pnl.Controls.Add(DbHelper.Lbl("Ім'я:")); pnl.Controls.Add(txtName);
            pnl.Controls.Add(DbHelper.Lbl("По батькові:")); pnl.Controls.Add(txtMiddle);
            pnl.Controls.Add(DbHelper.Lbl("Магазин:")); pnl.Controls.Add(cmbShop);
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
                txtSurname.Text = row["Прізвище"]?.ToString();
                txtName.Text = row["Ім'я"]?.ToString();
                txtMiddle.Text = row["По батькові"]?.ToString();
                string shop = row["Магазин"]?.ToString();
                if (shop != null && cmbShop.Items.Contains(shop)) cmbShop.SelectedItem = shop;
                btnSave.Text = "Зберегти зміни";
            };

            Load_();
        }

        void Load_() => DbHelper.Bind(dgv, DbHelper.Query(
            @"SELECT h.id ""ID"", h.surname ""Прізвище"", h.name ""Ім'я"",
                     h.middle_name ""По батькові"", h.shop ""Магазин""
              FROM public.heads h ORDER BY h.id"));

        void ClearFields()
        {
            _originalId = null; txtId.ReadOnly = false;
            txtId.Clear(); txtName.Clear(); txtSurname.Clear(); txtMiddle.Clear();
            btnSave.Text = "Зберегти";
        }

        void BtnSave_Click(object s, EventArgs e)
        {
            if (!int.TryParse(txtId.Text, out int id) || id <= 0)
            { MessageBox.Show("ID — ціле число > 0.", "Помилка"); return; }
            if (string.IsNullOrWhiteSpace(txtSurname.Text))
            { MessageBox.Show("Введіть прізвище.", "Помилка"); return; }
            if (cmbShop.SelectedItem == null)
            { MessageBox.Show("Оберіть магазин.", "Помилка"); return; }

            bool ok;
            if (_originalId.HasValue)
                ok = DbHelper.Execute(
                    "UPDATE public.heads SET name=@nm,surname=@sn,middle_name=@mn,shop=@sh WHERE id=@origId",
                    ("nm", txtName.Text.Trim()), ("sn", txtSurname.Text.Trim()),
                    ("mn", txtMiddle.Text.Trim()), ("sh", cmbShop.SelectedItem.ToString()),
                    ("origId", _originalId.Value));
            else
                ok = DbHelper.Execute(
                    "INSERT INTO public.heads(id,name,surname,middle_name,shop) VALUES(@id,@nm,@sn,@mn,@sh)",
                    ("id", id), ("nm", txtName.Text.Trim()), ("sn", txtSurname.Text.Trim()),
                    ("mn", txtMiddle.Text.Trim()), ("sh", cmbShop.SelectedItem.ToString()));

            if (ok) { Load_(); ClearFields(); }
        }

        void BtnDelete_Click(object s, EventArgs e)
        {
            if (dgv.CurrentRow == null) return;
            int id = Convert.ToInt32(dgv.CurrentRow.Cells[0].Value);
            if (MessageBox.Show($"Видалити керівника #{id}?", "Підтвердження",
                MessageBoxButtons.YesNo) == DialogResult.Yes)
                if (DbHelper.Execute("DELETE FROM public.heads WHERE id=@i", ("i", id)))
                    Load_();
        }
    }
}