using BittleBattleBaseball.Models.DomainModels;
using BittleBattleBaseball.Models.DTOs;
using BittleBattleBaseball.Models.MLBAPI.Responses;
using BittleBattleBaseball.Models.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace BittleBattleBaseball.ApplicationService
{
    public class PlayerApplicationService
    {
        private static List<PlayerImageSearchResult> _playerImageCache;

        public bool HasChanges { get; set; }

        public static List<PlayerImageSearchResult> PlayerImageCache
        {
            get
            {
                if(_playerImageCache == null)
                {
                    _playerImageCache = new List<PlayerImageSearchResult>();

                    //TODO - Load from a .json file from disk
                    LoadJson();
                }

                return _playerImageCache;
            }
        }

        public static void LoadJson()
        {
            if (File.Exists(@"C:\DEV\BittleBattleBaseballWebAPI\BittleBattleBaseballWebAPI\jsonPlayerDictionary.json"))
            {
                using (StreamReader r = new StreamReader(@"C:\DEV\BittleBattleBaseballWebAPI\BittleBattleBaseballWebAPI\jsonPlayerDictionary.json"))
                {
                    string json = r.ReadToEnd();
                    _playerImageCache = JsonConvert.DeserializeObject<IEnumerable<PlayerImageSearchResult>>(json).ToList();
                }
            }
        }

        public static void WriteChangesToFile()
        {
            string json = JsonConvert.SerializeObject(PlayerImageCache, Formatting.None);

            using (StreamWriter file = new StreamWriter(@"C:\DEV\BittleBattleBaseballWebAPI\BittleBattleBaseballWebAPI\jsonPlayerDictionary.json", false))
            {
                file.Write(json);
            }
        }

        #region Player Images

        public PlayerImageSearchResult GetPlayerImage(string playerName)
        {
            var cachedPlayerImage = PlayerImageCache.FirstOrDefault(x => x.PlayerName == playerName);
            if (cachedPlayerImage != null)
            {
                return new PlayerImageSearchResult { ImageURL = HttpUtility.UrlDecode(@cachedPlayerImage.ImageURL)  };
            }

            string jsonData = this.GetPlayerImagesJson(playerName);

            try
            {
                GetPlayerImagesDTO dto = GetPlayerImagesDTO.FromJson(jsonData);
                var result = GetPlayerImagesFromDTO(dto);
                PlayerImageCache.Add(new PlayerImageSearchResult { PlayerName = playerName, ImageURL = HttpUtility.UrlEncode(@result.ImageURL) });
                this.HasChanges = true;
                return result;
            }
            catch
            {
                return null;
            }
        }

        private static PlayerImageSearchResult GetPlayerImagesFromDTO(GetPlayerImagesDTO dto)
        {           
            if (dto != null && dto.results != null && dto.results.Any())
            {
                foreach (var dtoResult in dto.results)
                {
                    if (!string.IsNullOrEmpty(dtoResult.iurl) 
                        && (
                        dtoResult.iurl.ToLower().Contains("espncdn.com") && dtoResult.iurl.ToLower().Contains("headshots/mlb")
                         ||
                        (!string.IsNullOrWhiteSpace(dtoResult.domain) && 
                        (dtoResult.domain.ToLower().Contains("baseball")
                        || dtoResult.domain.ToLower().Contains("baseballhall.org")
                        || dtoResult.domain.ToLower().Contains("cloudfront.net"))))
                       )
                    {
                        return new PlayerImageSearchResult
                        {
                            ImageURL = dtoResult.iurl,
                            ImageSourceDomain = dtoResult.domain
                        };
                    }
                }
            }

            return null;
        }

        private string GetPlayerImagesJson(string playerName)
        {
            System.Threading.Thread.Sleep(1100);

            string url = $"https://faroo-faroo-web-search.p.rapidapi.com/api?q='{playerName} espn'";
            var request = (HttpWebRequest)WebRequest.Create(url);

            request.Method = "GET";
            request.Headers.Add("X-RapidAPI-Key", "af5352e3e5msh027e7a5c8c8cc76p157788jsndab27210c9c4");

            var content = string.Empty;

            using (var response = (HttpWebResponse)request.GetResponse())
            {
                using (var stream = response.GetResponseStream())
                {
                    using (var sr = new StreamReader(stream))
                    {
                        content = sr.ReadToEnd();
                    }
                }
            }

            return content;
        }

        #endregion

        #region Hitting
        //public HitterPlayerSeasonViewModel GetPlayerProjectedSeasonHittingStats(int season, int playerId, string leagueListId)
        //{            
        //    string jsonData = this.GetSeasonProjectedHittingStatsJson(season, playerId, leagueListId);

        //    try
        //    {
        //        GetPlayerProjectedSeasonHittingSingleTeamStatsDTO dto = GetPlayerProjectedSeasonHittingSingleTeamStatsDTO.FromJson(jsonData);
        //        return GetPlayerSeasonHittingSingleTeamViewModelFromDTO(dto);
        //    }
        //    catch (JsonSerializationException)
        //    {
        //        try
        //        {
        //            GetPlayerSeasonHittingMultiTeamStatsDTO dto = GetPlayerSeasonHittingMultiTeamStatsDTO.FromJson(jsonData);
        //            return GetPlayerSeasonHittingMultiTeamViewModelFromDTO(dto);
        //        }
        //        catch
        //        {
        //            return null;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine(ex.ToString());
        //        return null;
        //    }
        //}

        public async Task<HitterPlayerSeasonViewModel> GetPlayerSeasonHittingStats(int season, int playerId, int teamId)
        {
            using (HttpClient client = new HttpClient())
            {
                string responseBody = await client.GetStringAsync($"https://statsapi.mlb.com/api/v1/stats?stats=season&group=hitting&season={season}&teamId={teamId}&personId={playerId}");
                PlayerSeasonHittingStatsResponse dto = JsonConvert.DeserializeObject<PlayerSeasonHittingStatsResponse>(responseBody);
                return GetPlayerSeasonHittingSingleTeamViewModelFromDTO(dto, teamId, playerId);
            }
        }

        //private string GetSeasonHittingStatsJson(int season, int playerId, string leagueListId, string gameType)
        //{
        //    string url = $"https://mlb-data.p.rapidapi.com/json/named.sport_hitting_tm.bam?season=\'{season}\'&player_id=\'{playerId}\'&league_list_id=\'{leagueListId}\'&game_type=\'{gameType}\'";
        //    var request = (HttpWebRequest)WebRequest.Create(url);

        //    request.Method = "GET";
        //    request.Headers.Add("x-rapidapi-host", "mlb-data.p.rapidapi.com");
        //    request.Headers.Add("x-rapidapi-key", "af5352e3e5msh027e7a5c8c8cc76p157788jsndab27210c9c4");          

        //    var content = string.Empty;

        //    using (var response = (HttpWebResponse)request.GetResponse())
        //    {
        //        using (var stream = response.GetResponseStream())
        //        {
        //            using (var sr = new StreamReader(stream))
        //            {
        //                content = sr.ReadToEnd();
        //            }
        //        }
        //    }

        //    return content;
        //}

        //private string GetSeasonProjectedHittingStatsJson(int season, int playerId, string leagueListId)
        //{
        //    string url = $"https://mlb-data.p.rapidapi.com/json/named.proj_pecota_batting.bam?season=\'{season}\'&player_id=\'{playerId}\'&league_list_id=\'{leagueListId}\'";
        //    var request = (HttpWebRequest)WebRequest.Create(url);

        //    request.Method = "GET";
        //    request.Headers.Add("x-rapidapi-host", "mlb-data.p.rapidapi.com");
        //    request.Headers.Add("x-rapidapi-key", "af5352e3e5msh027e7a5c8c8cc76p157788jsndab27210c9c4");

        //    var content = string.Empty;

        //    using (var response = (HttpWebResponse)request.GetResponse())
        //    {
        //        using (var stream = response.GetResponseStream())
        //        {
        //            using (var sr = new StreamReader(stream))
        //            {
        //                content = sr.ReadToEnd();
        //            }
        //        }
        //    }

        //    return content;
        //}

        //private static HitterPlayerSeasonViewModel GetPlayerSeasonHittingMultiTeamViewModelFromDTO(GetPlayerSeasonHittingMultiTeamStatsDTO dto)
        //{
        //    HitterPlayerSeasonViewModel returnVal = new HitterPlayerSeasonViewModel();

        //    if(dto != null && dto.sport_hitting_tm != null && dto.sport_hitting_tm.queryResults != null && dto.sport_hitting_tm.queryResults.row != null)
        //    {
        //        foreach (Stat playerStats in dto.sport_hitting_tm.queryResults.row)
        //        {
        //            PopulateBattingStats(returnVal, playerStats);
        //        }
        //    }

        //    return returnVal;
        //}

        private static HitterPlayerSeasonViewModel GetPlayerSeasonHittingSingleTeamViewModelFromDTO(PlayerSeasonHittingStatsResponse dto, int teamId, int playerId)
        {
            HitterPlayerSeasonViewModel returnVal = new HitterPlayerSeasonViewModel();

            if (dto != null && dto.stats != null)
            {
                var playerStat = dto.stats.FirstOrDefault();
                var playerSplit = playerStat.splits.FirstOrDefault(s => s.rank == 1);
                if (playerSplit != null)
                {
                    var playerStats = playerSplit.stat;
                    PopulateBattingStats(returnVal, playerStats, playerId);
                }
            }

            return returnVal;
        }

        //private static HitterPlayerSeasonViewModel GetPlayerSeasonHittingSingleTeamViewModelFromDTO(GetPlayerProjectedSeasonHittingSingleTeamStatsDTO dto)
        //{
        //    HitterPlayerSeasonViewModel returnVal = new HitterPlayerSeasonViewModel();

        //    if (dto != null && dto.proj_pecota_batting != null && dto.proj_pecota_batting.queryResults != null && dto.proj_pecota_batting.queryResults.row != null)
        //    {
        //        var playerStats = dto.;
        //        PopulateBattingStats(returnVal, playerStats);
        //    }

        //    return returnVal;
        //}

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
                string responseBody = await client.GetStringAsync($"https://statsapi.mlb.com/api/v1/stats?stats=season&group=pitching&season={season}&teamId={teamId}&personId={playerId}");
                PlayerSeasonPitchingStatsResponse dto = JsonConvert.DeserializeObject<PlayerSeasonPitchingStatsResponse>(responseBody);
                return GetPlayerSeasonPitchingSingleTeamViewModelFromDTO(dto, teamId, playerId);
            }
        }

        //private string GetSeasonPitchingStatsJson(int season, int playerId, string leagueListId, string gameType)
        //{
        //    string url = $"https://mlb-data.p.rapidapi.com/json/named.sport_pitching_tm.bam?season=\'{season}\'&player_id=\'{playerId}\'&league_list_id=\'{leagueListId}\'&game_type=\'{gameType}\'";
        //    var request = (HttpWebRequest)WebRequest.Create(url);

        //    request.Method = "GET";
        //    request.Headers.Add("X-RapidAPI-Key", "af5352e3e5msh027e7a5c8c8cc76p157788jsndab27210c9c4");
        //    request.Headers.Add("x-rapidapi-host", "mlb-data.p.rapidapi.com");

        //    var content = string.Empty;

        //    using (var response = (HttpWebResponse)request.GetResponse())
        //    {
        //        using (var stream = response.GetResponseStream())
        //        {
        //            using (var sr = new StreamReader(stream))
        //            {
        //                content = sr.ReadToEnd();
        //            }
        //        }
        //    }

        //    return content;
        //}

        private static PitcherPlayerSeasonViewModel GetPlayerSeasonPitchingSingleTeamViewModelFromDTO(PlayerSeasonPitchingStatsResponse dto, int teamId, int playerId)
        {
            PitcherPlayerSeasonViewModel returnVal = new PitcherPlayerSeasonViewModel();

            if (dto != null && dto.stats != null)
            {
                var playerStat = dto.stats.FirstOrDefault();
                var playerSplit = playerStat.splits.FirstOrDefault(s => s.rank == 1);
                if (playerSplit != null)
                {
                    var playerStats = playerSplit.stat;
                    PopulatePitchingStats(returnVal, playerStats, playerId);
                }
            }

            return returnVal;
        }

        //private static PitcherPlayerSeasonViewModel GetPlayerSeasonPitchingMultiTeamViewModelFromDTO(GetPlayerSeasonPitchingMultiTeamStatsDTO dto)
        //{
        //    PitcherPlayerSeasonViewModel returnVal = new PitcherPlayerSeasonViewModel();

        //    if (dto != null && dto.stats != null)
        //    {
        //        var playerStat = dto.stats.FirstOrDefault();
        //        var playerStats = playerStat.splits.FirstOrDefault(s => s.team.id == teamId && s.player.id == playerId && s.stat != null).stat;
        //        PopulateBattingStats(returnVal, playerStats, playerId);
        //    }

        //    return returnVal;

        //    PitcherPlayerSeasonViewModel returnVal = new PitcherPlayerSeasonViewModel();

        //    if (dto != null && dto.sport_pitching_tm != null && dto.sport_pitching_tm.queryResults != null && dto.sport_pitching_tm.queryResults.row != null)
        //    {
        //        foreach (PitchingStat playerStats in dto.sport_pitching_tm.queryResults.row)
        //        {
        //            PopulatePitchingStats(returnVal, playerStats);
        //        }
        //    }

        //    return returnVal;
        //}

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
