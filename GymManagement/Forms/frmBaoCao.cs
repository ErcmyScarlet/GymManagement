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
    public class frmBaoCao : Form
    {
        private Guna2ComboBox cboNam;
        private Guna2Button btnXem;
        private Chart chartDoanhThuThang;
        private Chart chartTyLeGoiTap;
        private Chart chartNhanVien;

        private static readonly Color ColorAccent = ColorTranslator.FromHtml("#4E7CFF");
        private static readonly Color ColorSuccess = ColorTranslator.FromHtml("#2ECC71");
        private static readonly Color ColorDark = ColorTranslator.FromHtml("#1B2430");

        public frmBaoCao()
        {
            InitializeComponent();
            LoadNamComboBox();
            LoadBaoCao();
        }

        private void InitializeComponent()
        {
            this.AutoScaleMode = AutoScaleMode.None;
            this.Dock = DockStyle.Fill;
            this.BackColor = ColorTranslator.FromHtml("#F3F5F9");
            this.AutoScroll = true;

            var lblTitle = new Label
            {
                Text = "BÁO CÁO TỔNG HỢP",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = ColorDark,
                Location = new Point(20, 15),
                AutoSize = true
            };
            this.Controls.Add(lblTitle);

            var lblNam = new Label { Text = "Chọn năm:", Location = new Point(20, 60), AutoSize = true, ForeColor = Color.Gray };
            cboNam = new Guna2ComboBox { Location = new Point(100, 57), Width = 100, Height = 36, AutoSize = false, DropDownStyle = ComboBoxStyle.DropDownList, BorderRadius = 6 };
            btnXem = new Guna2Button
            {
                Text = "📊 Xem báo cáo",
                Location = new Point(220, 55),
                Size = new Size(140, 34),
                FillColor = ColorAccent,
                ForeColor = Color.White,
                BorderRadius = 8,
                Font = new Font("Segoe UI", 9.5f),
                Animated = true,
                Cursor = Cursors.Hand
            };
            btnXem.Click += (s, e) => LoadBaoCao();

            this.Controls.Add(lblNam);
            this.Controls.Add(cboNam);
            this.Controls.Add(btnXem);
            var lbl1 = new Label { Text = "Doanh thu theo tháng", Font = new Font("Segoe UI", 11, FontStyle.Bold), ForeColor = ColorDark, Location = new Point(20, 105), AutoSize = true };
            var cardChart1 = new Guna2Panel
            {
                Location = new Point(20, 135),
                Size = new Size(950, 290),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                FillColor = Color.White,
                BorderRadius = 10,
                ShadowDecoration = { Enabled = true },
                Padding = new Padding(10)
            };
            chartDoanhThuThang = new Chart { Dock = DockStyle.Fill };
            var area1 = new ChartArea("a1");
            chartDoanhThuThang.ChartAreas.Add(area1);
            chartDoanhThuThang.BackColor = Color.White;
            var s1 = new Series("Doanh thu") { ChartType = SeriesChartType.Spline, Color = ColorAccent, BorderWidth = 3, MarkerStyle = MarkerStyle.Circle, MarkerSize = 7 };
            chartDoanhThuThang.Series.Add(s1);
            cardChart1.Controls.Add(chartDoanhThuThang);

            this.Controls.Add(lbl1);
            this.Controls.Add(cardChart1);

            var lbl2 = new Label { Text = "Tỉ lệ doanh thu theo gói tập", Font = new Font("Segoe UI", 11, FontStyle.Bold), ForeColor = ColorDark, Location = new Point(20, 440), AutoSize = true };
            var cardChart2 = new Guna2Panel
            {
                Location = new Point(20, 470),
                Size = new Size(460, 330),
                FillColor = Color.White,
                BorderRadius = 10,
                ShadowDecoration = { Enabled = true },
                Padding = new Padding(10)
            };
            chartTyLeGoiTap = new Chart { Dock = DockStyle.Fill };
            var area2 = new ChartArea("a2");
            chartTyLeGoiTap.ChartAreas.Add(area2);
            chartTyLeGoiTap.BackColor = Color.White;
            var s2 = new Series("TyLe") { ChartType = SeriesChartType.Pie };
            s2["PieLabelStyle"] = "OutsideInColumn";
            s2.IsValueShownAsLabel = true;
            chartTyLeGoiTap.Series.Add(s2);
            chartTyLeGoiTap.Legends.Add(new Legend("l2"));
            cardChart2.Controls.Add(chartTyLeGoiTap);

            this.Controls.Add(lbl2);
            this.Controls.Add(cardChart2);

            var lbl3 = new Label { Text = "Doanh số theo nhân viên", Font = new Font("Segoe UI", 11, FontStyle.Bold), ForeColor = ColorDark, Location = new Point(510, 440), AutoSize = true };
            var cardChart3 = new Guna2Panel
            {
                Location = new Point(510, 470),
                Size = new Size(460, 330),
                FillColor = Color.White,
                BorderRadius = 10,
                ShadowDecoration = { Enabled = true },
                Padding = new Padding(10)
            };
            chartNhanVien = new Chart { Dock = DockStyle.Fill };
            var area3 = new ChartArea("a3");
            chartNhanVien.ChartAreas.Add(area3);
            chartNhanVien.BackColor = Color.White;
            var s3 = new Series("DoanhSo") { ChartType = SeriesChartType.Column, Color = ColorSuccess };
            chartNhanVien.Series.Add(s3);
            cardChart3.Controls.Add(chartNhanVien);

            this.Controls.Add(lbl3);
            this.Controls.Add(cardChart3);
        }

        private void LoadNamComboBox()
        {
            int currentYear = DateTime.Now.Year;
            for (int y = currentYear; y >= currentYear - 5; y--)
                cboNam.Items.Add(y);
            cboNam.SelectedIndex = 0;
        }

        private void LoadBaoCao()
        {
            if (cboNam.SelectedItem == null) return;
            int nam = (int)cboNam.SelectedItem;

            try
            {
                string sqlThang = @"
                    SELECT MONTH(NgayLap) AS Thang, SUM(ThanhTien) AS DoanhThu
                    FROM HoaDon
                    WHERE YEAR(NgayLap) = @nam
                    GROUP BY MONTH(NgayLap)
                    ORDER BY MONTH(NgayLap)";
                DataTable dtThang = DatabaseHelper.ExecuteQuery(sqlThang, new SqlParameter("@nam", nam));

                chartDoanhThuThang.Series["Doanh thu"].Points.Clear();
                for (int m = 1; m <= 12; m++)
                {
                    DataRow[] rows = dtThang.Select("Thang = " + m);
                    double doanhThu = rows.Length > 0 ? Convert.ToDouble(rows[0]["DoanhThu"]) : 0;
                    chartDoanhThuThang.Series["Doanh thu"].Points.AddXY("Tháng " + m, doanhThu);
                }

                string sqlGoi = @"
                    SELECT gt.TenGoi, SUM(hd.ThanhTien) AS DoanhThu
                    FROM HoaDon hd JOIN GoiTap gt ON hd.MaGoi = gt.MaGoi
                    WHERE YEAR(hd.NgayLap) = @nam
                    GROUP BY gt.TenGoi
                    ORDER BY DoanhThu DESC";
                DataTable dtGoi = DatabaseHelper.ExecuteQuery(sqlGoi, new SqlParameter("@nam", nam));

                chartTyLeGoiTap.Series["TyLe"].Points.Clear();
                foreach (DataRow row in dtGoi.Rows)
                    chartTyLeGoiTap.Series["TyLe"].Points.AddXY(row["TenGoi"].ToString(), Convert.ToDouble(row["DoanhThu"]));

                string sqlNv = @"
                    SELECT nv.HoTen, SUM(hd.ThanhTien) AS DoanhThu
                    FROM HoaDon hd JOIN NhanVien nv ON hd.MaNV = nv.MaNV
                    WHERE YEAR(hd.NgayLap) = @nam
                    GROUP BY nv.HoTen
                    ORDER BY DoanhThu DESC";
                DataTable dtNv = DatabaseHelper.ExecuteQuery(sqlNv, new SqlParameter("@nam", nam));

                chartNhanVien.Series["DoanhSo"].Points.Clear();
                foreach (DataRow row in dtNv.Rows)
                    chartNhanVien.Series["DoanhSo"].Points.AddXY(row["HoTen"].ToString(), Convert.ToDouble(row["DoanhThu"]));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải báo cáo: " + ex.Message, "Lỗi");
            }
        }
    }
}
