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
    public class ScopesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ScopesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// دریافت لیست تمام حوزه‌ها
        /// </summary>
        /// <returns>لیست حوزه‌ها</returns>
        [HttpGet]
        public async Task<ActionResult<List<ScopeDto>>> GetAll()
        {
            var query = new GetAllScopesQuery();
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// دریافت حوزه بر اساس ID
        /// </summary>
        /// <param name="id">شناسه حوزه</param>
        /// <returns>اطلاعات حوزه</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<ScopeDto>> GetById(int id)
        {
            var query = new GetScopeByIdQuery(id);
            var result = await _mediator.Send(query);

            if (result == null)
                return NotFound($"Scope with ID {id} not found.");

            return Ok(result);
        }

        /// <summary>
        /// دریافت حوزه‌های یک اداره کل خاص
        /// </summary>
        /// <param name="departmentId">شناسه اداره کل</param>
        /// <returns>لیست حوزه‌های اداره کل</returns>
        [HttpGet("department/{departmentId}")]
        public async Task<ActionResult<List<ScopeSimpleDto>>> GetByDepartmentId(int departmentId)
        {
            var query = new GetScopesByDepartmentIdQuery(departmentId);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// جستجو در حوزه‌ها
        /// </summary>
        /// <param name="searchTerm">عبارت جستجو</param>
        /// <param name="departmentId">شناسه اداره کل (اختیاری)</param>
        /// <returns>نتایج جستجو</returns>
        [HttpGet("search")]
        public async Task<ActionResult<List<ScopeDto>>> Search(
            [FromQuery] string searchTerm,
            [FromQuery] int? departmentId = null)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return BadRequest("عبارت جستجو نمی‌تواند خالی باشد");

            var query = new SearchScopesQuery(searchTerm, departmentId);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// ایجاد حوزه جدید
        /// </summary>
        /// <param name="request">اطلاعات حوزه</param>
        /// <returns>شناسه حوزه ایجاد شده</returns>
        [HttpPost]
        public async Task<ActionResult<int>> Create([FromBody] CreateScopeRequest request)
        {
            var command = new CreateScopeCommand(request.Name, request.DepartmentId);
            var scopeId = await _mediator.Send(command);

            return CreatedAtAction(
                nameof(GetById),
                new { id = scopeId },
                new { Id = scopeId, Message = "Scope created successfully" }
            );
        }

        /// <summary>
        /// ویرایش حوزه
        /// </summary>
        /// <param name="id">شناسه حوزه</param>
        /// <param name="request">اطلاعات جدید</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] UpdateScopeRequest request)
        {
            var command = new UpdateScopeCommand(id, request.Name);
            var result = await _mediator.Send(command);

            if (!result)
                return NotFound($"Scope with ID {id} not found.");

            return Ok(new { Message = "Scope updated successfully" });
        }

        /// <summary>
        /// حذف حوزه (Soft Delete)
        /// </summary>
        /// <param name="id">شناسه حوزه</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var command = new DeleteScopeCommand(id);
            var result = await _mediator.Send(command);

            if (!result)
                return NotFound($"Scope with ID {id} not found.");

            return Ok(new { Message = "Scope deleted successfully" });
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
        /// <summary>
        /// نام حوزه
        /// </summary>
        [Required(ErrorMessage = "نام حوزه الزامی است")]
        [MaxLength(250, ErrorMessage = "نام حوزه نباید بیشتر از 250 کاراکتر باشد")]
        [MinLength(2, ErrorMessage = "نام حوزه باید حداقل 2 کاراکتر باشد")]
        public string Name { get; set; } = string.Empty;
    }
}