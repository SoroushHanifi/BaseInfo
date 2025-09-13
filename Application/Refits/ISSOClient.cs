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
        

        [Get("/api/v1/User/GetCurrentUser")]
        Task<SSoResultApi<CurrentUserModel>> GetCurrentUser([Header("Cookie")] string token);

 
    }
}
