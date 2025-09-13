using Domain.Entities.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.Daroo
{
    /// <summary>
    /// انتیتی عنوان اصلی - هر حوزه می‌تواند چندین عنوان اصلی داشته باشد
    /// </summary>
    public class MainTitle : BaseEntity
    {
        /// <summary>
        /// نام عنوان اصلی
        /// </summary>
        [Required]
        [MaxLength(300)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// توضیحات عنوان اصلی
        /// </summary>
        [MaxLength(1000)]
        public string? Description { get; set; }

        /// <summary>
        /// مبلغ تعیین شده برای این عنوان اصلی
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        /// <summary>
        /// شناسه حوزه مربوطه
        /// </summary>
        public int ScopeId { get; set; }

        /// <summary>
        /// Navigation Property - حوزه مربوطه
        /// </summary>
        public Scope Scope { get; set; } = null!;

        /// <summary>
        /// اولویت نمایش (برای مرتب‌سازی)
        /// </summary>
        public int DisplayOrder { get; set; } = 0;
    }
}