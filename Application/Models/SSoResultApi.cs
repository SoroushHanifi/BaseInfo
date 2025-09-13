using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models
{
    public class SSoResultApi
    {
        public int StatusCode { get; set; }
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
    }
    public class SSoResultApi<TData> : SSoResultApi
    {
        public TData Data { get; set; }
    }
}
