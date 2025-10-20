using Application.CQRS;
using Application.Models;
using Domain.Entities.Daroo;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BaseInfo.Controllers
{
    // ===== BpmType CONTROLLER =====
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BpmTypeController : ControllerBase
    {
        private readonly IMediator _mediator;

        public BpmTypeController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// دریافت لیست تمام نوع فرایندها
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<ResultApi<List<BpmType>>>> GetAll()
        {
            try
            {
                var query = new GetAllBpmTypeQuery();
                var result = await _mediator.Send(query);

                return Ok(new ResultApi<List<BpmType>>
                {
                    StatusCode = 200,
                    IsSuccess = true,
                    Message = "لیست نوع فرایندها با موفقیت دریافت شد",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResultApi
                {
                    StatusCode = 500,
                    IsSuccess = false,
                    Message = $"خطا در دریافت لیست نوع فرایندها: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// دریافت نوع فرایند بر اساس شناسه
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ResultApi<BpmType>>> GetById(long id)
        {
            try
            {
                var query = new GetBpmTypeByIdQuery(id);
                var result = await _mediator.Send(query);

                if (result == null)
                {
                    return NotFound(new ResultApi
                    {
                        StatusCode = 404,
                        IsSuccess = false,
                        Message = "نوع فرایند یافت نشد"
                    });
                }

                return Ok(new ResultApi<BpmType>
                {
                    StatusCode = 200,
                    IsSuccess = true,
                    Message = "نوع فرایند با موفقیت دریافت شد",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResultApi
                {
                    StatusCode = 500,
                    IsSuccess = false,
                    Message = $"خطا در دریافت نوع فرایند: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// ایجاد نوع فرایند جدید
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ResultApi<BpmType>>> Create([FromBody] CreateBpmTypeCommand command)
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

                return Ok(new ResultApi<BpmType>
                {
                    StatusCode = 200,
                    IsSuccess = true,
                    Message = "نوع فرایند با موفقیت ایجاد شد",
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
                    Message = $"خطا در ایجاد نوع فرایند: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// ویرایش نوع فرایند
        /// </summary>
        [HttpPut]
        public async Task<ActionResult<ResultApi<BpmType>>> Update([FromBody] UpdateBpmTypeCommand command)
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

                return Ok(new ResultApi<BpmType>
                {
                    StatusCode = 200,
                    IsSuccess = true,
                    Message = "نوع فرایند با موفقیت ویرایش شد",
                    Data = result
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
                    Message = $"خطا در ویرایش نوع فرایند: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// حذف نوع فرایند (Soft Delete)
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult<ResultApi>> Delete(long id)
        {
            try
            {
                var command = new DeleteBpmTypeCommand(id);
                var result = await _mediator.Send(command);

                return Ok(new ResultApi
                {
                    StatusCode = 200,
                    IsSuccess = true,
                    Message = "نوع فرایند با موفقیت حذف شد"
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
                    Message = $"خطا در حذف نوع فرایند: {ex.Message}"
                });
            }
        }
    }
}