using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WhiteLabel.Utility
{
    internal class Settings
    {
        private readonly IConfiguration configuration;

        public IConfiguration Configuration
        {
            get { return this.configuration; }
        }

        internal Settings()
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            if (string.IsNullOrWhiteSpace(env))
                throw new ArgumentException("Environment variable 'ASPNETCORE_ENVIRONMENT' is not set.");

            try
            {
                var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                                                        .AddJsonFile($"appsettings.{env}.json", optional: true, reloadOnChange: true)
                                                        .AddEnvironmentVariables();
                this.configuration = builder.Build();
            }
            catch (FormatException e)
            {
                throw new InvalidOperationException("Configuration files not configured correctly! Check the.json file format.", e);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error loading configuration files.", ex);
            }
        }
    }
}