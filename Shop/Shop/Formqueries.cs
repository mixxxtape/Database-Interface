// FormQueries.cs
using System;
using System.Data;
using System.Windows.Forms;

namespace Shop
{
    public class FormQueries : Form
    {
        private DataGridView dgv = new DataGridView { Dock = DockStyle.Fill };
        private Label lblQueryName;

        public FormQueries()
        {
            Text = "Запити до бази даних";
            Size = new System.Drawing.Size(1150, 720);
            StartPosition = FormStartPosition.CenterScreen;

            lblQueryName = new Label
            {
                Dock = DockStyle.Top,
                Height = 30,
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
                Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold)
            };

            DbHelper.Bind(dgv, new DataTable());

            var pnlButtons = new Panel { Dock = DockStyle.Left, Width = 380, AutoScroll = true };
            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                ColumnCount = 1,
                AutoSize = true,
                Padding = new Padding(8)
            };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            layout.Controls.Add(SectionLbl("Параметризовані запити"));

            layout.Controls.Add(QueryBtn(
                "Магазини де загальна кількість працівників більша за X\n",
                () =>
                {
                    int? minWorkers = AskInt("Введіть мінімальну кількість працівників (X):");
                    if (minWorkers == null) return null;
                    return DbHelper.Query(
                        @"SELECT sh.name            ""Магазин"",
                                 sh.address         ""Адреса"",
                                 sh.contacts        ""Контакти"",
                                 SUM(d.workers_num) ""К-сть працівників""
                          FROM public.shops sh
                          LEFT JOIN public.departments d ON d.shop = sh.name
                          GROUP BY sh.name, sh.address, sh.contacts
                          HAVING SUM(d.workers_num) > @x
                          ORDER BY SUM(d.workers_num) DESC",
                        ("x", minWorkers.Value));
                }));

            layout.Controls.Add(QueryBtn(
                "Покупці що витратили загалом на покупки більше X грн\n",
                () =>
                {
                    decimal? minSpend = AskDecimal("Введіть мінімальну суму витрат (X) у грн:");
                    if (minSpend == null) return null;
                    return DbHelper.Query(
                        @"SELECT c.surname || ' ' || c.name ""Покупець"",
                                 c.phone                    ""Телефон"",
                                 COUNT(pu.product)          ""К-сть покупок"",
                                 SUM(pu.total)              ""Загальна сума""
                          FROM public.customer c
                          JOIN public.purchase pu ON pu.customer = c.phone
                          GROUP BY c.phone, c.surname, c.name
                          HAVING SUM(pu.total) > @x
                          ORDER BY SUM(pu.total) DESC",
                        ("x", minSpend.Value));
                }));

            layout.Controls.Add(QueryBtn(
                "Товари певного магазину що жодного разу не були куплені\n",
                () =>
                {
                    string shop = AskTextWithList(
                        "Введіть або оберіть назву магазину (X):",
                        "SELECT name FROM public.shops ORDER BY name", "name");
                    if (shop == null) return null;
                    return DbHelper.Query(
                        @"SELECT p.id    ""ID"",
                                 p.name  ""Назва товару"",
                                 p.brand ""Бренд"",
                                 p.price ""Ціна"",
                                 CASE
                                   WHEN c.id  IS NOT NULL THEN 'Одяг'
                                   WHEN s2.id IS NOT NULL THEN 'Взуття'
                                   WHEN j.id  IS NOT NULL THEN 'Прикраса'
                                   ELSE 'Інше'
                                 END     ""Тип""
                          FROM public.product p
                          LEFT JOIN public.clothes  c  ON c.id  = p.id
                          LEFT JOIN public.shoes    s2 ON s2.id = p.id
                          LEFT JOIN public.jewelry  j  ON j.id  = p.id
                          WHERE p.owner = @sh
                            AND NOT EXISTS (
                                SELECT 1 FROM public.purchase pu WHERE pu.product = p.id)
                          ORDER BY p.price DESC",
                        ("sh", shop));
                }));

