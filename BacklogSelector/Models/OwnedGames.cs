using Newtonsoft.Json;
using System.Collections.Generic;

namespace BacklogBrowser.Models
{
    public class OwnedGames
    {
        public OwnedGames()
        {
            Games = new List<Game>();
        }

        [JsonProperty("game_count")]
        public int GameCount { get; set; }

        [JsonProperty("games")]
        public List<Game> Games { get; set; }
    }
}
