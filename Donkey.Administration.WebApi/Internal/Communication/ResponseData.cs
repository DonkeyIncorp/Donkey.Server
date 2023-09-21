using System;
using System.Collections.Generic;
using System.Text;

namespace Donkey.Administration.WebApi
{
    public class ResponseData
    {
        public string Message { get; set; }
        public object Data { get; set; }

        public static ResponseData NegativeResponse(string message = "Некорректный запрос") => new ResponseData() 
        {
            Data = null, 
            Message = message 
        };

        public static ResponseData PositiveResponse(object data, string message = "") => new ResponseData()
        {
            Data = data,
            Message = message
        };
    }
}
