using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using Guna.UI2.WinForms;
using GymManagement.DAL;

namespace GymManagement.Forms
{
    public class frmThongKeDoanhSo : Form
    {
        private Guna2ComboBox cboKhachHang, cboGoiTap, cboNhanVien;
        private Guna2NumericUpDown numSoLuong;
        private Label lblThanhTien;
        private Guna2Button btnLapHoaDon;

        private Guna2DateTimePicker dtpTu, dtpDen;
        private Guna2Button btnLoc;
        private Label lblTongDoanhSo;
        private Guna2DataGridView dgv;
        private Chart chart;

        private static readonly Color ColorAccent = ColorTranslator.FromHtml("#4E7CFF");
        private static readonly Color ColorSuccess = ColorTranslator.FromHtml("#2ECC71");
        private static readonly Color ColorWarning = ColorTranslator.FromHtml("#F5A623");
        private static readonly Color ColorDark = ColorTranslator.FromHtml("#1B2430");

        public frmThongKeDoanhSo()
        {
            InitializeComponent();
            LoadComboBoxes();
            LoadThongKe();
        }

        private void InitializeComponent()
        {
            this.AutoScaleMode = AutoScaleMode.None;
            this.Dock = DockStyle.Fill;
            this.BackColor = ColorTranslator.FromHtml("#F3F5F9");
            this.AutoScroll = true;

            var lblTitle = new Label
            {
                Text = "THỐNG KÊ DOANH SỐ GÓI TẬP",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = ColorDark,
                Location = new Point(20, 15),
                AutoSize = true
            };
            this.Controls.Add(lblTitle);

            var pnlBan = new Guna2GroupBox
            {
                Text = "Lập hóa đơn bán gói tập",
                Location = new Point(20, 55),
                Size = new Size(950, 145),
                Font = new Font("Segoe UI", 9.5f),
                FillColor = Color.White,
                BorderRadius = 10,
                ForeColor = ColorDark
            };

            pnlBan.Controls.Add(new Label { Text = "Khách hàng:", Location = new Point(15, 35), AutoSize = true, ForeColor = Color.Gray });
            cboKhachHang = new Guna2ComboBox { Location = new Point(110, 32), Width = 200, Height = 36, AutoSize = false, DropDownStyle = ComboBoxStyle.DropDownList, BorderRadius = 6 };
            pnlBan.Controls.Add(cboKhachHang);

            pnlBan.Controls.Add(new Label { Text = "Gói tập:", Location = new Point(330, 35), AutoSize = true, ForeColor = Color.Gray });
            cboGoiTap = new Guna2ComboBox { Location = new Point(400, 32), Width = 220, Height = 36, AutoSize = false, DropDownStyle = ComboBoxStyle.DropDownList, BorderRadius = 6 };
            cboGoiTap.SelectedIndexChanged += (s, e) => UpdateThanhTien();
            pnlBan.Controls.Add(cboGoiTap);

            pnlBan.Controls.Add(new Label { Text = "Nhân viên bán:", Location = new Point(640, 35), AutoSize = true, ForeColor = Color.Gray });
            cboNhanVien = new Guna2ComboBox { Location = new Point(750, 32), Width = 180, Height = 36, AutoSize = false, DropDownStyle = ComboBoxStyle.DropDownList, BorderRadius = 6 };
            pnlBan.Controls.Add(cboNhanVien);

            pnlBan.Controls.Add(new Label { Text = "Số lượng:", Location = new Point(15, 92), AutoSize = true, ForeColor = Color.Gray });
            numSoLuong = new Guna2NumericUpDown { Location = new Point(110, 89), Width = 80, Height = 36, AutoSize = false, Minimum = 1, Maximum = 100, Value = 1, BorderRadius = 6 };
            numSoLuong.ValueChanged += (s, e) => UpdateThanhTien();
            pnlBan.Controls.Add(numSoLuong);

            pnlBan.Controls.Add(new Label { Text = "Thành tiền:", Location = new Point(330, 92), AutoSize = true, ForeColor = Color.Gray });
            lblThanhTien = new Label { Text = "0 đ", Location = new Point(400, 90), AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = ColorTranslator.FromHtml("#E85C5C") };
            pnlBan.Controls.Add(lblThanhTien);

            btnLapHoaDon = new Guna2Button
            {
                Text = "💰 Lập hóa đơn",
                Location = new Point(750, 86),
                Size = new Size(180, 36),
                FillColor = ColorSuccess,
                ForeColor = Color.White,
                BorderRadius = 8,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                Animated = true,
                Cursor = Cursors.Hand
            };
            btnLapHoaDon.Click += BtnLapHoaDon_Click;
            pnlBan.Controls.Add(btnLapHoaDon);

            this.Controls.Add(pnlBan);

            var lblLoc = new Label { Text = "Từ ngày:", Location = new Point(20, 218), AutoSize = true, ForeColor = Color.Gray };
            dtpTu = new Guna2DateTimePicker { Location = new Point(90, 215), Width = 140, Height = 36, AutoSize = false, Format = DateTimePickerFormat.Short, Value = DateTime.Now.AddMonths(-1), BorderRadius = 6 };
            var lblDen = new Label { Text = "Đến ngày:", Location = new Point(250, 218), AutoSize = true, ForeColor = Color.Gray };
            dtpDen = new Guna2DateTimePicker { Location = new Point(325, 215), Width = 140, Height = 36, AutoSize = false, Format = DateTimePickerFormat.Short, Value = DateTime.Now, BorderRadius = 6 };
            btnLoc = new Guna2Button
            {
                Text = "🔍 Xem thống kê",
                Location = new Point(480, 213),
                Size = new Size(150, 34),
                FillColor = ColorAccent,
                ForeColor = Color.White,
                BorderRadius = 8,
                Font = new Font("Segoe UI", 9.5f),
                Animated = true,
                Cursor = Cursors.Hand
            };
            btnLoc.Click += (s, e) => LoadThongKe();

            lblTongDoanhSo = new Label
            {
                Text = "Tổng doanh số: 0 đ",
                Location = new Point(650, 218),
                AutoSize = true,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = ColorDark
            };

            this.Controls.Add(lblLoc);
            this.Controls.Add(dtpTu);
            this.Controls.Add(lblDen);
            this.Controls.Add(dtpDen);
            this.Controls.Add(btnLoc);
            this.Controls.Add(lblTongDoanhSo);

            var gridCard = new Guna2Panel
            {
                Location = new Point(20, 265),
                Size = new Size(950, 220),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                FillColor = Color.White,
                BorderRadius = 10,
                ShadowDecoration = { Enabled = true },
                Padding = new Padding(2)
            };
            dgv = new Guna2DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None
            };
            StyleGrid(dgv);
            gridCard.Controls.Add(dgv);
            this.Controls.Add(gridCard);

