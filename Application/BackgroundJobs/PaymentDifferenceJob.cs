using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Application.BackgroundJobs
{
    public record PaymentDifferenceJob(
    PaymentDifferenceJobData Data,
    string SystemToken,
    string RequestId); // برای تراک کردن درخواست اصلی

    public record PaymentDifferenceJobData(
        string ApplicationName,
        int LicenseType,
        int WfClass,
        string NationalCode);
}
