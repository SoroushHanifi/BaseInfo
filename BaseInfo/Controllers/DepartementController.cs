using Application.Common;
using Application.CQRS;
using Application.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace BaseInfo.Controllers
{
    // ===== DEPARTMENTS CONTROLLER =====
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DepartmentsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public DepartmentsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<ResultApi<List<DepartmentDto>>>> GetAll()
        {
            try
            {
                var query = new GetAllDepartmentsQuery();
                var result = await _mediator.Send(query);
                return Ok(new ResultApi<List<DepartmentDto>>
                {
                    StatusCode = 200,
                    IsSuccess = true,
                    Message = "لیست ادارات کل با موفقیت دریافت شد",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResultApi
                {
                    StatusCode = 500,
                    IsSuccess = false,
                    Message = $"خطا در دریافت لیست ادارات کل: {ex.Message}"
                });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ResultApi<DepartmentDto>>> GetById(long id)
        {
            try
            {
                var query = new GetDepartmentByIdQuery(id);
                var result = await _mediator.Send(query);

                if (result == null)
                {
                    return NotFound(new ResultApi
                    {
                        StatusCode = 404,
                        IsSuccess = false,
                        Message = $"اداره کل با شناسه {id} یافت نشد"
                    });
                }

                return Ok(new ResultApi<DepartmentDto>
                {
                    StatusCode = 200,
                    IsSuccess = true,
                    Message = "اطلاعات اداره کل با موفقیت دریافت شد",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResultApi
                {
                    StatusCode = 500,
                    IsSuccess = false,
                    Message = $"خطا در دریافت اطلاعات اداره کل: {ex.Message}"
                });
            }
        }

        [HttpPost]
        public async Task<ActionResult<ResultApi<long>>> Create([FromBody] CreateDepartmentRequest request)
        {
            try
            {
                var command = new CreateDepartmentCommand(request.Name);
                var departmentId = await _mediator.Send(command);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = departmentId },
                    new ResultApi<long>
                    {
                        StatusCode = 201,
                        IsSuccess = true,
                        Message = "اداره کل با موفقیت ایجاد شد",
                        Data = departmentId
                    });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResultApi
                {
                    StatusCode = 500,
                    IsSuccess = false,
                    Message = $"خطا در ایجاد اداره کل: {ex.Message}"
                });
            }
        }

        [HttpPut]
        public async Task<ActionResult<ResultApi>> Update([FromBody] UpdateDepartmentRequest request)
        {
            try
            {
                var command = new UpdateDepartmentCommand(request.Id, request.Name);
                var result = await _mediator.Send(command);

                if (!result)
                {
                    return NotFound(new ResultApi
                    {
                        StatusCode = 404,
                        IsSuccess = false,
                        Message = $"اداره کل با شناسه {request.Id} یافت نشد"
                    });
                }

                return Ok(new ResultApi
                {
                    StatusCode = 200,
                    IsSuccess = true,
                    Message = "اداره کل با موفقیت به‌روزرسانی شد"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResultApi
                {
                    StatusCode = 500,
                    IsSuccess = false,
                    Message = $"خطا در به‌روزرسانی اداره کل: {ex.Message}"
                });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ResultApi>> Delete(long id)
        {
            try
            {
                var command = new DeleteDepartmentCommand(id);
                var result = await _mediator.Send(command);

                if (!result)
                {
                    return NotFound(new ResultApi
                    {
                        StatusCode = 404,
                        IsSuccess = false,
                        Message = $"اداره کل با شناسه {id} یافت نشد"
                    });
                }

                return Ok(new ResultApi
                {
                    StatusCode = 200,
                    IsSuccess = true,
                    Message = "اداره کل با موفقیت حذف شد"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResultApi
                {
                    StatusCode = 500,
                    IsSuccess = false,
                    Message = $"خطا در حذف اداره کل: {ex.Message}"
                });
            }
        }
    }


    // ===== Updated Request Models =====
    public class CreateDepartmentRequest
    {
        [Required(ErrorMessage = "نام اداره کل الزامی است")]
        [MaxLength(50, ErrorMessage = "نام اداره کل نباید بیشتر از 50 کاراکتر باشد")] // Updated limit
        [MinLength(2, ErrorMessage = "نام اداره کل باید حداقل 2 کاراکتر باشد")]
        public string Name { get; set; } = string.Empty;
    }

    public class UpdateDepartmentRequest
    {
        [Range(1, long.MaxValue, ErrorMessage = "شناسه اداره کل باید عددی مثبت باشد")]
        public long Id { get; set; } // Changed from int to long

        [Required(ErrorMessage = "نام اداره کل الزامی است")]
        [MaxLength(50, ErrorMessage = "نام اداره کل نباید بیشتر از 50 کاراکتر باشد")] // Updated limit
        [MinLength(2, ErrorMessage = "نام اداره کل باید حداقل 2 کاراکتر باشد")]
        public string Name { get; set; } = string.Empty;
    }
}