using System;
using System.Data;
using System.Windows.Forms;

namespace Shop
{
    public class FormCustomers : Form
    {
        private TextBox txtPhone = new TextBox { Width = 100 };
        private TextBox txtName = new TextBox { Width = 100 };
        private TextBox txtSur = new TextBox { Width = 100 };
        private TextBox txtMiddle = new TextBox { Width = 100 };
        private DataGridView dgv = new DataGridView { Dock = DockStyle.Fill };
        private Button btnSave = new Button { Text = "Зберегти", Width = 110 };
        private Button btnDelete = new Button { Text = "Видалити", Width = 90 };
        private Button btnRefresh = new Button { Text = "Оновити", Width = 90 };
        private Button btnClear = new Button { Text = "Очистити", Width = 90 };

        private string _originalPhone = null;

        public FormCustomers()
        {
            Text = "Покупці"; Size = new System.Drawing.Size(740, 500);
            StartPosition = FormStartPosition.CenterScreen;

            var pnl = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 90, Padding = new Padding(8), WrapContents = true };
            pnl.Controls.Add(L("Телефон:")); pnl.Controls.Add(txtPhone);
            pnl.Controls.Add(L("Ім'я:")); pnl.Controls.Add(txtName);
            pnl.Controls.Add(L("Прізвище:")); pnl.Controls.Add(txtSur);
            pnl.Controls.Add(L("По батькові:")); pnl.Controls.Add(txtMiddle);
            pnl.Controls.Add(btnSave); pnl.Controls.Add(btnDelete);
            pnl.Controls.Add(btnRefresh); pnl.Controls.Add(btnClear);

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
                _originalPhone = row["Телефон"]?.ToString();
                txtPhone.Text = _originalPhone;
                txtSur.Text = row["Прізвище"]?.ToString();
                txtName.Text = row["Ім'я"]?.ToString();
                txtMiddle.Text = row["По батькові"]?.ToString();
                btnSave.Text = "Зберегти зміни";
            };

