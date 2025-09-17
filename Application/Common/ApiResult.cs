using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common
{
    public class ApiResult<T>
    {
        public bool IsSuccess { get; set; }
        public string Messages { get; set; }
        public T Data { get; set; }
    }
}
