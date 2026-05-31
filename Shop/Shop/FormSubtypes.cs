using System;
using System.Data;
using System.Windows.Forms;

namespace Shop
{
    public class FormClothes : Form
    {
        private DataGridView dgv = new DataGridView { Dock = DockStyle.Fill };
        private ComboBox cmbId = new ComboBox { Width = 200, DropDownStyle = ComboBoxStyle.DropDownList };
        private TextBox txtType = new TextBox { Width = 120 };
        private Button btnSave = new Button { Text = "Зберегти", Width = 100 };
        private Button btnDelete = new Button { Text = "Видалити", Width = 90 };
        private Button btnRefresh = new Button { Text = "Оновити", Width = 90 };

        public FormClothes()
        {
            Text = "Одяг"; Size = new System.Drawing.Size(720, 460);
            StartPosition = FormStartPosition.CenterScreen;

            FillProducts();

            var pnl = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 60,
                Padding = new Padding(6),
                WrapContents = false
            };
            pnl.Controls.Add(DbHelper.Lbl("Товар:")); pnl.Controls.Add(cmbId);
            pnl.Controls.Add(DbHelper.Lbl("Тип одягу:")); pnl.Controls.Add(txtType);
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
                txtType.Text = row["Тип одягу"]?.ToString();
                int id = Convert.ToInt32(row["ID"]);
                foreach (var item in cmbId.Items)
                    if (item.ToString().StartsWith(id + " -")) { cmbId.SelectedItem = item; break; }
            };

