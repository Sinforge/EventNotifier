using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace EventNotifier.Models
{
    public class Notification
    {
        public int Id { get; set; }
        [Required]
        public string? HtmlText { get; set; }
        [JsonIgnore]
        public virtual User Receiver { get; set; } = null!;

        public bool IsChecked { get; set; } = false;
    }
}
