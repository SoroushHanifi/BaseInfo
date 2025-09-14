using Domain.Entities.Common;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.Daroo
{
    /// <summary>
    /// انتیتی ارتباطی بین عنوان اصلی و ویژگی خدمات (Many-to-Many)
    /// </summary>
    public class MainTitleServiceFeature : BaseEntity
    {
        /// <summary>
        /// شناسه عنوان اصلی
        /// </summary>
        public int MainTitleId { get; set; }

        /// <summary>
        /// Navigation Property - عنوان اصلی مربوطه
        /// </summary>
        public MainTitle MainTitle { get; set; } = null!;

        /// <summary>
        /// شناسه ویژگی خدمات
        /// </summary>
        public int ServiceFeatureId { get; set; }

        /// <summary>
        /// Navigation Property - ویژگی خدمات مربوطه
        /// </summary>
        public ServiceFeature ServiceFeature { get; set; } = null!;

        /// <summary>
        /// وضعیت فعال/غیرفعال این ویژگی برای این عنوان اصلی خاص
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// اولویت نمایش این ویژگی برای این عنوان اصلی خاص
        /// </summary>
        public int DisplayOrder { get; set; } = 0;

        /// <summary>
        /// یادداشت یا توضیح خاص برای این ارتباط
        /// </summary>
        [MaxLength(500)]
        public string? Notes { get; set; }

        /// <summary>
        /// تاریخ فعال‌سازی این ویژگی برای این عنوان
        /// </summary>
        public DateTime? ActivatedDate { get; set; }

        /// <summary>
        /// تاریخ غیرفعال‌سازی این ویژگی برای این عنوان
        /// </summary>
        public DateTime? DeactivatedDate { get; set; }
    }
}