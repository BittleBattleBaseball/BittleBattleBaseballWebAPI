using System.Collections.Generic;

namespace BittleBattleBaseball.Models.MLBAPI.Responses
{  
    public class Player
    {
        public int id { get; set; }
        public string fullName { get; set; }
        public string link { get; set; }
    }

    public class PlayerSeasonHittingStatsResponse
    {
        public string copyright { get; set; }
        public List<HittingStat> stats { get; set; }
    }

    public class HittingSplit
    {
        public string season { get; set; }
        public HittingStats stat { get; set; }
        public Team team { get; set; }
        public Player player { get; set; }
        public League league { get; set; }
        public Sport sport { get; set; }
        public string gameType { get; set; }
    }

    public class HittingStat
    {
        public Type type { get; set; }
        public Group group { get; set; }
        public List<object> exemptions { get; set; }
        public List<HittingSplit> splits { get; set; }
    }

    public class HittingStats
    {
        public int gamesPlayed { get; set; }
        public int groundOuts { get; set; }
        public int airOuts { get; set; }
        public int runs { get; set; }
        public int doubles { get; set; }
        public int triples { get; set; }
        public int homeRuns { get; set; }
        public int strikeOuts { get; set; }
        public int baseOnBalls { get; set; }
        public int intentionalWalks { get; set; }
        public int hits { get; set; }
        public int hitByPitch { get; set; }
        public string avg { get; set; }
        public int atBats { get; set; }
        public string obp { get; set; }
        public string slg { get; set; }
        public string ops { get; set; }
        public int caughtStealing { get; set; }
        public int stolenBases { get; set; }
        public string stolenBasePercentage { get; set; }
        public int groundIntoDoublePlay { get; set; }
        public int numberOfPitches { get; set; }
        public int plateAppearances { get; set; }
        public int totalBases { get; set; }
        public int rbi { get; set; }
        public int leftOnBase { get; set; }
        public int sacBunts { get; set; }
        public int sacFlies { get; set; }
        public string babip { get; set; }
        public string groundOutsToAirouts { get; set; }
        public int catchersInterference { get; set; }
        public string atBatsPerHomeRun { get; set; }
    }

    public class Type
    {
        public string displayName { get; set; }
    }


}
