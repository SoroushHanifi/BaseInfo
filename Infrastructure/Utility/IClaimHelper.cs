using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Utility
{
    public interface IClaimHelper
    {
        string GetUserId();
        List<string> GetAllRoleIds();
        string GetToken();
        string GetTokenFromCookie(bool byKey = true, string cookieName = null);
        List<string> GetRoleCodes();
        IPAddress GetUserIP();
        string GetRoleType();
        string GetUserName();
        List<string> GetUserRoleCode();
        DecodedBase64 BasicAuthContractors();
        DecodedBase64 GetBase64String(string encodedString);

    }
}
