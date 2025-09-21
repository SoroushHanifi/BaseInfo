using Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Daroo
{
    [Table("ProductType")]
    public class ProductType : BizagiBaseEntity
    {
        [Key]
        [Column("idProductType")]
        public long Id { get; set; }

        [Column("Name")]
        [MaxLength(50)]
        public string? Name { get; set; }

        [Column("MainTitleID")]
        public long? MainTitleID { get; set; }

        // Navigation Properties
        [ForeignKey("MainTitleID")]
        public virtual MainTitle MainTitle { get; set; } = null!;

        public override void PrepareForCreation()
        {
            FinalEnt = 10011;
            base.PrepareForCreation();
        }
    }
}
