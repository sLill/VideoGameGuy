using System.ComponentModel.DataAnnotations;

namespace VideoGameShowdown.Models
{
    public class RAWG_Store
    {
        #region Properties..
        [Key]
        public int StoreId { get; set; }

        public int id { get; set; }
        public RAWG_StoreDetail store { get; set; }
        #endregion Properties..
    }
}
