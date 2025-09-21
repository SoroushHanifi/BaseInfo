using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Common
{
    public abstract class BizagiBaseEntity
    {
        [Column("finalEnt")]
        public int FinalEnt { get; set; }

        [Column("baCreatedTime")]
        public long BaCreatedTime { get; set; }

        [Column("baGuid")]
        public Guid BaGuid { get; set; } = Guid.NewGuid();

        [Column("CreateDate")]
        public DateTime? CreateDate { get; set; }

        [Column("ModifyDate")]
        public DateTime? ModifyDate { get; set; }

        [Column("IsDeleted")]
        public bool? IsDeleted { get; set; }

        /// <summary>
        /// Sets BaCreatedTime to current Unix timestamp in milliseconds
        /// </summary>
        public void SetBaCreatedTimeToNow()
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            BaCreatedTime = (long)(DateTime.UtcNow - epoch).TotalMilliseconds;
        }

        /// <summary>
        /// Converts BaCreatedTime to DateTime
        /// </summary>
        public DateTime? BaCreatedTimeToDateTime()
        {
            if (BaCreatedTime <= 0) return null;
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddMilliseconds(BaCreatedTime);
        }

        /// <summary>
        /// Prepares entity for creation by setting common Bizagi fields
        /// </summary>
        public virtual void PrepareForCreation()
        {
            SetBaCreatedTimeToNow();
            BaGuid = Guid.NewGuid();
            CreateDate = DateTime.Now;
            ModifyDate = DateTime.Now;
            IsDeleted = false;
        }

        /// <summary>
        /// Prepares entity for update
        /// </summary>
        public virtual void PrepareForUpdate()
        {
            ModifyDate = DateTime.Now;
        }

        /// <summary>
        /// Soft delete the entity
        /// </summary>
        public virtual void SoftDelete()
        {
            IsDeleted = true;
            ModifyDate = DateTime.Now;
        }
    }

}
