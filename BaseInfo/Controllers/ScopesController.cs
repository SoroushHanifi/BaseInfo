using Application.Common;
using Application.CQRS;
using Application.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace BaseInfo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ScopesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ScopesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<ResultApi<PagedData<ScopeDto>>>> GetAll([FromQuery] long? departmentId, [FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                if (departmentId == null)
                {
                    var query = new GetAllScopesPaginationQuery(pageIndex, pageSize);
                    var result = await _mediator.Send(query);
                    return Ok(new ResultApi<PagedData<ScopeDto>>
                    {
                        StatusCode = 200,
                        IsSuccess = true,
                        Message = "لیست حوزه‌ها با موفقیت دریافت شد",
                        Data = result
                    });
                }
                else 
                {
                    var query = new GetScopesByDepartmentIdQuery(pageIndex, pageSize, (long)departmentId);
                    var result = await _mediator.Send(query);
                    return Ok(new ResultApi<PagedData<ScopeSimpleDto>>
                    {
                        StatusCode = 200,
                        IsSuccess = true,
                        Message = "لیست حوزه‌های اداره کل با موفقیت دریافت شد",
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
                    Message = $"خطا در دریافت لیست حوزه‌ها: {ex.Message}"
                });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ResultApi<ScopeDto>>> GetById(long id)
        {
            try
            {
                var query = new GetScopeByIdQuery(id);
                var result = await _mediator.Send(query);

                if (result == null)
                {
                    return NotFound(new ResultApi
                    {
                        StatusCode = 404,
                        IsSuccess = false,
                        Message = $"حوزه با شناسه {id} یافت نشد"
                    });
                }

                return Ok(new ResultApi<ScopeDto>
                {
                    StatusCode = 200,
                    IsSuccess = true,
                    Message = "اطلاعات حوزه با موفقیت دریافت شد",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResultApi
                {
                    StatusCode = 500,
                    IsSuccess = false,
                    Message = $"خطا در دریافت اطلاعات حوزه: {ex.Message}"
                });
            }
        }

        [HttpPost]
        public async Task<ActionResult<ResultApi<long>>> Create([FromBody] CreateScopeRequest request)
        {
            try
            {
                var command = new CreateScopeCommand(request.Name, request.DepartmentId);
                var scopeId = await _mediator.Send(command);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = scopeId },
                    new ResultApi<long>
                    {
                        StatusCode = 201,
                        IsSuccess = true,
                        Message = "حوزه با موفقیت ایجاد شد",
                        Data = scopeId
                    });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResultApi
                {
                    StatusCode = 500,
                    IsSuccess = false,
                    Message = $"خطا در ایجاد حوزه: {ex.Message}"
                });
            }
        }

        [HttpPut]
        public async Task<ActionResult<ResultApi>> Update([FromBody] UpdateScopeRequest request)
        {
            try
            {
                var command = new UpdateScopeCommand(request.Id, request.Name);
                var result = await _mediator.Send(command);

                if (!result)
                {
                    return NotFound(new ResultApi
                    {
                        StatusCode = 404,
                        IsSuccess = false,
                        Message = $"حوزه با شناسه {request.Id} یافت نشد"
                    });
                }

                return Ok(new ResultApi
                {
                    StatusCode = 200,
                    IsSuccess = true,
                    Message = "حوزه با موفقیت به‌روزرسانی شد"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResultApi
                {
                    StatusCode = 500,
                    IsSuccess = false,
                    Message = $"خطا در به‌روزرسانی حوزه: {ex.Message}"
                });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ResultApi>> Delete(long id)
        {
            try
            {
                var command = new DeleteScopeCommand(id);
                var result = await _mediator.Send(command);

                if (!result)
                {
                    return NotFound(new ResultApi
                    {
                        StatusCode = 404,
                        IsSuccess = false,
                        Message = $"حوزه با شناسه {id} یافت نشد"
                    });
                }

                return Ok(new ResultApi
                {
                    StatusCode = 200,
                    IsSuccess = true,
                    Message = "حوزه با موفقیت حذف شد"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResultApi
                {
                    StatusCode = 500,
                    IsSuccess = false,
                    Message = $"خطا در حذف حوزه: {ex.Message}"
                });
            }
        }
    }

    // ===== Request Models =====

    /// <summary>
    /// مدل درخواست ایجاد حوزه
    /// </summary>
    public class CreateScopeRequest
    {
        /// <summary>
        /// نام حوزه
        /// </summary>
        [Required(ErrorMessage = "نام حوزه الزامی است")]
        [MaxLength(250, ErrorMessage = "نام حوزه نباید بیشتر از 250 کاراکتر باشد")]
        [MinLength(2, ErrorMessage = "نام حوزه باید حداقل 2 کاراکتر باشد")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// شناسه اداره کل
        /// </summary>
        [Required(ErrorMessage = "شناسه اداره کل الزامی است")]
        [Range(1, int.MaxValue, ErrorMessage = "شناسه اداره کل باید عددی مثبت باشد")]
        public int DepartmentId { get; set; }
    }

    /// <summary>
    /// مدل درخواست ویرایش حوزه
    /// </summary>
    public class UpdateScopeRequest
    {
        public long Id { get; set; }

        [Required(ErrorMessage = "نام حوزه الزامی است")]
        [MaxLength(50, ErrorMessage = "نام حوزه نباید بیشتر از 50 کاراکتر باشد")]
        [MinLength(2, ErrorMessage = "نام حوزه باید حداقل 2 کاراکتر باشد")]
        public string Name { get; set; } = string.Empty;
    }
}