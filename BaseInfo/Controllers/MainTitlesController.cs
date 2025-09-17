using Application.Common;
using Application.CQRS;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace BaseInfo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MainTitlesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public MainTitlesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// دریافت لیست تمام عناوین اصلی
        /// </summary>
        /// <returns>لیست عناوین اصلی</returns>
        [HttpGet]
        public async Task<ActionResult<List<MainTitleDto>>> GetAll()
        {
            var query = new GetAllMainTitlesQuery();
            var result = await _mediator.Send(query);
            return Ok(new ApiResult<List<MainTitleDto>>
            {
                IsSuccess = true,
                Data = result   
            });
        }

        /// <summary>
        /// دریافت عنوان اصلی بر اساس ID
        /// </summary>
        /// <param name="id">شناسه عنوان اصلی</param>
        /// <returns>اطلاعات عنوان اصلی</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<MainTitleDto>> GetById(int id)
        {
            var query = new GetMainTitleByIdQuery(id);
            var result = await _mediator.Send(query);

            if (result == null)
                return NotFound($"MainTitle with ID {id} not found.");

            return Ok(new ApiResult<MainTitleDto> 
            {
                IsSuccess = true,
                Data= result
            });
        }

        /// <summary>
        /// دریافت عناوین اصلی یک حوزه خاص
        /// </summary>
        /// <param name="scopeId">شناسه حوزه</param>
        /// <returns>لیست عناوین اصلی حوزه</returns>
        [HttpGet("scope/{scopeId}")]
        public async Task<ActionResult<List<MainTitleSimpleDto>>> GetByScope(int scopeId)
        {
            var query = new GetMainTitlesByScopeQuery(scopeId);
            var result = await _mediator.Send(query);
            return Ok(new ApiResult<List<MainTitleSimpleDto>>
            {
                IsSuccess= true,
                Data= result
            });
        }

        /// <summary>
        /// دریافت عناوین اصلی یک اداره کل خاص
        /// </summary>
        /// <param name="departmentId">شناسه اداره کل</param>
        /// <returns>لیست عناوین اصلی اداره کل</returns>
        [HttpGet("department/{departmentId}")]
        public async Task<ActionResult<List<MainTitleDto>>> GetByDepartment(int departmentId)
        {
            var query = new GetMainTitlesByDepartmentQuery(departmentId);
            var result = await _mediator.Send(query);
            return Ok(new ApiResult<List<MainTitleDto>> 
            {
                IsSuccess = true,
                Data= result
            });
        }

        /// <summary>
        /// جستجوی پیشرفته در عناوین اصلی
        /// </summary>
        /// <param name="searchTerm">عبارت جستجو</param>
        /// <param name="scopeId">شناسه حوزه (اختیاری)</param>
        /// <param name="departmentId">شناسه اداره کل (اختیاری)</param>
        /// <param name="minAmount">حداقل مبلغ (اختیاری)</param>
        /// <param name="maxAmount">حداکثر مبلغ (اختیاری)</param>
        /// <returns>نتایج جستجو</returns>
        [HttpGet("search")]
        public async Task<ActionResult<List<MainTitleDto>>> Search(
            [FromQuery] string? searchTerm = null,
            [FromQuery] int? scopeId = null,
            [FromQuery] int? departmentId = null,
            [FromQuery] decimal? minAmount = null,
            [FromQuery] decimal? maxAmount = null)
        {
            // اعتبارسنجی محدوده مبلغ
            if (minAmount.HasValue && maxAmount.HasValue && minAmount > maxAmount)
            {
                return BadRequest("حداقل مبلغ نمی‌تواند بیشتر از حداکثر مبلغ باشد");
            }

            // اعتبارسنجی مبالغ منفی
            if (minAmount.HasValue && minAmount < 0)
            {
                return BadRequest("حداقل مبلغ نمی‌تواند منفی باشد");
            }

            if (maxAmount.HasValue && maxAmount < 0)
            {
                return BadRequest("حداکثر مبلغ نمی‌تواند منفی باشد");
            }

            var query = new SearchMainTitlesQuery(searchTerm, scopeId, departmentId, minAmount, maxAmount);
            var result = await _mediator.Send(query);
            return Ok(new ApiResult<List<MainTitleDto>>
            {
                IsSuccess = true,
                Data= result
            });
        }

        /// <summary>
        /// دریافت خلاصه مالی عناوین اصلی بر اساس حوزه
        /// </summary>
        /// <returns>خلاصه مالی بر اساس حوزه‌ها</returns>
        [HttpGet("summary")]
        public async Task<ActionResult<List<MainTitleSummaryDto>>> GetSummary()
        {
            var query = new GetMainTitlesSummaryQuery();
            var result = await _mediator.Send(query);
            return Ok(new ApiResult<List<MainTitleSummaryDto>> 
            {
                IsSuccess = true,
                Data= result
            });
        }

        /// <summary>
        /// دریافت گران‌ترین عناوین اصلی
        /// </summary>
        /// <param name="top">تعداد رکورد برتر (پیش‌فرض 10، حداکثر 100)</param>
        /// <returns>لیست گران‌ترین عناوین اصلی</returns>
        [HttpGet("top-expensive")]
        public async Task<ActionResult<List<MainTitleDto>>> GetTopExpensive([FromQuery] int top = 10)
        {
            if (top < 1 || top > 100)
            {
                return BadRequest("تعداد رکورد باید بین 1 تا 100 باشد");
            }

            var query = new GetTopExpensiveMainTitlesQuery(top);
            var result = await _mediator.Send(query);
            return Ok(new ApiResult<List<MainTitleDto>> 
            {
                IsSuccess = true,   
                Data = result
            });
        }

        /// <summary>
        /// ایجاد عنوان اصلی جدید
        /// </summary>
        /// <param name="request">اطلاعات عنوان اصلی</param>
        /// <returns>شناسه عنوان اصلی ایجاد شده</returns>
        [HttpPost]
        public async Task<ActionResult<int>> Create([FromBody] CreateMainTitleRequest request)
        {
            var command = new CreateMainTitleCommand(
                request.Name,
                request.Description,
                request.Amount,
                request.ScopeId,
                request.DisplayOrder
            );

            var mainTitleId = await _mediator.Send(command);

            return CreatedAtAction(
                nameof(GetById),
                new { id = mainTitleId },
                new { Id = mainTitleId, Message = "MainTitle created successfully" }
            );
        }

        /// <summary>
        /// ویرایش عنوان اصلی
        /// </summary>
        /// <param name="id">شناسه عنوان اصلی</param>
        /// <param name="request">اطلاعات جدید</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] UpdateMainTitleRequest request)
        {
            var command = new UpdateMainTitleCommand(
                id,
                request.Name,
                request.Description,
                request.Amount,
                request.DisplayOrder
            );

            var result = await _mediator.Send(command);

            if (!result)
                return NotFound($"MainTitle with ID {id} not found.");

            return Ok(new { Message = "MainTitle updated successfully" });
        }

        /// <summary>
        /// حذف عنوان اصلی (Soft Delete)
        /// </summary>
        /// <param name="id">شناسه عنوان اصلی</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var command = new DeleteMainTitleCommand(id);
            var result = await _mediator.Send(command);

            if (!result)
                return NotFound($"MainTitle with ID {id} not found.");

            return Ok(new { Message = "MainTitle deleted successfully" });
        }

        /// <summary>
        /// به‌روزرسانی ترتیب نمایش یک عنوان اصلی
        /// </summary>
        /// <param name="id">شناسه عنوان اصلی</param>
        /// <param name="request">ترتیب جدید</param>
        /// <returns></returns>
        [HttpPatch("{id}/display-order")]
        public async Task<ActionResult> UpdateDisplayOrder(int id, [FromBody] UpdateDisplayOrderRequest request)
        {
            var command = new UpdateMainTitleDisplayOrderCommand(id, request.DisplayOrder);
            var result = await _mediator.Send(command);

            if (!result)
                return NotFound($"MainTitle with ID {id} not found.");

            return Ok(new { Message = "Display order updated successfully" });
        }

        /// <summary>
        /// به‌روزرسانی انبوه ترتیب نمایش عناوین اصلی
        /// </summary>
        /// <param name="request">لیست عناوین با ترتیب جدید</param>
        /// <returns></returns>
        [HttpPatch("bulk-display-order")]
        public async Task<ActionResult> BulkUpdateDisplayOrder([FromBody] BulkUpdateDisplayOrderRequest request)
        {
            if (request.Items == null || !request.Items.Any())
            {
                return BadRequest("لیست آیتم‌ها نمی‌تواند خالی باشد");
            }

            if (request.Items.Count > 100)
            {
                return BadRequest("حداکثر 100 آیتم در هر درخواست مجاز است");
            }

            // بررسی تکراری نبودن ID ها
            var duplicateIds = request.Items
                .GroupBy(x => x.Id)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            if (duplicateIds.Any())
            {
                return BadRequest($"شناسه‌های تکراری: {string.Join(", ", duplicateIds)}");
            }

            var items = request.Items.Select(x => new MainTitleDisplayOrderItem
            {
                Id = x.Id,
                DisplayOrder = x.DisplayOrder
            }).ToList();

            var command = new BulkUpdateMainTitleDisplayOrderCommand(items);
            var result = await _mediator.Send(command);

            if (!result)
                return BadRequest("هیچ عنوان اصلی معتبری برای به‌روزرسانی یافت نشد");

            return Ok(new
            {
                Message = "Display orders updated successfully",
                Count = items.Count
            });
        }
    }

    // ===== Request Models =====

    /// <summary>
    /// مدل درخواست ایجاد عنوان اصلی
    /// </summary>
    public class CreateMainTitleRequest
    {
        /// <summary>
        /// نام عنوان اصلی
        /// </summary>
        [Required(ErrorMessage = "نام عنوان اصلی الزامی است")]
        [MaxLength(300, ErrorMessage = "نام عنوان اصلی نباید بیشتر از 300 کاراکتر باشد")]
        [MinLength(2, ErrorMessage = "نام عنوان اصلی باید حداقل 2 کاراکتر باشد")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// توضیحات عنوان اصلی
        /// </summary>
        [MaxLength(1000, ErrorMessage = "توضیحات نباید بیشتر از 1000 کاراکتر باشد")]
        public string? Description { get; set; }

        /// <summary>
        /// مبلغ عنوان اصلی
        /// </summary>
        [Required(ErrorMessage = "مبلغ الزامی است")]
        [Range(0, double.MaxValue, ErrorMessage = "مبلغ نمی‌تواند منفی باشد")]
        public decimal Amount { get; set; }

        /// <summary>
        /// شناسه حوزه مربوطه
        /// </summary>
        [Required(ErrorMessage = "شناسه حوزه الزامی است")]
        [Range(1, int.MaxValue, ErrorMessage = "شناسه حوزه باید عددی مثبت باشد")]
        public int ScopeId { get; set; }

        /// <summary>
        /// ترتیب نمایش
        /// </summary>
        [Range(0, int.MaxValue, ErrorMessage = "ترتیب نمایش نمی‌تواند منفی باشد")]
        public int DisplayOrder { get; set; } = 0;
    }

    /// <summary>
    /// مدل درخواست ویرایش عنوان اصلی
    /// </summary>
    public class UpdateMainTitleRequest
    {
        /// <summary>
        /// نام عنوان اصلی
        /// </summary>
        [Required(ErrorMessage = "نام عنوان اصلی الزامی است")]
        [MaxLength(300, ErrorMessage = "نام عنوان اصلی نباید بیشتر از 300 کاراکتر باشد")]
        [MinLength(2, ErrorMessage = "نام عنوان اصلی باید حداقل 2 کاراکتر باشد")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// توضیحات عنوان اصلی
        /// </summary>
        [MaxLength(1000, ErrorMessage = "توضیحات نباید بیشتر از 1000 کاراکتر باشد")]
        public string? Description { get; set; }

        /// <summary>
        /// مبلغ عنوان اصلی
        /// </summary>
        [Required(ErrorMessage = "مبلغ الزامی است")]
        [Range(0, double.MaxValue, ErrorMessage = "مبلغ نمی‌تواند منفی باشد")]
        public decimal Amount { get; set; }

        /// <summary>
        /// ترتیب نمایش
        /// </summary>
        [Range(0, int.MaxValue, ErrorMessage = "ترتیب نمایش نمی‌تواند منفی باشد")]
        public int DisplayOrder { get; set; } = 0;
    }

    /// <summary>
    /// مدل درخواست به‌روزرسانی ترتیب نمایش
    /// </summary>
    public class UpdateDisplayOrderRequest
    {
        /// <summary>
        /// ترتیب نمایش جدید
        /// </summary>
        [Range(0, int.MaxValue, ErrorMessage = "ترتیب نمایش نمی‌تواند منفی باشد")]
        public int DisplayOrder { get; set; }
    }

    /// <summary>
    /// مدل درخواست به‌روزرسانی انبوه ترتیب نمایش
    /// </summary>
    public class BulkUpdateDisplayOrderRequest
    {
        /// <summary>
        /// لیست آیتم‌ها برای به‌روزرسانی
        /// </summary>
        [Required(ErrorMessage = "لیست آیتم‌ها الزامی است")]
        [MinLength(1, ErrorMessage = "حداقل یک آیتم برای به‌روزرسانی لازم است")]
        [MaxLength(100, ErrorMessage = "حداکثر 100 آیتم در هر درخواست مجاز است")]
        public List<DisplayOrderItem> Items { get; set; } = new();
    }

    /// <summary>
    /// مدل آیتم برای به‌روزرسانی ترتیب نمایش
    /// </summary>
    public class DisplayOrderItem
    {
        /// <summary>
        /// شناسه عنوان اصلی
        /// </summary>
        [Required(ErrorMessage = "شناسه الزامی است")]
        [Range(1, int.MaxValue, ErrorMessage = "شناسه باید عددی مثبت باشد")]
        public int Id { get; set; }

        /// <summary>
        /// ترتیب نمایش جدید
        /// </summary>
        [Range(0, int.MaxValue, ErrorMessage = "ترتیب نمایش نمی‌تواند منفی باشد")]
        public int DisplayOrder { get; set; }
    }
}