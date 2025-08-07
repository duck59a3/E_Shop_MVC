using System.ComponentModel.DataAnnotations;

namespace Do_an_II.Models.ViewModels
{
    public class AdminSendMessageVM
    {
        [Required(ErrorMessage = "Tin nhắn không được để trống")]
        public string Message { get; set; }

        [Required]
        public int ChatRoomId { get; set; }
    }
}
