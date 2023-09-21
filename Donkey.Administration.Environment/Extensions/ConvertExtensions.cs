using System;
using System.Collections.Generic;
using System.Text;
using ObjectSystem;


namespace Donkey.Administration.Environment.Extensions
{
    public static class ConvertExtensions
    {
        private const string DATE_TIME_FORMAT = "dd.MM.yyyy HH:mm";
        public static string Format(this DateTime dateTime)
        {
            return dateTime.ToString(DATE_TIME_FORMAT);
        }
    }
}
