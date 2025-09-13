using Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Daroo
{
    public class Department : BaseEntity
    {
        public string Name { get; set; }
        public virtual ICollection<Scope> Scopes { get; set; } = new List<Scope>();
    }
}
