using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using Guna.UI2.WinForms;
using GymManagement.DAL;

namespace GymManagement.Forms
{





    public class frmQuanLyNguoiDung : Form
    {
        private Guna2DataGridView dgv;
        private TextBox txtHoTen, txtSDT, txtEmail, txtDiaChi, txtTimKiem;
        private Guna2ComboBox cboGioiTinh;
        private Guna2DateTimePicker dtpNgaySinh;
        private Guna2Button btnThem, btnSua, btnXoa, btnLamMoi, btnTimKiem;
        private int? selectedId = null;

        private static readonly Color ColorAccent = ColorTranslator.FromHtml("#4E7CFF");
        private static readonly Color ColorSuccess = ColorTranslator.FromHtml("#2ECC71");
        private static readonly Color ColorDanger = ColorTranslator.FromHtml("#E85C5C");
        private static readonly Color ColorNeutral = ColorTranslator.FromHtml("#95A5A6");
        private static readonly Color ColorDark = ColorTranslator.FromHtml("#1B2430");

        public frmQuanLyNguoiDung()
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
                Text = "QUẢN LÝ KHÁCH HÀNG",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = ColorDark,
                Location = new Point(20, 15),
                AutoSize = true
            };

            var pnlForm = new Guna2GroupBox
            {
                Text = "Thông tin khách hàng",
                Location = new Point(20, 55),
                Size = new Size(950, 155),
                Font = new Font("Segoe UI", 9.5f),
                FillColor = Color.White,
                BorderRadius = 10,
                ForeColor = ColorDark
            };

            Label MakeLabel(string text, Point loc) => new Label { Text = text, Location = loc, AutoSize = true, ForeColor = Color.Gray };
            TextBox MakeBox(Point loc, int width, string placeholder) =>
                new TextBox { Location = loc, Width = width, Font = new Font("Segoe UI", 9.5f), BorderStyle = BorderStyle.FixedSingle };

            pnlForm.Controls.Add(MakeLabel("Họ tên:", new Point(15, 35)));
            txtHoTen = MakeBox(new Point(100, 32), 230, "Họ và tên khách hàng");
            pnlForm.Controls.Add(txtHoTen);

            pnlForm.Controls.Add(MakeLabel("Giới tính:", new Point(350, 35)));
            cboGioiTinh = new Guna2ComboBox { Location = new Point(430, 32), Width = 120, Height = 36, AutoSize = false, DropDownStyle = ComboBoxStyle.DropDownList, BorderRadius = 6 };
            cboGioiTinh.Items.AddRange(new object[] { "Nam", "Nữ", "Khác" });
            cboGioiTinh.SelectedIndex = 0;
            pnlForm.Controls.Add(cboGioiTinh);

            pnlForm.Controls.Add(MakeLabel("Ngày sinh:", new Point(570, 35)));
            dtpNgaySinh = new Guna2DateTimePicker { Location = new Point(660, 32), Width = 150, Height = 36, AutoSize = false, Format = DateTimePickerFormat.Short, BorderRadius = 6 };
            pnlForm.Controls.Add(dtpNgaySinh);

            pnlForm.Controls.Add(MakeLabel("SĐT:", new Point(15, 92)));
            txtSDT = MakeBox(new Point(100, 89), 230, "090xxxxxxx");
            pnlForm.Controls.Add(txtSDT);

            pnlForm.Controls.Add(MakeLabel("Email:", new Point(350, 92)));
            txtEmail = MakeBox(new Point(430, 89), 230, "email@vd.com");
            pnlForm.Controls.Add(txtEmail);

            pnlForm.Controls.Add(MakeLabel("Địa chỉ:", new Point(680, 92)));
            txtDiaChi = MakeBox(new Point(750, 89), 190, "Địa chỉ");
            pnlForm.Controls.Add(txtDiaChi);

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

