using System;
using System.Drawing;
using System.Windows.Forms;
using Guna.UI2.WinForms;
using GymManagement.Forms;

namespace GymManagement
{
    public class MainForm : Form
    {
        private Panel pnlSidebar;
        private Panel pnlMain;
        private Panel pnlTop;
        private Label lblUserInfo;
        private Guna2Button btnActive;

        private readonly string _hoTen;
        private readonly string _vaiTro;
        private readonly int? _maNV;
        private readonly int _userId;

        
        private static readonly Color ColorSidebar = ColorTranslator.FromHtml("#1B2430");
        private static readonly Color ColorSidebarHover = ColorTranslator.FromHtml("#2E3A4E");
        private static readonly Color ColorAccent = ColorTranslator.FromHtml("#4E7CFF");
        private static readonly Color ColorBg = ColorTranslator.FromHtml("#F3F5F9");
        private static readonly Color ColorDanger = ColorTranslator.FromHtml("#E85C5C");

        public MainForm(string hoTen, string vaiTro, int? maNV, int userId)
        {
            _hoTen = hoTen;
            _vaiTro = vaiTro;
            _maNV = maNV;
            _userId = userId;
            InitializeComponent();
            OpenChildForm(new frmTrangChu());
            SetActiveButton((Guna2Button)pnlSidebar.Controls["btnTrangChu"]);
        }

        private void InitializeComponent()
        {
            this.Text = "Hệ thống Quản lý Phòng Gym";
            this.AutoScaleMode = AutoScaleMode.None;
            this.WindowState = FormWindowState.Maximized;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MinimumSize = new Size(1150, 760);
            this.BackColor = ColorBg;
            this.Font = new Font("Segoe UI", 9.5f);

            
            pnlSidebar = new Panel
            {
                Dock = DockStyle.Left,
                Width = 245,
                BackColor = ColorSidebar
            };

            var lblLogo = new Label
            {
                Text = "🏋  GYM  MANAGER",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 13, FontStyle.Bold),
                Dock = DockStyle.Top,
                Height = 80,
                TextAlign = ContentAlignment.MiddleCenter
            };

            
            var menuItems = new (string name, string text, bool adminOnly, Func<Form> openForm)[]
            {
                ("btnTrangChu",   "🏠   Trang chủ",              false, () => new frmTrangChu()),
                ("btnNguoiDung",  "🧍   Quản lý khách hàng",     false, () => new frmQuanLyNguoiDung()),
                ("btnTaiKhoan",   "🔑   Quản lý tài khoản",      true,  () => new frmQuanLyTaiKhoan()),
                ("btnNhanVien",   "🧑‍💼   Quản lý nhân viên",     true,  () => new frmQuanLyNhanVien()),
                ("btnAttendance", "🗓   Chấm công",              false, () => new frmAttendance(_vaiTro, _maNV)),
                ("btnGoiTap",     "📦   Quản lý gói tập",        true,  () => new frmQuanLyGoiTap()),
                ("btnThongKe",    "📊   Thống kê doanh số",      false, () => new frmThongKeDoanhSo()),
                ("btnBaoCao",     "📈   Báo cáo",                true,  () => new frmBaoCao()),
            };

            bool isAdmin = _vaiTro == "Admin";
            int y = 90;
            foreach (var item in menuItems)
            {
                if (item.adminOnly && !isAdmin) continue; 

                Guna2Button btn = CreateMenuButton(item.name, item.text, y);
                var openForm = item.openForm; 
                btn.Click += (s, e) => { OpenChildForm(openForm()); SetActiveButton(btn); };
                pnlSidebar.Controls.Add(btn);
                y += 50;
            }

            Guna2Button btnLogout = new Guna2Button
            {
                Name = "btnLogout",
                Text = "🚪   Đăng xuất",
                Dock = DockStyle.Bottom,
                Height = 55,
                FillColor = ColorSidebar,
                ForeColor = ColorDanger,
                Font = new Font("Segoe UI", 10.5f, FontStyle.Bold),
                TextAlign = HorizontalAlignment.Left,
                Padding = new Padding(25, 0, 0, 0),
                BorderRadius = 0,
                Animated = true,
                HoverState = { FillColor = ColorDanger, ForeColor = Color.White },
                Cursor = Cursors.Hand
            };
            btnLogout.Click += (s, e) =>
            {
                if (MessageBox.Show("Bạn có chắc muốn đăng xuất?", "Xác nhận",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    Application.Restart();
                }
            };

            pnlSidebar.Controls.Add(lblLogo);
            pnlSidebar.Controls.Add(btnLogout);

            
            pnlTop = new Panel
            {
                Dock = DockStyle.Top,
                Height = 55,
                BackColor = Color.White
            };
            var btnDoiMatKhau = new Guna2Button
            {
                Text = "🔒 Đổi mật khẩu",
                Dock = DockStyle.Right,
                Width = 150,
                FillColor = Color.White,
                ForeColor = ColorAccent,
                BorderColor = ColorAccent,
                BorderThickness = 1,
                BorderRadius = 8,
                Font = new Font("Segoe UI", 9.5f),
                Margin = new Padding(0, 10, 15, 10),
                Cursor = Cursors.Hand,
                HoverState = { FillColor = ColorAccent, ForeColor = Color.White }
            };
            btnDoiMatKhau.Click += (s, e) =>
            {
                using (var frm = new frmDoiMatKhau(_userId))
                {
                    frm.ShowDialog(this);
                }
            };
            lblUserInfo = new Label
            {
                Text = $"Xin chào, {_hoTen}   ({_vaiTro})",
                Dock = DockStyle.Right,
                Width = 300,
                TextAlign = ContentAlignment.MiddleRight,
                Font = new Font("Segoe UI", 10),
                ForeColor = ColorTranslator.FromHtml("#1B2430"),
                Padding = new Padding(0, 0, 20, 0)
            };
            pnlTop.Controls.Add(btnDoiMatKhau);
            pnlTop.Controls.Add(lblUserInfo);

            
            pnlMain = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = ColorBg,
                Padding = new Padding(15)
            };

            this.Controls.Add(pnlMain);
            this.Controls.Add(pnlTop);
            this.Controls.Add(pnlSidebar);
        }

        private Guna2Button CreateMenuButton(string name, string text, int top)
        {
            var btn = new Guna2Button
            {
                Name = name,
                Text = text,
                Top = top,
                Left = 12,
                Width = 221,
                Height = 44,
                FillColor = ColorSidebar,
                ForeColor = ColorTranslator.FromHtml("#E4E8F0"),
                Font = new Font("Segoe UI", 10.5f),
                TextAlign = HorizontalAlignment.Left,
                Padding = new Padding(18, 0, 0, 0),
                BorderRadius = 8,
                Animated = true,
                Cursor = Cursors.Hand,
                HoverState = { FillColor = ColorSidebarHover, ForeColor = Color.White }
            };
            return btn;
        }

        private void SetActiveButton(Guna2Button btn)
        {
            if (btnActive != null)
            {
                btnActive.FillColor = ColorSidebar;
                btnActive.ForeColor = ColorTranslator.FromHtml("#E4E8F0");
            }
            btnActive = btn;
            btnActive.FillColor = ColorAccent;
            btnActive.ForeColor = Color.White;
        }

        private void OpenChildForm(Form childForm)
        {
            pnlMain.Controls.Clear();

            childForm.TopLevel = false;
            childForm.FormBorderStyle = FormBorderStyle.None;
            childForm.Dock = DockStyle.Fill;

            pnlMain.Controls.Add(childForm);
            childForm.Show();
        }
    }
}
