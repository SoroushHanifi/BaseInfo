using Domain.Entities.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.Daroo
{
    /// <summary>
    /// انتیتی عنوان اصلی - هر حوزه می‌تواند چندین عنوان اصلی داشته باشد
    /// </summary>
    [Table("MainTitle")]
    public class MainTitle
    {
        [Key]
        [Column("idMainTitle")]
        public long Id { get; set; }

        [Column("finalEnt")]
        public int FinalEnt { get; set; } = 10012;

        [Column("baCreatedTime")]
        public long BaCreatedTime { get; set; }

        [Column("baGuid")]
        public Guid BaGuid { get; set; } = Guid.NewGuid();

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

        [Column("CreateDate")]
        public DateTime? CreateDate { get; set; }

        [Column("ModifyDate")]
        public DateTime? ModifyDate { get; set; }

        [Column("IsDeleted")]
        public bool? IsDeleted { get; set; }

        [Column("BpmType")]
        public long? BpmType { get; set; }

        // Navigation Properties
        [ForeignKey("ScopesId")]
        public virtual Scope Scope { get; set; } = null!;
    }

}