            var lblChart = new Label
            {
                Text = "Biểu đồ doanh số theo gói tập",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = ColorDark,
                Location = new Point(20, 500),
                AutoSize = true
            };
            this.Controls.Add(lblChart);

            var chartCard = new Guna2Panel
            {
                Location = new Point(20, 530),
                Size = new Size(950, 300),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                FillColor = Color.White,
                BorderRadius = 10,
                ShadowDecoration = { Enabled = true },
                Padding = new Padding(10)
            };
            chart = new Chart { Dock = DockStyle.Fill };
            ChartArea area = new ChartArea("area1");
            chart.ChartAreas.Add(area);
            chart.BackColor = Color.White;
            Series series = new Series("Doanh số") { ChartType = SeriesChartType.Bar, Color = ColorWarning };
            chart.Series.Add(series);
            chart.Legends.Add(new Legend("legend1"));
            chartCard.Controls.Add(chart);
            this.Controls.Add(chartCard);
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
            g.ThemeStyle.RowsStyle.Height = 32;
            g.ThemeStyle.AlternatingRowsStyle.BackColor = ColorTranslator.FromHtml("#F5F7FA");
        }

        private void LoadComboBoxes()
        {
            DataTable khachHang = DatabaseHelper.ExecuteQuery("SELECT MaKH, HoTen FROM KhachHang ORDER BY HoTen");
            cboKhachHang.DisplayMember = "HoTen";
            cboKhachHang.ValueMember = "MaKH";
            cboKhachHang.DataSource = khachHang;

            DataTable goiTap = DatabaseHelper.ExecuteQuery("SELECT MaGoi, TenGoi, DonGia FROM GoiTap WHERE TrangThai = 1 ORDER BY TenGoi");
            cboGoiTap.DisplayMember = "TenGoi";
            cboGoiTap.ValueMember = "MaGoi";
            cboGoiTap.DataSource = goiTap;

            DataTable nhanVien = DatabaseHelper.ExecuteQuery("SELECT MaNV, HoTen FROM NhanVien WHERE TrangThai = 1 ORDER BY HoTen");
            cboNhanVien.DisplayMember = "HoTen";
            cboNhanVien.ValueMember = "MaNV";
            cboNhanVien.DataSource = nhanVien;

            UpdateThanhTien();
        }

