using BacklogBrowser.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Net;
using System.Threading.Tasks;

namespace BacklogBrowser.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BacklogController : ControllerBase
    {      
        private readonly ISteamAPIService _apiService;

        public BacklogController(ISteamAPIService apiService)
        {            
            _apiService = apiService;
        }

        [HttpGet]
        [Route("ping")]
        public HttpStatusCode Ping()
        {
            return HttpStatusCode.OK;
        }

        [HttpPost]
        [Route("getgame")]
        public async Task<string> SelectBacklogGame([FromBody] string vanityUrl)
        {
            string response;

            try
            {
                var steamId = await _apiService.GetSteamId(vanityUrl);
                response = await _apiService.GetSelectedGame(steamId);
            }          
            catch(Exception ex)
            {
                //logging
                response = ex.Message;
            }
            return response;
        }


     
    }
}
