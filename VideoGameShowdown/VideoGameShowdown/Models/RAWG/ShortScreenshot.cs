using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VideoGameShowdown.Models
{
    public class ShortScreenshot
    {
        #region Properties..
        [JsonIgnore]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "Id")]
        [Key]
        public string ShortScreenshotId { get; set; }

        [Column(TypeName = "NVARCHAR(2048)")]
        public string? Image { get; set; }
        #endregion Properties..
    }
}
