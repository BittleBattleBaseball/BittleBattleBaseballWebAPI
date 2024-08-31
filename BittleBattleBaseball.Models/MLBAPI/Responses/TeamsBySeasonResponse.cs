using System;
using System.Collections.Generic;
using System.Text;

namespace BittleBattleBaseball.Models.MLBAPI.Responses
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class League
    {
        public int id { get; set; }
        public string name { get; set; }
        public string link { get; set; }
    }

    public class TeamsBySeasonResponse
    {
        public string copyright { get; set; }
        public List<Team> teams { get; set; }
    }

    public class Sport
    {
        public int id { get; set; }
        public string link { get; set; }
        public string name { get; set; }
    }

    public class Team
    {
        public string allStarStatus { get; set; }
        public int id { get; set; }
        public string name { get; set; }
        public string link { get; set; }
        public int season { get; set; }
        public Venue venue { get; set; }
        public string teamCode { get; set; }
        public string fileCode { get; set; }
        public string abbreviation { get; set; }
        public string teamName { get; set; }
        public string locationName { get; set; }
        public string firstYearOfPlay { get; set; }
        public League league { get; set; }
        public Sport sport { get; set; }
        public string shortName { get; set; }
        public string parentOrgName { get; set; }
        public int parentOrgId { get; set; }
        public string franchiseName { get; set; }
        public string clubName { get; set; }
        public bool active { get; set; }
    }

    public class Venue
    {
        public int id { get; set; }
        public string name { get; set; }
        public string link { get; set; }
    }


}
