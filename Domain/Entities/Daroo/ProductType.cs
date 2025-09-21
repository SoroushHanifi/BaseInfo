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
    public class ProductType
    {
        [Key]
        [Column("idProductType")]
        public long Id { get; set; }

        [Column("finalEnt")]
        public int FinalEnt { get; set; } = 10011;

        [Column("baCreatedTime")]
        public long BaCreatedTime { get; set; }

        [Column("baGuid")]
        public Guid BaGuid { get; set; } = Guid.NewGuid();

        [Column("Name")]
        [MaxLength(50)]
        public string? Name { get; set; }

        [Column("CreateDate")]
        public DateTime? CreateDate { get; set; }

        [Column("ModifyDate")]
        public DateTime? ModifyDate { get; set; }

        [Column("IsDeleted")]
        public bool? IsDeleted { get; set; }

        [Column("MainTitleID")]
        public long? MainTitleID { get; set; }

        // Navigation Properties
        [ForeignKey("MainTitleID")]
        public virtual MainTitle MainTitle { get; set; } = null!;
    }

}
