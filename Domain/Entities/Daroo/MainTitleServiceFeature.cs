using Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Daroo
{
    public class MainTitleServiceFeature : BaseEntity
    {
        public long MainTitle { get; set; }
        public long ServiceFeature { get; set; }
        public MainTitle MainTitleVirtual { get; set; }
        public ServiceFeature ServiceFeatureVirtual { get; set; }
        public bool IsActive { get; set; }
    }
}