            txtTimKiem = new TextBox { Location = new Point(650, 227), Width = 220, Font = new Font("Segoe UI", 9.5f), BorderStyle = BorderStyle.FixedSingle };
            txtTimKiem.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) LoadData(txtTimKiem.Text.Trim()); };
            btnTimKiem = MakeButton("🔍 Tìm", ColorDark, new Point(880, 225));
            btnTimKiem.Click += (s, e) => LoadData(txtTimKiem.Text.Trim());

            this.Controls.Add(txtTimKiem);
            this.Controls.Add(btnTimKiem);

            dgv = new Guna2DataGridView
            {
                Location = new Point(20, 275),
                Size = new Size(950, 345),
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

        private void LoadData(string keyword = "")
        {
            string sql = "SELECT MaKH, HoTen, GioiTinh, NgaySinh, SDT, Email, DiaChi, NgayDangKy FROM KhachHang";
            SqlParameter[] pars = null;
            if (!string.IsNullOrEmpty(keyword))
            {
                sql += " WHERE HoTen LIKE @kw OR SDT LIKE @kw";
                pars = new[] { new SqlParameter("@kw", "%" + keyword + "%") };
            }
            sql += " ORDER BY MaKH DESC";

            DataTable dt = pars == null ? DatabaseHelper.ExecuteQuery(sql) : DatabaseHelper.ExecuteQuery(sql, pars);
            dgv.DataSource = dt;

            var headers = new (string col, string header)[]
            {
                ("MaKH","Mã KH"), ("HoTen","Họ tên"), ("GioiTinh","Giới tính"), ("NgaySinh","Ngày sinh"),
                ("SDT","SĐT"), ("Email","Email"), ("DiaChi","Địa chỉ"), ("NgayDangKy","Ngày đăng ký")
            };
            foreach (var (col, header) in headers)
                if (dgv.Columns.Contains(col)) dgv.Columns[col].HeaderText = header;
        }

        private void Dgv_SelectionChanged(object sender, EventArgs e)
        {
            if (dgv.CurrentRow == null || dgv.CurrentRow.DataBoundItem == null) return;
            DataRowView row = (DataRowView)dgv.CurrentRow.DataBoundItem;

            selectedId = Convert.ToInt32(row["MaKH"]);
            txtHoTen.Text = row["HoTen"].ToString();
            cboGioiTinh.SelectedItem = row["GioiTinh"].ToString();
            if (DateTime.TryParse(row["NgaySinh"].ToString(), out DateTime ns)) dtpNgaySinh.Value = ns;
            txtSDT.Text = row["SDT"].ToString();
            txtEmail.Text = row["Email"].ToString();
            txtDiaChi.Text = row["DiaChi"].ToString();
        }

        private void BtnThem_Click(object sender, EventArgs e)
        {
            if (!ValidateForm()) return;
            try
            {
                string sql = @"INSERT INTO KhachHang (HoTen, GioiTinh, NgaySinh, SDT, Email, DiaChi, NgayDangKy)
                                VALUES (@ht,@gt,@ns,@sdt,@email,@dc,GETDATE())";
                DatabaseHelper.ExecuteNonQuery(sql, BuildParams());
                MessageBox.Show("Thêm khách hàng thành công! Khách hàng đã có thể chọn khi lập hóa đơn ở mục Thống kê doanh số.",
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            if (selectedId == null) { MessageBox.Show("Vui lòng chọn khách hàng cần sửa."); return; }
            if (!ValidateForm()) return;
            try
            {
                string sql = @"UPDATE KhachHang SET HoTen=@ht, GioiTinh=@gt, NgaySinh=@ns, SDT=@sdt, Email=@email, DiaChi=@dc
                                WHERE MaKH=@id";
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
            if (selectedId == null) { MessageBox.Show("Vui lòng chọn khách hàng cần xóa."); return; }
            if (MessageBox.Show("Bạn có chắc muốn xóa khách hàng này?", "Xác nhận",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            try
            {
                DatabaseHelper.ExecuteNonQuery("DELETE FROM KhachHang WHERE MaKH=@id", new SqlParameter("@id", selectedId.Value));
                ClearForm();
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể xóa (khách hàng có thể đang có hóa đơn liên quan): " + ex.Message, "Lỗi");
            }
        }

        private SqlParameter[] BuildParams()
        {
            return new[]
            {
                new SqlParameter("@ht", txtHoTen.Text.Trim()),
                new SqlParameter("@gt", cboGioiTinh.SelectedItem?.ToString() ?? "Nam"),
                new SqlParameter("@ns", dtpNgaySinh.Value.Date),
                new SqlParameter("@sdt", txtSDT.Text.Trim()),
                new SqlParameter("@email", (object)txtEmail.Text.Trim() ?? DBNull.Value),
                new SqlParameter("@dc", (object)txtDiaChi.Text.Trim() ?? DBNull.Value)
            };
        }

        private bool ValidateForm()
        {
            if (string.IsNullOrWhiteSpace(txtHoTen.Text))
            {
                MessageBox.Show("Vui lòng nhập họ tên khách hàng.", "Thiếu thông tin");
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtSDT.Text))
            {
                MessageBox.Show("Vui lòng nhập số điện thoại khách hàng.", "Thiếu thông tin");
                return false;
            }
            return true;
        }

        private void ClearForm()
        {
            selectedId = null;
            txtHoTen.Clear(); txtSDT.Clear(); txtEmail.Clear(); txtDiaChi.Clear();
            cboGioiTinh.SelectedIndex = 0;
            dtpNgaySinh.Value = DateTime.Now.AddYears(-20);
            dgv.ClearSelection();
        }
    }
}
