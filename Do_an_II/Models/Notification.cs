using System.ComponentModel.DataAnnotations.Schema;

namespace Do_an_II.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public string UserId { get; set; } // Người nhận (admin)
        [ForeignKey("UserId")]
        public AppUser AppUser { get; set; } // Thông tin người nhận
        public bool IsRead { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
