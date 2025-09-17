using Application.Common;
using Application.CQRS;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace BaseInfo.Controllers
{
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

        /// <summary>
        /// دریافت لیست تمام ادارات کل
        /// </summary>
        /// <returns>لیست ادارات کل</returns>
        [HttpGet]
        public async Task<ActionResult<List<DepartmentDto>>> GetAll()
        {
            var query = new GetAllDepartmentsQuery();
            var result = await _mediator.Send(query);
            return Ok(new ApiResult<List<DepartmentDto>>
            {
                IsSuccess = true,
                Data = result
            });
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<ApiResult<PagedData<DepartmentDto>>>> GetAllPagination([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
        {
            var query = new GetAllDepartmentsPaginationQuery(pageIndex, pageSize);
            var result = await _mediator.Send(query);
            return Ok(new ApiResult<PagedData<DepartmentDto>>
            {
                IsSuccess = true,
                Messages = null,
                Data = result
            });
        }

        /// <summary>
        /// دریافت اداره کل بر اساس ID
        /// </summary>
        /// <param name="id">شناسه اداره کل</param>
        /// <returns>اطلاعات اداره کل</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<DepartmentDto>> GetById(int id)
        {
            var query = new GetDepartmentByIdQuery(id);
            var result = await _mediator.Send(query);

            if (result == null)
                return NotFound($"Department with ID {id} not found.");

            return Ok(new ApiResult<DepartmentDto> 
            {
                IsSuccess = true,
                Data = result
            });
        }

        /// <summary>
        /// دریافت لیست ادارات کل فعال (مرتب شده)
        /// </summary>
        /// <returns>لیست ادارات کل فعال</returns>
        [HttpGet("active")]
        public async Task<ActionResult<List<DepartmentDto>>> GetActive()
        {
            var query = new GetActiveDepartmentsQuery();
            var result = await _mediator.Send(query);
            return Ok(new ApiResult<List<DepartmentDto>> 
            {
                IsSuccess = true,
                Data = result
            });
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<ApiResult<PagedData<DepartmentDto>>>> GetActivePagination([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
        {
            var query = new GetActiveDepartmentsPaginationQuery(pageIndex, pageSize);
            var result = await _mediator.Send(query);
            return Ok(new ApiResult<PagedData<DepartmentDto>>
            {
                IsSuccess = true,
                Messages = null,
                Data = result
            });
        }

        /// <summary>
        /// ایجاد اداره کل جدید
        /// </summary>
        /// <param name="request">اطلاعات اداره کل</param>
        /// <returns>شناسه اداره کل ایجاد شده</returns>
        [HttpPost]
        public async Task<ActionResult<int>> Create([FromBody] CreateDepartmentRequest request)
        {
            var command = new CreateDepartmentCommand(request.Name);
            var departmentId = await _mediator.Send(command);

            return CreatedAtAction(
                nameof(GetById),
                new { id = departmentId },
                new { Id = departmentId, Message = "Department created successfully" }
            );
        }

        /// <summary>
        /// ویرایش اداره کل
        /// </summary>
        /// <param name="id">شناسه اداره کل</param>
        /// <param name="request">اطلاعات جدید</param>
        /// <returns></returns>
        [HttpPut]
        public async Task<ActionResult> Update([FromBody] UpdateDepartmentRequest request)
        {
            var command = new UpdateDepartmentCommand(request.Id, request.Name);
            var result = await _mediator.Send(command);

            if (!result)
                return NotFound($"Department with ID {request.Id} not found.");

            return Ok(new { Message = "Department updated successfully" });
        }

        /// <summary>
        /// حذف اداره کل (Soft Delete)
        /// </summary>
        /// <param name="id">شناسه اداره کل</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var command = new DeleteDepartmentCommand(id);
            var result = await _mediator.Send(command);

            if (!result)
                return NotFound($"Department with ID {id} not found.");

            return Ok(new { Message = "Department deleted successfully" });
        }
    }

    // ===== Request Models =====

    /// <summary>
    /// مدل درخواست ایجاد اداره کل
    /// </summary>
    public class CreateDepartmentRequest
    {
        /// <summary>
        /// نام اداره کل
        /// </summary>
        [Required(ErrorMessage = "نام اداره کل الزامی است")]
        [MaxLength(200, ErrorMessage = "نام اداره کل نباید بیشتر از 200 کاراکتر باشد")]
        [MinLength(2, ErrorMessage = "نام اداره کل باید حداقل 2 کاراکتر باشد")]
        public string Name { get; set; } = string.Empty;
    }

    /// <summary>
    /// مدل درخواست ویرایش اداره کل
    /// </summary>
    public class UpdateDepartmentRequest
    {
        public int Id { get; set; }
        /// <summary>
        /// نام اداره کل
        /// </summary>
        [Required(ErrorMessage = "نام اداره کل الزامی است")]
        [MaxLength(200, ErrorMessage = "نام اداره کل نباید بیشتر از 200 کاراکتر باشد")]
        [MinLength(2, ErrorMessage = "نام اداره کل باید حداقل 2 کاراکتر باشد")]
        public string Name { get; set; } = string.Empty;
    }
}
