﻿namespace VideoGameGuy.Data
{
    public class DescriptionsSessionData : SessionDataBase
    {
        #region Records..
        public record DescriptionsRound
        {
            public string GameTitle { get; set; }
            public string GameMediaUrl { get; set; }
            public string GameDescription { get; set; }
            public bool IsSolved { get; set; }
            public TimeSpan TimeRemaining { get; set; } 
        }
        #endregion Records..

        #region Properties..
        public List<DescriptionsRound> DescriptionsRounds { get; set; } = new List<DescriptionsRound>();

        public int HighestStreak { get; set; }

        public DescriptionsRound CurrentRound
            => DescriptionsRounds.LastOrDefault(x => !x.IsSolved);
        #endregion Properties..
    }
}
