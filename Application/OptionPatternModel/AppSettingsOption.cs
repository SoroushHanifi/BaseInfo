using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.OptionPatternModel
{
    public class AppSettingsOption
    {
        public string AllowedOrigins { get; set; }
        public SettingsOption Settings { get; set; }
    }

    public class RedisOption
    {
        public string ConnectionString { get; set; }
        public string InstanceName { get; set; }
    }

    public class UrlsOption
    {
        public string SSO { get; set; }
        public string BizagiPanel { get; set; }
        public string[] BizagiPanelList { get; set; }
        public string Bizagi { get; set; }
        public string Organization { get; set; }
        public string External { get; set; }
        public string PaymentCallBackUrl { get; set; }
    }

    public class SettingsOption
    {
        public BizagiOption Bizagi { get; set; }
        public CookieInfoOption CookieInfo { get; set; }
        public UserSystemOption UserSystem { get; set; }
        public FilePathOption FilePath { get; set; }
        public JaegerOption Jaeger { get; set; }
        public IdentitySettingsOption IdentitySettings { get; set; }
    }

    public class BizagiOption
    {
        public int SessionTimeout { get; set; }
        public string ApplicationName { get; set; }
    }

    public class CookieInfoOption
    {
        public string Name { get; set; }
        public string Domain { get; set; }
        public string UsernameClaim { get; set; }
    }

    public class UserSystemOption
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string TokenKey { get; set; }
    }

    public class FilePathOption
    {
        public string BasePath { get; set; }
    }

    public class JaegerOption
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public float SamplingRate { get; set; }
        public bool IsEnabled { get; set; }
    }

    public class IdentitySettingsOption
    {
        public string Authority { get; set; }
        public string PasswordRequireDigit { get; set; }
        public string PasswordRequiredLength { get; set; }
        public string PasswordRequireNonAlphanumeric { get; set; }
        public string PasswordRequireUppercase { get; set; }
        public string PasswordRequireLowercase { get; set; }
        public string RequireUniqueEmail { get; set; }
        public bool RequireHttpsMetadata { get; set; }
        public string Audience { get; set; }
    }
}
