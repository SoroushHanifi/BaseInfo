using Domain.Entities.Common;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.Daroo
{
    /// <summary>
    /// انتیتی حوزه - هر اداره کل می‌تواند چندین حوزه داشته باشد
    /// </summary>
    public class Scope : BaseEntity
    {
        
        [Required]
        [MaxLength(250)]
        public string Name { get; set; } = string.Empty;

        
        public int DepartmentId { get; set; }

        
        public Department Department { get; set; } = null!;
        public virtual ICollection<MainTitle> MainTitles { get; set; } = new List<MainTitle>();
    }
}