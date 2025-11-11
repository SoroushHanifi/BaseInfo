using Application.Common;
using Application.CQRS;
using Application.Models;
using Infrastructure.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BaseInfo.Controllers
{
    /// <summary>
    /// کنترلر مدیریت انواع تولید (Produce Types)
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProduceTypeController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProduceTypeController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// دریافت لیست تمام انواع تولید (غیرحذف شده‌ها)
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<ResultApi<List<ProduceTypeDto>>>> GetAll()
        {
            try
            {
                var query = new GetAllProduceTypesQuery();
                var result = await _mediator.Send(query);

                return Ok(new ResultApi<List<ProduceTypeDto>>
                {
                    StatusCode = 200,
                    IsSuccess = true,
                    Message = "لیست انواع تولید با موفقیت دریافت شد",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResultApi
                {
                    StatusCode = 500,
                    IsSuccess = false,
                    Message = $"خطا در دریافت لیست انواع تولید: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// دریافت نوع تولید بر اساس شناسه
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ResultApi<ProduceTypeDto>>> GetById(long id)
        {
            try
            {
                var query = new GetProduceTypeByIdQuery(id);
                var result = await _mediator.Send(query);

                if (result == null)
                {
                    return NotFound(new ResultApi
                    {
                        StatusCode = 404,
                        IsSuccess = false,
                        Message = "نوع تولید یافت نشد"
                    });
                }

                return Ok(new ResultApi<ProduceTypeDto>
                {
                    StatusCode = 200,
                    IsSuccess = true,
                    Message = "نوع تولید با موفقیت دریافت شد",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResultApi
                {
                    StatusCode = 500,
                    IsSuccess = false,
                    Message = $"خطا در دریافت نوع تولید: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// دریافت انواع تولید فعال (برای dropdown و انتخاب)
        /// </summary>
        [HttpGet("active")]
        public async Task<ActionResult<ResultApi<List<ProduceTypeSimpleDto>>>> GetActive()
        {
            try
            {
                var query = new GetActiveProduceTypesQuery();
                var result = await _mediator.Send(query);

                return Ok(new ResultApi<List<ProduceTypeSimpleDto>>
                {
                    StatusCode = 200,
                    IsSuccess = true,
                    Message = "لیست انواع تولید فعال با موفقیت دریافت شد",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResultApi
                {
                    StatusCode = 500,
                    IsSuccess = false,
                    Message = $"خطا در دریافت انواع تولید فعال: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// جستجوی انواع تولید بر اساس عنوان یا وضعیت فعال بودن
        /// </summary>
        [HttpGet("search")]
        public async Task<ActionResult<ResultApi<List<ProduceTypeDto>>>> Search(
            [FromQuery] string? searchTerm = null,
            [FromQuery] bool? isActive = null)
        {
            try
            {
                var query = new SearchProduceTypesQuery(searchTerm, isActive);
                var result = await _mediator.Send(query);

                return Ok(new ResultApi<List<ProduceTypeDto>>
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
                    Message = $"خطا در جستجوی انواع تولید: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// دریافت انواع تولید با صفحه‌بندی
        /// </summary>
        [HttpGet("paged")]
        public async Task<ActionResult<ResultApi<PagedData<ProduceTypeDto>>>> GetPaged(
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var query = new GetAllProduceTypesPaginationQuery(pageIndex, pageSize);
                var result = await _mediator.Send(query);

                return Ok(new ResultApi<PagedData<ProduceTypeDto>>
                {
                    StatusCode = 200,
                    IsSuccess = true,
                    Message = "داده‌های صفحه‌بندی شده با موفقیت دریافت شد",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResultApi
                {
                    StatusCode = 500,
                    IsSuccess = false,
                    Message = $"خطا در دریافت داده‌های صفحه‌بندی شده: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// ایجاد نوع تولید جدید
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ResultApi<long>>> Create([FromBody] CreateProduceTypeCommand command)
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

                var result = await _mediator.Send(command);

                return Ok(new ResultApi<long>
                {
                    StatusCode = 200,
                    IsSuccess = true,
                    Message = "نوع تولید با موفقیت ایجاد شد",
                    Data = result
                });
            }
            catch (AppException ex)
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
                    Message = $"خطا در ایجاد نوع تولید: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// ویرایش نوع تولید
        /// </summary>
        [HttpPut]
        public async Task<ActionResult<ResultApi>> Update([FromBody] UpdateProduceTypeCommand command)
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

                var result = await _mediator.Send(command);

                if (!result)
                {
                    return NotFound(new ResultApi
                    {
                        StatusCode = 404,
                        IsSuccess = false,
                        Message = "نوع تولید یافت نشد"
                    });
                }

                return Ok(new ResultApi
                {
                    StatusCode = 200,
                    IsSuccess = true,
                    Message = "نوع تولید با موفقیت ویرایش شد"
                });
            }
            catch (AppException ex)
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
                    Message = $"خطا در ویرایش نوع تولید: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// حذف نرم نوع تولید
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult<ResultApi>> Delete(long id)
        {
            try
            {
                var command = new DeleteProduceTypeCommand(id);
                var result = await _mediator.Send(command);

                if (!result)
                {
                    return NotFound(new ResultApi
                    {
                        StatusCode = 404,
                        IsSuccess = false,
                        Message = "نوع تولید یافت نشد"
                    });
                }

                return Ok(new ResultApi
                {
                    StatusCode = 200,
                    IsSuccess = true,
                    Message = "نوع تولید با موفقیت حذف شد"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResultApi
                {
                    StatusCode = 500,
                    IsSuccess = false,
                    Message = $"خطا در حذف نوع تولید: {ex.Message}"
                });
            }
        }
    }
}