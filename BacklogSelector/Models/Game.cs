using Newtonsoft.Json;
using System;

namespace BacklogBrowser.Models
{
    public class Game
    {       
        [JsonProperty("appid")]
        public string AppId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("img_icon_url")]
        public String ImageIcon { get; set;}

        [JsonProperty("img_logo_url")]
        public string ImageLogo { get; set; }

        [JsonProperty("playtime_forever")]
        public string PlayTime { get; set; }

        [JsonProperty("playtime_windows_forever")]
        public string PlayTimeWindows { get; set; }

        [JsonProperty("playtime_mac_forever")]
        public string PlayTimeMac { get; set; }

        [JsonProperty("playtime_linux_forever")]
        public string PlayTimeLinux { get; set; }

    }
}
