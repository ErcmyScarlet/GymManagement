using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using Guna.UI2.WinForms;
using GymManagement.DAL;

namespace GymManagement.Forms
{



    public class frmDoiMatKhau : Form
    {
        private readonly int _userId;

        private TextBox txtMatKhauCu, txtMatKhauMoi, txtXacNhan;
        private Guna2Button btnLuu, btnHuy;
        private Label lblError;

        private static readonly Color ColorAccent = ColorTranslator.FromHtml("#4E7CFF");
        private static readonly Color ColorDark = ColorTranslator.FromHtml("#1B2430");

        public frmDoiMatKhau(int userId)
        {
            _userId = userId;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Đổi mật khẩu";
            this.AutoScaleMode = AutoScaleMode.None;
            this.Size = new Size(400, 400);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.White;

            var lblTitle = new Label
            {
                Text = "🔒 Đổi mật khẩu",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = ColorDark,
                Location = new Point(25, 20),
                AutoSize = true
            };
            this.Controls.Add(lblTitle);

            var lblCu = new Label { Text = "Mật khẩu hiện tại", Location = new Point(25, 65), AutoSize = true, ForeColor = Color.Gray, Font = new Font("Segoe UI", 9.5f) };
            txtMatKhauCu = new TextBox { Location = new Point(25, 88), Width = 320, Font = new Font("Segoe UI", 11), BorderStyle = BorderStyle.FixedSingle, UseSystemPasswordChar = true };

            var lblMoi = new Label { Text = "Mật khẩu mới", Location = new Point(25, 135), AutoSize = true, ForeColor = Color.Gray, Font = new Font("Segoe UI", 9.5f) };
            txtMatKhauMoi = new TextBox { Location = new Point(25, 158), Width = 320, Font = new Font("Segoe UI", 11), BorderStyle = BorderStyle.FixedSingle, UseSystemPasswordChar = true };

            var lblXacNhan = new Label { Text = "Xác nhận mật khẩu mới", Location = new Point(25, 205), AutoSize = true, ForeColor = Color.Gray, Font = new Font("Segoe UI", 9.5f) };
            txtXacNhan = new TextBox { Location = new Point(25, 228), Width = 320, Font = new Font("Segoe UI", 11), BorderStyle = BorderStyle.FixedSingle, UseSystemPasswordChar = true };

            lblError = new Label { Location = new Point(25, 268), Width = 320, Height = 40, ForeColor = Color.Red, Font = new Font("Segoe UI", 9) };

            btnLuu = new Guna2Button
            {
                Text = "Lưu thay đổi",
                Location = new Point(25, 315),
                Size = new Size(155, 42),
                FillColor = ColorAccent,
                ForeColor = Color.White,
                BorderRadius = 8,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Animated = true,
                Cursor = Cursors.Hand
            };
            btnLuu.Click += BtnLuu_Click;

            btnHuy = new Guna2Button
            {
                Text = "Hủy",
                Location = new Point(190, 315),
                Size = new Size(155, 42),
                FillColor = Color.White,
                ForeColor = Color.Gray,
                BorderColor = Color.LightGray,
                BorderThickness = 1,
                BorderRadius = 8,
                Font = new Font("Segoe UI", 10),
                Cursor = Cursors.Hand
            };
            btnHuy.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };

            this.AcceptButton = btnLuu;

            this.Controls.Add(lblCu);
            this.Controls.Add(txtMatKhauCu);
            this.Controls.Add(lblMoi);
            this.Controls.Add(txtMatKhauMoi);
            this.Controls.Add(lblXacNhan);
            this.Controls.Add(txtXacNhan);
            this.Controls.Add(lblError);
            this.Controls.Add(btnLuu);
            this.Controls.Add(btnHuy);
        }

        private void BtnLuu_Click(object sender, EventArgs e)
        {
            string matKhauCu = txtMatKhauCu.Text.Trim();
            string matKhauMoi = txtMatKhauMoi.Text.Trim();
            string xacNhan = txtXacNhan.Text.Trim();

            if (string.IsNullOrEmpty(matKhauCu) || string.IsNullOrEmpty(matKhauMoi) || string.IsNullOrEmpty(xacNhan))
            {
                lblError.Text = "Vui lòng nhập đầy đủ các ô.";
                return;
            }
            if (matKhauMoi.Length < 4)
            {
                lblError.Text = "Mật khẩu mới phải có ít nhất 4 ký tự.";
                return;
            }
            if (matKhauMoi != xacNhan)
            {
                lblError.Text = "Xác nhận mật khẩu mới không khớp.";
                return;
            }

            try
            {
                object dbMatKhau = DatabaseHelper.ExecuteScalar(
                    "SELECT MatKhau FROM TaiKhoan WHERE UserID = @id",
                    new SqlParameter("@id", _userId));

                if (dbMatKhau == null || dbMatKhau.ToString() != matKhauCu)
                {
                    lblError.Text = "Mật khẩu hiện tại không đúng.";
                    return;
                }

                DatabaseHelper.ExecuteNonQuery(
                    "UPDATE TaiKhoan SET MatKhau = @mk WHERE UserID = @id",
                    new SqlParameter("@mk", matKhauMoi),
                    new SqlParameter("@id", _userId));

                MessageBox.Show("Đổi mật khẩu thành công! Lần đăng nhập sau hãy dùng mật khẩu mới.",
                    "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
