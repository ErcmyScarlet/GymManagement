using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using Guna.UI2.WinForms;
using GymManagement.DAL;

namespace GymManagement.Forms
{
    public class frmQuanLyNhanVien : Form
    {
        private Guna2DataGridView dgv;
        private TextBox txtHoTen, txtSDT, txtEmail, txtDiaChi, txtChucVu, txtLuong;
        private Guna2ComboBox cboGioiTinh;
        private Guna2DateTimePicker dtpNgaySinh, dtpNgayVaoLam;
        private Guna2CheckBox chkTrangThai;
        private Guna2Button btnThem, btnSua, btnXoa, btnLamMoi;
        private int? selectedId = null;

        private static readonly Color ColorAccent = ColorTranslator.FromHtml("#4E7CFF");
        private static readonly Color ColorSuccess = ColorTranslator.FromHtml("#2ECC71");
        private static readonly Color ColorDanger = ColorTranslator.FromHtml("#E85C5C");
        private static readonly Color ColorNeutral = ColorTranslator.FromHtml("#95A5A6");
        private static readonly Color ColorDark = ColorTranslator.FromHtml("#1B2430");

        public frmQuanLyNhanVien()
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
                Text = "QUẢN LÝ NHÂN VIÊN",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = ColorDark,
                Location = new Point(20, 15),
                AutoSize = true
            };
            this.Controls.Add(lblTitle);

            var pnlForm = new Guna2GroupBox
            {
                Text = "Thông tin nhân viên",
                Location = new Point(20, 55),
                Size = new Size(950, 215),
                Font = new Font("Segoe UI", 9.5f),
                FillColor = Color.White,
                BorderRadius = 10,
                ForeColor = ColorDark
            };

            Label MakeLabel(string text, Point loc) => new Label { Text = text, Location = loc, AutoSize = true, ForeColor = Color.Gray };
            TextBox MakeBox(Point loc, int width) => new TextBox { Location = loc, Width = width, Font = new Font("Segoe UI", 9.5f), BorderStyle = BorderStyle.FixedSingle };

            pnlForm.Controls.Add(MakeLabel("Họ tên:", new Point(15, 35)));
            txtHoTen = MakeBox(new Point(100, 32), 220);
            pnlForm.Controls.Add(txtHoTen);

            pnlForm.Controls.Add(MakeLabel("Giới tính:", new Point(340, 35)));
            cboGioiTinh = new Guna2ComboBox { Location = new Point(420, 32), Width = 120, Height = 36, AutoSize = false, DropDownStyle = ComboBoxStyle.DropDownList, BorderRadius = 6 };
            cboGioiTinh.Items.AddRange(new object[] { "Nam", "Nữ", "Khác" });
            cboGioiTinh.SelectedIndex = 0;
            pnlForm.Controls.Add(cboGioiTinh);

            pnlForm.Controls.Add(MakeLabel("Ngày sinh:", new Point(560, 35)));
            dtpNgaySinh = new Guna2DateTimePicker { Location = new Point(650, 32), Width = 150, Height = 36, AutoSize = false, Format = DateTimePickerFormat.Short, BorderRadius = 6 };
            pnlForm.Controls.Add(dtpNgaySinh);

            pnlForm.Controls.Add(MakeLabel("SĐT:", new Point(15, 78)));
            txtSDT = MakeBox(new Point(100, 75), 220);
            pnlForm.Controls.Add(txtSDT);

            pnlForm.Controls.Add(MakeLabel("Email:", new Point(340, 78)));
            txtEmail = MakeBox(new Point(420, 75), 220);
            pnlForm.Controls.Add(txtEmail);

            pnlForm.Controls.Add(MakeLabel("Chức vụ:", new Point(650, 78)));
            txtChucVu = MakeBox(new Point(720, 75), 150);
            pnlForm.Controls.Add(txtChucVu);

            pnlForm.Controls.Add(MakeLabel("Địa chỉ:", new Point(15, 118)));
            txtDiaChi = MakeBox(new Point(100, 115), 340);
            pnlForm.Controls.Add(txtDiaChi);

            pnlForm.Controls.Add(MakeLabel("Đơn giá/ca:", new Point(460, 118)));
            txtLuong = MakeBox(new Point(520, 115), 130);
            pnlForm.Controls.Add(txtLuong);

            pnlForm.Controls.Add(MakeLabel("Ngày vào làm:", new Point(660, 118)));
            dtpNgayVaoLam = new Guna2DateTimePicker { Location = new Point(770, 115), Width = 150, Height = 36, AutoSize = false, Format = DateTimePickerFormat.Short, BorderRadius = 6 };
            pnlForm.Controls.Add(dtpNgayVaoLam);

            chkTrangThai = new Guna2CheckBox { Text = "Đang làm việc", Location = new Point(15, 170), Checked = true, AutoSize = true, CheckedState = { BorderColor = ColorAccent, FillColor = ColorAccent } };
            pnlForm.Controls.Add(chkTrangThai);

            this.Controls.Add(pnlForm);

            btnThem = MakeButton("➕ Thêm", ColorSuccess, new Point(20, 285));
            btnSua = MakeButton("✏ Sửa", ColorAccent, new Point(140, 285));
            btnXoa = MakeButton("🗑 Xóa", ColorDanger, new Point(260, 285));
            btnLamMoi = MakeButton("🔄 Làm mới", ColorNeutral, new Point(380, 285));

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
                Location = new Point(20, 330),
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
            string sql = @"SELECT MaNV, HoTen, GioiTinh, NgaySinh, SDT, Email, DiaChi, ChucVu, Luong, NgayVaoLam, TrangThai
                            FROM NhanVien ORDER BY MaNV DESC";
            DataTable dt = DatabaseHelper.ExecuteQuery(sql);
            dgv.DataSource = dt;

