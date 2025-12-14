using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Models;
using Bizagi.Application.Models;
using Microsoft.AspNetCore.Mvc;
using Refit;

namespace Application.Refits
{
    public interface IPaymentOrganization
    {


        [Get("/api/v1/Payment/CreateCaseDifference")]
        Task<OrganizationApiResult<BizagiApiResponse>> CreateCaseDifference([Header("Cookie")] string token,string ApplicationName,int LicenseType, int WfClass,string NationalCode);
    }

    public class OrganizationApiResult<T>
    {
        public T Data { get; set; }
    }
}