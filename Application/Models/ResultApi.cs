using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models
{
    public class ResultApi
    {
        public int? StatusCode { get; set; }
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    public class ResultApi<TData> : ResultApi
    {
        public TData Data { get; set; }
    }

}
