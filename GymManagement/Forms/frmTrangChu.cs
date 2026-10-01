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
    public class frmTrangChu : Form
    {
        private Chart chart;
        private Guna2DataGridView dgvHetHan;
        private Label lblSoLuongHetHan;

        private static readonly Color ColorAccent = ColorTranslator.FromHtml("#4E7CFF");
        private static readonly Color ColorSuccess = ColorTranslator.FromHtml("#2ECC71");
        private static readonly Color ColorWarning = ColorTranslator.FromHtml("#F5A623");
        private static readonly Color ColorDanger = ColorTranslator.FromHtml("#E85C5C");
        private static readonly Color ColorDark = ColorTranslator.FromHtml("#1B2430");

        public frmTrangChu()
        {
            InitializeComponent();
            LoadDashboard();
        }

        private void InitializeComponent()
        {
            this.AutoScaleMode = AutoScaleMode.None;
            this.BackColor = ColorTranslator.FromHtml("#F3F5F9");
            this.Dock = DockStyle.Fill;
            this.AutoScroll = true;

            var lblTitle = new Label
            {
                Text = "TRANG CHỦ - TỔNG QUAN HỆ THỐNG",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = ColorDark,
                Location = new Point(20, 15),
                AutoSize = true
            };
            this.Controls.Add(lblTitle);

            int cardWidth = 240, cardHeight = 110, gap = 20, startX = 20, startY = 60;

            Guna2Panel card1 = CreateCard("Tổng hội viên", "0", ColorAccent, new Point(startX, startY));
            Guna2Panel card2 = CreateCard("Nhân viên", "0", ColorSuccess, new Point(startX + (cardWidth + gap), startY));
            Guna2Panel card3 = CreateCard("Gói tập đang bán", "0", ColorWarning, new Point(startX + 2 * (cardWidth + gap), startY));
            Guna2Panel card4 = CreateCard("Doanh số tháng này", "0", ColorDanger, new Point(startX + 3 * (cardWidth + gap), startY));

            card1.Name = "cardHoiVien";
            card2.Name = "cardNhanVien";
            card3.Name = "cardGoiTap";
            card4.Name = "cardDoanhSo";

            this.Controls.Add(card1);
            this.Controls.Add(card2);
            this.Controls.Add(card3);
            this.Controls.Add(card4);

            var lblChart = new Label
            {
                Text = "Doanh số 6 tháng gần nhất",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = ColorDark,
                Location = new Point(20, startY + cardHeight + 20),
                AutoSize = true
            };
            this.Controls.Add(lblChart);

            var chartCard = new Guna2Panel
            {
                Location = new Point(20, startY + cardHeight + 50),
                Size = new Size(950, 360),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                FillColor = Color.White,
                BorderRadius = 12,
                ShadowDecoration = { Enabled = true },
                Padding = new Padding(10)
            };

            chart = new Chart { Dock = DockStyle.Fill };
            ChartArea area = new ChartArea("area1");
            area.AxisX.MajorGrid.LineColor = Color.WhiteSmoke;
            area.AxisY.MajorGrid.LineColor = Color.WhiteSmoke;
            chart.ChartAreas.Add(area);
            chart.BackColor = Color.White;

            Series series = new Series("Doanh số")
            {
                ChartType = SeriesChartType.Column,
                Color = ColorAccent
            };
            chart.Series.Add(series);
            chartCard.Controls.Add(chart);
            this.Controls.Add(chartCard);

            
            int canhBaoY = startY + cardHeight + 50 + 360 + 20;
            var lblCanhBao = new Label
            {
                Text = "⚠ Hội viên đã hết hạn gói tập",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = ColorDanger,
                Location = new Point(20, canhBaoY),
                AutoSize = true
            };
            this.Controls.Add(lblCanhBao);

            lblSoLuongHetHan = new Label
            {
                Text = "",
                Font = new Font("Segoe UI", 9.5f, FontStyle.Italic),
                ForeColor = Color.Gray,
                Location = new Point(300, canhBaoY + 2),
                AutoSize = true
            };
            this.Controls.Add(lblSoLuongHetHan);

            var cardHetHan = new Guna2Panel
            {
                Location = new Point(20, canhBaoY + 30),
                Size = new Size(950, 220),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
                FillColor = Color.White,
                BorderRadius = 12,
                ShadowDecoration = { Enabled = true },
                Padding = new Padding(2)
            };
            dgvHetHan = new Guna2DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None
            };
            dgvHetHan.ThemeStyle.HeaderStyle.BackColor = ColorDanger;
            dgvHetHan.ThemeStyle.HeaderStyle.ForeColor = Color.White;
            dgvHetHan.ThemeStyle.HeaderStyle.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
            dgvHetHan.ThemeStyle.HeaderStyle.Height = 38;
            dgvHetHan.ThemeStyle.RowsStyle.BackColor = Color.White;
            dgvHetHan.ThemeStyle.RowsStyle.ForeColor = ColorDark;
            dgvHetHan.ThemeStyle.RowsStyle.Height = 32;
            dgvHetHan.ThemeStyle.AlternatingRowsStyle.BackColor = ColorTranslator.FromHtml("#FDEDEC");
            cardHetHan.Controls.Add(dgvHetHan);
            this.Controls.Add(cardHetHan);
        }

        private Guna2Panel CreateCard(string title, string value, Color color, Point location)
        {
            var panel = new Guna2Panel
            {
                Location = location,
                Size = new Size(240, 110),
                FillColor = color,
                BorderRadius = 12,
                ShadowDecoration = { Enabled = true }
            };

            var lblTitle = new Label
            {
                Text = title,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10),
                Location = new Point(18, 16),
                AutoSize = true
            };
            var lblValue = new Label
            {
                Text = value,
                Name = "lblValue",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                Location = new Point(18, 46),
                AutoSize = true
            };

            panel.Controls.Add(lblValue);
            panel.Controls.Add(lblTitle);
            return panel;
        }

        private void LoadDashboard()
        {
            try
            {
                int soHoiVien = Convert.ToInt32(DatabaseHelper.ExecuteScalar("SELECT COUNT(*) FROM KhachHang"));
                int soNhanVien = Convert.ToInt32(DatabaseHelper.ExecuteScalar("SELECT COUNT(*) FROM NhanVien WHERE TrangThai = 1"));
                int soGoiTap = Convert.ToInt32(DatabaseHelper.ExecuteScalar("SELECT COUNT(*) FROM GoiTap WHERE TrangThai = 1"));
                object doanhSoObj = DatabaseHelper.ExecuteScalar(
                    "SELECT ISNULL(SUM(ThanhTien),0) FROM HoaDon WHERE MONTH(NgayLap)=MONTH(GETDATE()) AND YEAR(NgayLap)=YEAR(GETDATE())");
                decimal doanhSo = Convert.ToDecimal(doanhSoObj);

                SetCardValue("cardHoiVien", soHoiVien.ToString("N0"));
                SetCardValue("cardNhanVien", soNhanVien.ToString("N0"));
                SetCardValue("cardGoiTap", soGoiTap.ToString("N0"));
                SetCardValue("cardDoanhSo", doanhSo.ToString("N0") + " đ");

                string sql = @"
                    SELECT FORMAT(NgayLap,'MM/yyyy') AS Thang, SUM(ThanhTien) AS DoanhSo
                    FROM HoaDon
                    WHERE NgayLap >= DATEADD(MONTH,-5, DATEFROMPARTS(YEAR(GETDATE()),MONTH(GETDATE()),1))
                    GROUP BY FORMAT(NgayLap,'MM/yyyy'), YEAR(NgayLap), MONTH(NgayLap)
                    ORDER BY YEAR(NgayLap), MONTH(NgayLap)";
                DataTable dt = DatabaseHelper.ExecuteQuery(sql);

                chart.Series["Doanh số"].Points.Clear();
                foreach (DataRow row in dt.Rows)
                {
                    chart.Series["Doanh số"].Points.AddXY(row["Thang"].ToString(), Convert.ToDouble(row["DoanhSo"]));
                }

                LoadHetHanGoiTap();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải dữ liệu trang chủ: " + ex.Message);
            }
        }

        private void LoadHetHanGoiTap()
        {
            try
            {
                
                string sql = @"
                    ;WITH LastHoaDon AS (
                        SELECT hd.MaKH, hd.NgayLap, gt.SoThang, gt.TenGoi,
                               ROW_NUMBER() OVER (PARTITION BY hd.MaKH ORDER BY hd.NgayLap DESC) AS rn
                        FROM HoaDon hd
                        JOIN GoiTap gt ON hd.MaGoi = gt.MaGoi
                    )
                    SELECT kh.HoTen AS [Hội viên], kh.SDT AS [SĐT], l.TenGoi AS [Gói đã mua],
                           l.NgayLap AS [Ngày mua], DATEADD(MONTH, l.SoThang, l.NgayLap) AS [Ngày hết hạn]
                    FROM LastHoaDon l
                    JOIN KhachHang kh ON l.MaKH = kh.MaKH
                    WHERE l.rn = 1 AND DATEADD(MONTH, l.SoThang, l.NgayLap) < GETDATE()
                    ORDER BY [Ngày hết hạn] ASC";

                DataTable dt = DatabaseHelper.ExecuteQuery(sql);
                dgvHetHan.DataSource = dt;
                lblSoLuongHetHan.Text = dt.Rows.Count == 0
                    ? "Không có hội viên nào hết hạn 🎉"
                    : $"Có {dt.Rows.Count} hội viên cần liên hệ gia hạn";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải danh sách hết hạn: " + ex.Message);
            }
        }

        private void SetCardValue(string cardName, string value)
        {
            var card = this.Controls[cardName];
            if (card != null)
            {
                var lbl = card.Controls["lblValue"];
                if (lbl != null) lbl.Text = value;
            }
        }
    }
}
