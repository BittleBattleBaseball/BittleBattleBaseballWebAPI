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
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace BittleBattleBaseball.ApplicationService
{
    public class TeamApplicationService
    {
        // private const string RAPID_API_GET_TEAMS_BY_SEASON_ENDPOINT_URL = "https://mlb-data.p.rapidapi.com/json/named.team_all_season.bam";

        public async Task<List<TeamSearchResultViewModel>> GetTeamsBySeason(string league, int season)
        {
            if(league == null) throw new ArgumentNullException(nameof(league));

            if(season == 0) throw new ArgumentNullException(nameof(season));

            int sportId = 1;//Default to mlb

            using (HttpClient client = new HttpClient())
            {
                string apiResponse = await client.GetStringAsync($"https://statsapi.mlb.com/api/v1/teams?season={season}&sportId={sportId}");

                TeamsBySeasonResponse teamsBySeasonResponse = JsonConvert.DeserializeObject<TeamsBySeasonResponse>(apiResponse);
                return ConvertResponseToViewModel(teamsBySeasonResponse);
            }

            //*******************************************************************************************************

            //string jsonData = await this.GetTeamsBySeasonJsonAsync(season);

            //GetTeamsBySeasonDTO teamsBySeasonDto = GetTeamsBySeasonDTO.FromJson(jsonData);

            //if (string.IsNullOrEmpty(league))
            //    league = "mlb";

            //return await GetTeamsBySeasonViewModelFromDTO(league, season, teamsBySeasonDto);
        }

        #region NEW MLB API DIRECT API CALLS

        private static List<TeamSearchResultViewModel> ConvertResponseToViewModel(TeamsBySeasonResponse teamsBySeasonResponse)
        {
            List<TeamSearchResultViewModel> returnList = new List<TeamSearchResultViewModel>();

            if (teamsBySeasonResponse != null && teamsBySeasonResponse.teams != null)
            {
                foreach (var teamResult in teamsBySeasonResponse.teams)
                {
                    if (teamResult.league != null && (teamResult.league.id == 103 || teamResult.league.id == 104)) //103 is American League and 104 is National League (Ignore other results like negro leagues for now)
                    {
                        returnList.Add(new TeamSearchResultViewModel
                        {
                            Id = Convert.ToInt32(teamResult.id),
                            TeamName = teamResult.teamName,
                            League = teamResult.league.name,
                            Season = teamResult.season,
                            Ballpark = teamResult.venue.name,
                            Name = teamResult.name,
                            FullTeamName = teamResult.name,
                            City = teamResult.shortName, //TODO - This may be wrong
                            NameAbbrev = teamResult.abbreviation,
                            LogoUrl = "https://d2p3bygnnzw9w3.cloudfront.net/req/202001161/tlogo/br/" + teamResult.abbreviation + "-" + teamResult.season + ".png"
                        });
                    }
                    else
                    {
                        Debug.WriteLine(teamResult.ToString());
                    }
                }
            }

            if (DateTime.Now.Second % 2 != 0)
                return returnList.OrderBy(x => x.League).ToList();

            return returnList.OrderByDescending(x => x.League).ToList();
        }


        #endregion

        public async Task<RosterSearchResultViewModel> GetRosterBySeason(string league, int season, int teamId, bool isDesignatedHitterEnabled)
        {
            if (league == null) throw new ArgumentNullException(nameof(league));

            if (season == 0) throw new ArgumentNullException(nameof(season));

            using (HttpClient client = new HttpClient())
            {
                string apiResponse = await client.GetStringAsync($"https://statsapi.mlb.com/api/v1/teams/{teamId}/roster/fullRoster?season={season}");

                RosterBySeasonResponse rosterBySeasonResponse = JsonConvert.DeserializeObject<RosterBySeasonResponse>(apiResponse);
                return await ConvertResponseToViewModel(league, season, teamId, isDesignatedHitterEnabled, rosterBySeasonResponse);
            }

            //***********************************************************************************************************

            //string jsonData = await this.GetRosterBySeasonJsonAsync(season, teamId);

            //GetRosterBySeasonDTO getRosterBySeasonDTO = GetRosterBySeasonDTO.FromJson(jsonData);

            //return await GetRosterBySeasonViewModelFromDTO(league, season, teamId, isDesignatedHitterEnabled, getRosterBySeasonDTO);
        }


        //private static async Task<RosterSearchResultViewModel> GetRosterBySeasonViewModelFromDTO(string league, int season, int teamId, bool isDesignatedHitterEnabled, GetRosterBySeasonDTO dto)
        private async Task<RosterSearchResultViewModel> ConvertResponseToViewModel(string league, int season, int teamId, bool isDesignatedHitterEnabled, RosterBySeasonResponse dto)
        {
            RosterSearchResultViewModel returnVal = new RosterSearchResultViewModel(isDesignatedHitterEnabled = isDesignatedHitterEnabled) { Id = teamId, Season = season };

            if (dto != null && dto.roster != null && dto.roster.Any())
            {
                returnVal.Hitters = new List<HitterPlayerSeasonViewModel>();
                returnVal.Pitchers = new List<PitcherPlayerSeasonViewModel>();
                PlayerApplicationService playerService = new PlayerApplicationService();


                foreach (var rosterPlayerResult in dto.roster)
                {
                    var playerVm = new PlayerViewModel
                    {
                        Id = Convert.ToInt32(rosterPlayerResult.person.id),
                        Bats = "r",//rosterPlayerResult.bats,  TODO - Don't have this info
                        Throws = "r",// rosterPlayerResult.throws, TODO - Don't have this info
                        PlayerName = rosterPlayerResult.person.fullName,
                        DOB = new DateTime(), //rosterPlayerResult.birth_date, TODO - Don't have this
                        Position = rosterPlayerResult.position.abbreviation,
                        // NickName = rosterPlayerResult.name_sort
                        // Weight = Convert.ToInt32(rosterPlayerResult.weight),
                        // Height = Convert.ToDecimal(rosterPlayerResult.height_feet + "." + rosterPlayerResult.height_inches)
                    };

                    //if (season > 1995)
                    //{
                    //    var imageSearchResult = playerService.GetPlayerImage(playerVm.PlayerName);
                    //    if (imageSearchResult != null)
                    //    {
                    //        playerVm.PlayerImageURL = imageSearchResult.ImageURL;// HttpUtility.UrlDecode();
                    //    }
                    //    else
                    //    {


                    //        if (rosterPlayerResult.primary_position.ToLower().Trim().Contains("p"))
                    //            playerVm.PlayerImageURL = "../assets/images/emptyPitcherIcon.png";
                    //        else
                    //            playerVm.PlayerImageURL = "../assets/images/emptyHitterIcon.png";
                    //    }
                    //}
                    //else
                    //{
                    //    if (rosterPlayerResult.primary_position.ToLower().Trim().Contains("p"))
                    //        playerVm.PlayerImageURL = "../assets/images/emptyPitcherIcon.png";
                    //    else
                    //        playerVm.PlayerImageURL = "../assets/images/emptyHitterIcon.png";
                    //}

                    playerVm.PlayerImageURL = "https://securea.mlb.com/mlb/images/players/head_shot/" + playerVm.Id + ".jpg";

                    
                    if (rosterPlayerResult.position.abbreviation.ToLower().Trim().Contains("p"))
                    {
                        PitcherPlayerSeasonViewModel playerSeasonVm;

                        playerSeasonVm = await playerService.GetPlayerSeasonPitchingStats(season, playerVm.Id, teamId);
                        if (playerSeasonVm != null)
                        {
                            playerSeasonVm.GameType = "R";
                            playerSeasonVm.LeagueType = league;
                            playerSeasonVm.Player = playerVm;
                            playerSeasonVm.Season = season;
                            returnVal.Pitchers.Add(playerSeasonVm);
                        }
                    }
                    else
                    {
                        HitterPlayerSeasonViewModel playerSeasonVm;

                        //if (season == DateTime.Today.Year)
                        //    playerSeasonVm = playerService.GetPlayerProjectedSeasonHittingStats(season, playerVm.Id, league);
                        //else
                            playerSeasonVm = await playerService.GetPlayerSeasonHittingStats(season, playerVm.Id, teamId);

                        if (playerSeasonVm != null)
                        {
                            playerSeasonVm.GameType = "R";
                            playerSeasonVm.LeagueType = league;
                            playerSeasonVm.Player = playerVm;
                            playerSeasonVm.Season = season;
                            returnVal.Hitters.Add(playerSeasonVm);
                        }
                    }
                    


                }

                //if (playerService.HasChanges)
                //{
                //    PlayerApplicationService.WriteChangesToFile();
                //}
            }

            return returnVal;
        }     

      

        //private async Task<string> GetRosterBySeasonJsonAsync(int season, int teamId)
        //{
        //    string url = $"https://mlb-data.p.rapidapi.com/json/named.roster_team_alltime.bam?team_id=\'{teamId}\'&start_season=\'{season}\'&end_season=\'{season}\'";
        //    HttpClient _httpClient = new HttpClient();

        //    var request = (HttpWebRequest)WebRequest.Create(url);

        //    _httpClient.DefaultRequestHeaders.Add("X-RapidAPI-Key", "af5352e3e5msh027e7a5c8c8cc76p157788jsndab27210c9c4");

        //    var task = await _httpClient.GetAsync(url).ConfigureAwait(false);

        //    task.EnsureSuccessStatusCode();

        //    return await task.Content.ReadAsStringAsync();
        //}
    }
}
