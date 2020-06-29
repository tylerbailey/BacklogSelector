using System.Threading.Tasks;

namespace BacklogBrowser.Services
{
    public interface ISteamAPIService
    {
        Task<string> GetSteamId(string vanityURL);
        Task<string> GetSelectedGame(string steamId);
    }
}