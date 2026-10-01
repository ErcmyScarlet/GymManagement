using System;
using System.Windows.Forms;
using GymManagement.DAL;
using GymManagement.Forms;

namespace GymManagement
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (!DatabaseHelper.TestConnection(out string error))
            {
                MessageBox.Show(
                    "Không thể kết nối đến cơ sở dữ liệu SQL Server.\n\n" +
                    "Chi tiết lỗi: " + error + "\n\n" +
                    "Hãy kiểm tra lại chuỗi kết nối trong DAL/DatabaseHelper.cs",
                    "Lỗi kết nối",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            using (var frmLogin = new frmDangNhap())
            {
                if (frmLogin.ShowDialog() == DialogResult.OK)
                {
                    Application.Run(
                        new MainForm(
                            frmLogin.HoTenDangNhap,
                            frmLogin.VaiTro,
                            frmLogin.MaNVLienKet,
                            frmLogin.UserID));
                }
            }
        }
    }
}
