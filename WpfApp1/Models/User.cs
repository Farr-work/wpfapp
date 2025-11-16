using System;

namespace StudentManager.Models
{
    public class User
    {
        public int Id { get; set; }  // khóa chính tự tăng
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty; // DEMO: lưu plain text (thực tế nên hash)
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
