using System.ComponentModel.DataAnnotations;

namespace EventNotifier.Models
{
    public class Notification
    {
        public int Id { get; set; }
        [Required]
        public string? HtmlText { get; set; }
        public virtual User Receiver { get; set; } = null!;

        public bool IsChecked { get; set; } = false;
    }
}
