using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities.Common;

namespace Domain.Entities.Daroo
{
    [Table("BpmType")]
    public class BpmType : BizagiBaseEntity
    {
        [Key]
        [Column("idBpmType")]
        public long Id { get; set; }

        [Column("Name")]
        [MaxLength(50)]
        public string? Name { get; set; }

        [Column("CreateUserID")]
        public long? CreateUserID { get; set; }

        public override void PrepareForCreation()
        {
            FinalEnt = 10010;
            base.PrepareForCreation();
        }
    }
}