            Load_();
        }

        void FillProducts()
        {
            var dt = DbHelper.Query(
                "SELECT id, name FROM public.product ORDER BY id");
            cmbId.Items.Clear();
            foreach (DataRow r in dt.Rows)
                cmbId.Items.Add($"{r["id"]} - {r["name"]}");
            if (cmbId.Items.Count > 0) cmbId.SelectedIndex = 0;
        }

        void Load_() => DbHelper.Bind(dgv, DbHelper.Query(
            @"SELECT c.id ""ID"", p.name ""Назва товару"", c.type ""Тип одягу""
              FROM public.clothes c JOIN public.product p ON p.id = c.id ORDER BY c.id"));

        void BtnSave_Click(object s, EventArgs e)
        {
            if (cmbId.SelectedItem == null) { MessageBox.Show("Оберіть товар."); return; }
            int id = int.Parse(cmbId.SelectedItem.ToString().Split('—')[0].Trim());
            if (DbHelper.Execute(
                "INSERT INTO public.clothes(id,type) VALUES(@i,@t) ON CONFLICT(id) DO UPDATE SET type=@t",
                ("i", id), ("t", txtType.Text.Trim())))
            { Load_(); txtType.Clear(); }
        }

        void BtnDelete_Click(object s, EventArgs e)
        {
            if (dgv.CurrentRow == null) return;
            int id = Convert.ToInt32(dgv.CurrentRow.Cells[0].Value);
            if (MessageBox.Show($"Видалити запис #{id}?", "Підтвердження",
                MessageBoxButtons.YesNo) == DialogResult.Yes)
                if (DbHelper.Execute("DELETE FROM public.clothes WHERE id=@i", ("i", id)))
                    Load_();
        }
    }

    public class FormShoes : Form
    {
        private DataGridView dgv = new DataGridView { Dock = DockStyle.Fill };
        private ComboBox cmbId = new ComboBox { Width = 200, DropDownStyle = ComboBoxStyle.DropDownList };
        private TextBox txtSeason = new TextBox { Width = 120 };
        private Button btnSave = new Button { Text = "Зберегти", Width = 100 };
        private Button btnDelete = new Button { Text = "Видалити", Width = 90 };
        private Button btnRefresh = new Button { Text = "Оновити", Width = 90 };

        public FormShoes()
        {
            Text = "Взуття"; Size = new System.Drawing.Size(720, 460);
            StartPosition = FormStartPosition.CenterScreen;

            FillProducts();

            var pnl = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 60,
                Padding = new Padding(6),
                WrapContents = false
            };
            pnl.Controls.Add(DbHelper.Lbl("Товар:")); pnl.Controls.Add(cmbId);
            pnl.Controls.Add(DbHelper.Lbl("Сезон:")); pnl.Controls.Add(txtSeason);
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
                txtSeason.Text = row["Сезон"]?.ToString();
                int id = Convert.ToInt32(row["ID"]);
                foreach (var item in cmbId.Items)
                    if (item.ToString().StartsWith(id + " —")) { cmbId.SelectedItem = item; break; }
            };

            Load_();
        }

        void FillProducts()
        {
            var dt = DbHelper.Query("SELECT id, name FROM public.product ORDER BY id");
            cmbId.Items.Clear();
            foreach (DataRow r in dt.Rows)
                cmbId.Items.Add($"{r["id"]} — {r["name"]}");
            if (cmbId.Items.Count > 0) cmbId.SelectedIndex = 0;
        }

        void Load_() => DbHelper.Bind(dgv, DbHelper.Query(
            @"SELECT s.id ""ID"", p.name ""Назва товару"", s.season ""Сезон""
              FROM public.shoes s JOIN public.product p ON p.id = s.id ORDER BY s.id"));

        void BtnSave_Click(object s, EventArgs e)
        {
            if (cmbId.SelectedItem == null) { MessageBox.Show("Оберіть товар."); return; }
            int id = int.Parse(cmbId.SelectedItem.ToString().Split('—')[0].Trim());
            if (DbHelper.Execute(
                "INSERT INTO public.shoes(id,season) VALUES(@i,@s) ON CONFLICT(id) DO UPDATE SET season=@s",
                ("i", id), ("s", txtSeason.Text.Trim())))
            { Load_(); txtSeason.Clear(); }
        }

        void BtnDelete_Click(object s, EventArgs e)
        {
            if (dgv.CurrentRow == null) return;
            int id = Convert.ToInt32(dgv.CurrentRow.Cells[0].Value);
            if (MessageBox.Show($"Видалити запис #{id}?", "Підтвердження",
                MessageBoxButtons.YesNo) == DialogResult.Yes)
                if (DbHelper.Execute("DELETE FROM public.shoes WHERE id=@i", ("i", id)))
                    Load_();
        }
    }

    public class FormJewelry : Form
    {
        private DataGridView dgv = new DataGridView { Dock = DockStyle.Fill };
        private ComboBox cmbId = new ComboBox { Width = 200, DropDownStyle = ComboBoxStyle.DropDownList };
        private TextBox txtType = new TextBox { Width = 120 };
        private Button btnSave = new Button { Text = "Зберегти", Width = 100 };
        private Button btnDelete = new Button { Text = "Видалити", Width = 90 };
        private Button btnRefresh = new Button { Text = "Оновити", Width = 90 };

        public FormJewelry()
        {
            Text = "Прикраси"; Size = new System.Drawing.Size(720, 460);
            StartPosition = FormStartPosition.CenterScreen;

            FillProducts();

            var pnl = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 60,
                Padding = new Padding(6),
                WrapContents = false
            };
            pnl.Controls.Add(DbHelper.Lbl("Товар:")); pnl.Controls.Add(cmbId);
            pnl.Controls.Add(DbHelper.Lbl("Різновид:")); pnl.Controls.Add(txtType);
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
                txtType.Text = row["Різновид"]?.ToString();
                int id = Convert.ToInt32(row["ID"]);
                foreach (var item in cmbId.Items)
                    if (item.ToString().StartsWith(id + " —")) { cmbId.SelectedItem = item; break; }
            };

            Load_();
        }

        void FillProducts()
        {
            var dt = DbHelper.Query("SELECT id, name FROM public.product ORDER BY id");
            cmbId.Items.Clear();
            foreach (DataRow r in dt.Rows)
                cmbId.Items.Add($"{r["id"]} — {r["name"]}");
            if (cmbId.Items.Count > 0) cmbId.SelectedIndex = 0;
        }

        void Load_() => DbHelper.Bind(dgv, DbHelper.Query(
            @"SELECT j.id ""ID"", p.name ""Назва товару"", j.type ""Різновид""
              FROM public.jewelry j JOIN public.product p ON p.id = j.id ORDER BY j.id"));

        void BtnSave_Click(object s, EventArgs e)
        {
            if (cmbId.SelectedItem == null) { MessageBox.Show("Оберіть товар."); return; }
            int id = int.Parse(cmbId.SelectedItem.ToString().Split('—')[0].Trim());
            if (DbHelper.Execute(
                "INSERT INTO public.jewelry(id,type) VALUES(@i,@t) ON CONFLICT(id) DO UPDATE SET type=@t",
                ("i", id), ("t", txtType.Text.Trim())))
            { Load_(); txtType.Clear(); }
        }

        void BtnDelete_Click(object s, EventArgs e)
        {
            if (dgv.CurrentRow == null) return;
            int id = Convert.ToInt32(dgv.CurrentRow.Cells[0].Value);
            if (MessageBox.Show($"Видалити запис #{id}?", "Підтвердження",
                MessageBoxButtons.YesNo) == DialogResult.Yes)
                if (DbHelper.Execute("DELETE FROM public.jewelry WHERE id=@i", ("i", id)))
                    Load_();
        }
    }
}