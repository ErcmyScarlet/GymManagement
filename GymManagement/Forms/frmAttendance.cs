using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using Guna.UI2.WinForms;
using GymManagement.DAL;

namespace GymManagement.Forms
{
    public class frmAttendance : Form
    {
        private readonly string _vaiTro;
        private readonly int? _maNV;
        private static readonly TimeSpan GioChuan = new TimeSpan(8, 30, 0);

        private static readonly Color ColorAccent = ColorTranslator.FromHtml("#4E7CFF");
        private static readonly Color ColorSuccess = ColorTranslator.FromHtml("#2ECC71");
        private static readonly Color ColorDanger = ColorTranslator.FromHtml("#E85C5C");
        private static readonly Color ColorNeutral = ColorTranslator.FromHtml("#95A5A6");
        private static readonly Color ColorDark = ColorTranslator.FromHtml("#1B2430");

        public frmAttendance(string vaiTro, int? maNV)
        {
            _vaiTro = vaiTro;
            _maNV = maNV;

            if (_vaiTro == "NhanVien" && _maNV.HasValue)
                InitSelfCheckInUI();
            else
                InitAdminUI();
        }
        private Guna2Button btnTuDiemDanh;
        private Label lblTrangThaiHomNay;
        private Guna2DataGridView dgvLichSu;
        private Label lblTongCa;

        private void InitSelfCheckInUI()
        {
            this.AutoScaleMode = AutoScaleMode.None;
            this.Dock = DockStyle.Fill;
            this.BackColor = ColorTranslator.FromHtml("#F3F5F9");
            this.AutoScroll = true;

            string tenNV = "";
            decimal donGiaCa = 0;
            try
            {
                DataTable dt = DatabaseHelper.ExecuteQuery("SELECT HoTen, Luong FROM NhanVien WHERE MaNV=@id", new SqlParameter("@id", _maNV.Value));
                if (dt.Rows.Count > 0)
                {
                    tenNV = dt.Rows[0]["HoTen"].ToString();
                    donGiaCa = dt.Rows[0]["Luong"] == DBNull.Value ? 0 : Convert.ToDecimal(dt.Rows[0]["Luong"]);
                }
            }
            catch {}

            var lblTitle = new Label
            {
                Text = "CHẤM CÔNG CỦA TÔI",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = ColorDark,
                Location = new Point(20, 15),
                AutoSize = true
            };
            this.Controls.Add(lblTitle);
            var card = new Guna2Panel
            {
                Location = new Point(20, 60),
                Size = new Size(500, 230),
                FillColor = Color.White,
                BorderRadius = 14,
                ShadowDecoration = { Enabled = true }
            };

            var lblXinChao = new Label
            {
                Text = $"Xin chào, {tenNV}",
                Font = new Font("Segoe UI", 13, FontStyle.Bold),
                ForeColor = ColorDark,
                Location = new Point(30, 25),
                AutoSize = true
            };
            var lblNgayHomNay = new Label
            {
                Text = "Hôm nay: " + DateTime.Now.ToString("dd/MM/yyyy"),
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.Gray,
                Location = new Point(30, 58),
                AutoSize = true
            };

            btnTuDiemDanh = new Guna2Button
            {
                Text = "✅ ĐIỂM DANH HÔM NAY",
                Location = new Point(30, 95),
                Size = new Size(440, 55),
                FillColor = ColorSuccess,
                ForeColor = Color.White,
                BorderRadius = 10,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Animated = true,
                Cursor = Cursors.Hand
            };
            btnTuDiemDanh.Click += BtnTuDiemDanh_Click;

            lblTrangThaiHomNay = new Label
            {
                Text = "",
                Font = new Font("Segoe UI", 9.5f, FontStyle.Italic),
                ForeColor = ColorAccent,
                Location = new Point(30, 165),
                AutoSize = true
            };

            card.Controls.Add(lblXinChao);
            card.Controls.Add(lblNgayHomNay);
            card.Controls.Add(btnTuDiemDanh);
            card.Controls.Add(lblTrangThaiHomNay);
            this.Controls.Add(card);

            lblTongCa = new Label
            {
                Text = "",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = ColorDark,
                Location = new Point(20, 310),
                AutoSize = true
            };
            this.Controls.Add(lblTongCa);

            var lblLichSu = new Label
            {
                Text = "Lịch sử chấm công tháng này",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = ColorDark,
                Location = new Point(20, 345),
                AutoSize = true
            };
            this.Controls.Add(lblLichSu);

            dgvLichSu = new Guna2DataGridView
            {
                Location = new Point(20, 375),
                Size = new Size(950, 300),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None
            };
            StyleGrid(dgvLichSu);
            this.Controls.Add(dgvLichSu);

            LoadSelfHistory(donGiaCa);
        }

