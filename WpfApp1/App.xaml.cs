using System.Windows;

namespace StudentManager
{
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // Chỉ định: chương trình chỉ tắt khi TẤT CẢ window đều đóng
            this.ShutdownMode = ShutdownMode.OnLastWindowClose;

            // Tự tạo LoginWindow làm cửa sổ đầu tiên
            var login = new LoginWindow();
            this.MainWindow = login;
            login.Show();
        }
    }
}
