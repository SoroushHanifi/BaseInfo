using Domain.Entities.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.Daroo
{
    /// <summary>
    /// انتیتی نوع تولید
    /// </summary>
    [Table("ProduceType")]
    public class ProduceType : BizagiBaseEntity
    {
        [Key]
        [Column("idProduceType")]
        public long Id { get; set; }

        [Column("Title")]
        [MaxLength(50)]
        public string? Title { get; set; }

        [Column("Code")]
        public int? Code { get; set; }

        [Column("IsActive")]
        public bool? IsActive { get; set; }

        public override void PrepareForCreation()
        {
            FinalEnt = 10020;
            base.PrepareForCreation();
        }
    }
}