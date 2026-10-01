using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using Guna.UI2.WinForms;
using GymManagement.DAL;

namespace GymManagement.Forms
{
    public class frmQuanLyGoiTap : Form
    {
        private Guna2DataGridView dgv;
        private TextBox txtTenGoi, txtSoThang, txtDonGia, txtMoTa;
        private Guna2CheckBox chkTrangThai;
        private Guna2Button btnThem, btnSua, btnXoa, btnLamMoi;
        private int? selectedId = null;

        private static readonly Color ColorAccent = ColorTranslator.FromHtml("#4E7CFF");
        private static readonly Color ColorSuccess = ColorTranslator.FromHtml("#2ECC71");
        private static readonly Color ColorDanger = ColorTranslator.FromHtml("#E85C5C");
        private static readonly Color ColorNeutral = ColorTranslator.FromHtml("#95A5A6");
        private static readonly Color ColorDark = ColorTranslator.FromHtml("#1B2430");

        public frmQuanLyGoiTap()
        {
            InitializeComponent();
            LoadData();
        }

        private void InitializeComponent()
        {
            this.AutoScaleMode = AutoScaleMode.None;
            this.Dock = DockStyle.Fill;
            this.BackColor = ColorTranslator.FromHtml("#F3F5F9");
            this.AutoScroll = true;

            var lblTitle = new Label
            {
                Text = "QUẢN LÝ GÓI TẬP",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = ColorDark,
                Location = new Point(20, 15),
                AutoSize = true
            };
            this.Controls.Add(lblTitle);

            var pnlForm = new Guna2GroupBox
            {
                Text = "Thông tin gói tập",
                Location = new Point(20, 55),
                Size = new Size(950, 150),
                Font = new Font("Segoe UI", 9.5f),
                FillColor = Color.White,
                BorderRadius = 10,
                ForeColor = ColorDark
            };

            Label MakeLabel(string text, Point loc) => new Label { Text = text, Location = loc, AutoSize = true, ForeColor = Color.Gray };
            TextBox MakeBox(Point loc, int width) => new TextBox { Location = loc, Width = width, Font = new Font("Segoe UI", 9.5f), BorderStyle = BorderStyle.FixedSingle };

            pnlForm.Controls.Add(MakeLabel("Tên gói:", new Point(15, 35)));
            txtTenGoi = MakeBox(new Point(100, 32), 250);
            pnlForm.Controls.Add(txtTenGoi);

            pnlForm.Controls.Add(MakeLabel("Số tháng:", new Point(370, 35)));
            txtSoThang = MakeBox(new Point(450, 32), 100);
            pnlForm.Controls.Add(txtSoThang);

            pnlForm.Controls.Add(MakeLabel("Đơn giá (đ):", new Point(570, 35)));
            txtDonGia = MakeBox(new Point(670, 32), 150);
            pnlForm.Controls.Add(txtDonGia);

            pnlForm.Controls.Add(MakeLabel("Mô tả:", new Point(15, 92)));
            txtMoTa = MakeBox(new Point(100, 89), 500);
            pnlForm.Controls.Add(txtMoTa);

            chkTrangThai = new Guna2CheckBox { Text = "Đang bán", Location = new Point(620, 96), Checked = true, AutoSize = true, CheckedState = { BorderColor = ColorAccent, FillColor = ColorAccent } };
            pnlForm.Controls.Add(chkTrangThai);

            this.Controls.Add(pnlForm);

            btnThem = MakeButton("➕ Thêm", ColorSuccess, new Point(20, 225));
            btnSua = MakeButton("✏ Sửa", ColorAccent, new Point(140, 225));
            btnXoa = MakeButton("🗑 Xóa", ColorDanger, new Point(260, 225));
            btnLamMoi = MakeButton("🔄 Làm mới", ColorNeutral, new Point(380, 225));

            btnThem.Click += BtnThem_Click;
            btnSua.Click += BtnSua_Click;
            btnXoa.Click += BtnXoa_Click;
            btnLamMoi.Click += (s, e) => ClearForm();

            this.Controls.Add(btnThem);
            this.Controls.Add(btnSua);
            this.Controls.Add(btnXoa);
            this.Controls.Add(btnLamMoi);

            dgv = new Guna2DataGridView
            {
                Location = new Point(20, 270),
                Size = new Size(950, 350),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None
            };
            StyleGrid(dgv);
            dgv.SelectionChanged += Dgv_SelectionChanged;
            this.Controls.Add(dgv);
        }

        private void StyleGrid(Guna2DataGridView g)
        {
            g.ThemeStyle.HeaderStyle.BackColor = ColorDark;
            g.ThemeStyle.HeaderStyle.ForeColor = Color.White;
            g.ThemeStyle.HeaderStyle.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
            g.ThemeStyle.HeaderStyle.Height = 40;
            g.ThemeStyle.RowsStyle.BackColor = Color.White;
            g.ThemeStyle.RowsStyle.ForeColor = ColorDark;
            g.ThemeStyle.RowsStyle.SelectionBackColor = ColorAccent;
            g.ThemeStyle.RowsStyle.SelectionForeColor = Color.White;
            g.ThemeStyle.RowsStyle.Height = 34;
            g.ThemeStyle.AlternatingRowsStyle.BackColor = ColorTranslator.FromHtml("#F5F7FA");
        }

