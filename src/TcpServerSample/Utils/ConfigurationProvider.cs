using Microsoft.Extensions.Configuration;

namespace TcpServerSample.Utils
{
    /// <summary>
    /// 配置提供器
    /// </summary>
    public static class ConfigurationProvider
    {
        private static IConfiguration? _configuration;

        public static IConfiguration Instance
        {
            get
            {
                _configuration ??= new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                        .Build();
                return _configuration;
            }
        }
    }
}
