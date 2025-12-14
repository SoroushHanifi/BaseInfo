using Application.Common;
using Application.CQRS;
using Application.Models;
using Infrastructure.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace BaseInfo.Controllers
{
    /// <summary>
    /// کنترلر مدیریت تاییدیه‌ها
    /// توجه: تعرفه‌ها بعد از ایجاد قابل ویرایش نیستند، فقط می‌توان حذف کرد
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ApprovalsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ApprovalsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// دریافت لیست تمام تاییدیه‌ها
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<ResultApi<List<ApprovalDto>>>> GetAll()
        {
            try
            {
                var query = new GetAllApprovalsQuery();
                var result = await _mediator.Send(query);

                return Ok(new ResultApi<List<ApprovalDto>>
                {
                    StatusCode = 200,
                    IsSuccess = true,
                    Message = "لیست تاییدیه‌ها با موفقیت دریافت شد",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResultApi
                {
                    StatusCode = 500,
                    IsSuccess = false,
                    Message = $"خطا در دریافت لیست تاییدیه‌ها: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// دریافت لیست تاییدیه‌ها با صفحه‌بندی و فیلتر
        /// </summary>
        [HttpGet("paginated")]
        public async Task<ActionResult<ResultApi<PagedData<ApprovalDto>>>> GetAllPaginated(
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] long? mainTitleId = null,
            [FromQuery] bool? isActive = null,
            [FromQuery] bool? onlyActive = null)
        {
            try
            {
                var query = new GetAllApprovalsPaginationQuery(pageIndex, pageSize, mainTitleId, isActive, onlyActive);
                var result = await _mediator.Send(query);

                return Ok(new ResultApi<PagedData<ApprovalDto>>
                {
                    StatusCode = 200,
                    IsSuccess = true,
                    Message = "لیست تاییدیه‌ها با موفقیت دریافت شد",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResultApi
                {
                    StatusCode = 500,
                    IsSuccess = false,
                    Message = $"خطا در دریافت لیست تاییدیه‌ها: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// دریافت تاییدیه بر اساس شناسه
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ResultApi<ApprovalDto>>> GetById(long id)
        {
            try
            {
                var query = new GetApprovalByIdQuery(id);
                var result = await _mediator.Send(query);

                if (result == null)
                {
                    return NotFound(new ResultApi
                    {
                        StatusCode = 404,
                        IsSuccess = false,
                        Message = $"تاییدیه با شناسه {id} یافت نشد"
                    });
                }

                return Ok(new ResultApi<ApprovalDto>
                {
                    StatusCode = 200,
                    IsSuccess = true,
                    Message = "اطلاعات تاییدیه با موفقیت دریافت شد",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResultApi
                {
                    StatusCode = 500,
                    IsSuccess = false,
                    Message = $"خطا در دریافت اطلاعات تاییدیه: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// دریافت تاییدیه‌های یک عنوان اصلی
        /// </summary>
        [HttpGet("by-maintitle/{mainTitleId}")]
        public async Task<ActionResult<ResultApi<List<ApprovalDto>>>> GetByMainTitle(
            long mainTitleId,
            [FromQuery] bool activeOnly = false)
        {
            try
            {
                var query = new GetApprovalsByMainTitleQuery(mainTitleId, activeOnly);
                var result = await _mediator.Send(query);

                return Ok(new ResultApi<List<ApprovalDto>>
                {
                    StatusCode = 200,
                    IsSuccess = true,
                    Message = "لیست تاییدیه‌های عنوان اصلی با موفقیت دریافت شد",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResultApi
                {
                    StatusCode = 500,
                    IsSuccess = false,
                    Message = $"خطا در دریافت تاییدیه‌ها: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// دریافت تاییدیه‌های فعال (در حال اجرا)
        /// </summary>
        [HttpGet("active")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetActiveApprovals()
        {
            var approvals = await _mediator.Send(new GetActiveApprovalsQuery());

            var result = new ResultApi<List<ApprovalDto>>
            {
                StatusCode = 200,
                IsSuccess = true,
                Message = "لیست تاییدیه‌های فعال با موفقیت دریافت شد",
                Data = approvals
            };

            return Ok(result);
        }
        /// <summary>
        /// دریافت تاییدیه‌هایی که به زودی منقضی می‌شوند
        /// </summary>
        [HttpGet("expiring-soon")]
        public async Task<ActionResult<ResultApi<List<ApprovalDto>>>> GetExpiringSoon(
            [FromQuery] int daysThreshold = 30)
        {
            try
            {
                if (daysThreshold < 1 || daysThreshold > 365)
                {
                    return BadRequest(new ResultApi
                    {
                        StatusCode = 400,
                        IsSuccess = false,
                        Message = "آستانه روز باید بین 1 تا 365 باشد"
                    });
                }

                var query = new GetExpiringSoonApprovalsQuery(daysThreshold);
                var result = await _mediator.Send(query);

                return Ok(new ResultApi<List<ApprovalDto>>
                {
                    StatusCode = 200,
                    IsSuccess = true,
                    Message = $"لیست تاییدیه‌های منقضی شونده در {daysThreshold} روز آینده با موفقیت دریافت شد",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResultApi
                {
                    StatusCode = 500,
                    IsSuccess = false,
                    Message = $"خطا در دریافت تاییدیه‌های منقضی شونده: {ex.Message}"
                });
            }
        }


        /// <summary>
        /// ایجاد تاییدیه جدید
        /// توجه: ExecutionDate به صورت خودکار با تاریخ الان پر می‌شود
        /// TariffEndDate NULL است و با ساخت تعرفه بعدی پر می‌شود
        /// در صورت همپوشانی با تعرفه‌های قبلی، آن‌ها به صورت خودکار تنظیم می‌شوند
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ResultApi<CreateApprovalResultDto>>> Create([FromBody] CreateApprovalRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ResultApi
                    {
                        StatusCode = 400,
                        IsSuccess = false,
                        Message = "اطلاعات ورودی نامعتبر است"
                    });
                }

                var command = new CreateApprovalCommand(
                    request.TariffStartDate,
                    request.MainTitleId,
                    request.Amount,
                    request.IsActive
                );

                var result = await _mediator.Send(command);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = result.ApprovalId },
                    new ResultApi<CreateApprovalResultDto>
                    {
                        StatusCode = 201,
                        IsSuccess = true,
                        Message = result.Message,
                        Data = new CreateApprovalResultDto
                        {
                            ApprovalId = result.ApprovalId,
                            UpdatedApprovalIds = result.UpdatedApprovalIds,
                            Message = result.Message
                        }
                    });
            }
            catch (Infrastructure.Exceptions.AppException ex)
            {
                return BadRequest(new ResultApi
                {
                    StatusCode = 400,
                    IsSuccess = false,
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResultApi
                {
                    StatusCode = 500,
                    IsSuccess = false,
                    Message = $"خطا در ایجاد تاییدیه: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// حذف تاییدیه
        /// توجه: تعرفه‌ها قابل ویرایش نیستند و فقط می‌توان حذف کرد
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult<ResultApi<DeleteApprovalResultDto>>> Delete(
            long id,
            [FromQuery] bool adjustPreviousTariff = true)
        {
            try
            {
                var command = new DeleteApprovalCommand(id, adjustPreviousTariff);
                var result = await _mediator.Send(command);

                if (!result.Success)
                {
                    return NotFound(new ResultApi
                    {
                        StatusCode = 404,
                        IsSuccess = false,
                        Message = result.Message
                    });
                }

                return Ok(new ResultApi<DeleteApprovalResultDto>
                {
                    StatusCode = 200,
                    IsSuccess = true,
                    Message = result.Message,
                    Data = new DeleteApprovalResultDto
                    {
                        Success = result.Success,
                        AdjustedApprovalId = result.AdjustedApprovalId,
                        Message = result.Message
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResultApi
                {
                    StatusCode = 500,
                    IsSuccess = false,
                    Message = $"خطا در حذف تاییدیه: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// تغییر وضعیت فعال/غیرفعال تاییدیه
        /// تنها خاصیت قابل تغییر بعد از ایجاد تعرفه
        /// </summary>
        [HttpPatch("{id}/toggle-active")]
        public async Task<ActionResult<ResultApi>> ToggleActiveStatus(long id)
        {
            try
            {
                var command = new ToggleApprovalActiveStatusCommand(id);
                var result = await _mediator.Send(command);

                if (!result)
                {
                    return NotFound(new ResultApi
                    {
                        StatusCode = 404,
                        IsSuccess = false,
                        Message = $"تاییدیه با شناسه {id} یافت نشد"
                    });
                }

                return Ok(new ResultApi
                {
                    StatusCode = 200,
                    IsSuccess = true,
                    Message = "وضعیت تاییدیه با موفقیت تغییر کرد"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResultApi
                {
                    StatusCode = 500,
                    IsSuccess = false,
                    Message = $"خطا در تغییر وضعیت تاییدیه: {ex.Message}"
                });
            }
        }
    }

    // ===== REQUEST/RESPONSE MODELS =====

    /// <summary>
    /// مدل درخواست ایجاد تاییدیه
    /// توجه: ExecutionDate خودکار پر می‌شود و TariffEndDate همیشه NULL است
    /// </summary>
    public class CreateApprovalRequest
    {
        /// <summary>
        /// تاریخ شروع تعرفه - توسط کاربر انتخاب می‌شود
        /// می‌تواند تاریخ گذشته، حال یا آینده باشد
        /// </summary>
        [Required(ErrorMessage = "تاریخ شروع تعرفه الزامی است")]
        public DateTime TariffStartDate { get; set; }

        /// <summary>
        /// شناسه عنوان اصلی
        /// </summary>
        [Required(ErrorMessage = "شناسه عنوان اصلی الزامی است")]
        [Range(1, long.MaxValue, ErrorMessage = "شناسه عنوان اصلی باید عددی مثبت باشد")]
        public long MainTitleId { get; set; }
        public long Amount { get; set; }

        /// <summary>
        /// وضعیت فعال/غیرفعال
        /// </summary>
        public bool IsActive { get; set; } = true;
    }

    /// <summary>
    /// مدل پاسخ ایجاد تاییدیه
    /// </summary>
    public class CreateApprovalResultDto
    {
        /// <summary>
        /// شناسه تعرفه ایجاد شده
        /// </summary>
        public long ApprovalId { get; set; }

        /// <summary>
        /// لیست شناسه تعرفه‌هایی که به‌روزرسانی شدند
        /// </summary>
        public List<long> UpdatedApprovalIds { get; set; } = new();

        /// <summary>
        /// پیام توضیحی
        /// </summary>
        public string Message { get; set; } = string.Empty;
    }

    /// <summary>
    /// مدل پاسخ حذف تاییدیه
    /// </summary>
    public class DeleteApprovalResultDto
    {
        /// <summary>
        /// آیا حذف موفق بود؟
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// شناسه تعرفه‌ای که به دنبال حذف تنظیم شد
        /// </summary>
        public long? AdjustedApprovalId { get; set; }

        /// <summary>
        /// پیام توضیحی
        /// </summary>
        public string Message { get; set; } = string.Empty;
    }
}