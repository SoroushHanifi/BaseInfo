using Application.Common;
using Application.CQRS;
using Application.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace BaseInfo.Controllers
{
    // ===== ResultApi Classes =====


    // ===== SCOPES CONTROLLER =====


    // ===== MAIN TITLES CONTROLLER =====
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

        [HttpGet]
        public async Task<ActionResult<ResultApi<PagedData<MainTitleDto>>>> GetAll([FromQuery] int? pageIndex,[FromQuery] int? pageSize,[FromQuery] long? scopeId, [FromQuery] long? bpmTypeId)
        {
            try
            {
                if (bpmTypeId != null || scopeId != null)
                {
                    var query = new GetAllMainTitlesPaginationQuery(pageIndex ?? 1, pageSize ?? 10, scopeId, bpmTypeId);
                    var result = await _mediator.Send(query);
                    return Ok(new ResultApi<PagedData<MainTitleDto>>
                    {
                        StatusCode = 200,
                        IsSuccess = true,
                        Message = "لیست عناوین اصلی با موفقیت دریافت شد",
                        Data = result
                    });
                }
                else 
                {
                    var query = new GetAllMainTitlesPaginationQuery(pageIndex ?? 1, pageSize ?? 10);
                    var result = await _mediator.Send(query);
                    return Ok(new ResultApi<PagedData<MainTitleDto>>
                    {
                        StatusCode = 200,
                        IsSuccess = true,
                        Message = "لیست عناوین اصلی با موفقیت دریافت شد",
                        Data = result
                    });
                }
                
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResultApi
                {
                    StatusCode = 500,
                    IsSuccess = false,
                    Message = $"خطا در دریافت لیست عناوین اصلی: {ex.Message}"
                });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ResultApi<MainTitleDto>>> GetById(long id)
        {
            try
            {
                var query = new GetMainTitleByIdQuery(id);
                var result = await _mediator.Send(query);

                if (result == null)
                {
                    return NotFound(new ResultApi
                    {
                        StatusCode = 404,
                        IsSuccess = false,
                        Message = $"عنوان اصلی با شناسه {id} یافت نشد"
                    });
                }

                return Ok(new ResultApi<MainTitleDto>
                {
                    StatusCode = 200,
                    IsSuccess = true,
                    Message = "اطلاعات عنوان اصلی با موفقیت دریافت شد",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResultApi
                {
                    StatusCode = 500,
                    IsSuccess = false,
                    Message = $"خطا در دریافت اطلاعات عنوان اصلی: {ex.Message}"
                });
            }
        }

        [HttpGet("scope/{scopeId}")]
        public async Task<ActionResult<ResultApi<List<MainTitleSimpleDto>>>> GetByScope(long scopeId)
        {
            try
            {
                var query = new GetMainTitlesByScopeQuery(scopeId);
                var result = await _mediator.Send(query);
                return Ok(new ResultApi<List<MainTitleSimpleDto>>
                {
                    StatusCode = 200,
                    IsSuccess = true,
                    Message = "لیست عناوین اصلی حوزه با موفقیت دریافت شد",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResultApi
                {
                    StatusCode = 500,
                    IsSuccess = false,
                    Message = $"خطا در دریافت عناوین اصلی حوزه: {ex.Message}"
                });
            }
        }

        [HttpGet("department/{departmentId}")]
        public async Task<ActionResult<ResultApi<List<MainTitleDto>>>> GetByDepartment(long departmentId)
        {
            try
            {
                var query = new GetMainTitlesByDepartmentQuery(departmentId);
                var result = await _mediator.Send(query);
                return Ok(new ResultApi<List<MainTitleDto>>
                {
                    StatusCode = 200,
                    IsSuccess = true,
                    Message = "لیست عناوین اصلی اداره کل با موفقیت دریافت شد",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResultApi
                {
                    StatusCode = 500,
                    IsSuccess = false,
                    Message = $"خطا در دریافت عناوین اصلی اداره کل: {ex.Message}"
                });
            }
        }

        [HttpGet("search")]
        public async Task<ActionResult<ResultApi<List<MainTitleDto>>>> Search(
            [FromQuery] string? searchTerm = null,
            [FromQuery] long? scopeId = null,
            [FromQuery] long? departmentId = null,
            [FromQuery] decimal? minAmount = null,
            [FromQuery] decimal? maxAmount = null)
        {
            try
            {
                // اعتبارسنجی
                if (minAmount.HasValue && maxAmount.HasValue && minAmount > maxAmount)
                {
                    return BadRequest(new ResultApi
                    {
                        StatusCode = 400,
                        IsSuccess = false,
                        Message = "حداقل مبلغ نمی‌تواند بیشتر از حداکثر مبلغ باشد"
                    });
                }

                if (minAmount.HasValue && minAmount < 0)
                {
                    return BadRequest(new ResultApi
                    {
                        StatusCode = 400,
                        IsSuccess = false,
                        Message = "حداقل مبلغ نمی‌تواند منفی باشد"
                    });
                }

                if (maxAmount.HasValue && maxAmount < 0)
                {
                    return BadRequest(new ResultApi
                    {
                        StatusCode = 400,
                        IsSuccess = false,
                        Message = "حداکثر مبلغ نمی‌تواند منفی باشد"
                    });
                }

                var query = new SearchMainTitlesQuery(searchTerm, scopeId, departmentId, minAmount, maxAmount);
                var result = await _mediator.Send(query);
                return Ok(new ResultApi<List<MainTitleDto>>
                {
                    StatusCode = 200,
                    IsSuccess = true,
                    Message = "نتایج جستجو با موفقیت دریافت شد",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResultApi
                {
                    StatusCode = 500,
                    IsSuccess = false,
                    Message = $"خطا در جستجوی عناوین اصلی: {ex.Message}"
                });
            }
        }

        [HttpGet("summary")]
        public async Task<ActionResult<ResultApi<List<MainTitleSummaryDto>>>> GetSummary()
        {
            try
            {
                var query = new GetMainTitlesSummaryQuery();
                var result = await _mediator.Send(query);
                return Ok(new ResultApi<List<MainTitleSummaryDto>>
                {
                    StatusCode = 200,
                    IsSuccess = true,
                    Message = "خلاصه مالی عناوین اصلی با موفقیت دریافت شد",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResultApi
                {
                    StatusCode = 500,
                    IsSuccess = false,
                    Message = $"خطا در دریافت خلاصه مالی: {ex.Message}"
                });
            }
        }

        [HttpGet("top-expensive")]
        public async Task<ActionResult<ResultApi<List<MainTitleDto>>>> GetTopExpensive([FromQuery] int top = 10)
        {
            try
            {
                if (top < 1 || top > 100)
                {
                    return BadRequest(new ResultApi
                    {
                        StatusCode = 400,
                        IsSuccess = false,
                        Message = "تعداد رکورد باید بین 1 تا 100 باشد"
                    });
                }

                var query = new GetTopExpensiveMainTitlesQuery(top);
                var result = await _mediator.Send(query);
                return Ok(new ResultApi<List<MainTitleDto>>
                {
                    StatusCode = 200,
                    IsSuccess = true,
                    Message = "لیست گران‌ترین عناوین اصلی با موفقیت دریافت شد",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResultApi
                {
                    StatusCode = 500,
                    IsSuccess = false,
                    Message = $"خطا در دریافت گران‌ترین عناوین اصلی: {ex.Message}"
                });
            }
        }

        [HttpPost]
        public async Task<ActionResult<ResultApi<long>>> Create([FromBody] CreateMainTitleRequest request)
        {
            try
            {
                var command = new CreateMainTitleCommand(
                    request.Name,
                    request.Description,
                    request.Amount,
                    request.ScopeId,
                    request.DisplayOrder,
                    request.BpmType
                );

                var mainTitleId = await _mediator.Send(command);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = mainTitleId },
                    new ResultApi<long>
                    {
                        StatusCode = 201,
                        IsSuccess = true,
                        Message = "عنوان اصلی با موفقیت ایجاد شد",
                        Data = mainTitleId
                    });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResultApi
                {
                    StatusCode = 500,
                    IsSuccess = false,
                    Message = $"خطا در ایجاد عنوان اصلی: {ex.Message}"
                });
            }
        }

        [HttpPut]
        public async Task<ActionResult<ResultApi>> Update([FromBody] UpdateMainTitleRequest request)
        {
            try
            {
                var command = new UpdateMainTitleCommand(
                    request.Id,
                    request.Name,
                    request.Description,
                    request.Amount,
                    request.DisplayOrder,
                    request.BpmType
                );

                var result = await _mediator.Send(command);

                if (!result)
                {
                    return NotFound(new ResultApi
                    {
                        StatusCode = 404,
                        IsSuccess = false,
                        Message = $"عنوان اصلی با شناسه {request.Id} یافت نشد"
                    });
                }

                return Ok(new ResultApi
                {
                    StatusCode = 200,
                    IsSuccess = true,
                    Message = "عنوان اصلی با موفقیت به‌روزرسانی شد"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResultApi
                {
                    StatusCode = 500,
                    IsSuccess = false,
                    Message = $"خطا در به‌روزرسانی عنوان اصلی: {ex.Message}"
                });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ResultApi>> Delete(long id)
        {
            try
            {
                var command = new DeleteMainTitleCommand(id);
                var result = await _mediator.Send(command);

                if (!result)
                {
                    return NotFound(new ResultApi
                    {
                        StatusCode = 404,
                        IsSuccess = false,
                        Message = $"عنوان اصلی با شناسه {id} یافت نشد"
                    });
                }

                return Ok(new ResultApi
                {
                    StatusCode = 200,
                    IsSuccess = true,
                    Message = "عنوان اصلی با موفقیت حذف شد"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResultApi
                {
                    StatusCode = 500,
                    IsSuccess = false,
                    Message = $"خطا در حذف عنوان اصلی: {ex.Message}"
                });
            }
        }

    }





    public class CreateMainTitleRequest
    {
        [Required(ErrorMessage = "نام عنوان اصلی الزامی است")]
        [MaxLength(50, ErrorMessage = "نام عنوان اصلی نباید بیشتر از 50 کاراکتر باشد")]
        [MinLength(2, ErrorMessage = "نام عنوان اصلی باید حداقل 2 کاراکتر باشد")]
        public string Name { get; set; } = string.Empty;

        [MaxLength(150, ErrorMessage = "توضیحات نباید بیشتر از 150 کاراکتر باشد")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "مبلغ الزامی است")]
        [Range(0, double.MaxValue, ErrorMessage = "مبلغ نمی‌تواند منفی باشد")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "شناسه حوزه الزامی است")]
        [Range(1, long.MaxValue, ErrorMessage = "شناسه حوزه باید عددی مثبت باشد")]
        public long ScopeId { get; set; }

        [MaxLength(50, ErrorMessage = "ترتیب نمایش نباید بیشتر از 50 کاراکتر باشد")]
        public string? DisplayOrder { get; set; } = "0";

        public long? BpmType { get; set; }
    }
    public class UpdateMainTitleRequest
    {
        [Required(ErrorMessage = "شناسه عنوان اصلی الزامی است")]
        [Range(1, long.MaxValue, ErrorMessage = "شناسه عنوان اصلی باید عددی مثبت باشد")]
        public long Id { get; set; }

        [Required(ErrorMessage = "نام عنوان اصلی الزامی است")]
        [MaxLength(50, ErrorMessage = "نام عنوان اصلی نباید بیشتر از 50 کاراکتر باشد")]
        [MinLength(2, ErrorMessage = "نام عنوان اصلی باید حداقل 2 کاراکتر باشد")]
        public string Name { get; set; } = string.Empty;

        [MaxLength(150, ErrorMessage = "توضیحات نباید بیشتر از 150 کاراکتر باشد")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "مبلغ الزامی است")]
        [Range(0, double.MaxValue, ErrorMessage = "مبلغ نمی‌تواند منفی باشد")]
        public decimal Amount { get; set; }

        [MaxLength(50, ErrorMessage = "ترتیب نمایش نباید بیشتر از 50 کاراکتر باشد")]
        public string? DisplayOrder { get; set; } = "0";

        /// <summary>
        /// نوع BPM (اختیاری)
        /// </summary>
        public long? BpmType { get; set; }
    }
}