        private void BtnTuDiemDanh_Click(object sender, EventArgs e)
        {
            try
            {
                DateTime homNay = DateTime.Now.Date;
                object existing = DatabaseHelper.ExecuteScalar(
                    "SELECT GioVao FROM ChamCongNhanVien WHERE MaNV=@nv AND NgayLam=@ngay",
                    new SqlParameter("@nv", _maNV.Value), new SqlParameter("@ngay", homNay));

                if (existing != null && existing != DBNull.Value)
                {
                    MessageBox.Show("Bạn đã điểm danh hôm nay rồi!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                TimeSpan gioHienTai = DateTime.Now.TimeOfDay;
                string trangThai = gioHienTai > GioChuan ? "Đi trễ" : "Đúng giờ";

                DatabaseHelper.ExecuteNonQuery(
                    "INSERT INTO ChamCongNhanVien (MaNV, NgayLam, GioVao, TrangThai) VALUES (@nv, @ngay, @gio, @tt)",
                    new SqlParameter("@nv", _maNV.Value),
                    new SqlParameter("@ngay", homNay),
                    new SqlParameter("@gio", gioHienTai),
                    new SqlParameter("@tt", trangThai));

                MessageBox.Show($"Điểm danh thành công lúc {DateTime.Now:HH:mm:ss} — {trangThai}!\nCa làm hôm nay đã được ghi nhận để tính lương.",
                    "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                decimal donGiaCa = 0;
                try
                {
                    object luongObj = DatabaseHelper.ExecuteScalar("SELECT Luong FROM NhanVien WHERE MaNV=@id", new SqlParameter("@id", _maNV.Value));
                    donGiaCa = luongObj == DBNull.Value || luongObj == null ? 0 : Convert.ToDecimal(luongObj);
                }
                catch { }
                LoadSelfHistory(donGiaCa);
            }
            catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601)
            {
                MessageBox.Show("Bạn đã điểm danh hôm nay rồi!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadSelfHistory(decimal donGiaCa)
        {
            try
            {
                DateTime today = DateTime.Now.Date;

                
                object gioVaoHomNay = DatabaseHelper.ExecuteScalar(
                    "SELECT GioVao FROM ChamCongNhanVien WHERE MaNV=@nv AND NgayLam=@ngay",
                    new SqlParameter("@nv", _maNV.Value), new SqlParameter("@ngay", today));

                if (gioVaoHomNay != null && gioVaoHomNay != DBNull.Value)
                {
                    TimeSpan gio = (TimeSpan)gioVaoHomNay;
                    btnTuDiemDanh.Enabled = false;
                    btnTuDiemDanh.Text = "✅ ĐÃ ĐIỂM DANH HÔM NAY";
                    btnTuDiemDanh.FillColor = ColorNeutral;
                    lblTrangThaiHomNay.Text = $"Bạn đã điểm danh lúc {gio:hh\\:mm\\:ss} hôm nay.";
                }
                else
                {
                    btnTuDiemDanh.Enabled = true;
                    btnTuDiemDanh.Text = "✅ ĐIỂM DANH HÔM NAY";
                    btnTuDiemDanh.FillColor = ColorSuccess;
                    lblTrangThaiHomNay.Text = "Bạn chưa điểm danh hôm nay.";
                }

                
                string sql = @"
                    SELECT NgayLam AS [Ngày làm], GioVao AS [Giờ vào], TrangThai AS [Trạng thái]
                    FROM ChamCongNhanVien
                    WHERE MaNV=@nv AND MONTH(NgayLam)=MONTH(GETDATE()) AND YEAR(NgayLam)=YEAR(GETDATE())
                    ORDER BY NgayLam DESC";
                DataTable dt = DatabaseHelper.ExecuteQuery(sql, new SqlParameter("@nv", _maNV.Value));
                dgvLichSu.DataSource = dt;

                int soCa = dt.Rows.Count;
                decimal luongTamTinh = soCa * donGiaCa;
                lblTongCa.Text = $"Số ca đã làm tháng này: {soCa} ca   —   Lương tạm tính: {luongTamTinh:N0} đ";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải lịch sử chấm công: " + ex.Message, "Lỗi");
            }
        }
        private Guna2ComboBox cboNhanVien;
        private Guna2DateTimePicker dtpNgay;
        private Guna2Button btnVao, btnRa;
        private Guna2DataGridView dgvNgay;
        private Guna2DataGridView dgvBangLuong;

        private void InitAdminUI()
        {
            this.AutoScaleMode = AutoScaleMode.None;
            this.Dock = DockStyle.Fill;
            this.BackColor = ColorTranslator.FromHtml("#F3F5F9");
            this.AutoScroll = true;

            var lblTitle = new Label
            {
                Text = "CHẤM CÔNG NHÂN VIÊN",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = ColorDark,
                Location = new Point(20, 15),
                AutoSize = true
            };
            this.Controls.Add(lblTitle);
            var pnlForm = new Guna2GroupBox
            {
                Text = "Chấm công (dành cho Admin chấm hộ hoặc sửa)",
                Location = new Point(20, 55),
                Size = new Size(950, 115),
                Font = new Font("Segoe UI", 9.5f),
                FillColor = Color.White,
                BorderRadius = 10,
                ForeColor = ColorDark
            };

            pnlForm.Controls.Add(new Label { Text = "Nhân viên:", Location = new Point(15, 35), AutoSize = true, ForeColor = Color.Gray });
            cboNhanVien = new Guna2ComboBox { Location = new Point(110, 32), Width = 250, DropDownStyle = ComboBoxStyle.DropDownList, BorderRadius = 6 };
            pnlForm.Controls.Add(cboNhanVien);

            pnlForm.Controls.Add(new Label { Text = "Ngày:", Location = new Point(390, 35), AutoSize = true, ForeColor = Color.Gray });
            dtpNgay = new Guna2DateTimePicker { Location = new Point(440, 32), Width = 150, Format = DateTimePickerFormat.Short, Value = DateTime.Now, BorderRadius = 6 };
            pnlForm.Controls.Add(dtpNgay);

            btnVao = new Guna2Button
            {
                Text = "🟢 Chấm công",
                Location = new Point(610, 29),
                Size = new Size(160, 36),
                FillColor = ColorSuccess,
                ForeColor = Color.White,
                BorderRadius = 8,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                Animated = true,
                Cursor = Cursors.Hand
            };
            btnVao.Click += BtnVao_Click;
            pnlForm.Controls.Add(btnVao);

            btnRa = new Guna2Button
            {
                Text = "🗑 Xóa dòng",
                Location = new Point(780, 29),
                Size = new Size(150, 36),
                FillColor = ColorDanger,
                ForeColor = Color.White,
                BorderRadius = 8,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                Animated = true,
                Cursor = Cursors.Hand
            };
            btnRa.Click += BtnXoaDong_Click;
            pnlForm.Controls.Add(btnRa);

            this.Controls.Add(pnlForm);

            var btnXem = new Guna2Button
            {
                Text = "🔍 Xem theo ngày đã chọn",
                Location = new Point(20, 185),
                Size = new Size(200, 33),
                FillColor = ColorAccent,
                ForeColor = Color.White,
                BorderRadius = 8,
                Font = new Font("Segoe UI", 9.5f),
                Animated = true,
                Cursor = Cursors.Hand
            };
            btnXem.Click += (s, e) => LoadDataTheoNgay();
            this.Controls.Add(btnXem);

            dgvNgay = new Guna2DataGridView
            {
                Location = new Point(20, 230),
                Size = new Size(950, 200),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None
            };
            StyleGrid(dgvNgay);
            this.Controls.Add(dgvNgay);
            var lblBangLuong = new Label
            {
                Text = $"Bảng công tháng {DateTime.Now.Month}/{DateTime.Now.Year} (dùng để tính lương)",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = ColorDark,
                Location = new Point(20, 445),
                AutoSize = true
            };
            this.Controls.Add(lblBangLuong);

            var btnLamMoiBangLuong = new Guna2Button
            {
                Text = "🔄 Làm mới",
                Location = new Point(870, 440),
                Size = new Size(100, 32),
                FillColor = ColorNeutral,
                ForeColor = Color.White,
                BorderRadius = 8,
                Font = new Font("Segoe UI", 9),
                Animated = true,
                Cursor = Cursors.Hand
            };
            btnLamMoiBangLuong.Click += (s, e) => LoadBangLuong();
            this.Controls.Add(btnLamMoiBangLuong);

            dgvBangLuong = new Guna2DataGridView
            {
                Location = new Point(20, 480),
                Size = new Size(950, 250),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None
            };
            StyleGrid(dgvBangLuong);
            this.Controls.Add(dgvBangLuong);

            LoadNhanVien();
            LoadDataTheoNgay();
            LoadBangLuong();
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

        private void LoadNhanVien()
        {
            DataTable dt = DatabaseHelper.ExecuteQuery("SELECT MaNV, HoTen FROM NhanVien WHERE TrangThai = 1 ORDER BY HoTen");
            cboNhanVien.DisplayMember = "HoTen";
            cboNhanVien.ValueMember = "MaNV";
            cboNhanVien.DataSource = dt;
        }

        private void BtnVao_Click(object sender, EventArgs e)
        {
            if (cboNhanVien.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn nhân viên.", "Thiếu thông tin");
                return;
            }
            try
            {
                int maNV = Convert.ToInt32(cboNhanVien.SelectedValue);
                DateTime ngay = dtpNgay.Value.Date;

                object existing = DatabaseHelper.ExecuteScalar(
                    "SELECT GioVao FROM ChamCongNhanVien WHERE MaNV=@nv AND NgayLam=@ngay",
                    new SqlParameter("@nv", maNV), new SqlParameter("@ngay", ngay));

                if (existing != null && existing != DBNull.Value)
                {
                    MessageBox.Show("Nhân viên này đã có chấm công cho ngày đã chọn rồi.", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                TimeSpan gioHienTai = DateTime.Now.TimeOfDay;
                string trangThai = gioHienTai > GioChuan ? "Đi trễ" : "Đúng giờ";

                DatabaseHelper.ExecuteNonQuery(
                    "INSERT INTO ChamCongNhanVien (MaNV, NgayLam, GioVao, TrangThai) VALUES (@nv, @ngay, @gio, @tt)",
                    new SqlParameter("@nv", maNV),
                    new SqlParameter("@ngay", ngay),
                    new SqlParameter("@gio", gioHienTai),
                    new SqlParameter("@tt", trangThai));

                MessageBox.Show("Chấm công thành công.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadDataTheoNgay();
                LoadBangLuong();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnXoaDong_Click(object sender, EventArgs e)
        {
            if (!(dgvNgay.CurrentRow?.DataBoundItem is DataRowView row))
            {
                MessageBox.Show("Vui lòng chọn dòng chấm công cần xóa trong bảng bên dưới.", "Thông báo");
                return;
            }
            if (MessageBox.Show("Xóa dòng chấm công này?", "Xác nhận",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            try
            {
                int maChamCong = Convert.ToInt32(row["MaChamCong"]);
                DatabaseHelper.ExecuteNonQuery("DELETE FROM ChamCongNhanVien WHERE MaChamCong=@id", new SqlParameter("@id", maChamCong));
                LoadDataTheoNgay();
                LoadBangLuong();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadDataTheoNgay()
        {
            try
            {
                string sql = @"
                    SELECT cc.MaChamCong, nv.HoTen AS [Nhân viên], cc.NgayLam AS [Ngày làm],
                           cc.GioVao AS [Giờ vào], cc.TrangThai AS [Trạng thái]
                    FROM ChamCongNhanVien cc
                    JOIN NhanVien nv ON cc.MaNV = nv.MaNV
                    WHERE cc.NgayLam = @ngay
                    ORDER BY nv.HoTen";
                DataTable dt = DatabaseHelper.ExecuteQuery(sql, new SqlParameter("@ngay", dtpNgay.Value.Date));
                dgvNgay.DataSource = dt;
                if (dgvNgay.Columns.Contains("MaChamCong")) dgvNgay.Columns["MaChamCong"].Visible = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải dữ liệu: " + ex.Message, "Lỗi");
            }
        }

        private void LoadBangLuong()
        {
            try
            {
                string sql = @"
                    SELECT nv.MaNV AS [Mã NV], nv.HoTen AS [Họ tên], nv.ChucVu AS [Chức vụ],
                           ISNULL(nv.Luong,0) AS [Đơn giá/ca],
                           (SELECT COUNT(*) FROM ChamCongNhanVien cc
                            WHERE cc.MaNV = nv.MaNV AND cc.GioVao IS NOT NULL
                              AND MONTH(cc.NgayLam)=MONTH(GETDATE()) AND YEAR(cc.NgayLam)=YEAR(GETDATE())) AS [Số ca làm],
                           ISNULL(nv.Luong,0) *
                           (SELECT COUNT(*) FROM ChamCongNhanVien cc
                            WHERE cc.MaNV = nv.MaNV AND cc.GioVao IS NOT NULL
                              AND MONTH(cc.NgayLam)=MONTH(GETDATE()) AND YEAR(cc.NgayLam)=YEAR(GETDATE())) AS [Lương tạm tính]
                    FROM NhanVien nv
                    WHERE nv.TrangThai = 1
                    ORDER BY nv.HoTen";
                DataTable dt = DatabaseHelper.ExecuteQuery(sql);
                dgvBangLuong.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải bảng lương: " + ex.Message, "Lỗi");
            }
        }
    }
}