        private Guna2Button MakeButton(string text, Color color, Point loc)
        {
            var btn = new Guna2Button
            {
                Text = text,
                Location = loc,
                Size = new Size(110, 36),
                FillColor = color,
                ForeColor = Color.White,
                BorderRadius = 8,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                Animated = true,
                Cursor = Cursors.Hand
            };
            return btn;
        }

        private void LoadData()
        {
            DataTable dt = DatabaseHelper.ExecuteQuery(
                "SELECT MaGoi, TenGoi, SoThang, DonGia, MoTa, TrangThai FROM GoiTap ORDER BY MaGoi DESC");
            dgv.DataSource = dt;

            var headers = new (string col, string header)[]
            {
                ("MaGoi","Mã gói"), ("TenGoi","Tên gói"), ("SoThang","Số tháng"),
                ("DonGia","Đơn giá"), ("MoTa","Mô tả"), ("TrangThai","Đang bán")
            };
            foreach (var (col, header) in headers)
                if (dgv.Columns.Contains(col)) dgv.Columns[col].HeaderText = header;
        }

        private void Dgv_SelectionChanged(object sender, EventArgs e)
        {
            if (dgv.CurrentRow == null || dgv.CurrentRow.DataBoundItem == null) return;
            DataRowView row = (DataRowView)dgv.CurrentRow.DataBoundItem;

            selectedId = Convert.ToInt32(row["MaGoi"]);
            txtTenGoi.Text = row["TenGoi"].ToString();
            txtSoThang.Text = row["SoThang"].ToString();
            txtDonGia.Text = row["DonGia"].ToString();
            txtMoTa.Text = row["MoTa"].ToString();
            chkTrangThai.Checked = Convert.ToBoolean(row["TrangThai"]);
        }

        private void BtnThem_Click(object sender, EventArgs e)
        {
            if (!ValidateForm(out int soThang, out decimal donGia)) return;
            try
            {
                DatabaseHelper.ExecuteNonQuery(
                    "INSERT INTO GoiTap (TenGoi, SoThang, DonGia, MoTa, TrangThai) VALUES (@tg,@st,@dg,@mt,@tt)",
                    new SqlParameter("@tg", txtTenGoi.Text.Trim()),
                    new SqlParameter("@st", soThang),
                    new SqlParameter("@dg", donGia),
                    new SqlParameter("@mt", (object)txtMoTa.Text.Trim() ?? DBNull.Value),
                    new SqlParameter("@tt", chkTrangThai.Checked));
                MessageBox.Show("Thêm gói tập thành công!", "Thông báo");
                ClearForm();
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnSua_Click(object sender, EventArgs e)
        {
            if (selectedId == null) { MessageBox.Show("Vui lòng chọn gói tập cần sửa."); return; }
            if (!ValidateForm(out int soThang, out decimal donGia)) return;
            try
            {
                DatabaseHelper.ExecuteNonQuery(
                    "UPDATE GoiTap SET TenGoi=@tg, SoThang=@st, DonGia=@dg, MoTa=@mt, TrangThai=@tt WHERE MaGoi=@id",
                    new SqlParameter("@tg", txtTenGoi.Text.Trim()),
                    new SqlParameter("@st", soThang),
                    new SqlParameter("@dg", donGia),
                    new SqlParameter("@mt", (object)txtMoTa.Text.Trim() ?? DBNull.Value),
                    new SqlParameter("@tt", chkTrangThai.Checked),
                    new SqlParameter("@id", selectedId.Value));
                MessageBox.Show("Cập nhật thành công!", "Thông báo");
                ClearForm();
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnXoa_Click(object sender, EventArgs e)
        {
            if (selectedId == null) { MessageBox.Show("Vui lòng chọn gói tập cần xóa."); return; }
            if (MessageBox.Show("Bạn có chắc muốn xóa gói tập này?", "Xác nhận",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            try
            {
                DatabaseHelper.ExecuteNonQuery("DELETE FROM GoiTap WHERE MaGoi=@id", new SqlParameter("@id", selectedId.Value));
                ClearForm();
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể xóa (gói tập có thể đang được sử dụng trong hóa đơn): " + ex.Message, "Lỗi");
            }
        }

        private bool ValidateForm(out int soThang, out decimal donGia)
        {
            soThang = 0; donGia = 0;
            if (string.IsNullOrWhiteSpace(txtTenGoi.Text))
            {
                MessageBox.Show("Vui lòng nhập tên gói tập.", "Thiếu thông tin");
                return false;
            }
            if (!int.TryParse(txtSoThang.Text.Trim(), out soThang) || soThang <= 0)
            {
                MessageBox.Show("Số tháng phải là số nguyên dương.", "Sai định dạng");
                return false;
            }
            if (!decimal.TryParse(txtDonGia.Text.Trim(), out donGia) || donGia < 0)
            {
                MessageBox.Show("Đơn giá phải là số hợp lệ.", "Sai định dạng");
                return false;
            }
            return true;
        }

        private void ClearForm()
        {
            selectedId = null;
            txtTenGoi.Clear(); txtSoThang.Clear(); txtDonGia.Clear(); txtMoTa.Clear();
            chkTrangThai.Checked = true;
            dgv.ClearSelection();
        }
    }
}
