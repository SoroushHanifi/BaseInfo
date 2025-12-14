using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Models;
using Refit;

namespace Application.Refits
{
    public interface ISSOClient
    {
        [Post("/api/v1/Authenticate")]
        Task<HttpResponseMessage> Authenticate([Body] GetUserSystemTokenModel body);


        [Get("/api/v1/User/GetCurrentUser")]
        Task<SSoResultApi<CurrentUserModel>> GetCurrentUser([Header("Cookie")] string token);

 
    }
}
