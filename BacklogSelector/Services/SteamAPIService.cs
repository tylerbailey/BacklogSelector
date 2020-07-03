using BacklogBrowser.Exceptions;
using BacklogBrowser.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace BacklogBrowser.Services
{
    public class SteamAPIService : ISteamAPIService
    {
        private readonly IConfiguration _config;
        private readonly IHttpClientFactory _httpClientFactory;
        public SteamAPIService(IConfiguration config, IHttpClientFactory httpClientFactory)
        {
            _config = config;
            _httpClientFactory = httpClientFactory;

        }

        /// <summary>
        /// Takes a vanity URL and attempts to get the internal id for the user
        /// </summary>
        /// <param name="vanityURL">the vanity url to get the Steam id for</param>
        /// <returns></returns>
        public async Task<string> GetSteamId(String vanityURL)
        {
            var key = _config["AppSettings:APIKey"];
            String userId;

            try
            {
                using (HttpClient client = _httpClientFactory.CreateClient())
                {
                    var httpResponse = await client.GetAsync($"https://api.steampowered.com/ISteamUser/ResolveVanityURL/v1/?key={key}&vanityurl={vanityURL}");
                    var responseJson = await httpResponse.Content.ReadAsStringAsync();
                    var user = JsonConvert.DeserializeObject<User>(JObject.Parse(responseJson)["response"].ToString());
                    if (user.UserId == null)
                        throw new SteamUserException("Could not resolve Steam user Id");
                    userId = user.UserId;
                }              
                return userId;
            }
            catch (SteamUserException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new SteamUserException("An error occured finding the Steam user Id", ex);
            }
        }

        /// <summary>
        /// Gets a list of games from a given Steam id and randomly selects one
        /// </summary>
        /// <param name="steamId">The Steam id to get info for</param>
        /// <returns></returns>        
        public async Task<Game> GetSelectedGame(string steamId)
        {
            var key = _config["AppSettings:APIKey"];
            Game selected;

            try
            {
                using (HttpClient client = _httpClientFactory.CreateClient())
                {
                    var httpResponse = await client.GetAsync("https://api.steampowered.com/IPlayerService/GetOwnedGames/v1/?key=" + key + "&steamid=" + steamId + "&include_appinfo=true");
                    var responseJson = await httpResponse.Content.ReadAsStringAsync();
                    var ownedGames = JsonConvert.DeserializeObject<OwnedGames>(JObject.Parse(responseJson)["response"].ToString());
                    var noPlayTime = (from game in ownedGames.Games where game.PlayTime.Equals("0") && game.PlayTimeWindows.Equals("0") && game.PlayTimeMac.Equals("0") && game.PlayTimeLinux.Equals("0") select game).ToList();
                    if (ownedGames.Games.Count < 1)
                        throw new SteamGamesException("No games found in library");
                    var index = new Random().Next(0, noPlayTime.Count);
                    selected = noPlayTime[index];
                }
                return selected;
            }
            catch (SteamGamesException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new SteamGamesException("An error occured finding the Steam user owned games", ex);
            }
        }
    }
}
