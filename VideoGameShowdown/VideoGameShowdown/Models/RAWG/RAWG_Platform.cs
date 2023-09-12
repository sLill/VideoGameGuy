using System.ComponentModel.DataAnnotations;

namespace VideoGameShowdown.Models
{
    public class RAWG_Platform
    {
        #region Properties..
        [Key]
        public int PlatformId { get;set; }

        public RAWG_PlatformDetail platform { get; set; }
        public DateTime released_at { get; set; }
        public object requirements_en { get; set; }
        public object requirements_ru { get; set; } 
        #endregion Properties..
    }
}


