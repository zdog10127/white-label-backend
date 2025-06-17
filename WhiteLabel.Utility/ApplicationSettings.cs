using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WhiteLabel.Utility
{
    public class ApplicationSettings
    {
        public static string GetStringConnectionDB()
        {
            Settings settings = new Settings();
            return settings.Configuration["ConnectionString"];
        }
    }
}