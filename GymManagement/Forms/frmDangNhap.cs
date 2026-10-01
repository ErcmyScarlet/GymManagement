using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using Guna.UI2.WinForms;
using GymManagement.DAL;

namespace GymManagement.Forms
{
    public class frmDangNhap : Form
    {
        private TextBox txtUsername;
        private TextBox txtPassword;
        private Guna2Button btnLogin;
        private Guna2Button btnExit;
        private Label lblError;

        public string HoTenDangNhap { get; private set; }
        public string VaiTro { get; private set; }
        public int? MaNVLienKet { get; private set; }
        public int UserID { get; private set; }

        private static readonly Color ColorAccent = ColorTranslator.FromHtml("#4E7CFF");
        private static readonly Color ColorDark = ColorTranslator.FromHtml("#1B2430");

        public frmDangNhap()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Đăng nhập - Quản lý Phòng Gym";
            this.AutoScaleMode = AutoScaleMode.None;
            this.Size = new Size(440, 500);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = ColorTranslator.FromHtml("#F3F5F9");

            
            var pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 130,
                BackColor = ColorAccent
            };
            var lblTitle = new Label
            {
                Text = "🏋 PHÒNG GYM FITNESS",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 17, FontStyle.Bold),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill
            };
            pnlHeader.Controls.Add(lblTitle);

            
            var card = new Guna2Panel
            {
                Location = new Point(30, 155),
                Size = new Size(360, 270),
                FillColor = Color.White,
                BorderRadius = 14,
                ShadowDecoration = { Enabled = true }
            };

            var lblUser = new Label { Text = "Tài khoản", Location = new Point(28, 22), AutoSize = true, Font = new Font("Segoe UI", 9.5f), ForeColor = Color.Gray };
            txtUsername = new TextBox
            {
                Location = new Point(28, 45),
                Width = 300,
                Font = new Font("Segoe UI", 11),
                BorderStyle = BorderStyle.FixedSingle
            };

            var lblPass = new Label { Text = "Mật khẩu", Location = new Point(28, 92), AutoSize = true, Font = new Font("Segoe UI", 9.5f), ForeColor = Color.Gray };
            txtPassword = new TextBox
            {
                Location = new Point(28, 115),
                Width = 300,
                Font = new Font("Segoe UI", 11),
                BorderStyle = BorderStyle.FixedSingle,
                UseSystemPasswordChar = true
            };

            lblError = new Label
            {
                Location = new Point(28, 155),
                Width = 300,
                Height = 30,
                ForeColor = Color.Red,
                Font = new Font("Segoe UI", 9)
            };

            btnLogin = new Guna2Button
            {
                Text = "ĐĂNG NHẬP",
                Location = new Point(28, 192),
                Size = new Size(300, 44),
                FillColor = ColorAccent,
                ForeColor = Color.White,
                BorderRadius = 8,
                Font = new Font("Segoe UI", 10.5f, FontStyle.Bold),
                Animated = true,
                Cursor = Cursors.Hand
            };
            btnLogin.Click += BtnLogin_Click;

            card.Controls.Add(btnLogin);
            card.Controls.Add(lblError);
            card.Controls.Add(txtPassword);
            card.Controls.Add(lblPass);
            card.Controls.Add(txtUsername);
            card.Controls.Add(lblUser);

            btnExit = new Guna2Button
            {
                Text = "Thoát",
                Location = new Point(160, 440),
                Size = new Size(120, 32),
                FillColor = Color.Transparent,
                ForeColor = Color.Gray,
                BorderColor = Color.LightGray,
                BorderThickness = 1,
                BorderRadius = 8,
                Font = new Font("Segoe UI", 9.5f),
                Cursor = Cursors.Hand
            };
            btnExit.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };

            this.AcceptButton = btnLogin;

            this.Controls.Add(btnExit);
            this.Controls.Add(card);
            this.Controls.Add(pnlHeader);
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                lblError.Text = "Vui lòng nhập đầy đủ tài khoản và mật khẩu.";
                return;
            }

            try
            {
                string sql = "SELECT UserID, HoTen, VaiTro, TrangThai, MaNV FROM TaiKhoan WHERE TenDangNhap = @tk AND MatKhau = @mk";
                DataTable dt = DatabaseHelper.ExecuteQuery(sql,
                    new SqlParameter("@tk", username),
                    new SqlParameter("@mk", password));

                if (dt.Rows.Count == 0)
                {
                    lblError.Text = "Sai tài khoản hoặc mật khẩu.";
                    return;
                }

                DataRow row = dt.Rows[0];
                if (!(bool)row["TrangThai"])
                {
                    lblError.Text = "Tài khoản đã bị khóa.";
                    return;
                }

                HoTenDangNhap = row["HoTen"].ToString();
                UserID = Convert.ToInt32(row["UserID"]);
                VaiTro = row["VaiTro"].ToString();
                MaNVLienKet = row["MaNV"] == DBNull.Value ? (int?)null : Convert.ToInt32(row["MaNV"]);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                lblError.Text = "Lỗi: " + ex.Message;
            }
        }
    }
}
