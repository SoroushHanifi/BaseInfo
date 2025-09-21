using Domain.Entities.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.Daroo
{
    /// <summary>
    /// انتیتی حوزه - هر اداره کل می‌تواند چندین حوزه داشته باشد
    /// </summary>
    [Table("Scopes")]
    public class Scope : BizagiBaseEntity
    {
        [Key]
        [Column("idScopes")]
        public long Id { get; set; }

        [Column("Name")]
        [MaxLength(50)]
        public string? Name { get; set; }

        [Column("Department")]
        public long? DepartmentId { get; set; }

        [Column("CreateUserID")]
        public long? CreateUserID { get; set; }

        // Navigation Properties
        [ForeignKey("DepartmentId")]
        public virtual Department Department { get; set; } = null!;

        public virtual ICollection<MainTitle> MainTitles { get; set; } = new List<MainTitle>();

        public override void PrepareForCreation()
        {
            FinalEnt = 10009;
            base.PrepareForCreation();
        }
    }

}