            layout.Controls.Add(QueryBtn(
                "Середня зарплата для певного відділу магазину X\n",
                () =>
                {
                    string shop = AskTextWithList(
                        "Введіть або оберіть назву магазину (X):",
                        "SELECT name FROM public.shops ORDER BY name", "name");
                    if (shop == null) return null;
                    return DbHelper.Query(
                        @"SELECT d.name                  ""Відділ"",
                                 COUNT(w.id)             ""К-сть працівників"",
                                 MIN(w.salary)           ""Мін. зарплата"",
                                 MAX(w.salary)           ""Макс. зарплата"",
                                 ROUND(AVG(w.salary), 2) ""Середня зарплата""
                          FROM public.departments d
                          JOIN public.workers w ON w.department = d.name AND w.shop = d.shop
                          WHERE d.shop = @sh
                          GROUP BY d.name
                          ORDER BY AVG(w.salary) DESC",
                        ("sh", shop));
                }));

            layout.Controls.Add(QueryBtn(
                "Постачальники що постачають товари дешевші за X грн\n",
                () =>
                {
                    decimal? maxPrice = AskDecimal("Введіть максимальну ціну товару (X) у грн:");
                    if (maxPrice == null) return null;
                    return DbHelper.Query(
                        @"SELECT pr.name     ""Постачальник"",
                                 pr.address  ""Адреса"",
                                 p.name      ""Товар"",
                                 p.price     ""Ціна товару""
                          FROM public.provider pr
                          JOIN public.supply   s  ON s.provider = pr.name
                          JOIN public.product  p  ON p.id = s.product
                          WHERE p.price < @x
                          ORDER BY pr.name, p.price",
                        ("x", maxPrice.Value));
                }));

            layout.Controls.Add(SectionLbl("Множинні порівняння"));

            layout.Controls.Add(QueryBtn(
                "Постачальники що постачають точно такі ж товари що і постачальник X",
                () =>
                {
                    string prov = AskTextWithList(
                        "Введіть або оберіть постачальника (X):",
                        "SELECT name FROM public.provider ORDER BY name", "name");
                    if (prov == null) return null;
                    return DbHelper.Query(
                        @"SELECT s.provider ""Постачальник""
                          FROM public.supply s
                          WHERE s.provider <> @p
                          GROUP BY s.provider
                          HAVING
                            NOT EXISTS (
                                SELECT product FROM public.supply WHERE provider = @p
                                EXCEPT
                                SELECT product FROM public.supply WHERE provider = s.provider)
                            AND
                            NOT EXISTS (
                                SELECT product FROM public.supply WHERE provider = s.provider
                                EXCEPT
                                SELECT product FROM public.supply WHERE provider = @p)
                          ORDER BY s.provider",
                        ("p", prov));
                }));

