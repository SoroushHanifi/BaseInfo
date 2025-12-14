using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Daroo
{
    public class Payment
    {
        public long Id { get; set; }
        public long MainTitleId { get; set; }
        public long PaymentAmount { get; set; }
        public long Founder { get; set; }
        public string NationalCode { get; set; }
        public long PaymentStatus { get; set; }
        public long PaymentResult { get; set; }
        public DateTime PaymentDate { get; set; }
        public long BaCreatedTime { get; set; }

        // Navigation Properties
        [ForeignKey("MainTitleId")]
        public virtual MainTitle MainTitle { get; set; } = null!;
    }
}
