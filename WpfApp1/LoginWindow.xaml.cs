using System.Linq;
using System.Windows;
using StudentManager.Models;

namespace StudentManager
{
    public partial class LoginWindow : Window
    {
        private readonly StudentContext _context;

        public LoginWindow()
        {
            InitializeComponent();
            _context = new StudentContext();
            _context.Database.EnsureCreated();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = pbPassword.Password.Trim();

            txtMessage.Text = "";

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                txtMessage.Text = "Vui lòng nhập đầy đủ tên đăng nhập và mật khẩu.";
                return;
            }

            var user = _context.Users
                               .FirstOrDefault(u => u.Username == username && u.Password == password);

            if (user == null)
            {
                txtMessage.Text = "Sai tên đăng nhập hoặc mật khẩu.";
                return;
            }

            // Đăng nhập đúng → mở MainWindow
            var main = new MainWindow();
            Application.Current.MainWindow = main;   // đặt lại mainwindow cho đẹp
            main.Show();

            // Chỉ đóng LoginWindow, app vẫn còn MainWindow
            this.Close();
        }

        private void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            // Mở form đăng ký dạng modal, KHÔNG đóng LoginWindow
            var reg = new RegisterWindow
            {
                Owner = this
            };
            reg.ShowDialog();
            // Sau khi reg.Close(), LoginWindow vẫn đang mở → app không tắt
        }

        protected override void OnClosed(System.EventArgs e)
        {
            base.OnClosed(e);
            _context.Dispose();
        }
    }
}
