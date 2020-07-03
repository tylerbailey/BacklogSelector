using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BacklogSelector.Exceptions
{
    public class JSONResponse
    {
        public JSONResponse(bool success, Object data)
        {
            Success = success;
            Data = data;
        }
        public bool Success { get; set; }
        public Object Data { get; set; }
    }
}