            layout.Controls.Add(QueryBtn(
                "Покупці які купили товари всіх існуючих брендів",
                () => DbHelper.Query(
                    @"SELECT c.surname||' '||c.name AS ""Покупець"",
                             c.phone                AS ""Телефон"",
                             COUNT(pu.product)      AS ""К-сть покупок""
                      FROM public.customer c
                      JOIN public.purchase pu ON pu.customer = c.phone
                      WHERE NOT EXISTS (
                          SELECT DISTINCT brand FROM public.product WHERE brand IS NOT NULL
                          EXCEPT
                          SELECT DISTINCT p.brand FROM public.purchase pu2
                          JOIN public.product p ON p.id = pu2.product
                          WHERE pu2.customer = c.phone)
                      GROUP BY c.phone, c.surname, c.name
                      ORDER BY c.surname")));

            pnlButtons.Controls.Add(layout);

            pnlButtons.Controls.Add(layout);

            Controls.Add(dgv);
            Controls.Add(lblQueryName);
            Controls.Add(pnlButtons);
            Controls.Add(DbHelper.MakeBottomPanel(() => Close(), this));
        }

        string AskTextWithList(string prompt, string sql, string valueColumn)
        {
            var dt = DbHelper.Query(sql);

            var frm = new Form
            {
                Text = "Введіть параметр",
                Size = new System.Drawing.Size(380, 200),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false
            };

            var lbl = new Label
            {
                Text = prompt,
                Left = 12,
                Top = 10,
                Width = 340,
                AutoSize = true
            };

            var cmb = new ComboBox
            {
                Left = 12,
                Top = 34,
                Width = 340,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmb.Items.Add("оберіть зі списку або введіть нижче");
            foreach (DataRow r in dt.Rows)
                cmb.Items.Add(r[valueColumn].ToString());
            cmb.SelectedIndex = 0;

            var lblManual = new Label
            {
                Text = "Або введіть вручну:",
                Left = 12,
                Top = 68,
                AutoSize = true
            };
            var txt = new TextBox
            {
                Left = 12,
                Top = 88,
                Width = 340
            };

            cmb.SelectedIndexChanged += (s, e) =>
            {
                if (cmb.SelectedIndex > 0)
                    txt.Text = cmb.SelectedItem.ToString();
            };

            var btnOk = new Button
            {
                Text = "OK",
                Left = 175,
                Top = 122,
                Width = 80,
                DialogResult = DialogResult.OK
            };
            var btnCnl = new Button
            {
                Text = "Скасувати",
                Left = 265,
                Top = 122,
                Width = 90,
                DialogResult = DialogResult.Cancel
            };

            frm.Controls.AddRange(new System.Windows.Forms.Control[]
                { lbl, cmb, lblManual, txt, btnOk, btnCnl });
            frm.AcceptButton = btnOk;
            frm.CancelButton = btnCnl;

            if (frm.ShowDialog() != DialogResult.OK) return null;

            string val = txt.Text.Trim();
            if (string.IsNullOrWhiteSpace(val))
            {
                MessageBox.Show("Введіть або оберіть значення.");
                return null;
            }
            return val;
        }

        int? AskInt(string prompt)
        {
            string val = ShowSimpleInput(prompt);
            if (val == null) return null;
            if (!int.TryParse(val.Trim(), out int result))
            { MessageBox.Show("Введіть ціле число."); return null; }
            return result;
        }
        decimal? AskDecimal(string prompt)
        {
            string val = ShowSimpleInput(prompt);
            if (val == null) return null;
            if (!decimal.TryParse(val.Trim().Replace(',', '.'),
                System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out decimal result))
            { MessageBox.Show("Введіть числове значення."); return null; }
            return result;
        }

        string ShowSimpleInput(string prompt)
        {
            var frm = new Form
            {
                Text = "Введіть параметр",
                Size = new System.Drawing.Size(360, 140),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false
            };
            var lbl = new Label { Text = prompt, Left = 12, Top = 12, Width = 320, AutoSize = true };
            var txt = new TextBox { Left = 12, Top = 40, Width = 320 };
            var btnOk = new Button { Text = "OK", Left = 170, Top = 72, Width = 75, DialogResult = DialogResult.OK };
            var btnCnl = new Button { Text = "Скасувати", Left = 255, Top = 72, Width = 85, DialogResult = DialogResult.Cancel };
            frm.Controls.AddRange(new System.Windows.Forms.Control[] { lbl, txt, btnOk, btnCnl });
            frm.AcceptButton = btnOk;
            frm.CancelButton = btnCnl;
            return frm.ShowDialog() == DialogResult.OK ? txt.Text : null;
        }
        Button QueryBtn(string text, Func<DataTable> query)
        {
            var btn = new Button
            {
                Text = text,
                Height = 78,
                Dock = DockStyle.Top,
                TextAlign = System.Drawing.ContentAlignment.MiddleLeft,
                Padding = new Padding(8, 0, 0, 0),
                Margin = new Padding(0, 4, 0, 0),
                Font = new System.Drawing.Font("Segoe UI", 8.5F)
            };
            btn.Click += (s, e) =>
            {
                lblQueryName.Text = btn.Text.Split('\n')[0];
                var dt = query();
                if (dt != null) DbHelper.Bind(dgv, dt);
            };
            return btn;
        }

        Label SectionLbl(string text) => new Label
        {
            Text = text,
            AutoSize = false,
            Height = 26,
            Dock = DockStyle.Top,
            TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
            Font = new System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Bold),
            Margin = new Padding(0, 10, 0, 2)
        };
    }
}