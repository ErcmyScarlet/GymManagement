using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using Guna.UI2.WinForms;
using GymManagement.DAL;

namespace GymManagement.Forms
{




    public class frmQuanLyTaiKhoan : Form
    {
        private Guna2DataGridView dgv;
        private TextBox txtUsername, txtPassword, txtHoTen, txtEmail, txtTimKiem;
        private Guna2ComboBox cboVaiTro;
        private Guna2ComboBox cboNhanVien;
        private Guna2CheckBox chkTrangThai;
        private Guna2Button btnThem, btnSua, btnXoa, btnLamMoi, btnTimKiem;
        private int? selectedId = null;

        private static readonly Color ColorAccent = ColorTranslator.FromHtml("#4E7CFF");
        private static readonly Color ColorSuccess = ColorTranslator.FromHtml("#2ECC71");
        private static readonly Color ColorDanger = ColorTranslator.FromHtml("#E85C5C");
        private static readonly Color ColorNeutral = ColorTranslator.FromHtml("#95A5A6");
        private static readonly Color ColorDark = ColorTranslator.FromHtml("#1B2430");

        public frmQuanLyTaiKhoan()
        {
            InitializeComponent();
            LoadNhanVienComboBox();
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
                Text = "QUẢN LÝ TÀI KHOẢN",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = ColorDark,
                Location = new Point(20, 15),
                AutoSize = true
            };

            var pnlForm = new Guna2GroupBox
            {
                Text = "Thông tin tài khoản",
                Location = new Point(20, 55),
                Size = new Size(950, 200),
                Font = new Font("Segoe UI", 9.5f),
                FillColor = Color.White,
                BorderRadius = 10,
                ForeColor = ColorDark
            };

            Label MakeLabel(string text, Point loc) => new Label { Text = text, Location = loc, AutoSize = true, ForeColor = Color.Gray };
            TextBox MakeBox(Point loc, int width, string placeholder) =>
                new TextBox { Location = loc, Width = width, Font = new Font("Segoe UI", 9.5f), BorderStyle = BorderStyle.FixedSingle };

            pnlForm.Controls.Add(MakeLabel("Tên đăng nhập:", new Point(15, 35)));
            txtUsername = MakeBox(new Point(140, 32), 180, "vd: nv2");
            pnlForm.Controls.Add(txtUsername);

            pnlForm.Controls.Add(MakeLabel("Mật khẩu:", new Point(340, 35)));
            txtPassword = MakeBox(new Point(430, 32), 180, "để trống nếu không đổi");
            pnlForm.Controls.Add(txtPassword);

            pnlForm.Controls.Add(MakeLabel("Họ tên:", new Point(630, 35)));
            txtHoTen = MakeBox(new Point(700, 32), 220, "Họ và tên");
            pnlForm.Controls.Add(txtHoTen);

            pnlForm.Controls.Add(MakeLabel("Email:", new Point(15, 90)));
            txtEmail = MakeBox(new Point(140, 87), 180, "email@vd.com");
            pnlForm.Controls.Add(txtEmail);

            pnlForm.Controls.Add(MakeLabel("Vai trò:", new Point(340, 90)));
            cboVaiTro = new Guna2ComboBox { Location = new Point(430, 87), Width = 180, Height = 36, AutoSize = false, DropDownStyle = ComboBoxStyle.DropDownList, BorderRadius = 6 };
            cboVaiTro.Items.AddRange(new object[] { "Admin", "NhanVien" });
            cboVaiTro.SelectedIndex = 1;
            pnlForm.Controls.Add(cboVaiTro);

            chkTrangThai = new Guna2CheckBox { Text = "Hoạt động", Location = new Point(630, 90), Checked = true, AutoSize = true, CheckedState = { BorderColor = ColorAccent, FillColor = ColorAccent } };
            pnlForm.Controls.Add(chkTrangThai);

            pnlForm.Controls.Add(MakeLabel("Liên kết nhân viên:", new Point(15, 145)));
            cboNhanVien = new Guna2ComboBox { Location = new Point(140, 142), Width = 280, Height = 36, AutoSize = false, DropDownStyle = ComboBoxStyle.DropDownList, BorderRadius = 6 };
            pnlForm.Controls.Add(cboNhanVien);
            pnlForm.Controls.Add(new Label { Text = "(chỉ áp dụng khi vai trò là Nhân viên — dùng để nhận diện khi tự chấm công)", Location = new Point(430, 148), AutoSize = true, ForeColor = Color.Gray, Font = new Font("Segoe UI", 8.5f, FontStyle.Italic) });

            this.Controls.Add(pnlForm);

