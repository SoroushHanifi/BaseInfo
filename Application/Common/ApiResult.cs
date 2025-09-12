using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common
{
    public class ApiResult<T>
    {
        public int StatusCode { get; set; }
        public string ErrorMessage { get; set; }
        public T Data { get; set; }
    }
}
