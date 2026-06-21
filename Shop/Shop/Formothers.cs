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
        private Button btnAdd = new Button { Text = "Додати", Width = 90 };
        private Button btnDelete = new Button { Text = "Видалити", Width = 90 };

        public FormCustomers()
        {
            Text = "Покупці"; Size = new System.Drawing.Size(700, 480);
            StartPosition = FormStartPosition.CenterScreen;

            var pnl = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 70, Padding = new Padding(8), WrapContents = true };
            pnl.Controls.Add(L("Телефон:")); pnl.Controls.Add(txtPhone);
            pnl.Controls.Add(L("Ім'я:")); pnl.Controls.Add(txtName);
            pnl.Controls.Add(L("Прізвище:")); pnl.Controls.Add(txtSur);
            pnl.Controls.Add(L("По батькові:")); pnl.Controls.Add(txtMiddle);
            pnl.Controls.Add(btnAdd); pnl.Controls.Add(btnDelete);

            var back = DbHelper.MakeBackButton(); back.Click += (s, e) => Close();
            Controls.Add(dgv); Controls.Add(pnl); Controls.Add(back);
            btnAdd.Click += BtnAdd_Click;
            btnDelete.Click += BtnDelete_Click;
            Load_();
        }

        void Load_()
        {
            DbHelper.Bind(dgv, DbHelper.Query(
                @"SELECT phone AS ""Телефон"", surname AS ""Прізвище"", name AS ""Ім'я"",
                         middle_name AS ""По батькові"" FROM public.customer ORDER BY surname"));
        }

        void BtnAdd_Click(object s, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtPhone.Text)) { MessageBox.Show("Введіть телефон."); return; }
            if (DbHelper.Execute(
                @"INSERT INTO public.customer(phone,name,surname,middle_name) VALUES(@ph,@nm,@sn,@mn)
                  ON CONFLICT(phone) DO UPDATE SET name=@nm,surname=@sn,middle_name=@mn",
                ("ph", txtPhone.Text.Trim()), ("nm", txtName.Text.Trim()),
                ("sn", txtSur.Text.Trim()), ("mn", txtMiddle.Text.Trim())))
            { Load_(); txtPhone.Clear(); txtName.Clear(); txtSur.Clear(); txtMiddle.Clear(); }
        }

        void BtnDelete_Click(object s, EventArgs e)
        {
            if (dgv.CurrentRow == null) return;
            string ph = dgv.CurrentRow.Cells[0].Value.ToString();
            if (MessageBox.Show($"Видалити покупця {ph}?", "Підтвердження", MessageBoxButtons.YesNo) == DialogResult.Yes)
                if (DbHelper.Execute("DELETE FROM public.customer WHERE phone=@p", ("p", ph))) Load_();
        }

        static Label L(string t) => new Label
        {
            Text = t,
            AutoSize = true,
            TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        };
    }

    public class FormProviders : Form
    {
        private TextBox txtName = new TextBox { Width = 140 };
        private TextBox txtAddr = new TextBox { Width = 220 };
        private DataGridView dgv = new DataGridView { Dock = DockStyle.Fill };
        private Button btnAdd = new Button { Text = "Додати", Width = 90 };
        private Button btnDelete = new Button { Text = "Видалити", Width = 90 };

        public FormProviders()
        {
            Text = "Постачальники"; Size = new System.Drawing.Size(620, 420);
            StartPosition = FormStartPosition.CenterScreen;

            var pnl = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 60, Padding = new Padding(8) };
            pnl.Controls.Add(L("Назва:")); pnl.Controls.Add(txtName);
            pnl.Controls.Add(L("Адреса:")); pnl.Controls.Add(txtAddr);
            pnl.Controls.Add(btnAdd); pnl.Controls.Add(btnDelete);

            var back = DbHelper.MakeBackButton(); back.Click += (s, e) => Close();
            Controls.Add(dgv); Controls.Add(pnl); Controls.Add(back);
            btnAdd.Click += BtnAdd_Click;
            btnDelete.Click += BtnDelete_Click;
            Load_();
        }

        void Load_() => DbHelper.Bind(dgv, DbHelper.Query(
            @"SELECT name AS ""Назва"", address AS ""Адреса"" FROM public.provider ORDER BY name"));

        void BtnAdd_Click(object s, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text)) { MessageBox.Show("Введіть назву."); return; }
            if (DbHelper.Execute(
                "INSERT INTO public.provider(name,address) VALUES(@n,@a) ON CONFLICT(name) DO UPDATE SET address=@a",
                ("n", txtName.Text.Trim()), ("a", txtAddr.Text.Trim())))
            { Load_(); txtName.Clear(); txtAddr.Clear(); }
        }

        void BtnDelete_Click(object s, EventArgs e)
        {
            if (dgv.CurrentRow == null) return;
            string n = dgv.CurrentRow.Cells[0].Value.ToString();
            if (MessageBox.Show($"Видалити постачальника «{n}»?", "Підтвердження", MessageBoxButtons.YesNo) == DialogResult.Yes)
                if (DbHelper.Execute("DELETE FROM public.provider WHERE name=@n", ("n", n))) Load_();
        }

        static Label L(string t) => new Label
        {
            Text = t,
            AutoSize = true,
            TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        };
    }

    public class FormSupply : Form
    {
        private ComboBox cmbProvider = new ComboBox { Width = 140, DropDownStyle = ComboBoxStyle.DropDownList };
        private ComboBox cmbProduct = new ComboBox { Width = 160, DropDownStyle = ComboBoxStyle.DropDownList };
        private ComboBox cmbShop = new ComboBox { Width = 130, DropDownStyle = ComboBoxStyle.DropDownList };
        private TextBox txtAmount = new TextBox { Width = 60 };
        private DataGridView dgv = new DataGridView { Dock = DockStyle.Fill };
        private Button btnAdd = new Button { Text = "Додати", Width = 90 };
        private Button btnDelete = new Button { Text = "Видалити", Width = 90 };

        public FormSupply()
        {
            Text = "Постачання"; Size = new System.Drawing.Size(820, 480);
            StartPosition = FormStartPosition.CenterScreen;

            DbHelper.FillCombo(cmbProvider, "SELECT name FROM public.provider ORDER BY name", "name");
            DbHelper.FillCombo(cmbShop, "SELECT name FROM public.shops ORDER BY name", "name");
            FillProducts();
            if (cmbProvider.Items.Count > 0) cmbProvider.SelectedIndex = 0;
            if (cmbShop.Items.Count > 0) cmbShop.SelectedIndex = 0;

            var pnl = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 70, Padding = new Padding(8), WrapContents = true };
            pnl.Controls.Add(L("Постачальник:")); pnl.Controls.Add(cmbProvider);
            pnl.Controls.Add(L("Товар:")); pnl.Controls.Add(cmbProduct);
            pnl.Controls.Add(L("Магазин:")); pnl.Controls.Add(cmbShop);
            pnl.Controls.Add(L("Кількість:")); pnl.Controls.Add(txtAmount);
            pnl.Controls.Add(btnAdd); pnl.Controls.Add(btnDelete);

            var back = DbHelper.MakeBackButton(); back.Click += (s, e) => Close();
            Controls.Add(dgv); Controls.Add(pnl); Controls.Add(back);
            btnAdd.Click += BtnAdd_Click;
            btnDelete.Click += BtnDelete_Click;
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
                     s.shop AS ""Магазин"", s.amount AS ""Кількість""
              FROM public.supply s
              JOIN public.product p ON p.id = s.product
              ORDER BY s.provider"));

        void BtnAdd_Click(object s, EventArgs e)
        {
            if (cmbProvider.SelectedItem == null || cmbProduct.SelectedItem == null || cmbShop.SelectedItem == null)
            { MessageBox.Show("Заповніть всі поля."); return; }
            if (!int.TryParse(txtAmount.Text, out int amount)) { MessageBox.Show("Кількість — ціле число."); return; }

            int productId = int.Parse(cmbProduct.SelectedItem.ToString().Split('—')[0].Trim());
            DbHelper.Execute(
                @"INSERT INTO public.supply(provider,product,shop,amount)
                  VALUES(@prov,@prod,@shop,@amt)
                  ON CONFLICT(provider,product,shop) DO UPDATE SET amount=@amt",
                ("prov", cmbProvider.SelectedItem.ToString()),
                ("prod", productId),
                ("shop", cmbShop.SelectedItem.ToString()),
                ("amt", amount));
            Load_();
        }

        void BtnDelete_Click(object s, EventArgs e)
        {
            if (dgv.CurrentRow == null) return;
            string prov = dgv.CurrentRow.Cells[0].Value.ToString();
            string prod = dgv.CurrentRow.Cells[1].Value.ToString();
            string shop = dgv.CurrentRow.Cells[2].Value.ToString();
            if (MessageBox.Show($"Видалити запис постачання?", "Підтвердження", MessageBoxButtons.YesNo) == DialogResult.Yes)
                if (DbHelper.Execute(
                    @"DELETE FROM public.supply
                      WHERE provider=@prov AND shop=@shop AND product=(SELECT id FROM public.product WHERE name=@prod LIMIT 1)",
                    ("prov", prov), ("prod", prod), ("shop", shop))) Load_();
        }

        static Label L(string t) => new Label
        {
            Text = t,
            AutoSize = true,
            TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        };
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
        private Button btnClear = new Button { Text = "Очистити", Width = 90 };

        private int? _selProductId = null;
        private string _selPhone = null;
        private string _selDateRaw = null;

        public FormPurchases()
        {
            Text = "Покупки"; Size = new System.Drawing.Size(960, 500);
            StartPosition = FormStartPosition.CenterScreen;

            FillProducts();
            FillCustomers();

            var pnl = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 70,
                Padding = new Padding(8),
                WrapContents = true
            };
            pnl.Controls.Add(L("Товар:")); pnl.Controls.Add(cmbProduct);
            pnl.Controls.Add(L("Покупець:")); pnl.Controls.Add(cmbCustomer);
            pnl.Controls.Add(L("Дата:")); pnl.Controls.Add(dtpDate);
            pnl.Controls.Add(L("Сума:")); pnl.Controls.Add(txtTotal);
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
            btnClear.Click += (s, e) => ClearSelection();

            dgv.CellClick += (s, e) =>
            {
                if (e.RowIndex < 0 || dgv.Rows[e.RowIndex].DataBoundItem == null) return;
                var row = ((DataRowView)dgv.Rows[e.RowIndex].DataBoundItem).Row;
                _selProductId = Convert.ToInt32(row["ProductId"]);
                _selPhone = row["Phone"]?.ToString();
                _selDateRaw = row["PurchaseDateRaw"]?.ToString();
                txtTotal.Text = row["Сума"]?.ToString();
                if (DateTime.TryParse(_selDateRaw, out DateTime dt))
                    dtpDate.Value = dt.Date;
                btnSave.Text = "Зберегти зміни";
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

        void FillCustomers()
        {
            cmbCustomer.Items.Clear();
            var dt = DbHelper.Query(
                "SELECT phone, surname||' '||name AS fn FROM public.customer ORDER BY surname");
            foreach (DataRow r in dt.Rows)
                cmbCustomer.Items.Add($"{r["phone"]} — {r["fn"]}");
            if (cmbCustomer.Items.Count > 0) cmbCustomer.SelectedIndex = 0;
        }

        void Load_()
        {
            DbHelper.Bind(dgv, DbHelper.Query(
                @"SELECT pu.purchase_date::text                    AS ""PurchaseDateRaw"",
                         TO_CHAR(pu.purchase_date, 'DD.MM.YYYY')   AS ""Дата"",
                         p.name                                    AS ""Товар"",
                         pu.product                                AS ""ProductId"",
                         pu.customer                               AS ""Phone"",
                         c.surname||' '||c.name                    AS ""Покупець"",
                         pu.total                                  AS ""Сума""
                  FROM public.purchase pu
                  JOIN public.product  p ON p.id   = pu.product
                  JOIN public.customer c ON c.phone = pu.customer
                  ORDER BY pu.purchase_date DESC"));

            if (dgv.Columns["PurchaseDateRaw"] != null) dgv.Columns["PurchaseDateRaw"].Visible = false;
            if (dgv.Columns["ProductId"] != null) dgv.Columns["ProductId"].Visible = false;
            if (dgv.Columns["Phone"] != null) dgv.Columns["Phone"].Visible = false;
        }

        void ClearSelection()
        {
            _selProductId = null;
            _selPhone = null;
            _selDateRaw = null;
            txtTotal.Clear();
            btnSave.Text = "Зберегти";
        }

        void BtnSave_Click(object s, EventArgs e)
        {
            if (cmbProduct.SelectedItem == null || cmbCustomer.SelectedItem == null)
            { MessageBox.Show("Оберіть товар і покупця."); return; }

            decimal total = 0;
            string totalText = txtTotal.Text.Trim().Replace(',', '.');
            if (!string.IsNullOrWhiteSpace(totalText))
            {
                if (!decimal.TryParse(totalText,
                    System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture, out total))
                {
                    MessageBox.Show("Сума повинна бути числом (наприклад: 150 або 150.50).",
                        "Помилка вводу", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtTotal.Focus();
                    txtTotal.SelectAll();
                    return;
                }
            }

            bool ok;
            if (_selProductId.HasValue && _selPhone != null && _selDateRaw != null)
            {
                ok = DbHelper.Execute(
                    @"UPDATE public.purchase SET total=@tot
                      WHERE product=@prod AND customer=@cust
                        AND purchase_date = @dt::timestamptz",
                    ("tot", total),
                    ("prod", _selProductId.Value),
                    ("cust", _selPhone),
                    ("dt", _selDateRaw));
            }
            else
            {
                int productId = int.Parse(cmbProduct.SelectedItem.ToString().Split('—')[0].Trim());
                string phone = cmbCustomer.SelectedItem.ToString().Split('—')[0].Trim();
                ok = DbHelper.Execute(
                    @"INSERT INTO public.purchase(product,customer,purchase_date,total)
                      VALUES(@prod,@cust,@dt,@tot) ON CONFLICT DO NOTHING",
                    ("prod", productId),
                    ("cust", phone),
                    ("dt", dtpDate.Value.Date),
                    ("tot", total));
            }
            if (ok) { ClearSelection(); Load_(); }
        }

        void BtnDelete_Click(object s, EventArgs e)
        {
            if (_selProductId == null || _selPhone == null || _selDateRaw == null)
            {
                MessageBox.Show("Спочатку клікніть на рядок у таблиці щоб обрати запис.",
                    "Нічого не обрано", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (MessageBox.Show("Видалити цей запис покупки?", "Підтвердження",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                bool ok = DbHelper.Execute(
                    @"DELETE FROM public.purchase
                      WHERE product=@prod AND customer=@cust
                        AND purchase_date = @dt::timestamptz",
                    ("prod", _selProductId.Value),
                    ("cust", _selPhone),
                    ("dt", _selDateRaw));

                if (ok) { ClearSelection(); Load_(); }
            }
        }

        static Label L(string t) => new Label
        {
            Text = t,
            AutoSize = true,
            TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        };
    }
}