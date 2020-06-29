using Newtonsoft.Json;

namespace BacklogBrowser.Models
{
    public class User
    {            
        //{ "response":{ "steamid":"76561197992537377","success":1} }
        [JsonProperty("steamid")]
        public string UserId { get; set; }

        [JsonProperty("success")]
        public int Success { get; set; }
    }
}
