using System.Collections.Generic;

namespace BittleBattleBaseball.Models.MLBAPI.Responses
{
    public class Group
    {
        public string displayName { get; set; }
    }


    public class PlayerSeasonPitchingStatsResponse
    {
        public string copyright { get; set; }
        public List<PitchingStat> stats { get; set; }
    }

    public class PitchingSplit
    {
        public string season { get; set; }
        public PitchingStats stat { get; set; }
        public Team team { get; set; }
        public Player player { get; set; }
        public League league { get; set; }
        public Sport sport { get; set; }
        public string gameType { get; set; }
    }

    public class Sport
    {
        public int id { get; set; }
        public string link { get; set; }
        public string abbreviation { get; set; }
    }

    public class PitchingStat
    {
        public Type type { get; set; }
        public Group group { get; set; }
        public List<object> exemptions { get; set; }
        public List<PitchingSplit> splits { get; set; }
    }

    public class PitchingStats
    {
        public int gamesPlayed { get; set; }
        public int gamesStarted { get; set; }
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
        public string era { get; set; }
        public string inningsPitched { get; set; }
        public int wins { get; set; }
        public int losses { get; set; }
        public int saves { get; set; }
        public int saveOpportunities { get; set; }
        public int holds { get; set; }
        public int blownSaves { get; set; }
        public int earnedRuns { get; set; }
        public string whip { get; set; }
        public int battersFaced { get; set; }
        public int outs { get; set; }
        public int gamesPitched { get; set; }
        public int completeGames { get; set; }
        public int shutouts { get; set; }
        public int strikes { get; set; }
        public string strikePercentage { get; set; }
        public int hitBatsmen { get; set; }
        public int balks { get; set; }
        public int wildPitches { get; set; }
        public int pickoffs { get; set; }
        public int totalBases { get; set; }
        public string groundOutsToAirouts { get; set; }
        public string winPercentage { get; set; }
        public string pitchesPerInning { get; set; }
        public int gamesFinished { get; set; }
        public string strikeoutWalkRatio { get; set; }
        public string strikeoutsPer9Inn { get; set; }
        public string walksPer9Inn { get; set; }
        public string hitsPer9Inn { get; set; }
        public string runsScoredPer9 { get; set; }
        public string homeRunsPer9 { get; set; }
        public int inheritedRunners { get; set; }
        public int inheritedRunnersScored { get; set; }
        public int catchersInterference { get; set; }
        public int sacBunts { get; set; }
        public int sacFlies { get; set; }
    }
}
