using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

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
        public string Image { get; set; }

        [ForeignKey("Game")]
        public string GameId { get; set; }
        public Game Game { get; set; }
        #endregion Properties..
    }
}
