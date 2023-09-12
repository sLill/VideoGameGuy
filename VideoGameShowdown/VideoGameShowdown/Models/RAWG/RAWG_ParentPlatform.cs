using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VideoGameShowdown.Models
{
    public class RAWG_ParentPlatform
    {
        #region Properties..
        [Key]
        public int ParentPlatformId { get; set; }

        public RAWG_PlatformDetail platform { get; set; }
        #endregion Properties..
    }
}

