using Application.CQRS;
using Application.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BaseInfo.Controllers
{
    /// <summary>
    /// کنترلر مدیریت انواع محصولات
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProductTypeController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductTypeController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// دریافت لیست تمام انواع محصولات
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<ResultApi<List<ProductTypeDto>>>> GetAll()
        {
            try
            {
                var query = new GetAllProductTypesQuery();
                var result = await _mediator.Send(query);

                return Ok(new ResultApi<List<ProductTypeDto>>
                {
                    StatusCode = 200,
                    IsSuccess = true,
                    Message = "لیست انواع محصولات با موفقیت دریافت شد",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResultApi
                {
                    StatusCode = 500,
                    IsSuccess = false,
                    Message = $"خطا در دریافت لیست انواع محصولات: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// دریافت نوع محصول بر اساس شناسه
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ResultApi<ProductTypeDto>>> GetById(long id)
        {
            try
            {
                var query = new GetProductTypeByIdQuery(id);
                var result = await _mediator.Send(query);

                if (result == null)
                {
                    return NotFound(new ResultApi
                    {
                        StatusCode = 404,
                        IsSuccess = false,
                        Message = "نوع محصول یافت نشد"
                    });
                }

                return Ok(new ResultApi<ProductTypeDto>
                {
                    StatusCode = 200,
                    IsSuccess = true,
                    Message = "نوع محصول با موفقیت دریافت شد",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResultApi
                {
                    StatusCode = 500,
                    IsSuccess = false,
                    Message = $"خطا در دریافت نوع محصول: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// دریافت انواع محصولات بر اساس عنوان اصلی
        /// </summary>
        [HttpGet("by-maintitle/{mainTitleId}")]
        public async Task<ActionResult<ResultApi<List<ProductTypeSimpleDto>>>> GetByMainTitle(long mainTitleId)
        {
            try
            {
                var query = new GetProductTypesByMainTitleQuery(mainTitleId);
                var result = await _mediator.Send(query);

                return Ok(new ResultApi<List<ProductTypeSimpleDto>>
                {
                    StatusCode = 200,
                    IsSuccess = true,
                    Message = "لیست انواع محصولات عنوان اصلی با موفقیت دریافت شد",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResultApi
                {
                    StatusCode = 500,
                    IsSuccess = false,
                    Message = $"خطا در دریافت انواع محصولات: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// جستجوی انواع محصولات
        /// </summary>
        [HttpGet("search")]
        public async Task<ActionResult<ResultApi<List<ProductTypeDto>>>> Search(
            [FromQuery] string? searchTerm = null,
            [FromQuery] long? mainTitleId = null)
        {
            try
            {
                var query = new SearchProductTypesQuery(searchTerm ?? "", mainTitleId);
                var result = await _mediator.Send(query);

                return Ok(new ResultApi<List<ProductTypeDto>>
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
                    Message = $"خطا در جستجوی انواع محصولات: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// ایجاد نوع محصول جدید
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ResultApi<long>>> Create([FromBody] CreateProductTypeCommand command)
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
                    Message = "نوع محصول با موفقیت ایجاد شد",
                    Data = result
                });
            }
            catch (InvalidOperationException ex)
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
                    Message = $"خطا در ایجاد نوع محصول: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// ویرایش نوع محصول
        /// </summary>
        [HttpPut]
        public async Task<ActionResult<ResultApi>> Update([FromBody] UpdateProductTypeCommand command)
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
                        Message = "نوع محصول یافت نشد"
                    });
                }

                return Ok(new ResultApi
                {
                    StatusCode = 200,
                    IsSuccess = true,
                    Message = "نوع محصول با موفقیت ویرایش شد"
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ResultApi
                {
                    StatusCode = 404,
                    IsSuccess = false,
                    Message = ex.Message
                });
            }
            catch (InvalidOperationException ex)
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
                    Message = $"خطا در ویرایش نوع محصول: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// حذف نوع محصول (Soft Delete)
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult<ResultApi>> Delete(long id)
        {
            try
            {
                var command = new DeleteProductTypeCommand(id);
                var result = await _mediator.Send(command);

                if (!result)
                {
                    return NotFound(new ResultApi
                    {
                        StatusCode = 404,
                        IsSuccess = false,
                        Message = "نوع محصول یافت نشد"
                    });
                }

                return Ok(new ResultApi
                {
                    StatusCode = 200,
                    IsSuccess = true,
                    Message = "نوع محصول با موفقیت حذف شد"
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ResultApi
                {
                    StatusCode = 404,
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
                    Message = $"خطا در حذف نوع محصول: {ex.Message}"
                });
            }
        }
    }
}