            btnThem = MakeButton("➕ Thêm", ColorSuccess, new Point(20, 270));
            btnSua = MakeButton("✏ Sửa", ColorAccent, new Point(140, 270));
            btnXoa = MakeButton("🗑 Xóa", ColorDanger, new Point(260, 270));
            btnLamMoi = MakeButton("🔄 Làm mới", ColorNeutral, new Point(380, 270));

            btnThem.Click += BtnThem_Click;
            btnSua.Click += BtnSua_Click;
            btnXoa.Click += BtnXoa_Click;
            btnLamMoi.Click += (s, e) => ClearForm();

            this.Controls.Add(btnThem);
            this.Controls.Add(btnSua);
            this.Controls.Add(btnXoa);
            this.Controls.Add(btnLamMoi);

            txtTimKiem = new TextBox { Location = new Point(650, 272), Width = 220, Font = new Font("Segoe UI", 9.5f), BorderStyle = BorderStyle.FixedSingle };
            txtTimKiem.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) LoadData(txtTimKiem.Text.Trim()); };
            btnTimKiem = MakeButton("🔍 Tìm", ColorDark, new Point(880, 270));
            btnTimKiem.Click += (s, e) => LoadData(txtTimKiem.Text.Trim());

            this.Controls.Add(txtTimKiem);
            this.Controls.Add(btnTimKiem);

            dgv = new Guna2DataGridView
            {
                Location = new Point(20, 320),
                Size = new Size(950, 300),
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

            this.Controls.Add(lblTitle);
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

        private void LoadNhanVienComboBox()
        {
            DataTable dt = DatabaseHelper.ExecuteQuery("SELECT MaNV, HoTen FROM NhanVien WHERE TrangThai = 1 ORDER BY HoTen");
            dt.Columns["MaNV"].AllowDBNull = true;
            DataRow blank = dt.NewRow();
            blank["MaNV"] = DBNull.Value;
            blank["HoTen"] = "(Không liên kết)";
            dt.Rows.InsertAt(blank, 0);

            cboNhanVien.DisplayMember = "HoTen";
            cboNhanVien.ValueMember = "MaNV";
            cboNhanVien.DataSource = dt;
            cboNhanVien.SelectedIndex = 0;
        }

        private void LoadData(string keyword = "")
        {
            string sql = @"SELECT tk.UserID, tk.TenDangNhap, tk.HoTen, tk.VaiTro, tk.Email, tk.TrangThai, tk.NgayTao, tk.MaNV,
                                   nv.HoTen AS NhanVienLienKet
                            FROM TaiKhoan tk
                            LEFT JOIN NhanVien nv ON tk.MaNV = nv.MaNV";
            SqlParameter[] pars = null;
            if (!string.IsNullOrEmpty(keyword))
            {
                sql += " WHERE tk.TenDangNhap LIKE @kw OR tk.HoTen LIKE @kw";
                pars = new[] { new SqlParameter("@kw", "%" + keyword + "%") };
            }
            sql += " ORDER BY tk.UserID DESC";

            DataTable dt = pars == null ? DatabaseHelper.ExecuteQuery(sql) : DatabaseHelper.ExecuteQuery(sql, pars);
            dgv.DataSource = dt;

            if (dgv.Columns.Contains("UserID")) dgv.Columns["UserID"].HeaderText = "ID";
            if (dgv.Columns.Contains("TenDangNhap")) dgv.Columns["TenDangNhap"].HeaderText = "Tên đăng nhập";
            if (dgv.Columns.Contains("HoTen")) dgv.Columns["HoTen"].HeaderText = "Họ tên";
            if (dgv.Columns.Contains("VaiTro")) dgv.Columns["VaiTro"].HeaderText = "Vai trò";
            if (dgv.Columns.Contains("TrangThai")) dgv.Columns["TrangThai"].HeaderText = "Hoạt động";
            if (dgv.Columns.Contains("NgayTao")) dgv.Columns["NgayTao"].HeaderText = "Ngày tạo";
            if (dgv.Columns.Contains("NhanVienLienKet")) dgv.Columns["NhanVienLienKet"].HeaderText = "Nhân viên liên kết";
            if (dgv.Columns.Contains("MaNV")) dgv.Columns["MaNV"].Visible = false;
        }

        private void Dgv_SelectionChanged(object sender, EventArgs e)
        {
            if (dgv.CurrentRow == null || dgv.CurrentRow.DataBoundItem == null) return;
            DataRowView row = (DataRowView)dgv.CurrentRow.DataBoundItem;

            selectedId = Convert.ToInt32(row["UserID"]);
            txtUsername.Text = row["TenDangNhap"].ToString();
            txtHoTen.Text = row["HoTen"].ToString();
            txtEmail.Text = row["Email"].ToString();
            cboVaiTro.SelectedItem = row["VaiTro"].ToString();
            chkTrangThai.Checked = Convert.ToBoolean(row["TrangThai"]);
            cboNhanVien.SelectedValue = row["MaNV"] == DBNull.Value ? (object)DBNull.Value : Convert.ToInt32(row["MaNV"]);
            if (cboNhanVien.SelectedIndex < 0) cboNhanVien.SelectedIndex = 0;
            txtPassword.Text = "";
        }

        private void BtnThem_Click(object sender, EventArgs e)
        {
            if (!ValidateForm(requirePassword: true)) return;
            try
            {
                string sql = @"INSERT INTO TaiKhoan (TenDangNhap, MatKhau, HoTen, VaiTro, Email, TrangThai, MaNV)
                                VALUES (@u,@p,@h,@v,@e,@t,@nv)";
                DatabaseHelper.ExecuteNonQuery(sql,
                    new SqlParameter("@u", txtUsername.Text.Trim()),
                    new SqlParameter("@p", txtPassword.Text.Trim()),
                    new SqlParameter("@h", txtHoTen.Text.Trim()),
                    new SqlParameter("@v", cboVaiTro.SelectedItem.ToString()),
                    new SqlParameter("@e", (object)txtEmail.Text.Trim() ?? DBNull.Value),
                    new SqlParameter("@t", chkTrangThai.Checked),
                    new SqlParameter("@nv", cboNhanVien.SelectedValue ?? DBNull.Value));

                MessageBox.Show("Thêm tài khoản thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearForm();
                LoadData();
            }
            catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601)
            {
                MessageBox.Show("Tên đăng nhập đã tồn tại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnSua_Click(object sender, EventArgs e)
        {
            if (selectedId == null)
            {
                MessageBox.Show("Vui lòng chọn tài khoản cần sửa.", "Thông báo");
                return;
            }
            if (!ValidateForm(requirePassword: false)) return;

            try
            {
                string sql = @"UPDATE TaiKhoan SET TenDangNhap=@u, HoTen=@h, VaiTro=@v, Email=@e, TrangThai=@t, MaNV=@nv
                                " + (string.IsNullOrEmpty(txtPassword.Text.Trim()) ? "" : ", MatKhau=@p") + @"
                                WHERE UserID=@id";

                var pars = new System.Collections.Generic.List<SqlParameter>
                {
                    new SqlParameter("@u", txtUsername.Text.Trim()),
                    new SqlParameter("@h", txtHoTen.Text.Trim()),
                    new SqlParameter("@v", cboVaiTro.SelectedItem.ToString()),
                    new SqlParameter("@e", (object)txtEmail.Text.Trim() ?? DBNull.Value),
                    new SqlParameter("@t", chkTrangThai.Checked),
                    new SqlParameter("@nv", cboNhanVien.SelectedValue ?? DBNull.Value),
                    new SqlParameter("@id", selectedId.Value)
                };
                if (!string.IsNullOrEmpty(txtPassword.Text.Trim()))
                    pars.Add(new SqlParameter("@p", txtPassword.Text.Trim()));

                DatabaseHelper.ExecuteNonQuery(sql, pars.ToArray());
                MessageBox.Show("Cập nhật thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            if (selectedId == null)
            {
                MessageBox.Show("Vui lòng chọn tài khoản cần xóa.", "Thông báo");
                return;
            }
            if (MessageBox.Show("Bạn có chắc muốn xóa tài khoản này?", "Xác nhận",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            try
            {
                DatabaseHelper.ExecuteNonQuery("DELETE FROM TaiKhoan WHERE UserID=@id",
                    new SqlParameter("@id", selectedId.Value));
                ClearForm();
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ValidateForm(bool requirePassword)
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text) || string.IsNullOrWhiteSpace(txtHoTen.Text))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ Tên đăng nhập và Họ tên.", "Thiếu thông tin");
                return false;
            }
            if (requirePassword && string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Vui lòng nhập mật khẩu.", "Thiếu thông tin");
                return false;
            }
            return true;
        }

        private void ClearForm()
        {
            selectedId = null;
            txtUsername.Clear();
            txtPassword.Clear();
            txtHoTen.Clear();
            txtEmail.Clear();
            cboVaiTro.SelectedIndex = 1;
            chkTrangThai.Checked = true;
            if (cboNhanVien.Items.Count > 0) cboNhanVien.SelectedIndex = 0;
            dgv.ClearSelection();
        }
    }
}
