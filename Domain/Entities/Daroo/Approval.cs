using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.Daroo
{
    /// <summary>
    /// جدول تاییدیه‌ها - مدیریت تاییدیه‌های مرتبط با عناوین اصلی
    /// توجه: تعرفه‌ها قابل ویرایش نیستند و فقط می‌توان آن‌ها را حذف کرد
    /// </summary>
    public class Approval
    {
        /// <summary>
        /// شناسه یکتای تاییدیه
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// شناسه نهایی موجودیت در سیستم Bizagi
        /// </summary>
        public int FinalEnt { get; set; }

        /// <summary>
        /// زمان ایجاد در سیستم Bizagi (Unix timestamp in milliseconds)
        /// </summary>
        public long BaCreatedTime { get; set; }

        /// <summary>
        /// شناسه یکتای جهانی Bizagi
        /// </summary>
        public Guid BaGuid { get; set; }

        /// <summary>
        /// تاریخ اجرا - همیشه برابر با زمان ساخت تعرفه است
        /// این تاریخ نشان‌دهنده زمان تصویب/اجرای تعرفه است
        /// </summary>
        public DateTime? ExecutionDate { get; set; }

        /// <summary>
        /// تاریخ شروع تعرفه - توسط کاربر انتخاب می‌شود
        /// می‌تواند تاریخ گذشته، حال یا آینده باشد
        /// </summary>
        public DateTime? TariffStartDate { get; set; }

        /// <summary>
        /// تاریخ پایان تعرفه - در زمان ساخت NULL است
        /// زمانی که تعرفه بعدی ساخته می‌شود، این مقدار با TariffStartDate تعرفه بعدی پر می‌شود
        /// </summary>
        public DateTime? TariffEndDate { get; set; }

        /// <summary>
        /// شناسه عنوان اصلی مرتبط
        /// </summary>
        public long? MainTitleId { get; set; }

        /// <summary>
        /// مبلغ تعرفه
        /// </summary>
        public decimal? Amount { get; set; }

        /// <summary>
        /// وضعیت فعال/غیرفعال بودن تاییدیه
        /// </summary>
        public bool? IsActive { get; set; }

        // Navigation Properties
        /// <summary>
        /// عنوان اصلی مرتبط با این تاییدیه
        /// </summary>
        public virtual MainTitle? MainTitle { get; set; }   

        /// <summary>
        /// بررسی می‌کند که آیا این تعرفه در حال حاضر فعال است
        /// </summary>
        public bool IsCurrentlyActive
        {
            get
            {
                if (!IsActive.HasValue || !IsActive.Value)
                    return false;

                var now = DateTime.Now;

                // اگر تاریخ شروع مشخص نیست، فعال نیست
                if (!TariffStartDate.HasValue)
                    return false;

                // اگر هنوز شروع نشده، فعال نیست
                if (TariffStartDate.Value > now)
                    return false;

                // اگر تاریخ پایان مشخص نیست، یعنی آخرین تعرفه است و فعال است
                if (!TariffEndDate.HasValue)
                    return true;

                // اگر تاریخ پایان گذشته، دیگر فعال نیست
                return TariffEndDate.Value >= now;
            }
        }

        /// <summary>
        /// بررسی می‌کند که آیا این تعرفه آخرین تعرفه است (تاریخ پایان ندارد)
        /// </summary>
        public bool IsLatestTariff => !TariffEndDate.HasValue;

        
    }
}