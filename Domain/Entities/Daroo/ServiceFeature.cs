using Domain.Entities.Common;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.Daroo
{
    /// <summary>
    /// انتیتی ویژگی خدمات - می‌تواند به چندین عنوان اصلی تعلق داشته باشد
    /// </summary>
    public class ServiceFeature : BaseEntity
    {
        /// <summary>
        /// نام ویژگی خدمات
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// توضیحات ویژگی خدمات
        /// </summary>
        [MaxLength(500)]
        public string? Description { get; set; }

        /// <summary>
        /// کد ویژگی (اختیاری)
        /// </summary>
        [MaxLength(50)]
        public string? Code { get; set; }

        /// <summary>
        /// آیکون یا نماد ویژگی
        /// </summary>
        [MaxLength(100)]
        public string? Icon { get; set; }

        /// <summary>
        /// رنگ نمایش ویژگی (برای UI)
        /// </summary>
        [MaxLength(20)]
        public string? Color { get; set; }

        /// <summary>
        /// ترتیب نمایش عمومی
        /// </summary>
        public int DisplayOrder { get; set; } = 0;

        /// <summary>
        /// وضعیت فعال/غیرفعال کلی ویژگی
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Navigation Property - رابطه با عناوین اصلی
        /// </summary>
        public virtual ICollection<MainTitleServiceFeature> MainTitleServiceFeatures { get; set; } = new List<MainTitleServiceFeature>();
    }
}