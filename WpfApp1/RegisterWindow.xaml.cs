using System.Linq;
using System.Windows;
using StudentManager.Models;

namespace StudentManager
{
    public partial class RegisterWindow : Window
    {
        private readonly StudentContext _context;

        public RegisterWindow()
        {
            InitializeComponent();
            _context = new StudentContext();
            _context.Database.EnsureCreated();
        }

        private void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = pbPassword.Password.Trim();
            string confirm = pbConfirm.Password.Trim();

            txtMessage.Text = "";

            if (string.IsNullOrEmpty(username) ||
                string.IsNullOrEmpty(password) ||
                string.IsNullOrEmpty(confirm))
            {
                txtMessage.Text = "Vui lòng nhập đầy đủ thông tin.";
                return;
            }

            if (password != confirm)
            {
                txtMessage.Text = "Mật khẩu nhập lại không khớp.";
                return;
            }

            if (_context.Users.Any(u => u.Username == username))
            {
                txtMessage.Text = "Tên đăng nhập đã tồn tại.";
                return;
            }

            var user = new User
            {
                Username = username,
                Password = password
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            MessageBox.Show("Đăng ký thành công. Bạn có thể quay lại đăng nhập.",
                            "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);

            // CHỈ đóng RegisterWindow, LoginWindow vẫn còn
            this.Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        protected override void OnClosed(System.EventArgs e)
        {
            base.OnClosed(e);
            _context.Dispose();
        }
    }
}
