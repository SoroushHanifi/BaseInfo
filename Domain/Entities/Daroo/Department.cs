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
    [Table("Department")]
    public class Department : BizagiBaseEntity
    {
        [Key]
        [Column("idDepartment")]
        public long Id { get; set; }

        [Column("Name")]
        [MaxLength(50)]
        public string? Name { get; set; }

        [Column("CreateUserID")]
        public int? CreateUserID { get; set; }

        // Navigation Properties
        public virtual ICollection<Scope> Scopes { get; set; } = new List<Scope>();

        public override void PrepareForCreation()
        {
            FinalEnt = 10008;
            base.PrepareForCreation();
        }
    }
}
