using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models
{
    public class MainTitleServiceFeatureInput
    {
        public long ServiceFeatureId { get; set; }
        public bool IsActive { get; set; }
        public int DisplayOrder { get; set; } = 0;
        public string? Notes { get; set; }
    }
}
