using BacklogBrowser.Models;
using System.Threading.Tasks;

namespace BacklogBrowser.Services
{
    public interface ISteamAPIService
    {
        Task<string> GetSteamId(string vanityURL);
        Task<Game> GetSelectedGame(string steamId);
    }
}