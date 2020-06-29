using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace BacklogBrowser.Exceptions
{
    public class SteamUserException : Exception
    {

        public SteamUserException()
        {
            
        }

        public SteamUserException(string message)
        : base(message)
        {

        }

        public SteamUserException(string message, Exception inner)
        : base(message, inner)
        {

        }
    }
}