        private void UpdateThanhTien()
        {
            if (cboGoiTap.SelectedValue == null) return;
            DataRowView row = cboGoiTap.SelectedItem as DataRowView;
            if (row == null) return;
            decimal donGia = Convert.ToDecimal(row["DonGia"]);
            decimal thanhTien = donGia * numSoLuong.Value;
            lblThanhTien.Text = thanhTien.ToString("N0") + " đ";
        }

        private void BtnLapHoaDon_Click(object sender, EventArgs e)
        {
            if (cboKhachHang.SelectedValue == null || cboGoiTap.SelectedValue == null || cboNhanVien.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn đầy đủ khách hàng, gói tập và nhân viên.", "Thiếu thông tin");
                return;
            }
            try
            {
                DataRowView goiRow = (DataRowView)cboGoiTap.SelectedItem;
                decimal donGia = Convert.ToDecimal(goiRow["DonGia"]);
                decimal thanhTien = donGia * numSoLuong.Value;

                string sql = @"INSERT INTO HoaDon (MaKH, MaGoi, MaNV, NgayLap, SoLuong, ThanhTien)
                                VALUES (@kh, @goi, @nv, GETDATE(), @sl, @tt)";
                DatabaseHelper.ExecuteNonQuery(sql,
                    new SqlParameter("@kh", cboKhachHang.SelectedValue),
                    new SqlParameter("@goi", cboGoiTap.SelectedValue),
                    new SqlParameter("@nv", cboNhanVien.SelectedValue),
                    new SqlParameter("@sl", numSoLuong.Value),
                    new SqlParameter("@tt", thanhTien));

                MessageBox.Show("Lập hóa đơn thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                numSoLuong.Value = 1;
                LoadThongKe();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadThongKe()
        {
            try
            {
                string sql = @"
                    SELECT hd.MaHD, kh.HoTen AS KhachHang, gt.TenGoi, nv.HoTen AS NhanVien,
                           hd.NgayLap, hd.SoLuong, hd.ThanhTien
                    FROM HoaDon hd
                    JOIN KhachHang kh ON hd.MaKH = kh.MaKH
                    JOIN GoiTap gt ON hd.MaGoi = gt.MaGoi
                    JOIN NhanVien nv ON hd.MaNV = nv.MaNV
                    WHERE hd.NgayLap >= @tu AND hd.NgayLap < DATEADD(DAY,1,@den)
                    ORDER BY hd.NgayLap DESC";

                DataTable dt = DatabaseHelper.ExecuteQuery(sql,
                    new SqlParameter("@tu", dtpTu.Value.Date),
                    new SqlParameter("@den", dtpDen.Value.Date));

                dgv.DataSource = dt;
                var headers = new (string col, string header)[]
                {
                    ("MaHD","Mã HĐ"), ("KhachHang","Khách hàng"), ("TenGoi","Gói tập"),
                    ("NhanVien","Nhân viên bán"), ("NgayLap","Ngày lập"), ("SoLuong","SL"), ("ThanhTien","Thành tiền")
                };
                foreach (var (col, header) in headers)
                    if (dgv.Columns.Contains(col)) dgv.Columns[col].HeaderText = header;

                decimal tong = 0;
                foreach (DataRow r in dt.Rows) tong += Convert.ToDecimal(r["ThanhTien"]);
                lblTongDoanhSo.Text = "Tổng doanh số: " + tong.ToString("N0") + " đ";

                string sqlChart = @"
                    SELECT gt.TenGoi, SUM(hd.ThanhTien) AS DoanhSo
                    FROM HoaDon hd
                    JOIN GoiTap gt ON hd.MaGoi = gt.MaGoi
                    WHERE hd.NgayLap >= @tu AND hd.NgayLap < DATEADD(DAY,1,@den)
                    GROUP BY gt.TenGoi
                    ORDER BY DoanhSo DESC";
                DataTable dtChart = DatabaseHelper.ExecuteQuery(sqlChart,
                    new SqlParameter("@tu", dtpTu.Value.Date),
                    new SqlParameter("@den", dtpDen.Value.Date));

                chart.Series["Doanh số"].Points.Clear();
                foreach (DataRow row in dtChart.Rows)
                    chart.Series["Doanh số"].Points.AddXY(row["TenGoi"].ToString(), Convert.ToDouble(row["DoanhSo"]));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải thống kê: " + ex.Message, "Lỗi");
            }
        }
    }
}
