using Domain.Entities.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.Daroo
{
    /// <summary>
    /// انتیتی عنوان اصلی - هر حوزه می‌تواند چندین عنوان اصلی داشته باشد
    /// </summary>
    [Table("MainTitle")]
    public class MainTitle : BizagiBaseEntity
    {
        [Key]
        [Column("idMainTitle")]
        public long Id { get; set; }

        [Column("Name")]
        [MaxLength(50)]
        public string? Name { get; set; }

        [Column("Description")]
        [MaxLength(150)]
        public string? Description { get; set; }

        [Column("Amount")]
        public decimal? Amount { get; set; }

        [Column("ScopesId")]
        public long? ScopesId { get; set; }

        [Column("DisplayOrder")]
        [MaxLength(50)]
        public string? DisplayOrder { get; set; }

        [Column("CreateUserID")]
        public long? CreateUserID { get; set; }

        [Column("BpmType")]
        public long? BpmType { get; set; }

        // Navigation Properties
        [ForeignKey("ScopesId")]
        public virtual Scope Scope { get; set; } = null!;

        public virtual ICollection<ProductType> ProductTypes { get; set; } = new List<ProductType>();

        public override void PrepareForCreation()
        {
            FinalEnt = 10012;
            base.PrepareForCreation();
        }
    }

}