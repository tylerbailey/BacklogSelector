using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BacklogBrowser.Exceptions
{
    public class SteamGamesException : Exception
    {

        public SteamGamesException()
        {

        }

        public SteamGamesException(string message)
        : base(message)
        {

        }

        public SteamGamesException(string message, Exception inner)
        : base(message, inner)
        {

        }
    }
}