            var headers = new (string col, string header)[]
            {
                ("MaNV","Mã NV"), ("HoTen","Họ tên"), ("GioiTinh","Giới tính"), ("NgaySinh","Ngày sinh"),
                ("SDT","SĐT"), ("Email","Email"), ("DiaChi","Địa chỉ"), ("ChucVu","Chức vụ"),
                ("Luong","Đơn giá/ca"), ("NgayVaoLam","Ngày vào làm"), ("TrangThai","Đang làm")
            };
            foreach (var (col, header) in headers)
                if (dgv.Columns.Contains(col)) dgv.Columns[col].HeaderText = header;
        }

        private void Dgv_SelectionChanged(object sender, EventArgs e)
        {
            if (dgv.CurrentRow == null || dgv.CurrentRow.DataBoundItem == null) return;
            DataRowView row = (DataRowView)dgv.CurrentRow.DataBoundItem;

            selectedId = Convert.ToInt32(row["MaNV"]);
            txtHoTen.Text = row["HoTen"].ToString();
            cboGioiTinh.SelectedItem = row["GioiTinh"].ToString();
            if (DateTime.TryParse(row["NgaySinh"].ToString(), out DateTime ns)) dtpNgaySinh.Value = ns;
            txtSDT.Text = row["SDT"].ToString();
            txtEmail.Text = row["Email"].ToString();
            txtDiaChi.Text = row["DiaChi"].ToString();
            txtChucVu.Text = row["ChucVu"].ToString();
            txtLuong.Text = row["Luong"].ToString();
            if (DateTime.TryParse(row["NgayVaoLam"].ToString(), out DateTime nvl)) dtpNgayVaoLam.Value = nvl;
            chkTrangThai.Checked = Convert.ToBoolean(row["TrangThai"]);
        }

        private void BtnThem_Click(object sender, EventArgs e)
        {
            if (!ValidateForm()) return;
            try
            {
                string sql = @"INSERT INTO NhanVien (HoTen, GioiTinh, NgaySinh, SDT, Email, DiaChi, ChucVu, Luong, NgayVaoLam, TrangThai)
                                VALUES (@ht,@gt,@ns,@sdt,@email,@dc,@cv,@luong,@nvl,@tt)";
                DatabaseHelper.ExecuteNonQuery(sql, BuildParams());
                MessageBox.Show("Thêm nhân viên thành công!", "Thông báo");
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
            if (selectedId == null) { MessageBox.Show("Vui lòng chọn nhân viên cần sửa."); return; }
            if (!ValidateForm()) return;
            try
            {
                string sql = @"UPDATE NhanVien SET HoTen=@ht, GioiTinh=@gt, NgaySinh=@ns, SDT=@sdt, Email=@email,
                                DiaChi=@dc, ChucVu=@cv, Luong=@luong, NgayVaoLam=@nvl, TrangThai=@tt WHERE MaNV=@id";
                var pars = BuildParams();
                Array.Resize(ref pars, pars.Length + 1);
                pars[pars.Length - 1] = new SqlParameter("@id", selectedId.Value);
                DatabaseHelper.ExecuteNonQuery(sql, pars);
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
            if (selectedId == null) { MessageBox.Show("Vui lòng chọn nhân viên cần xóa."); return; }
            if (MessageBox.Show("Bạn có chắc muốn xóa nhân viên này?", "Xác nhận",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            try
            {
                DatabaseHelper.ExecuteNonQuery("DELETE FROM NhanVien WHERE MaNV=@id", new SqlParameter("@id", selectedId.Value));
                ClearForm();
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể xóa (nhân viên có thể đang có hóa đơn liên quan): " + ex.Message, "Lỗi");
            }
        }

        private SqlParameter[] BuildParams()
        {
            decimal.TryParse(txtLuong.Text.Trim(), out decimal luong);
            return new[]
            {
                new SqlParameter("@ht", txtHoTen.Text.Trim()),
                new SqlParameter("@gt", cboGioiTinh.SelectedItem?.ToString() ?? "Nam"),
                new SqlParameter("@ns", dtpNgaySinh.Value.Date),
                new SqlParameter("@sdt", txtSDT.Text.Trim()),
                new SqlParameter("@email", (object)txtEmail.Text.Trim() ?? DBNull.Value),
                new SqlParameter("@dc", (object)txtDiaChi.Text.Trim() ?? DBNull.Value),
                new SqlParameter("@cv", (object)txtChucVu.Text.Trim() ?? DBNull.Value),
                new SqlParameter("@luong", luong),
                new SqlParameter("@nvl", dtpNgayVaoLam.Value.Date),
                new SqlParameter("@tt", chkTrangThai.Checked)
            };
        }

        private bool ValidateForm()
        {
            if (string.IsNullOrWhiteSpace(txtHoTen.Text))
            {
                MessageBox.Show("Vui lòng nhập họ tên nhân viên.", "Thiếu thông tin");
                return false;
            }
            if (!string.IsNullOrWhiteSpace(txtLuong.Text) && !decimal.TryParse(txtLuong.Text.Trim(), out _))
            {
                MessageBox.Show("Lương phải là số.", "Sai định dạng");
                return false;
            }
            return true;
        }

        private void ClearForm()
        {
            selectedId = null;
            txtHoTen.Clear(); txtSDT.Clear(); txtEmail.Clear(); txtDiaChi.Clear(); txtChucVu.Clear(); txtLuong.Clear();
            cboGioiTinh.SelectedIndex = 0;
            dtpNgaySinh.Value = DateTime.Now.AddYears(-20);
            dtpNgayVaoLam.Value = DateTime.Now;
            chkTrangThai.Checked = true;
            dgv.ClearSelection();
        }
    }
}
