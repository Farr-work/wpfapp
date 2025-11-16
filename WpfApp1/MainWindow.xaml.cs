using StudentManager.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace StudentManager
{
    public partial class MainWindow : Window
    {
        private readonly StudentContext _context;
        public ObservableCollection<Student> Students { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            _context = new StudentContext();
            _context.Database.EnsureCreated(); // Tạo DB nếu chưa có

            LoadStudents();
        }

        private void LoadStudents()
        {
            var list = _context.Students
                               .OrderBy(s => s.Id)
                               .ToList();

            Students = new ObservableCollection<Student>(list);
            dgStudents.ItemsSource = Students;
        }

        // Lấy giới tính đang chọn
        private string GetSelectedGender()
        {
            if (cbGioiTinh.SelectedItem is ComboBoxItem item)
            {
                return item.Content?.ToString();
            }
            return null;
        }

        private bool ValidateInput(bool checkIdExists = false)
        {
            if (string.IsNullOrWhiteSpace(txtMaSV.Text) ||
                string.IsNullOrWhiteSpace(txtHoTen.Text) ||
                string.IsNullOrWhiteSpace(txtLop.Text) ||
                dpNgaySinh.SelectedDate == null ||
                GetSelectedGender() == null)
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin.",
                                "Thiếu dữ liệu", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (checkIdExists)
            {
                var id = txtMaSV.Text.Trim();
                if (_context.Students.Any(s => s.Id == id))
                {
                    MessageBox.Show("Mã sinh viên đã tồn tại.",
                                    "Trùng mã", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }
            }

            return true;
        }

        private void ClearInput()
        {
            txtMaSV.Clear();
            txtHoTen.Clear();
            txtLop.Clear();
            dpNgaySinh.SelectedDate = null;
            cbGioiTinh.SelectedIndex = -1;
        }

        // Thêm mới
        private void BtnThem_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInput(checkIdExists: true))
                return;

            var student = new Student
            {
                Id = txtMaSV.Text.Trim(),
                Name = txtHoTen.Text.Trim(),
                ClassName = txtLop.Text.Trim(),
                BirthDate = dpNgaySinh.SelectedDate.Value,
                Gender = GetSelectedGender()
            };

            _context.Students.Add(student);
            _context.SaveChanges();

            Students.Add(student);
            ClearInput();
            MessageBox.Show("Đã thêm sinh viên.", "Thông báo",
                            MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // Sửa
        private void BtnSua_Click(object sender, RoutedEventArgs e)
        {
            if (dgStudents.SelectedItem is not Student selected)
            {
                MessageBox.Show("Hãy chọn sinh viên cần sửa.",
                                "Chưa chọn", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!ValidateInput(checkIdExists: false))
                return;

            // Không cho sửa mã SV (khóa chính)
            selected.Name = txtHoTen.Text.Trim();
            selected.ClassName = txtLop.Text.Trim();
            selected.BirthDate = dpNgaySinh.SelectedDate.Value;
            selected.Gender = GetSelectedGender();

            _context.SaveChanges();
            dgStudents.Items.Refresh();

            MessageBox.Show("Đã cập nhật thông tin sinh viên.",
                            "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // Xóa
        private void BtnXoa_Click(object sender, RoutedEventArgs e)
        {
            if (dgStudents.SelectedItem is not Student selected)
            {
                MessageBox.Show("Hãy chọn sinh viên cần xóa.",
                                "Chưa chọn", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (MessageBox.Show($"Bạn chắc chắn muốn xóa {selected.Name}?",
                                "Xác nhận xóa",
                                MessageBoxButton.YesNo,
                                MessageBoxImage.Question) != MessageBoxResult.Yes)
                return;

            _context.Students.Remove(selected);
            _context.SaveChanges();

            Students.Remove(selected);

            MessageBox.Show("Đã xóa sinh viên.",
                            "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // Lọc / tìm kiếm
        private void BtnLoc_Click(object sender, RoutedEventArgs e)
        {
            string keyword = txtSearch.Text.Trim().ToLower();

            var query = _context.Students.AsQueryable();

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(s =>
                    s.Id.ToLower().Contains(keyword) ||
                    s.Name.ToLower().Contains(keyword) ||
                    s.ClassName.ToLower().Contains(keyword));
            }

            Students.Clear();
            foreach (var s in query.OrderBy(s => s.Id))
            {
                Students.Add(s);
            }
        }

        // Reset về toàn bộ danh sách
        private void BtnReset_Click(object sender, RoutedEventArgs e)
        {
            txtSearch.Clear();
            LoadStudents();
        }

        // Khi chọn 1 dòng trên DataGrid → đổ dữ liệu lên ô input
        private void dgStudents_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (dgStudents.SelectedItem is Student selected)
            {
                txtMaSV.Text = selected.Id;
                txtHoTen.Text = selected.Name;
                txtLop.Text = selected.ClassName;
                dpNgaySinh.SelectedDate = selected.BirthDate;

                // chọn lại giới tính
                if (selected.Gender == "NAM")
                    cbGioiTinh.SelectedIndex = 0;
                else if (selected.Gender == "NỮ")
                    cbGioiTinh.SelectedIndex = 1;
                else
                    cbGioiTinh.SelectedIndex = -1;

                // Không cho sửa mã SV
                txtMaSV.IsEnabled = false;
            }
            else
            {
                txtMaSV.IsEnabled = true;
            }
        }

        // Đóng app thì dispose DbContext
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            _context.Dispose();
        }
    }
}
