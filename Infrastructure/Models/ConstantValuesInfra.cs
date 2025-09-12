using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Models
{
    public static class ConstantValuesInfra
    {
        public static string UsernameClaim;
        public static string CookieName;
        public const string X_Authorization = "X_Authorization";
        public const string BasicAuthorization = "basicAuthorization";
        public const string Pid = "pid";
        public const string API_KEY = "API_KEY";
        public const string X_Domain = "X-Domain";
        public const string ApplicationJson = "application/json";
        public const string ApplicationSoapXml = "application/soap+xml";
        public const string FormUrlEncodedContent = "FormUrlEncodedContent";
        public const string NoCache = "no-cache";
        public const string AuthorizationCode = "authorizationCode";
        public const string Basic = "basic";
        public const string SetCookie = "Set-Cookie";
        public const string Sub = "sub";
        public const string Role = "role";
        public const string username = "username";
        public const string password = "password";
        public const string Src = "Src";
        public const string FdaApiKey = "fda-api-key";

        public static class Seprator
        {
            public const char Comma = ',';
            public const char Colon = ':';
            public const char Semicolon = ';';
            public const string ExceptionMessageSeprator = "%&";
        }
    }
}
