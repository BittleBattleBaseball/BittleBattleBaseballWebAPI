using BittleBattleBaseball.Models.MLBAPI.Responses;
using BittleBattleBaseball.Models.ViewModels;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace BittleBattleBaseball.ApplicationService
{
    public class PlayerApplicationService
    {
        public bool HasChanges { get; set; }

        #region Hitting    

        public async Task<HitterPlayerSeasonViewModel> GetPlayerSeasonHittingStats(int season, int playerId, int teamId)
        {
            using (HttpClient client = new HttpClient())
            {
                string responseBody = await client.GetStringAsync($"https://statsapi.mlb.com/api/v1/people/{playerId}/stats?stats=statsSingleSeason&group=hitting&gameType=R&season={season}&teamId={teamId}");
            
                PlayerSeasonHittingStatsResponse dto = JsonConvert.DeserializeObject<PlayerSeasonHittingStatsResponse>(responseBody);
                return GetPlayerSeasonHittingSingleTeamViewModelFromDTO(dto, playerId);
            }
        }

        private static HitterPlayerSeasonViewModel GetPlayerSeasonHittingSingleTeamViewModelFromDTO(PlayerSeasonHittingStatsResponse dto, int playerId)
        {
            HitterPlayerSeasonViewModel returnVal = new HitterPlayerSeasonViewModel();

            if (dto != null && dto.stats != null)
            {
                var playerStat = dto.stats.FirstOrDefault();
                var playerSplit = playerStat?.splits.FirstOrDefault(s => s.player.id == playerId);
                if (playerSplit != null)
                {
                    var playerStats = playerSplit.stat;
                    PopulateBattingStats(returnVal, playerStats, playerId);
                }
            }

            return returnVal;
        }       

        private static void PopulateBattingStats(HitterPlayerSeasonViewModel hitterInfo, HittingStats playerStats, int playerId)
        {
            hitterInfo.AB = playerStats.atBats;
            hitterInfo.AVG = Convert.ToDecimal( playerStats.avg);
            hitterInfo.OBP = Convert.ToDecimal(playerStats.obp);
            hitterInfo.SLG = Convert.ToDecimal(playerStats.slg);
            hitterInfo.HR = playerStats.homeRuns;
            hitterInfo.RBI = playerStats.rbi;
            hitterInfo.BB = playerStats.baseOnBalls;
            hitterInfo.SB = playerStats.stolenBases;
            hitterInfo.CS = playerStats.caughtStealing;
            hitterInfo.GIDP = playerStats.groundIntoDoublePlay;
            hitterInfo.H = playerStats.hits;
            hitterInfo.R = playerStats.runs;
            hitterInfo.SO = playerStats.strikeOuts;
            hitterInfo.XBH =  playerStats.doubles + playerStats.triples + playerStats.homeRuns;

            hitterInfo.Player = new PlayerViewModel
            {
                Id = playerId
            };
        }

        #endregion

        #region Pitching

        public async Task<PitcherPlayerSeasonViewModel> GetPlayerSeasonPitchingStats(int season, int playerId, int teamId)
        {
            using (HttpClient client = new HttpClient())
            {
                string responseBody = await client.GetStringAsync($"https://statsapi.mlb.com/api/v1/people/{playerId}/stats?stats=statsSingleSeason&group=pitching&gameType=R&season={season}&teamId={teamId}");
            
                PlayerSeasonPitchingStatsResponse dto = JsonConvert.DeserializeObject<PlayerSeasonPitchingStatsResponse>(responseBody);
                return GetPlayerSeasonPitchingSingleTeamViewModelFromDTO(dto, playerId);
            }
        }

        private static PitcherPlayerSeasonViewModel GetPlayerSeasonPitchingSingleTeamViewModelFromDTO(PlayerSeasonPitchingStatsResponse dto, int playerId)
        {
            PitcherPlayerSeasonViewModel returnVal = new PitcherPlayerSeasonViewModel();

            if (dto != null && dto.stats != null)
            {
                var playerStat = dto.stats.FirstOrDefault();
                var playerSplit = playerStat?.splits.FirstOrDefault(s => s.player.id == playerId);
                if (playerSplit != null)
                {
                    var playerStats = playerSplit.stat;
                    PopulatePitchingStats(returnVal, playerStats, playerId);
                }
            }

            return returnVal;
        }

        private static void PopulatePitchingStats(PitcherPlayerSeasonViewModel pitcherInfo, PitchingStats playerStats, int playerId)
        {
            pitcherInfo.Wins = playerStats.wins;
            pitcherInfo.Losses = playerStats.losses;
            pitcherInfo.ERA = Convert.ToDecimal(playerStats.era);
            pitcherInfo.WHIP = Convert.ToDecimal(playerStats.whip);


            pitcherInfo.Player = new PlayerViewModel
            {
                Id = playerId
            };
        }


        #endregion
    }
}
