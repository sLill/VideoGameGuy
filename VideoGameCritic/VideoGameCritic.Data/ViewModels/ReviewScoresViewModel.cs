using VideoGameCritic.Common;

namespace VideoGameCritic.Data
{
    public class ReviewScoresViewModel
    {
        #region Properties..
        public Game GameOne { get; set; }
        public Game GameTwo { get; set; }
        public Guid UserChoiceId { get; set; }
        #endregion Properties..
    }
}
