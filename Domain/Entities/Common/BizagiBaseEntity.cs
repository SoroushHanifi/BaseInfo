using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Common
{
    public abstract class BizagiBaseEntity
    {
        public int FinalEnt { get; set; }
        public long BaCreatedTime { get; set; }
        public Guid BaGuid { get; set; } = Guid.NewGuid();
        public DateTime? CreateDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public bool? IsDeleted { get; set; }

        // Helper method to set BaCreatedTime to current Unix timestamp in milliseconds
        public void SetBaCreatedTimeToNow()
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            BaCreatedTime = (long)(DateTime.UtcNow - epoch).TotalMilliseconds;
        }
    }
}
