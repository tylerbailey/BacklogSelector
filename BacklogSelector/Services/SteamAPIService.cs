using BacklogBrowser.Exceptions;
using BacklogBrowser.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
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
            var client = _httpClientFactory.CreateClient();

            try
            {
                var httpResponse = await client.GetAsync($"https://api.steampowered.com/ISteamUser/ResolveVanityURL/v1/?key={key}&vanityurl={vanityURL}");
                var responseJson = await httpResponse.Content.ReadAsStringAsync();
                var user = JsonConvert.DeserializeObject<User>(JObject.Parse(responseJson)["response"].ToString());
                if (user.UserId == null)
                    throw new SteamUserException("Could not resolve Steam user Id");
                return user.UserId;
            }
            catch (SteamUserException)
            {
                throw;
            }
            catch(Exception ex)
            {
                throw new SteamUserException("An error occured finding the Steam user Id", ex);
            }
        }

        /// <summary>
        /// Gets a list of games from a given Steam id and randomly selects one
        /// </summary>
        /// <param name="steamId">The Steam id to get info for</param>
        /// <returns></returns>        
        public async Task<string> GetSelectedGame(string steamId)
        {
            var key = _config["AppSettings:APIKey"];
            var client = _httpClientFactory.CreateClient();

            try
            {
                var httpResponse = await client.GetAsync("https://api.steampowered.com/IPlayerService/GetOwnedGames/v1/?key=" + key + "&steamid=" + steamId + "&include_appinfo=true");
                var responseJson = await httpResponse.Content.ReadAsStringAsync();
                var ownedGames = JsonConvert.DeserializeObject<OwnedGames>(JObject.Parse(responseJson)["response"].ToString());
                //TODO: Select a subset with zero play time 
                if (ownedGames.Games.Count < 1)
                    throw new SteamGamesException("No games found in library");

                var selected = ownedGames.Games[new Random().Next(0, ownedGames.Games.Count)];
                var returnedJson = JsonConvert.SerializeObject(selected);
                return returnedJson;
            }
            catch (SteamGamesException)
            {
                throw;
            }
            catch(Exception ex)
            {
                throw new SteamGamesException("An error occured finding the Steam user owned games", ex);
            }
        }
    }
}
