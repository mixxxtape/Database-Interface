using System;
using System.Data;
using System.Windows.Forms;
using Npgsql;

namespace Shop
{
    internal static class DbHelper
    {
        public static readonly string ConnStr =
            "Host=localhost;Port=5432;Database=shop;Username=postgres;Password=dasha2007";

        public static DataTable Query(string sql, params (string name, object value)[] parameters)
        {
            var dt = new DataTable();
            try
            {
                using var con = new NpgsqlConnection(ConnStr);
                using var cmd = new NpgsqlCommand(sql, con);
                foreach (var (name, value) in parameters)
                    cmd.Parameters.AddWithValue(name, value ?? DBNull.Value);
                con.Open();
                using var da = new NpgsqlDataAdapter(cmd);
                da.Fill(dt);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Помилка БД", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return dt;
        }

        public static bool Execute(string sql, params (string name, object value)[] parameters)
        {
            try
            {
                using var con = new NpgsqlConnection(ConnStr);
                using var cmd = new NpgsqlCommand(sql, con);
                foreach (var (name, value) in parameters)
                    cmd.Parameters.AddWithValue(name, value ?? DBNull.Value);
                con.Open();
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Помилка БД", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
        public static bool ExecuteTransaction((string sql, (string name, object value)[] parms)[] commands)
        {
            try
            {
                using var con = new NpgsqlConnection(ConnStr);
                con.Open();
                using var tx = con.BeginTransaction();
                foreach (var (sql, parms) in commands)
                {
                    using var cmd = new NpgsqlCommand(sql, con, tx);
                    foreach (var (name, value) in parms)
                        cmd.Parameters.AddWithValue(name, value ?? DBNull.Value);
                    cmd.ExecuteNonQuery();
                }
                tx.Commit();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Помилка БД", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public static void Bind(DataGridView dgv, DataTable dt)
        {
            dgv.DataSource = null;
            dgv.DataSource = dt;
            dgv.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
            dgv.ReadOnly = true;
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }

        public static void FillCombo(ComboBox cb, string sql, string col,
            params (string name, object value)[] parameters)
        {
            cb.Items.Clear();
            var dt = Query(sql, parameters);
            foreach (DataRow row in dt.Rows)
                cb.Items.Add(row[col].ToString());
            if (cb.Items.Count > 0) cb.SelectedIndex = 0;
        }

        public static Panel MakeBottomPanel(Action backAction, Form owner)
        {
            var pnl = new Panel { Dock = DockStyle.Bottom, Height = 40 };

            var btnBack = new Button
            {
                Text = "Повернутись до меню",
                Dock = DockStyle.Left,
                Width = 200,
                Height = 40,
                Font = new System.Drawing.Font("Segoe UI", 9.5F),
                FlatStyle = FlatStyle.System
            };
            btnBack.Click += (s, e) => backAction();

            var btnMin = new Button
            {
                Text = "Згорнути",
                Dock = DockStyle.Right,
                Width = 130,
                Height = 40,
                Font = new System.Drawing.Font("Segoe UI", 9.5F),
                FlatStyle = FlatStyle.System
            };
            btnMin.Click += (s, e) => owner.WindowState = FormWindowState.Minimized;

            pnl.Controls.Add(btnBack);
            pnl.Controls.Add(btnMin);
            return pnl;
        }

        public static Button MakeBackButton()
        {
            return new Button
            {
                Text = "Повернутись до меню",
                Dock = DockStyle.Bottom,
                Height = 36,
                Font = new System.Drawing.Font("Segoe UI", 9.5F),
                FlatStyle = FlatStyle.System
            };
        }

        public static Label Lbl(string text) =>
            new Label
            {
                Text = text,
                AutoSize = true,
                TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            };

        public static bool ValidateText(string val, string fieldName, int maxLen, out string error)
        {
            error = null;
            if (string.IsNullOrWhiteSpace(val))
            { error = $"Поле «{fieldName}» не може бути порожнім."; return false; }
            if (val.Length > maxLen)
            { error = $"Поле «{fieldName}» не може бути довшим за {maxLen} символів."; return false; }
            return true;
        }

        public static bool ValidatePositiveDecimal(string val, string fieldName, out decimal result, out string error)
        {
            error = null; result = 0;
            if (!decimal.TryParse(val.Replace(',', '.'),
                System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out result))
            { error = $"Поле «{fieldName}» повинно бути числом."; return false; }
            if (result < 0)
            { error = $"Поле «{fieldName}» повинно бути невід'ємним."; return false; }
            return true;
        }

        public static bool ValidatePositiveInt(string val, string fieldName, out int result, out string error)
        {
            error = null; result = 0;
            if (!int.TryParse(val, out result))
            { error = $"Поле «{fieldName}» повинно бути цілим числом."; return false; }
            if (result <= 0)
            { error = $"Поле «{fieldName}» повинно бути більшим за 0."; return false; }
            return true;
        }
    }
}