using System.ComponentModel.DataAnnotations;

namespace VideoGameShowdown.Models
{
    public class RAWG_AddedByStatus
    {
        #region Properties..
        [Key]
        public int AddedByStatusId { get; set; }

        public int yet { get; set; }
        public int owned { get; set; }
        public int beaten { get; set; }
        public int toplay { get; set; }
        public int dropped { get; set; }
        public int playing { get; set; }
        #endregion Properties..
    }
}