            Load_();
        }

        void Load_() => DbHelper.Bind(dgv, DbHelper.Query(
            @"SELECT phone AS ""Телефон"", surname AS ""Прізвище"", name AS ""Ім'я"",
                     middle_name AS ""По батькові"" FROM public.customer ORDER BY surname"));

        void ClearFields()
        {
            _originalPhone = null;
            txtPhone.Clear(); txtName.Clear(); txtSur.Clear(); txtMiddle.Clear();
            btnSave.Text = "Зберегти";
        }

        void BtnSave_Click(object s, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtPhone.Text)) { MessageBox.Show("Введіть телефон."); return; }
            bool ok;
            if (_originalPhone != null)
                ok = DbHelper.Execute(
                    "UPDATE public.customer SET phone=@newPh,name=@nm,surname=@sn,middle_name=@mn WHERE phone=@origPh",
                    ("newPh", txtPhone.Text.Trim()), ("nm", txtName.Text.Trim()),
                    ("sn", txtSur.Text.Trim()), ("mn", txtMiddle.Text.Trim()),
                    ("origPh", _originalPhone));
            else
                ok = DbHelper.Execute(
                    "INSERT INTO public.customer(phone,name,surname,middle_name) VALUES(@ph,@nm,@sn,@mn)",
                    ("ph", txtPhone.Text.Trim()), ("nm", txtName.Text.Trim()),
                    ("sn", txtSur.Text.Trim()), ("mn", txtMiddle.Text.Trim()));
            if (ok) { Load_(); ClearFields(); }
        }

        void BtnDelete_Click(object s, EventArgs e)
        {
            if (dgv.CurrentRow == null) return;
            string ph = dgv.CurrentRow.Cells[0].Value.ToString();
            if (MessageBox.Show($"Видалити покупця {ph}?", "Підтвердження", MessageBoxButtons.YesNo) == DialogResult.Yes)
                if (DbHelper.Execute("DELETE FROM public.customer WHERE phone=@p", ("p", ph)))
                { Load_(); ClearFields(); }
        }

        static Label L(string t) => new Label { Text = t, AutoSize = true, TextAlign = System.Drawing.ContentAlignment.MiddleLeft };
    }

    public class FormProviders : Form
    {
        private TextBox txtName = new TextBox { Width = 140 };
        private TextBox txtAddr = new TextBox { Width = 220 };
        private DataGridView dgv = new DataGridView { Dock = DockStyle.Fill };
        private Button btnSave = new Button { Text = "Зберегти", Width = 110 };
        private Button btnDelete = new Button { Text = "Видалити", Width = 90 };
        private Button btnRefresh = new Button { Text = "Оновити", Width = 90 };
        private Button btnClear = new Button { Text = "Очистити", Width = 90 };

        private string _originalName = null;

        public FormProviders()
        {
            Text = "Постачальники"; Size = new System.Drawing.Size(680, 460);
            StartPosition = FormStartPosition.CenterScreen;

            var pnl = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 90, Padding = new Padding(8), WrapContents = true };
            pnl.Controls.Add(L("Назва:")); pnl.Controls.Add(txtName);
            pnl.Controls.Add(L("Адреса:")); pnl.Controls.Add(txtAddr);
            pnl.Controls.Add(btnSave); pnl.Controls.Add(btnDelete);
            pnl.Controls.Add(btnRefresh); pnl.Controls.Add(btnClear);

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
                btnSave.Text = "Зберегти зміни";
            };

            Load_();
        }

        void Load_() => DbHelper.Bind(dgv, DbHelper.Query(
            @"SELECT name AS ""Назва"", address AS ""Адреса"" FROM public.provider ORDER BY name"));

        void ClearFields()
        {
            _originalName = null;
            txtName.Clear(); txtAddr.Clear();
            btnSave.Text = "Зберегти";
        }

        void BtnSave_Click(object s, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text)) { MessageBox.Show("Введіть назву."); return; }
            bool ok;
            if (_originalName != null)
                ok = DbHelper.Execute(
                    "UPDATE public.provider SET name=@newN, address=@a WHERE name=@origN",
                    ("newN", txtName.Text.Trim()), ("a", txtAddr.Text.Trim()), ("origN", _originalName));
            else
                ok = DbHelper.Execute(
                    "INSERT INTO public.provider(name,address) VALUES(@n,@a)",
                    ("n", txtName.Text.Trim()), ("a", txtAddr.Text.Trim()));
            if (ok) { Load_(); ClearFields(); }
        }

        void BtnDelete_Click(object s, EventArgs e)
        {
            if (dgv.CurrentRow == null) return;
            string n = dgv.CurrentRow.Cells[0].Value.ToString();
            if (MessageBox.Show($"Видалити постачальника «{n}»?", "Підтвердження", MessageBoxButtons.YesNo) == DialogResult.Yes)
                if (DbHelper.Execute("DELETE FROM public.provider WHERE name=@n", ("n", n)))
                { Load_(); ClearFields(); }
        }

        static Label L(string t) => new Label { Text = t, AutoSize = true, TextAlign = System.Drawing.ContentAlignment.MiddleLeft };
    }

    public class FormSupply : Form
    {
        private ComboBox cmbProvider = new ComboBox { Width = 140, DropDownStyle = ComboBoxStyle.DropDownList };
        private ComboBox cmbProduct = new ComboBox { Width = 160, DropDownStyle = ComboBoxStyle.DropDownList };
        private ComboBox cmbShop = new ComboBox { Width = 130, DropDownStyle = ComboBoxStyle.DropDownList };
        private TextBox txtAmount = new TextBox { Width = 60 };
        private DataGridView dgv = new DataGridView { Dock = DockStyle.Fill };
        private Button btnSave = new Button { Text = "Зберегти", Width = 110 };
        private Button btnDelete = new Button { Text = "Видалити", Width = 90 };
        private Button btnRefresh = new Button { Text = "Оновити", Width = 90 };

        private int? _selProductId = null;
        private string _selProvider = null;
        private string _selShop = null;

        public FormSupply()
        {
            Text = "Постачання"; Size = new System.Drawing.Size(860, 500);
            StartPosition = FormStartPosition.CenterScreen;

            DbHelper.FillCombo(cmbProvider, "SELECT name FROM public.provider ORDER BY name", "name");
            DbHelper.FillCombo(cmbShop, "SELECT name FROM public.shops ORDER BY name", "name");
            FillProducts();

            var pnl = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 70, Padding = new Padding(8), WrapContents = true };
            pnl.Controls.Add(L("Постачальник:")); pnl.Controls.Add(cmbProvider);
            pnl.Controls.Add(L("Товар:")); pnl.Controls.Add(cmbProduct);
            pnl.Controls.Add(L("Магазин:")); pnl.Controls.Add(cmbShop);
            pnl.Controls.Add(L("Кількість:")); pnl.Controls.Add(txtAmount);
            pnl.Controls.Add(btnSave); pnl.Controls.Add(btnDelete); pnl.Controls.Add(btnRefresh);

            Controls.Add(dgv);
            Controls.Add(pnl);
            Controls.Add(DbHelper.MakeBottomPanel(() => Close(), this));

            btnSave.Click += BtnSave_Click;
            btnDelete.Click += BtnDelete_Click;
            btnRefresh.Click += (s, e) => Load_();

            dgv.CellClick += (s, e) =>
            {
                if (e.RowIndex < 0 || dgv.Rows[e.RowIndex].DataBoundItem == null) return;
                var row = ((DataRowView)dgv.Rows[e.RowIndex].DataBoundItem).Row;
                _selProvider = row["Постачальник"]?.ToString();
                _selProductId = Convert.ToInt32(row["ProductId"]);
                _selShop = row["Магазин"]?.ToString();
                txtAmount.Text = row["Кількість"]?.ToString();
                if (cmbProvider.Items.Contains(_selProvider)) cmbProvider.SelectedItem = _selProvider;
                if (cmbShop.Items.Contains(_selShop)) cmbShop.SelectedItem = _selShop;
            };

            Load_();
        }

        void FillProducts()
        {
            cmbProduct.Items.Clear();
            var dt = DbHelper.Query("SELECT id, name FROM public.product ORDER BY id");
            foreach (DataRow r in dt.Rows)
                cmbProduct.Items.Add($"{r["id"]} — {r["name"]}");
            if (cmbProduct.Items.Count > 0) cmbProduct.SelectedIndex = 0;
        }

        void Load_() => DbHelper.Bind(dgv, DbHelper.Query(
            @"SELECT s.provider AS ""Постачальник"", p.name AS ""Товар"",
                     s.product AS ""ProductId"",
                     s.shop AS ""Магазин"", s.amount AS ""Кількість""
              FROM public.supply s
              JOIN public.product p ON p.id = s.product
              ORDER BY s.provider"));

        void BtnSave_Click(object s, EventArgs e)
        {
            if (cmbProvider.SelectedItem == null || cmbProduct.SelectedItem == null || cmbShop.SelectedItem == null)
            { MessageBox.Show("Заповніть всі поля."); return; }
            if (!int.TryParse(txtAmount.Text, out int amount)) { MessageBox.Show("Кількість — ціле число."); return; }
            int productId = int.Parse(cmbProduct.SelectedItem.ToString().Split('—')[0].Trim());
            if (DbHelper.Execute(
                @"INSERT INTO public.supply(provider,product,shop,amount) VALUES(@prov,@prod,@shop,@amt)
                  ON CONFLICT(provider,product,shop) DO UPDATE SET amount=@amt",
                ("prov", cmbProvider.SelectedItem.ToString()),
                ("prod", productId),
                ("shop", cmbShop.SelectedItem.ToString()),
                ("amt", amount)))
                Load_();
        }

        void BtnDelete_Click(object s, EventArgs e)
        {
            if (_selProductId == null || _selProvider == null || _selShop == null)
            { MessageBox.Show("Спочатку оберіть рядок у таблиці."); return; }
            if (MessageBox.Show("Видалити запис постачання?", "Підтвердження", MessageBoxButtons.YesNo) == DialogResult.Yes)
                if (DbHelper.Execute(
                    "DELETE FROM public.supply WHERE provider=@prov AND product=@prod AND shop=@shop",
                    ("prov", _selProvider), ("prod", _selProductId.Value), ("shop", _selShop)))
                {
                    _selProductId = null; _selProvider = null; _selShop = null;
                    Load_();
                }
        }

        static Label L(string t) => new Label { Text = t, AutoSize = true, TextAlign = System.Drawing.ContentAlignment.MiddleLeft };
    }

    public class FormPurchases : Form
    {
        private ComboBox cmbProduct = new ComboBox { Width = 200, DropDownStyle = ComboBoxStyle.DropDownList };
        private ComboBox cmbCustomer = new ComboBox { Width = 180, DropDownStyle = ComboBoxStyle.DropDownList };
        private DateTimePicker dtpDate = new DateTimePicker { Width = 160, Format = DateTimePickerFormat.Short };
        private TextBox txtTotal = new TextBox { Width = 80 };
        private DataGridView dgv = new DataGridView { Dock = DockStyle.Fill };
        private Button btnSave = new Button { Text = "Зберегти", Width = 110 };
        private Button btnDelete = new Button { Text = "Видалити", Width = 90 };
        private Button btnRefresh = new Button { Text = "Оновити", Width = 90 };

        private int? _selProductId = null;
        private string _selPhone = null;
        private DateTime? _selDate = null;

        public FormPurchases()
        {
            Text = "Покупки"; Size = new System.Drawing.Size(960, 500);
            StartPosition = FormStartPosition.CenterScreen;

            FillProducts(); FillCustomers();

            var pnl = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 70, Padding = new Padding(8), WrapContents = true };
            pnl.Controls.Add(L("Товар:")); pnl.Controls.Add(cmbProduct);
            pnl.Controls.Add(L("Покупець:")); pnl.Controls.Add(cmbCustomer);
            pnl.Controls.Add(L("Дата:")); pnl.Controls.Add(dtpDate);
            pnl.Controls.Add(L("Сума:")); pnl.Controls.Add(txtTotal);
            pnl.Controls.Add(btnSave); pnl.Controls.Add(btnDelete); pnl.Controls.Add(btnRefresh);

            Controls.Add(dgv);
            Controls.Add(pnl);
            Controls.Add(DbHelper.MakeBottomPanel(() => Close(), this));

            btnSave.Click += BtnSave_Click;
            btnDelete.Click += BtnDelete_Click;
            btnRefresh.Click += (s, e) => Load_();

            dgv.CellClick += (s, e) =>
            {
                if (e.RowIndex < 0 || dgv.Rows[e.RowIndex].DataBoundItem == null) return;
                var row = ((DataRowView)dgv.Rows[e.RowIndex].DataBoundItem).Row;
                _selProductId = Convert.ToInt32(row["ProductId"]);
                _selPhone = row["Phone"]?.ToString();
                _selDate = Convert.ToDateTime(row["Дата"]);
                txtTotal.Text = row["Сума"]?.ToString();
                dtpDate.Value = _selDate.Value.Date;
            };

            Load_();
        }

        void FillProducts()
        {
            cmbProduct.Items.Clear();
            var dt = DbHelper.Query("SELECT id, name FROM public.product ORDER BY id");
            foreach (DataRow r in dt.Rows) cmbProduct.Items.Add($"{r["id"]} — {r["name"]}");
            if (cmbProduct.Items.Count > 0) cmbProduct.SelectedIndex = 0;
        }

        void FillCustomers()
        {
            cmbCustomer.Items.Clear();
            var dt = DbHelper.Query("SELECT phone, surname||' '||name AS fn FROM public.customer ORDER BY surname");
            foreach (DataRow r in dt.Rows) cmbCustomer.Items.Add($"{r["phone"]} — {r["fn"]}");
            if (cmbCustomer.Items.Count > 0) cmbCustomer.SelectedIndex = 0;
        }

        void Load_() => DbHelper.Bind(dgv, DbHelper.Query(
            @"SELECT pu.purchase_date AS ""Дата"",
                     p.name           AS ""Товар"",
                     pu.product       AS ""ProductId"",
                     pu.customer      AS ""Phone"",
                     c.surname||' '||c.name AS ""Покупець"",
                     pu.total         AS ""Сума""
              FROM public.purchase pu
              JOIN public.product p  ON p.id    = pu.product
              JOIN public.customer c ON c.phone = pu.customer
              ORDER BY pu.purchase_date DESC"));

        void BtnSave_Click(object s, EventArgs e)
        {
            if (cmbProduct.SelectedItem == null || cmbCustomer.SelectedItem == null)
            { MessageBox.Show("Оберіть товар і покупця."); return; }

            int productId = int.Parse(cmbProduct.SelectedItem.ToString().Split('—')[0].Trim());
            string phone = cmbCustomer.SelectedItem.ToString().Split('—')[0].Trim();

            decimal total = 0;
            decimal.TryParse(txtTotal.Text.Replace(',', '.'),
                System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out total);

            bool ok;
            if (_selProductId.HasValue && _selPhone != null && _selDate.HasValue)
            {
                ok = DbHelper.Execute(
                    "UPDATE public.purchase SET total=@tot WHERE product=@prod AND customer=@cust AND purchase_date=@dt",
                    ("tot", total), ("prod", _selProductId.Value), ("cust", _selPhone), ("dt", _selDate.Value));
            }
            else
            {
                ok = DbHelper.Execute(
                    @"INSERT INTO public.purchase(product,customer,purchase_date,total)
                      VALUES(@prod,@cust,@dt,@tot) ON CONFLICT DO NOTHING",
                    ("prod", productId), ("cust", phone),
                    ("dt", dtpDate.Value.Date), ("tot", total));
            }
            if (ok) { _selProductId = null; _selPhone = null; _selDate = null; Load_(); }
        }

        void BtnDelete_Click(object s, EventArgs e)
        {
            if (_selProductId == null || _selPhone == null || _selDate == null)
            { MessageBox.Show("Спочатку клікніть на рядок у таблиці."); return; }

            if (MessageBox.Show("Видалити запис покупки?", "Підтвердження", MessageBoxButtons.YesNo) == DialogResult.Yes)
                if (DbHelper.Execute(
                    "DELETE FROM public.purchase WHERE product=@prod AND customer=@cust AND purchase_date=@dt",
                    ("prod", _selProductId.Value), ("cust", _selPhone), ("dt", _selDate.Value)))
                {
                    _selProductId = null; _selPhone = null; _selDate = null;
                    Load_();
                }
        }

        static Label L(string t) => new Label { Text = t, AutoSize = true, TextAlign = System.Drawing.ContentAlignment.MiddleLeft };
    }
}